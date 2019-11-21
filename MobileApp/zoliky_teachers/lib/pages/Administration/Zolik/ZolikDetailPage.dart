import 'package:firebase_analytics/firebase_analytics.dart';
import 'package:firebase_analytics/observer.dart';
import 'package:flutter/material.dart';
import 'package:zoliky_teachers/components/Admin/ZolikDetail.dart';
import 'package:zoliky_teachers/utils/api/models/Zolik.dart';

class ZolikDetailPage extends StatefulWidget {
  ZolikDetailPage({Key key, this.analytics, this.observer, this.zolik})
      : super(key: key);

  final FirebaseAnalytics analytics;
  final FirebaseAnalyticsObserver observer;

  final Zolik zolik;

  @override
  ZolikDetailState createState() => ZolikDetailState(zolik);
}
