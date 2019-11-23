import "package:appcenter/appcenter.dart";
import "package:appcenter_analytics/appcenter_analytics.dart";
import "package:appcenter_crashes/appcenter_crashes.dart";
import "package:firebase_analytics/firebase_analytics.dart";
import "package:firebase_analytics/observer.dart";
import "package:flutter/material.dart";
import "package:scoped_model/scoped_model.dart";
import "package:wakelock/wakelock.dart";
import "package:zoliky_teachers/pages/Account/LoginPage.dart";
import "package:zoliky_teachers/pages/Administration/DashboardPage.dart";
import "package:zoliky_teachers/utils/Global.dart";

import "models/MainModel.dart";

final ThemeData _mainTheme = ThemeData(
  primarySwatch: Colors.blue,
);

class ZolikApp extends StatefulWidget {
  @override
  ZolikAppState createState() => ZolikAppState();
}

class ZolikAppState extends State<ZolikApp> {
  static String appSecret = "e890654b-e9d0-465d-b53c-88e31a42e7e3";
  static FirebaseAnalytics analytics = FirebaseAnalytics();
  static FirebaseAnalyticsObserver observer =
      FirebaseAnalyticsObserver(analytics: analytics);

  final MainModel _model = MainModel();

  Widget _default;

  @override
  void initState() {
    AppCenter.start(appSecret, [AppCenterAnalytics.id, AppCenterCrashes.id]);
    _default = LoginPage(
      analytics: analytics,
      observer: observer,
    );

    if (Global.isInDebugMode) {
      Wakelock.enable();
    }

    super.initState();
  }

  @override
  Widget build(BuildContext context) {
    return ScopedModel<MainModel>(
      model: _model,
      child: MaterialApp(
        title: "Žolíky",
        theme: _mainTheme,
        home: _default,
        navigatorObservers: <NavigatorObserver>[observer],
        routes: <String, WidgetBuilder>{
          "/login": (context) => LoginPage(
                analytics: analytics,
                observer: observer,
              ),
          "/dashboard": (context) => DashboardPage(
                analytics: analytics,
                observer: observer,
              ),
        },
        debugShowCheckedModeBanner: false,
      ),
    );
  }
}
