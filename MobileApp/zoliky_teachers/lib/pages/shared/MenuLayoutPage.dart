import 'package:firebase_analytics/firebase_analytics.dart';
import 'package:firebase_analytics/observer.dart';
import 'package:flutter/material.dart';
import 'package:zoliky_teachers/components/shared/MenuLayout.dart';

class MenuLayoutPage extends StatefulWidget {
  MenuLayoutPage({Key key, this.analytics, this.observer}) : super(key: key);

  final FirebaseAnalytics analytics;
  final FirebaseAnalyticsObserver observer;

  @override
  MenuLayoutState createState() => MenuLayoutState(analytics, observer);
}
