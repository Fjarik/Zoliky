import 'package:firebase_analytics/firebase_analytics.dart';
import 'package:firebase_analytics/observer.dart';
import 'package:flutter/material.dart';
import 'package:zoliky_teachers/components/App/Settings.dart';

class SettingsPage extends StatefulWidget {
  SettingsPage({Key key, this.analytics, this.observer}) : super(key: key);

  final FirebaseAnalytics analytics;
  final FirebaseAnalyticsObserver observer;

  @override
  SettingsPageState createState() => SettingsPageState(analytics, observer);
}
