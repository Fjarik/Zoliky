import 'package:firebase_analytics/firebase_analytics.dart';
import 'package:firebase_analytics/observer.dart';
import 'package:flutter/material.dart';
import 'package:zoliky_teachers/components/Student/Students.dart';

class StudentsPage extends StatefulWidget {
  StudentsPage({Key key, this.analytics, this.observer}) : super(key: key);

  final FirebaseAnalytics analytics;
  final FirebaseAnalyticsObserver observer;

  @override
  StudentsPageState createState() => StudentsPageState(analytics, observer);
}
