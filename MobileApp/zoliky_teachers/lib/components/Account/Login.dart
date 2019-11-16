import 'package:google_sign_in/google_sign_in.dart';
import 'package:appcenter_analytics/appcenter_analytics.dart';
import 'package:firebase_analytics/firebase_analytics.dart';
import 'package:firebase_analytics/observer.dart';
import 'package:flutter/material.dart';
import 'package:flutter_facebook_login/flutter_facebook_login.dart';
import 'package:font_awesome_flutter/font_awesome_flutter.dart';
import 'package:shared_preferences/shared_preferences.dart';
import 'package:url_launcher/url_launcher.dart';
import 'package:zoliky_teachers/pages/Account/LoginPage.dart';
import 'package:zoliky_teachers/pages/Administration/DashboardPage.dart';
import 'package:zoliky_teachers/utils/Global.dart';
import 'package:zoliky_teachers/utils/MenuPainter.dart';
import 'package:zoliky_teachers/utils/SettingKeys.dart';
import 'package:zoliky_teachers/utils/Singleton.dart';
import 'package:zoliky_teachers/utils/api/connectors/PublicConnector.dart';
import 'package:zoliky_teachers/utils/api/connectors/UserConnector.dart';
import 'package:zoliky_teachers/utils/api/enums/PageStatus.dart';
import 'package:zoliky_teachers/utils/api/enums/StatusCode.dart';
import 'package:zoliky_teachers/utils/api/enums/UserRoles.dart';
import 'package:zoliky_teachers/utils/api/models/Unavailability.dart';
import 'package:zoliky_teachers/utils/api/models/User.dart';
import 'package:zoliky_teachers/utils/api/models/WebStatus.dart';
import 'package:zoliky_teachers/utils/api/models/universal/MActionResult.dart';

class LoginPageState extends State<LoginPage>
    with SingleTickerProviderStateMixin {
  LoginPageState(this.analytics, this.observer, this.autoLogin);

  final FirebaseAnalytics analytics;
  final FirebaseAnalyticsObserver observer;
  final bool autoLogin;

  static String _mainUrl = "https://www.zoliky.eu";
  static String _registerUrl = "$_mainUrl/Account/Register";
  static String _forgotPwdUrl = "$_mainUrl/Account/ForgotPassword";

  final GlobalKey<ScaffoldState> _key = GlobalKey<ScaffoldState>();

  final facebookLogin = FacebookLogin();

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

  Color left = Colors.black;
  Color right = Colors.white;

  bool _pwdVisible = false;
  bool _isLoading = false;

  @override
  void initState() {
    super.initState();
    _focusPassword.addListener(_txtPasswordFocusChanges);

    SharedPreferences.getInstance().then((prefs) {
      var username = prefs.getString(SettingKeys.lastUsername);
      if (username != null && username.isNotEmpty) {
        _txtUsername.text = username;
      }
      if (autoLogin) {
        _initByTokenAsync(prefs).then((res) {
          _setLoading(false);
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
    super.dispose();
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
      analytics.logLogin();
      Route r = MaterialPageRoute(
        builder: (context) => DashboardPage(
          analytics: this.analytics,
          observer: this.observer,
        ),
      );
      await Navigator.pushReplacement(context, r);
      return;
    }
    _setLoading(false);
  }

  Future<bool> _checkLogin(MActionResult<User> res) async {
    if (res == null) {
      _showError("Vyskytla se nespecifikovaná chyba");
    }

    String msg = res.getStatusMessage();

    if (res.status == StatusCode.NotFound ||
        res.status == StatusCode.WrongPassword) {
      msg = "Neplatné jméno nebo heslo";
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
    AppCenterAnalytics.trackEvent(
        "Login", {'user': '${user.fullName}', 'userID': '${user.id}'});

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
        _showSnackbar("Akce zrušena uživatelem");
        return;
      case FacebookLoginStatus.error:
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
      var auth = await account.authentication;
      token = auth.accessToken;
    } catch (error) {
      _showError(error.toString());
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
    PublicConnector pc = new PublicConnector();
    var status = pc.checkStatusAsync();
    var res = await status.timeout(Duration(seconds: 7), onTimeout: () {
      var back = new WebStatus();
      back.status = PageStatus.Unfunctional;
      back.message = "Vypršel časový limit pro přihlášení";
      back.content = "AAA";
      return back;
    });

    var title = "Server je nepřístupný";
    if (res == null) {
      _showSnackbar("Nepodařilo se zjistit stav serverů.");
      return false;
    }

    if (!res.canAccess) {
      if (res.status == PageStatus.Limited) {
        title = "Omezený přístup";
      } else if (res.status == PageStatus.Unfunctional) {
        if (res.content != null &&
            res.content == "AAA" &&
            res.message != null &&
            res.message.isNotEmpty) {
          _showError(res.message);
          return false;
        }
      } else if (res.status == PageStatus.NotAvailable) {
        title = "Probíhá údržba";
        if (res.content != null && res.content is Unavailability) {
          if (Global.isInDebugMode) return true;
          Unavailability unv = res.content;
          _showSnackbar("Důvod: ${unv.reason} \nPřepokládaný konec: ${unv.to}");
          return false;
        }
      }
      if (res.message == null) {
        _showSnackbar("Zkuste to prosím později.");
        return false;
      }
      _showDialog(title, res.message);
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
      body: NotificationListener<OverscrollIndicatorNotification>(
        onNotification: (overscroll) {
          overscroll.disallowGlow();
          return true;
        },
        child: SingleChildScrollView(
          child: Container(
            width: MediaQuery.of(context).size.width,
            height: MediaQuery.of(context).size.height >= 775.0
                ? MediaQuery.of(context).size.height
                : 775.0,
            decoration: BoxDecoration(
                gradient: LinearGradient(
              colors: [
                Color(0xFF2196f3),
                Color(0xFF40c4ff),
              ],
              begin: const FractionalOffset(0.2, 0.2),
              end: const FractionalOffset(1, 1),
              stops: [0, 1],
              tileMode: TileMode.clamp,
            )),
            child: _isLoading
                ? _loading(context)
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
                          image:
                              AssetImage("assets/ZolikyHeaderWhite_1024.png"),
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
                                DateTime.now().year.toString() +
                                    " \u00a9 Žolíky",
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
      ),
    );
  }

  Widget _loginOrCreate(BuildContext context) {
    return Container(
      width: 300,
      height: 50,
      decoration: BoxDecoration(
          color: Color(0x552B2B2B),
          borderRadius: BorderRadius.all(Radius.circular(25))),
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
                color: Colors.white,
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
                            color: Colors.black,
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
                                  color: Colors.black,
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
                                color: Colors.black,
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
                  borderRadius: BorderRadius.all(Radius.circular(5)),
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
                    fontSize: 16),
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
                    decoration: new BoxDecoration(
                      shape: BoxShape.circle,
                      color: Colors.white,
                    ),
                    child: new Icon(
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
                    decoration: new BoxDecoration(
                      shape: BoxShape.circle,
                      color: Colors.white,
                    ),
                    child: new Icon(
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
                            color: Colors.black,
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
                            color: Colors.black,
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
                            color: Colors.black,
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
                  borderRadius: BorderRadius.all(Radius.circular(5)),
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

  Widget _loading(BuildContext context) {
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
