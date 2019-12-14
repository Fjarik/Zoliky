import "package:firebase_analytics/firebase_analytics.dart";
import "package:firebase_analytics/observer.dart";
import "package:flutter/material.dart";
import 'package:provider/provider.dart';
import 'package:shared_preferences/shared_preferences.dart';
import "package:wakelock/wakelock.dart";
import "package:zoliky_teachers/pages/Account/LoginPage.dart";
import "package:zoliky_teachers/pages/Administration/DashboardPage.dart";
import "package:zoliky_teachers/utils/Global.dart";
import 'package:zoliky_teachers/utils/SettingKeys.dart';
import 'package:zoliky_teachers/utils/Singleton.dart';
import 'package:zoliky_teachers/utils/ThemeChanger.dart';

class ZolikApp extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return FutureBuilder(
      future: SharedPreferences.getInstance(),
      builder: (BuildContext ctx, AsyncSnapshot<SharedPreferences> snapshot) {
        if (snapshot.connectionState != ConnectionState.done) {
          return Container();
        }
        var theme = ThemeData.light();
        if (snapshot.data != null) {
          var settings = snapshot.data;
          if (settings.containsKey(SettingKeys.darkmode) &&
              settings.getBool(SettingKeys.darkmode)) {
            Singleton().darkmode = true;
            theme = ThemeData.dark();
          }
        }
        return ChangeNotifierProvider<ThemeChanger>(
          create: (BuildContext ctx) => ThemeChanger(theme),
          child: ZolikAppWithTheme(),
        );
      },
    );
  }
}

class ZolikAppWithTheme extends StatefulWidget {
  @override
  ZolikAppWithThemeState createState() => ZolikAppWithThemeState();
}

class ZolikAppWithThemeState extends State<ZolikAppWithTheme> {
  static String appSecret = "e890654b-e9d0-465d-b53c-88e31a42e7e3";
  static FirebaseAnalytics analytics = FirebaseAnalytics();
  static FirebaseAnalyticsObserver observer =
      FirebaseAnalyticsObserver(analytics: analytics);

  Widget _default;

  @override
  void initState() {
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
    final theme = Provider.of<ThemeChanger>(context);

    return MaterialApp(
      title: "Žolíky",
      theme: theme.getTheme(),
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
    );
  }
}
