import 'dart:developer';

import 'package:firebase_analytics/firebase_analytics.dart';
import 'package:firebase_analytics/observer.dart';
import 'package:flutter/material.dart';
import 'package:package_info/package_info.dart';
import 'package:provider/provider.dart';
import 'package:shared_preferences/shared_preferences.dart';
import 'package:url_launcher/url_launcher.dart';
import 'package:zoliky_teachers/pages/App/SettingsPage.dart';
import 'package:zoliky_teachers/utils/Global.dart';
import 'package:local_auth/local_auth.dart';
import 'package:zoliky_teachers/utils/SettingKeys.dart';
import 'package:zoliky_teachers/utils/Singleton.dart';
import 'package:zoliky_teachers/utils/ThemeChanger.dart';

class SettingsPageState extends State<SettingsPage> {
  SettingsPageState(this.analytics, this.observer);

  final FirebaseAnalytics analytics;
  final FirebaseAnalyticsObserver observer;

  @override
  void initState() {
    super.initState();

    darkMode = Singleton().darkmode;
  }

  LocalAuthentication localAuth = LocalAuthentication();
  String get version => info == null ? "1.0.0" : info.version;
  PackageInfo info;

  bool darkMode = false;
  bool biometrics = false;
  bool autoLogin = false;

  Future<bool> _getFuture() async {
    if (canCheck) {
      return canCheck;
    }

    canCheck = await localAuth.canCheckBiometrics;
    availableTypes = await localAuth.getAvailableBiometrics();

    if (canCheck) {
      var settings = await SharedPreferences.getInstance();
      if (settings.containsKey(SettingKeys.biometics) &&
          settings.getBool(SettingKeys.biometics)) {
        setState(() {
          biometrics = true;
        });
      }
      if (settings.containsKey(SettingKeys.autoLogin) &&
          settings.getBool(SettingKeys.autoLogin)) {
        autoLogin = true;
      }
    }

    return canCheck && availableTypes.length > 0;
  }

  bool canCheck = false;
  List<BiometricType> availableTypes = List<BiometricType>();

  Future<void> _bioLogin(bool val) async {
    var settings = await SharedPreferences.getInstance();
    if (!val) {
      settings.setBool(SettingKeys.biometics, val);
      setState(() {
        biometrics = val;
        autoLogin = val ? autoLogin : false;
      });
      return;
    }
    var res = false;
    try {
      res = await localAuth.authenticateWithBiometrics(
        localizedReason: "Ověřte svou identitu",
        stickyAuth: false,
        useErrorDialogs: true,
        sensitiveTransaction: false,
      );
    } catch (ex) {
      log(ex);
    }

    settings.setBool(SettingKeys.biometics, res);
    if (!res) {
      settings.setBool(SettingKeys.autoLogin, false);
    }

    setState(() {
      biometrics = res;
      autoLogin = res ? autoLogin : false;
    });
  }

  Future<void> _autoLogin(bool val) async {
    var settings = await SharedPreferences.getInstance();
    settings.setBool(SettingKeys.autoLogin, val);
    setState(() {
      autoLogin = val;
    });
  }

  Future<void> _changeTheme(bool val) async {
    var _changer = Provider.of<ThemeChanger>(context);
    if (val) {
      _changer.setTheme(ThemeData.dark());
    } else {
      _changer.setTheme(ThemeData.light());
    }
    Singleton().darkmode = val;
    
    var settings = await SharedPreferences.getInstance();
    settings.setBool(SettingKeys.darkmode, val);

    setState(() {
      darkMode = val;
    });
  }

  @override
  Widget build(BuildContext context) {
    return SingleChildScrollView(
      child: Stack(
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
                      child: Text(
                        DateTime.now().year.toString() + " \u00a9 Žolíky",
                      ),
                    ),
                  ],
                )
              ],
            ),
          ),
        ],
      ),
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
                SwitchListTile(
                  value: darkMode,
                  title: Text("Tmavý režim"),
                  subtitle: Text(
                    "Zapnout/Vypnout tmavý řežim aplikace",
                  ),
                  onChanged: _changeTheme,
                ),
                FutureBuilder(
                  future: _getFuture(),
                  builder: (BuildContext ctx, AsyncSnapshot<bool> snapshot) {
                    if (snapshot.connectionState != ConnectionState.done) {
                      return Container();
                    }
                    if (!snapshot.data) {
                      return Padding(
                        padding: EdgeInsets.only(left: 16, top: 16, bottom: 16),
                        child: Text(
                          "Vaše zařízení nepodporuje biometriku :(",
                          style: TextStyle(
                              color: Colors.black54,
                              fontSize: 18,
                              fontWeight: FontWeight.bold),
                        ),
                      );
                    }

                    return SwitchListTile(
                      value: biometrics,
                      title: Text("Biometrika"),
                      subtitle: Text(
                        "Zapnout/Vypnout ověřování přihlašování přes biometriku (Otisk prstu, rozpoznání obličeje...)",
                      ),
                      onChanged: _bioLogin,
                      isThreeLine: true,
                    );
                  },
                ),
                SwitchListTile(
                  value: autoLogin,
                  title: Text("Automatické přihlašování"),
                  isThreeLine: true,
                  subtitle: Text(
                    "Zapnout/Vypnout automatické přihlašování při spuštění aplikace. (Pouze se zapnutou biometrikou)",
                    maxLines: 3,
                  ),
                  onChanged: biometrics ? _autoLogin : null,
                ),
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
            padding: EdgeInsets.symmetric(horizontal: 10),
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
