import 'package:flutter/material.dart';
import 'package:zoliky_teachers/components/Account/Login.dart';
import 'package:zoliky_teachers/models/MainModel.dart';

class LoginPage extends StatefulWidget {
  final MainModel _model;

  LoginPage(this._model);

  @override
  LoginPageState createState() => LoginPageState();
}
