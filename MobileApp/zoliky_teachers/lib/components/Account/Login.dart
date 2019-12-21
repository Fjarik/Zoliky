import 'dart:developer';

import 'package:flutter/services.dart';
import 'package:google_sign_in/google_sign_in.dart';
import 'package:firebase_analytics/firebase_analytics.dart';
import 'package:firebase_analytics/observer.dart';
import 'package:flutter/material.dart';
import 'package:flutter_facebook_login/flutter_facebook_login.dart';
import 'package:font_awesome_flutter/font_awesome_flutter.dart';
import 'package:local_auth/local_auth.dart';
import 'package:shared_preferences/shared_preferences.dart';
import 'package:url_launcher/url_launcher.dart';
import 'package:zoliky_teachers/pages/Account/LoginPage.dart';
import 'package:zoliky_teachers/pages/shared/MenuLayoutPage.dart';
import 'package:zoliky_teachers/utils/Global.dart';
import 'package:zoliky_teachers/utils/MenuPainter.dart';
import 'package:zoliky_teachers/utils/SettingKeys.dart';
import 'package:zoliky_teachers/utils/Singleton.dart';
import 'package:zoliky_teachers/utils/api/connectors/PublicConnector.dart';
import 'package:zoliky_teachers/utils/api/connectors/UserConnector.dart';
import 'package:zoliky_teachers/utils/api/enums/StatusCode.dart';
import 'package:zoliky_teachers/utils/api/enums/UserRoles.dart';
import 'package:zoliky_teachers/utils/api/models/User.dart';
import 'package:zoliky_teachers/utils/api/models/universal/MActionResult.dart';

