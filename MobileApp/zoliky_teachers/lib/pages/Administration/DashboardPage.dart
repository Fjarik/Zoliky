import 'package:firebase_analytics/firebase_analytics.dart';
import 'package:firebase_analytics/observer.dart';
import 'package:flutter/material.dart';
import 'package:zoliky_teachers/components/Main/MainDashboard.dart';

class DashboardPage extends StatefulWidget {
  DashboardPage({Key key, this.analytics, this.observer}) : super(key: key);

  final FirebaseAnalytics analytics;
  final FirebaseAnalyticsObserver observer;

  @override
  DashboardPageState createState() => DashboardPageState(analytics, observer);
}
