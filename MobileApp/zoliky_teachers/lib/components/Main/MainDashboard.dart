import 'package:firebase_analytics/firebase_analytics.dart';
import 'package:firebase_analytics/observer.dart';
import 'package:flutter/material.dart';
import 'package:flutter_staggered_grid_view/flutter_staggered_grid_view.dart';
import 'package:font_awesome_flutter/font_awesome_flutter.dart';
import 'package:shared_preferences/shared_preferences.dart';
import 'package:zoliky_teachers/pages/Account/LoginPage.dart';
import 'package:zoliky_teachers/pages/Administration/DashboardPage.dart';
import 'package:zoliky_teachers/utils/ProjectSettingKeys.dart';
import 'package:zoliky_teachers/utils/SettingKeys.dart';
import 'package:zoliky_teachers/utils/Singleton.dart';
import 'package:zoliky_teachers/utils/api/connectors/ProjectSettingsConnector.dart';
import 'package:zoliky_teachers/utils/api/connectors/StatisticsConnector.dart';
import 'package:zoliky_teachers/utils/api/models/ProjectSettings.dart';
import 'package:zoliky_teachers/utils/api/models/User.dart';
import 'package:charts_flutter/flutter.dart' as charts;

class DashboardPageState extends State<DashboardPage> {
  DashboardPageState(this.analytics, this.observer);

  final FirebaseAnalytics analytics;
  final FirebaseAnalyticsObserver observer;

  final GlobalKey<ScaffoldState> _key = GlobalKey<ScaffoldState>();

  User get _logged => Singleton().user;

  _logOut() async {
    var prefs = await SharedPreferences.getInstance();
    await prefs.remove(SettingKeys.lastToken);
    Route r = MaterialPageRoute(
        builder: (context) => LoginPage(
              analytics: this.analytics,
              observer: this.observer,
              autoLogin: false,
            ));
    await Navigator.pushReplacement(context, r);
  }

  @override
  void initState() {
    super.initState();
  }

  ProjectSettingsConnector get _pConnector =>
      ProjectSettingsConnector(Singleton().token);