class LoginPageState extends State<LoginPage>
    with SingleTickerProviderStateMixin, WidgetsBindingObserver {
  LoginPageState(this.analytics, this.observer, this.autoLogin);

  final FirebaseAnalytics analytics;
  final FirebaseAnalyticsObserver observer;
  final bool autoLogin;

  static String _mainUrl = "https://www.zoliky.eu";
  static String _registerUrl = "$_mainUrl/Account/Register";
  static String _forgotPwdUrl = "$_mainUrl/Account/ForgotPassword";

  bool get _darkmode => Singleton().darkmode;

  LocalAuthentication _localAuth = LocalAuthentication();

  final GlobalKey<ScaffoldState> _key = GlobalKey<ScaffoldState>();

  final FacebookLogin facebookLogin = FacebookLogin();

  TextEditingController _txtUsername = TextEditingController();
  TextEditingController _txtPassword = TextEditingController();

  TextEditingController _txtRegEmail = TextEditingController();
  TextEditingController _txtRegFirstname = TextEditingController();
  TextEditingController _txtRegLastname = TextEditingController();

  FocusNode _focusUsername = FocusNode();
  FocusNode _focusPassword = FocusNode();

  FocusNode _focusRegEmail = FocusNode();
  FocusNode _focusRegFirstname = FocusNode();
  FocusNode _focusRegLastname = FocusNode();

  PageController _pageController = PageController();

  Widget _defaultPage;

  Color left = Colors.black;
  Color right = Colors.white;

  bool _pwdVisible = false;
  bool _isLoading = false;
  bool shouldLogin = false;

  @override
  void initState() {
    super.initState();

    _defaultPage = MenuLayoutPage(
      analytics: this.analytics,
      observer: this.observer,
    );

    _focusPassword.addListener(_txtPasswordFocusChanges);
    WidgetsBinding.instance.addObserver(this);

    SharedPreferences.getInstance().then((prefs) {
      var username = prefs.getString(SettingKeys.lastUsername);
      if (username != null && username.isNotEmpty) {
        _txtUsername.text = username;
      }
      var signIn = prefs.containsKey(SettingKeys.autoLogin) &&
          prefs.getBool(SettingKeys.autoLogin);
      if (autoLogin && signIn) {
        _initByTokenAsync(prefs).then((res) {
          _setLoading(false);
          shouldLogin = true;
        });
      }
    });
  }

  @override
  void dispose() {
    _txtUsername?.dispose();
    _txtPassword?.dispose();
    _txtRegEmail?.dispose();
    _txtRegFirstname?.dispose();
    _txtRegLastname?.dispose();

    _focusUsername?.dispose();
    _focusPassword?.dispose();
    _focusRegEmail?.dispose();
    _focusRegFirstname?.dispose();
    _focusRegLastname?.dispose();

    _pageController?.dispose();

    WidgetsBinding.instance.removeObserver(this);
    super.dispose();
  }

  @override
  void didChangeAppLifecycleState(AppLifecycleState state) async {
    log("state = $state");
    if (state == AppLifecycleState.resumed) {
      if (!shouldLogin) {
        return;
      }
      var settings = await SharedPreferences.getInstance();
      await _initByTokenAsync(settings);
      _setLoading(false);
      shouldLogin = false;
    }
  }

  Future<bool> _initByTokenAsync(SharedPreferences settings) async {
    if (settings != null && settings.containsKey(SettingKeys.lastToken)) {
      var token = settings.getString(SettingKeys.lastToken);
      if (token != null && token.isNotEmpty) {
        _setLoading(true);
        Singleton().token = token;
        return await _tryTokenAsync(token);
      }
    }
    return false;
  }

  Future<bool> _tryTokenAsync(String token) async {
    if (token.isEmpty) {
      return false;
    }
    var uc = UserConnector();
    uc.usedToken = token;

    var res = await uc.getMeAsync();
    await _login(res);
    return true;
  }

  Future _loginAsync() async {
    if (_isLoading) {
      return;
    }

    var username = _txtUsername.text;
    var password = _txtPassword.text;
    FocusScope.of(context).unfocus();

    if (username.isEmpty || username.length < 4) {
      _showError("Musíte vyplnit přihlašovací jméno");
      return;
    }

    if (password.isEmpty || password.length < 5) {
      _showError("Musíte vyplnit heslo");
      return;
    }

    _setLoading(true);
    var connection = await _checkConnection();
    if (!connection) {
      _setLoading(false);
      return;
    }
    var res = await UserConnector().login(username, password);
    SharedPreferences prefs = await SharedPreferences.getInstance();
    await prefs.setString(SettingKeys.lastUsername, username);

    await _login(res);
  }

  Future _login(MActionResult<User> res) async {
    var success = await _checkLogin(res);
    if (success) {
      var settings = await SharedPreferences.getInstance();
      if (settings.containsKey(SettingKeys.biometics) &&
          settings.getBool(SettingKeys.biometics)) {
        var bio = await _checkBiometrics();
        if (!bio) {
          _setLoading(false);
          _showSnackbar("Špatná autorizace přes biometriku");
          return;
        }
      }

      analytics.logLogin();
      var r = MaterialPageRoute(
        builder: (context) => _defaultPage,
        settings: RouteSettings(
          name: "/dashboard",
        ),
      );
      await Navigator.pushReplacement(context, r);
      return;
    }
    _setLoading(false);
  }

  Future<bool> _checkBiometrics() async {
    await _localAuth.stopAuthentication();
    var canCheck = await _localAuth.canCheckBiometrics;
    if (!canCheck) {
      return false;
    }
    try {
      return await _localAuth.authenticateWithBiometrics(
        localizedReason: "Ověřte svou identitu",
        stickyAuth: false,
        useErrorDialogs: true,
        sensitiveTransaction: false,
      );
    } on PlatformException catch (ex) {
      // AndroidX má v sobě bug - Když se appka zavře/minimalizuje při biometrice, začne házet chyby a poté přestane fungovat
      log(ex.message);
    }
    return false;
  }

  Future<bool> _checkLogin(MActionResult<User> res) async {
    if (res == null) {
      _showError("Vyskytla se nespecifikovaná chyba");
    }

    String msg = res.getStatusMessage();

    if (res.status == StatusCode.NotFound ||
        res.status == StatusCode.WrongPassword) {
      msg = "Neplatné jméno nebo heslo";
    } else if (res.status == StatusCode.InternalError) {
      msg = "Platnost přihlášení vypršela";
    }

    if (!res.isSuccess) {
      _showSnackbar(msg);
      _txtPassword.clear();
      return false;
    }

    var user = res.content;

    if (!user.isInRolesOr([
      UserRole.Teacher,
      UserRole.SchoolManager,
      UserRole.Administrator,
      UserRole.Developer
    ])) {
      _showSnackbar("Tato aplikace je určena pouze pro vyučující");
      _txtPassword.clear();
      return false;
    }

    await Global.loadApp(Singleton().token);
    Singleton().user = user;
    SharedPreferences prefs = await SharedPreferences.getInstance();
    await prefs.setString(SettingKeys.lastToken, Singleton().token);
    this.analytics?.logEvent(
      name: "CustomLogin",
      parameters: {
        'user': user.fullName,
        'userID': user.id,
      },
    );

    return true;
  }

  Future _fbLoginAsync() async {
    if (_isLoading) {
      return;
    }
    _setLoading(true);
    final result = await facebookLogin.logIn(["email"]);
    var token = "";
    switch (result.status) {
      case FacebookLoginStatus.loggedIn:
        token = result.accessToken.token;
        if (token.isEmpty) {
          return;
        }
        break;
      case FacebookLoginStatus.cancelledByUser:
        _setLoading(false);
        _showSnackbar("Akce zrušena uživatelem");
        return;
      case FacebookLoginStatus.error:
        _setLoading(false);
        _showSnackbar("Vyskytla se chyba při přihlašování");
        return;
    }

    var connection = await _checkConnection();
    if (!connection) {
      _setLoading(false);
      return;
    }
    var res = await UserConnector().loginExternal(token, "Facebook");
    await _login(res);
  }

  Future _googleLoginAsync() async {
    if (_isLoading) {
      return;
    }
    _setLoading(true);
    var signIn = GoogleSignIn(scopes: ["email"]);
    String token = "";
    try {
      var account = await signIn.signIn();
      if (account == null) {
        _showSnackbar("Nezdařilo se přihlásit přes Google účet");
        _setLoading(false);
        return;
      }
      var auth = await account.authentication;
      token = auth.accessToken;
    } catch (error) {
      _showSnackbar("Nezdařilo se přihlásit přes Google účet");
      if (Global.isInDebugMode) {
        _showError(error.toString());
      }
      _setLoading(false);
      return;
    }
    var connection = await _checkConnection();
    if (!connection) {
      _setLoading(false);
      return;
    }
    var res = await UserConnector().loginExternal(token, "Google");
    await _login(res);
  }

  Future<bool> _checkConnection() async {
    var pc = PublicConnector();
    var status = pc.checkConnectionsAsync();
    var res = await status.timeout(Duration(seconds: 7), onTimeout: () {
      return false;
    });

    if (!res) {
      _showDialog("Špatné připojení", "Nezdařilo se připojit k serverům");
      return false;
    }
    return true;
  }

  Future _registerAsync() async {
    var email = _txtRegEmail.text;
    var firstname = _txtRegFirstname.text;
    var lastname = _txtRegLastname.text;

    if (email.isEmpty) {
      _showError("Musíte vyplnit email");
      return;
    }

    _setLoading(true);

    var query = "?";
    if (email.isNotEmpty) {
      query += 'Email=$email&';
    }
    if (firstname.isNotEmpty) {
      query += 'Name=$firstname&';
    }
    if (lastname.isNotEmpty) {
      query += 'Lastname=$lastname&';
    }

    var uri = Uri.tryParse(_registerUrl + query);
    if (uri == null) {
      _showError("Neplatné vstupní parametry");
      _setLoading(false);
      return;
    }

    await _launchURL(uri.toString());

    _setLoading(false);
  }

  void _setLoading(bool loading) {
    setState(() {
      _isLoading = loading;
    });
  }

  void _txtPasswordFocusChanges() {
    this.setState(() {
      _pwdVisible = _pwdVisible && _focusPassword.hasFocus;
    });
  }

  void _tooglePassword() {
    setState(() {
      _pwdVisible = !_pwdVisible;
    });
  }

  void _onSignInButtonPress() {
    _pageController.animateToPage(0,
        duration: Duration(milliseconds: 500), curve: Curves.decelerate);
  }

  void _onSignUpButtonPress() {
    _pageController?.animateToPage(1,
        duration: Duration(milliseconds: 500), curve: Curves.decelerate);
  }

  void _onPageChange(int page) {
    FocusScope.of(context).unfocus();
    if (page == 0) {
      setState(() {
        left = Colors.black;
        right = Colors.white;
      });
      return;
    }
    setState(() {
      left = Colors.white;
      right = Colors.black;
    });
  }

  void _showError(String content) {
    _showDialog("Chyba", content);
  }

  void _showDialog(String title, String content) {
    showDialog(
        context: context,
        builder: (BuildContext context) {
          return AlertDialog(
            title: Text(title),
            content: Text(content),
            actions: <Widget>[
              FlatButton(
                child: Text("OK"),
                onPressed: () {
                  Navigator.of(context).pop();
                },
              )
            ],
          );
        });
  }

  void _showSnackbar(String content) {
    var snack = SnackBar(
      content: Text(content),
      backgroundColor: Colors.red[700],
      duration: Duration(seconds: 2),
    );
    _key.currentState.showSnackBar(snack);
  }

  Future _launchURL(String url) async {
    if (await canLaunch(url)) {
      await launch(url);
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      key: _key,
      body: SingleChildScrollView(
        child: Container(
          width: MediaQuery.of(context).size.width,
          height: MediaQuery.of(context).size.height >= 775.0
              ? MediaQuery.of(context).size.height
              : 775.0,
          decoration: BoxDecoration(
            gradient: LinearGradient(
              colors: _darkmode
                  ? [
                      Colors.black38,
                      Colors.black26,
                    ]
                  : [
                      Color(0xFF2196f3),
                      Color(0xFF40c4ff),
                    ],
              begin: const FractionalOffset(0.2, 0.2),
              end: const FractionalOffset(1, 1),
              stops: [0, 1],
              tileMode: TileMode.clamp,
            ),
          ),
          child: _isLoading
              ? _loading()
              : Column(
                  mainAxisSize: MainAxisSize.max,
                  children: <Widget>[
                    Padding(
                      padding: EdgeInsets.only(
                        top: 100,
                        bottom: 50,
                        left: 50,
                        right: 50,
                      ),
                      child: Image(
                        fit: BoxFit.fill,
                        image: AssetImage("assets/ZolikyHeaderWhite_1024.png"),
                      ),
                    ),
                    Padding(
                      child: _loginOrCreate(context),
                      padding: EdgeInsets.only(top: 20),
                    ),
                    Expanded(
                      flex: 2,
                      child: PageView(
                        controller: _pageController,
                        onPageChanged: _onPageChange,
                        children: <Widget>[
                          ConstrainedBox(
                            constraints: const BoxConstraints.expand(),
                            child: _signIn(context),
                          ),
                          ConstrainedBox(
                            constraints: const BoxConstraints.expand(),
                            child: _signUp(context),
                          ),
                        ],
                      ),
                    ),
                    Container(
                      child: Column(
                        mainAxisAlignment: MainAxisAlignment.center,
                        children: <Widget>[
                          Padding(
                            padding: EdgeInsets.symmetric(vertical: 15),
                            child: Text(
                              DateTime.now().year.toString() + " \u00a9 Žolíky",
                              style: TextStyle(
                                color: Colors.grey[200],
                              ),
                            ),
                          ),
                        ],
                      ),
                    ),
                  ],
                ),
        ),
      ),
    );
  }

  Widget _loginOrCreate(BuildContext context) {
    return Container(
      width: 300,
      height: 50,
      decoration: BoxDecoration(
        color: _darkmode ? Colors.grey[600] : Color(0x552B2B2B),
        borderRadius: BorderRadius.all(
          Radius.circular(25),
        ),
      ),
      child: CustomPaint(
        painter: TabIndicationPainter(pageController: _pageController),
        child: Row(
          mainAxisAlignment: MainAxisAlignment.spaceEvenly,
          children: <Widget>[
            Expanded(
              child: FlatButton(
                splashColor: Colors.transparent,
                highlightColor: Colors.transparent,
                child: Text(
                  "Přihlášení",
                  style: TextStyle(
                    color: left,
                    fontSize: 16,
                  ),
                ),
                onPressed: _onSignInButtonPress,
              ),
            ),
            Expanded(
              child: FlatButton(
                splashColor: Colors.transparent,
                highlightColor: Colors.transparent,
                child: Text(
                  "Registrace",
                  style: TextStyle(
                    color: right,
                    fontSize: 16,
                  ),
                ),
                onPressed: _onSignUpButtonPress,
              ),
            )
          ],
        ),
      ),
    );
  }

  Widget _signIn(BuildContext context) {
    return Container(
      padding: EdgeInsets.only(top: 25),
      child: Column(
        children: <Widget>[
          Stack(
            alignment: Alignment.topCenter,
            overflow: Overflow.visible,
            children: <Widget>[
              Card(
                elevation: 2,
                shape: RoundedRectangleBorder(
                  borderRadius: BorderRadius.circular(8.0),
                ),
                child: Container(
                  width: MediaQuery.of(context).size.width * 0.8,
                  height: 190,
                  child: Column(
                    children: <Widget>[
                      Padding(
                        padding:
                            EdgeInsets.symmetric(vertical: 20, horizontal: 25),
                        child: TextField(
                          controller: this._txtUsername,
                          focusNode: this._focusUsername,
                          autocorrect: false,
                          autofocus: false,
                          maxLines: 1,
                          keyboardType: TextInputType.emailAddress,
                          textInputAction: TextInputAction.next,
                          onEditingComplete: () {
                            _focusPassword.requestFocus();
                          },
                          style: TextStyle(
                            fontSize: 16,
                          ),
                          decoration: InputDecoration(
                            border: InputBorder.none,
                            hintText: "Přihlašovací jméno",
                          ),
                        ),
                      ),
                      _line(),
                      Padding(
                        padding:
                            EdgeInsets.symmetric(vertical: 20, horizontal: 25),
                        child: Stack(
                          alignment: Alignment.centerRight,
                          children: <Widget>[
                            Padding(
                              padding: EdgeInsets.only(right: 47),
                              child: TextField(
                                controller: this._txtPassword,
                                focusNode: this._focusPassword,
                                autocorrect: false,
                                autofocus: false,
                                obscureText: !_pwdVisible,
                                maxLines: 1,
                                textInputAction: TextInputAction.done,
                                onEditingComplete: this._loginAsync,
                                style: TextStyle(
                                  fontSize: 16,
                                ),
                                decoration: InputDecoration(
                                  border: InputBorder.none,
                                  hintText: "Heslo",
                                ),
                              ),
                            ),
                            IconButton(
                              icon: Icon(
                                _pwdVisible
                                    ? FontAwesomeIcons.eyeSlash
                                    : FontAwesomeIcons.eye,
                                size: 18,
                              ),
                              onPressed: _tooglePassword,
                            ),
                          ],
                        ),
                      ),
                    ],
                  ),
                ),
              ),
              Container(
                margin: EdgeInsets.only(top: 170),
                decoration: BoxDecoration(
                  borderRadius: BorderRadius.all(
                    Radius.circular(5),
                  ),
                  color: Color(0xFF007adf),
                ),
                child: MaterialButton(
                  child: Padding(
                    padding: EdgeInsets.symmetric(vertical: 10, horizontal: 42),
                    child: Text(
                      "Přihlásit se",
                      style: TextStyle(
                        color: Colors.white,
                        fontSize: 25,
                      ),
                    ),
                  ),
                  onPressed: this._loginAsync,
                ),
              )
            ],
          ),
          Padding(
            padding: EdgeInsets.only(top: 10),
            child: FlatButton(
              child: Text(
                "Zapomenuté heslo?",
                style: TextStyle(
                  decoration: TextDecoration.underline,
                  color: Colors.white,
                  fontSize: 16,
                ),
              ),
              onPressed: () async {
                await _launchURL(_forgotPwdUrl);
              },
            ),
          ),
          _splitLine("Nebo"),
          Row(
            mainAxisAlignment: MainAxisAlignment.center,
            children: <Widget>[
              Padding(
                padding: EdgeInsets.only(top: 10, right: 40),
                child: GestureDetector(
                  child: Container(
                    padding: const EdgeInsets.all(15.0),
                    decoration: BoxDecoration(
                      shape: BoxShape.circle,
                      color: Colors.white,
                    ),
                    child: Icon(
                      FontAwesomeIcons.facebookF,
                      color: Color(0xFF0084ff),
                    ),
                  ),
                  onTap: _fbLoginAsync,
                ),
              ),
              Padding(
                padding: EdgeInsets.only(top: 10),
                child: GestureDetector(
                  child: Container(
                    padding: const EdgeInsets.all(15.0),
                    decoration: BoxDecoration(
                      shape: BoxShape.circle,
                      color: Colors.white,
                    ),
                    child: Icon(
                      FontAwesomeIcons.google,
                      color: Color(0xFF0084ff),
                    ),
                  ),
                  onTap: _googleLoginAsync,
                ),
              ),
            ],
          ),
        ],
      ),
    );
  }

  Widget _signUp(BuildContext context) {
    return Container(
      padding: EdgeInsets.only(top: 25),
      child: Column(
        children: <Widget>[
          Stack(
            alignment: Alignment.topCenter,
            overflow: Overflow.visible,
            children: <Widget>[
              Card(
                elevation: 2,
                shape: RoundedRectangleBorder(
                  borderRadius: BorderRadius.circular(8.0),
                ),
                child: Container(
                  width: MediaQuery.of(context).size.width * 0.8,
                  // height: 190,
                  child: Column(
                    children: <Widget>[
                      Padding(
                        padding:
                            EdgeInsets.symmetric(vertical: 20, horizontal: 25),
                        child: TextField(
                          controller: this._txtRegEmail,
                          focusNode: this._focusRegEmail,
                          autocorrect: false,
                          autofocus: false,
                          maxLines: 1,
                          keyboardType: TextInputType.emailAddress,
                          textInputAction: TextInputAction.next,
                          onEditingComplete: () {
                            _focusRegFirstname.requestFocus();
                          },
                          style: TextStyle(
                            fontSize: 16,
                          ),
                          decoration: InputDecoration(
                            border: InputBorder.none,
                            hintText: "Email",
                          ),
                        ),
                      ),
                      _line(),
                      Padding(
                        padding:
                            EdgeInsets.symmetric(vertical: 20, horizontal: 25),
                        child: TextField(
                          controller: this._txtRegFirstname,
                          focusNode: this._focusRegFirstname,
                          textCapitalization: TextCapitalization.words,
                          autocorrect: false,
                          autofocus: false,
                          maxLines: 1,
                          textInputAction: TextInputAction.next,
                          onEditingComplete: () {
                            _focusRegLastname.requestFocus();
                          },
                          style: TextStyle(
                            fontSize: 16,
                          ),
                          decoration: InputDecoration(
                            border: InputBorder.none,
                            hintText: "Jméno",
                          ),
                        ),
                      ),
                      _line(),
                      Padding(
                        padding: EdgeInsets.only(
                            top: 25, bottom: 35, left: 20, right: 20),
                        child: TextField(
                          controller: this._txtRegLastname,
                          focusNode: this._focusRegLastname,
                          textCapitalization: TextCapitalization.words,
                          autocorrect: false,
                          autofocus: false,
                          maxLines: 1,
                          textInputAction: TextInputAction.done,
                          onEditingComplete: this._registerAsync,
                          style: TextStyle(
                            fontSize: 16,
                          ),
                          decoration: InputDecoration(
                            border: InputBorder.none,
                            hintText: "Příjmení",
                          ),
                        ),
                      ),
                    ],
                  ),
                ),
              ),
              Container(
                margin: EdgeInsets.only(top: 255),
                decoration: BoxDecoration(
                  borderRadius: BorderRadius.all(
                    Radius.circular(5),
                  ),
                  boxShadow: <BoxShadow>[
                    BoxShadow(),
                  ],
                  color: Color(0xFF007adf),
                ),
                child: MaterialButton(
                  child: Padding(
                    padding: EdgeInsets.symmetric(vertical: 10, horizontal: 42),
                    child: Text(
                      "Registrovat se",
                      style: TextStyle(
                        color: Colors.white,
                        fontSize: 25,
                      ),
                    ),
                  ),
                  onPressed: this._registerAsync,
                ),
              )
            ],
          ),
          Padding(
            padding: EdgeInsets.only(top: 30),
          ),
          _splitLine("Nebo"),
          Padding(
            padding: EdgeInsets.only(top: 10),
            child: FlatButton(
              child: Text(
                "Přímá registrace",
                style: TextStyle(
                    decoration: TextDecoration.underline,
                    color: Colors.white,
                    fontSize: 16),
              ),
              onPressed: () async {
                await _launchURL(_registerUrl);
              },
            ),
          ),
        ],
      ),
    );
  }

  Widget _line() {
    return Padding(
      padding: EdgeInsets.symmetric(horizontal: 20),
      child: Container(
        height: 1,
        color: Colors.grey[400],
      ),
    );
  }

  Widget _splitLine(String text) {
    return Padding(
      padding: EdgeInsets.only(top: 10),
      child: Row(
        mainAxisAlignment: MainAxisAlignment.center,
        children: <Widget>[
          Container(
            width: 100,
            height: 1,
            decoration: BoxDecoration(
              color: Colors.white,
            ),
          ),
          Padding(
            padding: EdgeInsets.symmetric(horizontal: 15),
            child: Text(
              text,
              style: TextStyle(
                color: Colors.white,
                fontSize: 16,
              ),
            ),
          ),
          Container(
            width: 100,
            height: 1,
            decoration: BoxDecoration(
              color: Colors.white,
            ),
          ),
        ],
      ),
    );
  }

  Widget _loading() {
    return Column(
      mainAxisAlignment: MainAxisAlignment.center,
      crossAxisAlignment: CrossAxisAlignment.center,
      children: <Widget>[
        CircularProgressIndicator(
          valueColor: AlwaysStoppedAnimation<Color>(Colors.white),
        ),
        Padding(
          padding: EdgeInsets.only(top: 30),
          child: Text(
            "Probíhá přihlašování...",
            style: TextStyle(
              color: Colors.white,
            ),
          ),
        )
      ],
    );
  }
}
