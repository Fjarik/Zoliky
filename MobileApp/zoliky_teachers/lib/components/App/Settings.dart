import 'package:firebase_analytics/firebase_analytics.dart';
import 'package:firebase_analytics/observer.dart';
import 'package:flutter/material.dart';
import 'package:package_info/package_info.dart';
import 'package:url_launcher/url_launcher.dart';
import 'package:zoliky_teachers/pages/App/SettingsPage.dart';
import 'package:zoliky_teachers/utils/Global.dart';

class SettingsPageState extends State<SettingsPage> {
  SettingsPageState(this.analytics, this.observer);

  final FirebaseAnalytics analytics;
  final FirebaseAnalyticsObserver observer;

  @override
  void initState() {
    super.initState();
  }

  String get version => info == null ? "1.0.0" : info.version;
  PackageInfo info;

  @override
  Widget build(BuildContext context) {
    return Stack(
      children: <Widget>[
        Container(
          child: Column(
            children: <Widget>[
              _general(),
              _about(),
              Row(
                mainAxisAlignment: MainAxisAlignment.center,
                crossAxisAlignment: CrossAxisAlignment.center,
                children: <Widget>[
                  Padding(
                    padding: EdgeInsets.symmetric(vertical: 30),
                    child:
                        Text(DateTime.now().year.toString() + " \u00a9 Žolíky"),
                  ),
                ],
              )
            ],
          ),
        ),
      ],
    );
  }

  Widget _general() {
    return Container(
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: <Widget>[
          Global.title("Obecné"),
          Container(
            padding: EdgeInsets.symmetric(horizontal: 5),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: <Widget>[
                Text("Bude implementováno"),
              ],
            ),
          ),
        ],
      ),
    );
  }

  Widget _about() {
    return Container(
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: <Widget>[
          Global.title(
            "O aplikaci",
            topPadding: 15,
          ),
          Container(
            padding: EdgeInsets.symmetric(horizontal: 5),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: <Widget>[
                Padding(
                  padding: EdgeInsets.symmetric(vertical: 10),
                  child: Text(
                    "Vývojář:",
                    style: Theme.of(context).textTheme.title,
                  ),
                ),
                Padding(
                  padding: EdgeInsets.symmetric(horizontal: 5),
                  child: Text("Jiří Falta"),
                ),
                Padding(
                  padding: EdgeInsets.symmetric(vertical: 10),
                  child: Text(
                    "Aktuální verze:",
                    style: Theme.of(context).textTheme.title,
                  ),
                ),
                Padding(
                  padding: EdgeInsets.symmetric(horizontal: 5),
                  child: FutureBuilder(
                    future: PackageInfo.fromPlatform(),
                    builder: (BuildContext ctx,
                        AsyncSnapshot<PackageInfo> snapshot) {
                      if (snapshot.connectionState != ConnectionState.done) {
                        Text(version);
                      }
                      if (snapshot.data != null) {
                        info = snapshot.data;
                      }
                      return Text(version);
                    },
                  ),
                ),
                Padding(
                  padding: EdgeInsets.symmetric(vertical: 10),
                  child: Text(
                    "Kontakt:",
                    style: Theme.of(context).textTheme.title,
                  ),
                ),
                Padding(
                  padding: EdgeInsets.symmetric(horizontal: 5),
                  child: InkWell(
                    child: Text(
                      "podpora@zoliky.eu",
                      style: TextStyle(
                        decoration: TextDecoration.underline,
                        color: Colors.blue,
                      ),
                    ),
                    onTap: () async {
                      var url = "mailto:podpora@zoliky.eu";
                      if (await canLaunch(url)) {
                        await launch(url);
                      }
                    },
                  ),
                ),
                Padding(
                  padding: EdgeInsets.symmetric(vertical: 10),
                  child: Text(
                    "Závěřečná ustanovení:",
                    style: Theme.of(context).textTheme.title,
                  ),
                ),
                Padding(
                  padding: EdgeInsets.symmetric(horizontal: 5),
                  child: Text(
                      "Projekt byl vytvořen v době studia na Střední škole informatiky a ekonomie - DELTA, Pardubice. Vzhledem k povaze projektu je přísně zakázano jeho používání za účelem vydělávání peněz."),
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }
}
