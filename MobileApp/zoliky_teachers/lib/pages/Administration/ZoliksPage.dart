import 'package:firebase_analytics/firebase_analytics.dart';
import 'package:firebase_analytics/observer.dart';
import 'package:flutter/material.dart';
import 'package:zoliky_teachers/components/Admin/Zoliks.dart';

class ZoliksPage extends StatefulWidget {
  ZoliksPage({Key key, this.analytics, this.observer}) : super(key: key);

  final FirebaseAnalytics analytics;
  final FirebaseAnalyticsObserver observer;

  @override
  ZoliksPageState createState() => ZoliksPageState(analytics, observer);
}
