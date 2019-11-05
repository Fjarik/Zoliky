import 'dart:developer';

import 'package:flutter/material.dart';
import 'package:zoliky_teachers/pages/Account/LoginPage.dart';
import 'package:zoliky_teachers/utils/MenuPainter.dart';

abstract class ILogin {
  TextEditingController txtUsername;
  TextEditingController txtPassword;
  bool isLoading;

  void login();
}

class LoginPageState extends State<LoginPage>
    with SingleTickerProviderStateMixin
    implements ILogin {
  TextEditingController txtUsername = TextEditingController();
  TextEditingController txtPassword = TextEditingController();
  final PageController _pageController = PageController();

  bool isLoading = false;

  @override
  void initState() {
    super.initState();
  }

  @override
  void dispose() {
    _pageController?.dispose();
    super.dispose();
  }

  @override
  void login() {
    log("Test");
  }

  void _onSignInButtonPress() {
    _pageController.animateToPage(0,
        duration: Duration(milliseconds: 500), curve: Curves.decelerate);
  }

  void _onSignUpButtonPress() {
    _pageController?.animateToPage(1,
        duration: Duration(milliseconds: 500), curve: Curves.decelerate);
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      key: GlobalKey<ScaffoldState>(),
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
                Color(0xFFfbab66),
                Color(0xFFf7418c),
              ],
              stops: [0, 1],
              tileMode: TileMode.clamp,
            )),
            child: Column(
              mainAxisSize: MainAxisSize.max,
              children: <Widget>[
                Padding(
                  child: Text("Test"),
                  padding: EdgeInsets.only(top: 75),
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
                      setState(() {});
                      log(i.toString());
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
          borderRadius: BorderRadius.all(Radius.circular(0.25))),
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
                    color: Colors.black,
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
                  "Nový účet",
                  style: TextStyle(
                    color: Colors.white,
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
      child: Column(
        children: <Widget>[
          Stack(
            children: <Widget>[
              Card(
                elevation: 2,
                color: Colors.white,
                child: Container(
                  child: Column(
                    children: <Widget>[
                      TextField(
                        controller: this.txtUsername,
                        autocorrect: false,
                        autofocus: false,
                        keyboardType: TextInputType.emailAddress,
                        textInputAction: TextInputAction.next,
                      ),
                      TextField(
                        controller: this.txtPassword,
                        autocorrect: false,
                        autofocus: false,
                        obscureText: true,
                        textInputAction: TextInputAction.done,
                      ),
                      RaisedButton(
                        child: Text("Přihlásit se"),
                        onPressed: () => {this.login()},
                      ),
                    ],
                  ),
                ),
              )
            ],
          )
        ],
      ),
    );
  }

  Widget _signUp(BuildContext context) {
    return Container(
      child: Column(
        children: <Widget>[
          Stack(
            children: <Widget>[
              Card(
                child: Container(
                  child: Column(
                    children: <Widget>[
                      TextField(
                        autocorrect: false,
                        autofocus: false,
                        keyboardType: TextInputType.emailAddress,
                        textInputAction: TextInputAction.next,
                        decoration: InputDecoration(
                          labelText: "Test",
                          filled: false,
                        ),
                      ),
                      TextField(
                        autocorrect: false,
                        autofocus: false,
                        obscureText: true,
                        textInputAction: TextInputAction.done,
                        decoration: InputDecoration(
                          labelText: "Test",
                        ),
                      ),
                      RaisedButton(
                        child: Text("Přihlásit se"),
                        onPressed: () => {this.login()},
                      ),
                    ],
                  ),
                ),
              )
            ],
          )
        ],
      ),
    );
  }
}
