import 'dart:developer';

import 'package:flutter/material.dart';
import 'package:zoliky_teachers/pages/Account/LoginPage.dart';

abstract class ILogin {
  TextEditingController txtUsername;
  TextEditingController txtPassword;
  bool isLoading;

  void login();
}

class LoginPageState extends State<LoginPage> implements ILogin {
  TextEditingController txtUsername = TextEditingController();
  TextEditingController txtPassword = TextEditingController();

  bool isLoading = false;

  @override
  void login() {
    log("Test");
  }

  @override
  Widget build(BuildContext context) {
    return loginWidget(context);
  }

  Widget loginWidget(BuildContext context) {
    return Scaffold(
      body: Center(
        child: Column(
          children: <Widget>[
            TextField(
              controller: this.txtUsername,
              autocorrect: false,
              autofocus: false,
              keyboardType: TextInputType.emailAddress,
              textInputAction: TextInputAction.next,
              decoration: InputDecoration(
                labelText: "Přihlašovací jméno",
                filled: false,
              ),
            ),
            TextField(
              controller: this.txtPassword,
              autocorrect: false,
              autofocus: false,
              obscureText: true,
              textInputAction: TextInputAction.done,
              decoration: InputDecoration(
                labelText: "Zadejte jméno",
              ),
            ),
            RaisedButton(
              child: Text("Přihlásit se"),
              onPressed: () => {this.login()},
            ),
          ],
        ),
      ),
    );
  }
}
