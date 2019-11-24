import 'package:firebase_analytics/firebase_analytics.dart';
import 'package:firebase_analytics/observer.dart';
import 'package:flutter/material.dart';
import 'package:zoliky_teachers/components/Zolik/ZolikCreate.dart';

class ZolikCreatePage extends StatefulWidget {
  ZolikCreatePage({Key key, this.analytics, this.observer})
      : super(key: key);

  final FirebaseAnalytics analytics;
  final FirebaseAnalyticsObserver observer;

  @override
  ZolikCreatePageState createState() =>
      ZolikCreatePageState(analytics, observer);
}
