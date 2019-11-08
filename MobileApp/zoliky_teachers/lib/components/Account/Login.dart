import 'dart:developer';

import 'package:firebase_analytics/firebase_analytics.dart';
import 'package:firebase_analytics/observer.dart';
import 'package:flutter/material.dart';
import 'package:font_awesome_flutter/font_awesome_flutter.dart';
import 'package:url_launcher/url_launcher.dart';
import 'package:zoliky_teachers/pages/Account/LoginPage.dart';
import 'package:zoliky_teachers/utils/MenuPainter.dart';

class LoginPageState extends State<LoginPage>
    with SingleTickerProviderStateMixin {
  LoginPageState(this.analytics, this.observer);

  final FirebaseAnalytics analytics;
  final FirebaseAnalyticsObserver observer;

  static String _registerUrl = "https://www.zoliky.eu/Account/Register";
  static String _forgotPwdUrl = "https://www.zoliky.eu/Account/ForgotPassword";

  final GlobalKey<ScaffoldState> _key = GlobalKey<ScaffoldState>();

  TextEditingController _txtUsername = TextEditingController();
  TextEditingController _txtPassword = TextEditingController();

  FocusNode _focusUsername = FocusNode();
  FocusNode _focusPassword = FocusNode();

  PageController _pageController = PageController();

  Color left = Colors.black;
  Color right = Colors.white;

  bool _pwdVisible = false;
  bool _isLoading = false;

  @override
  void initState() {
    super.initState();
    _focusPassword.addListener(_txtPasswordFocusChanges);
    analytics.logEvent(
      name: "Test",
      parameters: <String, dynamic>{
        "Test": "Test",
      },
    );
  }

  @override
  void dispose() {
    _txtUsername?.dispose();
    _txtPassword?.dispose();
    _focusUsername?.dispose();
    _focusPassword.dispose();
    _pageController?.dispose();
    super.dispose();
  }

  void _login() {
    setState(() {
      _isLoading = true;
    });

    log("Test");

    setState(() {
      _isLoading = false;
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

  Future _launchURL(String url) async {
    if (await canLaunch(url)) {
      await launch(url);
      return;
    }
    // TODO: Info
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
            child: Column(
              mainAxisSize: MainAxisSize.max,
              children: <Widget>[
                Padding(
                  padding: EdgeInsets.only(
                    top: 100,
                    bottom: 80,
                  ),
                  child: Text(
                    "Žolíky",
                    style: TextStyle(
                      fontSize: 40,
                      color: Colors.white,
                    ),
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
                    onPageChanged: (i) {
                      if (i == 0) {
                        setState(() {
                          left = Colors.black;
                          right = Colors.white;
                        });
                      } else {
                        setState(() {
                          left = Colors.white;
                          right = Colors.black;
                        });
                      }
                      FocusScope.of(context).unfocus();
                    },
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
                  onPressed: () => {this._login()},
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
                  onTap: () {
                    // TODO: Facebook login
                    log("Facebook");
                  },
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
                  onTap: () {
                    // TODO: Google login
                    log("Google");
                  },
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
                          // controller: this._txtUsername,
                          // focusNode: this._focusUsername,
                          autocorrect: false,
                          autofocus: false,
                          maxLines: 1,
                          keyboardType: TextInputType.emailAddress,
                          textInputAction: TextInputAction.next,
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
                          // controller: this._txtUsername,
                          // focusNode: this._focusUsername,
                          autocorrect: false,
                          autofocus: false,
                          maxLines: 1,
                          textInputAction: TextInputAction.next,
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
                          // controller: this._txtUsername,
                          // focusNode: this._focusUsername,
                          autocorrect: false,
                          autofocus: false,
                          maxLines: 1,
                          textInputAction: TextInputAction.done,
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
                  onPressed: () => {
                    // TODO: Register
                  },
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
}
