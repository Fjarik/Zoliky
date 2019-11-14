import 'package:firebase_analytics/firebase_analytics.dart';
import 'package:firebase_analytics/observer.dart';
import 'package:flutter/material.dart';
import 'package:zoliky_teachers/components/Account/Login.dart';

class LoginPage extends StatefulWidget {
  LoginPage({Key key, this.analytics, this.observer, this.autoLogin = true})
      : super(key: key);

  final FirebaseAnalytics analytics;
  final FirebaseAnalyticsObserver observer;
  final bool autoLogin;

  @override
  LoginPageState createState() =>
      LoginPageState(analytics, observer, autoLogin);
}
