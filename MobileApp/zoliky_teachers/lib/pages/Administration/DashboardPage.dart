import 'package:firebase_analytics/firebase_analytics.dart';
import 'package:firebase_analytics/observer.dart';
import 'package:flutter/material.dart';
import 'package:zoliky_teachers/components/Main/Dashboard.dart';
import 'package:zoliky_teachers/utils/enums/Pages.dart';

class DashboardPage extends StatefulWidget {
  DashboardPage({Key key, this.analytics, this.observer, this.changePage})
      : super(key: key);

  final FirebaseAnalytics analytics;
  final FirebaseAnalyticsObserver observer;
  final void Function(Pages page, {String title}) changePage;

  @override
  DashboardPageState createState() =>
      DashboardPageState(analytics, observer, changePage);
}