  StatisticsConnector get _sConnector => StatisticsConnector(Singleton().token);

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      key: _key,
      appBar: _getAppBar(),
      body: Stack(
        children: <Widget>[
          Padding(
            padding: EdgeInsets.only(top: 10),
            child: AnimatedOpacity(
              opacity: 1,
              duration: Duration(milliseconds: 170),
              child: StaggeredGridView.count(
                crossAxisCount: 4,
                crossAxisSpacing: 12,
                mainAxisSpacing: 12,
                padding: EdgeInsets.symmetric(horizontal: 16, vertical: 8),
                staggeredTiles: [
                  StaggeredTile.extent(2, 180),
                  StaggeredTile.extent(2, 180),
                  StaggeredTile.extent(4, 180),
                  StaggeredTile.extent(3, 180),
                ],
                children: <Widget>[
                  FutureBuilder(
                    future: _sConnector.getZolikCountAsync(),
                    builder: (context, AsyncSnapshot<int> snapshot) {
                      if (snapshot.connectionState != ConnectionState.done) {
                        return _getLoadingTile();
                      }
                      var count = "0";
                      if (snapshot.data != null) {
                        count = snapshot.data.toString();
                      }
                      if (snapshot.hasError) {
                        count = "-1";
                      }
                      return _getIconTile(
                        content: count,
                        title: "Počet žolíků",
                        subtitle: "Vč. žolíků, jokérů...",
                        color: Colors.blue,
                        icon: FontAwesomeIcons.wallet,
                        iconColor: Colors.white,
                        onTap: null,
                      );
                    },
                  ),
                  FutureBuilder(
                    future: _sConnector.getStudentCountAsync(),
                    builder: (context, AsyncSnapshot<int> snapshot) {
                      if (snapshot.connectionState != ConnectionState.done) {
                        return _getLoadingTile();
                      }
                      var count = "0";
                      if (snapshot.data != null) {
                        count = snapshot.data.toString();
                      }
                      if (snapshot.hasError) {
                        count = "-1";
                      }
                      return _getIconTile(
                        content: count,
                        title: "Počet studentů",
                        subtitle: "Na celé škole",
                        color: Colors.amber,
                        icon: FontAwesomeIcons.solidUser,
                        iconColor: Colors.white,
                        onTap: null,
                      );
                    },
                  ),
                  FutureBuilder(
                    future: _pConnector.getAllAsync(),
                    builder: (context,
                        AsyncSnapshot<List<ProjectSettings>> snapshot) {
                      if (snapshot.connectionState != ConnectionState.done) {
                        return _getLoadingTile();
                      }
                      var title = "Testovací událost";
                      var date = "ERROR";
                      var icon = FontAwesomeIcons.calendarTimes;

                      if (snapshot.hasError) {
                        title = snapshot.error.toString();
                      }

                      var res = snapshot.data;
                      if (res == null || res.isEmpty) {
                        title = "Nevrácena žádná data";
                      } else {
                        var titleSetting = res.firstWhere(
                            (x) => x.key == ProjectSettingKeys.specialText);
                        if (titleSetting != null) {
                          title = titleSetting.value;
                        }
                        var dateSetting = res.firstWhere(
                            (x) => x.key == ProjectSettingKeys.specialDate);
                        if (dateSetting != null &&
                            dateSetting.value.isNotEmpty) {
                          var value = dateSetting.value;
                          date = value.substring(0, value.length - 8);
                        }
                        icon = FontAwesomeIcons.calendarAlt;
                      }

                      return _getIconTile(
                        content: date,
                        title: title,
                        subtitle: "Nejbližší událost",
                        color: Colors.red,
                        icon: icon,
                        iconColor: Colors.white,
                        onTap: null,
                      );
                    },
                  ),
                  FutureBuilder(
                    future: _sConnector.getTeacherCountAsync(),
                    builder: (context, AsyncSnapshot<int> snapshot) {
                      if (snapshot.connectionState != ConnectionState.done) {
                        return _getLoadingTile();
                      }
                      var count = "0";
                      if (snapshot.data != null) {
                        count = snapshot.data.toString();
                      }
                      if (snapshot.hasError) {
                        count = "-1";
                      }
                      return _getIconTile(
                        content: count,
                        title: "Počet vyučujících",
                        subtitle: "Na škole",
                        color: Colors.teal,
                        icon: Icons.school,
                        iconColor: Colors.white,
                        onTap: null,
                      );
                    },
                  ),
                  Padding(
                    padding: EdgeInsets.all(24),
                    child: Column(
                      mainAxisAlignment: MainAxisAlignment.start,
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: <Widget>[
                        // charts.PieChart([
                        //   new charts.Series(
                        //     id: "Žolíky",
                        //     data: [
                        //       10,
                        //       5,
                        //     ],
                        //   ),
                        // ]),
                      ],
                    ),
                  ),
                ],
              ),
            ),
          ),
        ],
      ),
      bottomNavigationBar: BottomNavigationBar(
        type: BottomNavigationBarType.fixed,
        items: [
          BottomNavigationBarItem(
            title: Text("Přehled"),
            icon: Icon(
              Icons.dashboard,
            ),
          ),
          BottomNavigationBarItem(
            title: Text("Nastavení"),
            icon: Icon(
              Icons.settings,
            ),
          ),
        ],
      ),
    );
  }

  Widget _getTile(Widget child, {Function() onTap}) {
    return Material(
      elevation: 14,
      borderRadius: BorderRadius.circular(12),
      shadowColor: Color(0x802196F3),
      child: InkWell(
        onTap: onTap ?? null,
        child: child,
      ),
    );
  }

  Widget _getLoadingTile() {
    return _getTile(Column(
      crossAxisAlignment: CrossAxisAlignment.center,
      mainAxisAlignment: MainAxisAlignment.center,
      children: <Widget>[
        SizedBox(
          width: 80,
          height: 80,
          child: CircularProgressIndicator(),
        ),
      ],
    ));
  }

  Widget _getIconTile(
      {@required String content,
      @required String title,
      String subtitle,
      Color color,
      Color iconColor,
      double iconSize = 30,
      IconData icon,
      Function() onTap}) {
    return _getTile(
      Padding(
        padding: EdgeInsets.all(24),
        child: Column(
          mainAxisAlignment: MainAxisAlignment.start,
          crossAxisAlignment: CrossAxisAlignment.start,
          children: <Widget>[
            Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: <Widget>[
                Material(
                  color: color,
                  shape: CircleBorder(),
                  child: Padding(
                    padding: EdgeInsets.all(16),
                    child: Icon(
                      icon,
                      color: iconColor,
                      size: iconSize,
                    ),
                  ),
                ),
                Padding(
                  padding: EdgeInsets.only(right: 15),
                  child: Text(
                    content,
                    style: TextStyle(
                      color: color,
                      fontSize: 30,
                    ),
                  ),
                ),
              ],
            ),
            Padding(
              padding: EdgeInsets.only(bottom: 16),
            ),
            Expanded(
              child: Text(
                title,
                maxLines: 1,
                overflow: TextOverflow.ellipsis,
                softWrap: false,
                style: TextStyle(
                  color: Colors.black,
                  fontWeight: FontWeight.w700,
                  fontSize: 17,
                ),
              ),
            ),
            subtitle == null
                ? Column()
                : Expanded(
                    child: Text(
                      subtitle ?? "",
                      style: TextStyle(color: Colors.black45, fontSize: 12),
                    ),
                  ),
          ],
        ),
      ),
      onTap: null,
    );
  }

  Widget _getAppBar() {
    return AppBar(
      elevation: 2,
      backgroundColor: Colors.white,
      title: Text(
        "Přehled",
        style: TextStyle(
          color: Colors.black,
          fontWeight: FontWeight.w700,
          fontSize: 30.0,
        ),
      ),
      actions: <Widget>[
        Tooltip(
          message: "Odhlásit se",
          child: IconButton(
            onPressed: _logOut,
            icon: Icon(
              Icons.exit_to_app,
              color: Colors.black,
            ),
            color: Colors.black,
          ),
        ),
      ],
    );
  }

  Widget _oldAppBar() {
    return Positioned(
      child: Container(
        decoration: BoxDecoration(
          borderRadius: BorderRadius.only(
            bottomLeft: Radius.circular(16),
            bottomRight: Radius.circular(16),
          ),
          gradient: LinearGradient(
            colors: [
              Colors.blueAccent,
              Colors.lightBlueAccent,
            ],
          ),
        ),
        height: 85,
        child: Column(
          mainAxisAlignment: MainAxisAlignment.start,
          children: <Widget>[
            Padding(
              padding: EdgeInsets.all(10),
              child: Row(
                mainAxisAlignment: MainAxisAlignment.spaceAround,
                children: <Widget>[
                  Expanded(
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: <Widget>[
                        Text(
                          _logged.fullName,
                          maxLines: 1,
                          overflow: TextOverflow.ellipsis,
                          style: TextStyle(
                            fontWeight: FontWeight.w700,
                            fontSize: 20,
                            color: Colors.white,
                          ),
                        ),
                        Text(
                          _logged.schoolName,
                          maxLines: 1,
                          overflow: TextOverflow.ellipsis,
                          style: TextStyle(
                            fontWeight: FontWeight.w600,
                            fontSize: 17,
                            color: Colors.white,
                          ),
                        ),
                      ],
                    ),
                  ),
                  Tooltip(
                    message: "Odhlásit se",
                    child: IconButton(
                      onPressed: _logOut,
                      icon: Icon(
                        Icons.exit_to_app,
                        color: Colors.white,
                      ),
                      color: Colors.white,
                    ),
                  ),
                ],
              ),
            ),
          ],
        ),
      ),
    );
  }
}
