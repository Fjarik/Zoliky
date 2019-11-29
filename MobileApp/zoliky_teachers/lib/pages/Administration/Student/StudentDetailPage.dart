import 'package:firebase_analytics/firebase_analytics.dart';
import 'package:firebase_analytics/observer.dart';
import 'package:flutter/material.dart';
import 'package:zoliky_teachers/components/Student/StudentDetail.dart';
import 'package:zoliky_teachers/utils/api/models/Student.dart';

class StudentDetailPage extends StatefulWidget {
  StudentDetailPage({Key key, this.analytics, this.observer, this.student})
      : super(key: key);

  final FirebaseAnalytics analytics;
  final FirebaseAnalyticsObserver observer;

  final Student student;

  @override
  StudentDetailPageState createState() => StudentDetailPageState(student);
}
