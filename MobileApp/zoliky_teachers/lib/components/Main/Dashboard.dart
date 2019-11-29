import 'package:firebase_analytics/firebase_analytics.dart';
import 'package:firebase_analytics/observer.dart';
import 'package:flutter/material.dart';
import 'package:flutter_staggered_grid_view/flutter_staggered_grid_view.dart';
import 'package:font_awesome_flutter/font_awesome_flutter.dart';
import 'package:pull_to_refresh/pull_to_refresh.dart';
import 'package:zoliky_teachers/pages/Administration/DashboardPage.dart';
import 'package:zoliky_teachers/utils/ProjectSettingKeys.dart';
import 'package:zoliky_teachers/utils/Singleton.dart';
import 'package:zoliky_teachers/utils/api/connectors/ProjectSettingsConnector.dart';
import 'package:zoliky_teachers/utils/api/connectors/StatisticsConnector.dart';
import 'package:zoliky_teachers/utils/api/models/ProjectSettings.dart';
import 'package:charts_flutter/flutter.dart' as charts;
import 'package:zoliky_teachers/utils/api/models/universal/ClassLeaderboardData.dart';
import 'package:zoliky_teachers/utils/api/models/universal/ZolikTypesData.dart';
import 'package:zoliky_teachers/utils/enums/Pages.dart';

class DashboardPageState extends State<DashboardPage> {
  DashboardPageState(this.analytics, this.observer, this.changePage);

  final FirebaseAnalytics analytics;
  final FirebaseAnalyticsObserver observer;

  final RefreshController _refreshController = RefreshController();

  final void Function(Pages page, {String title}) changePage;

  bool showMenuItems = true;

  @override
  void initState() {
    super.initState();
  }

  ProjectSettingsConnector get _pConnector =>
      ProjectSettingsConnector(Singleton().token);

  StatisticsConnector get _sConnector => StatisticsConnector(Singleton().token);

  @override
  Widget build(BuildContext context) {
    return Stack(
      children: <Widget>[
        Padding(
          padding: EdgeInsets.only(top: 10),
          child: SmartRefresher(
            controller: _refreshController,
            enablePullDown: true,
            enablePullUp: false,
            onRefresh: () async {
              await Future.delayed(Duration(milliseconds: 500));
              setState(() {});
              _refreshController.refreshCompleted();
            },
            child: StaggeredGridView.count(
              crossAxisCount: 2,
              crossAxisSpacing: 12,
              mainAxisSpacing: 12,
              padding: EdgeInsets.symmetric(horizontal: 16, vertical: 8),
              staggeredTiles: [
                StaggeredTile.extent(1, 180),
                StaggeredTile.extent(1, 180),
                StaggeredTile.extent(2, 180),
                StaggeredTile.extent(2, 150),
                StaggeredTile.extent(2, 360),
                StaggeredTile.extent(2, 400),
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
                      onTap: () {
                        changePage(Pages.zoliks);
                      },
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
                      onTap: () {
                        changePage(Pages.students);
                      },
                    );
                  },
                ),
                FutureBuilder(
                  future: _pConnector.getAllAsync(),
                  builder:
                      (context, AsyncSnapshot<List<ProjectSettings>> snapshot) {
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
                      if (dateSetting != null && dateSetting.value.isNotEmpty) {
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
                      title: "Počet zapojených vyučujících",
                      color: Colors.teal,
                      icon: Icons.school,
                      iconColor: Colors.white,
                      onTap: null,
                    );
                  },
                ),
                FutureBuilder(
                  future: _sConnector.getZolikTypesDataAsync(),
                  builder:
                      (context, AsyncSnapshot<List<ZolikTypesData>> snapshot) {
                    if (snapshot.connectionState != ConnectionState.done) {
                      return _getLoadingTile();
                    }
                    var list = List<ZolikTypesData>();
                    if (snapshot.data != null) {
                      list = snapshot.data;
                    }

                    var series = [
                      charts.Series<ZolikTypesData, String>(
                          domainFn: (ZolikTypesData data, _) => data.label,
                          measureFn: (ZolikTypesData data, _) => data.count,
                          data: list,
                          labelAccessorFn: (ZolikTypesData data, _) =>
                              data.count.toString(),
                          id: "ZolikLeaderboard"),
                    ];

                    return _getTile(Padding(
                      padding: EdgeInsets.all(24),
                      child: charts.PieChart(
                        series,
                        defaultRenderer: charts.ArcRendererConfig(
                          arcRendererDecorators: [
                            charts.ArcLabelDecorator(
                              labelPosition: charts.ArcLabelPosition.auto,
                            ),
                          ],
                        ),
                        behaviors: [
                          charts.ChartTitle(
                            "Poměr žolíků",
                            subTitle: "Počet jednotlivých druhů žolíků",
                            behaviorPosition: charts.BehaviorPosition.top,
                            titleOutsideJustification:
                                charts.OutsideJustification.start,
                            innerPadding: 18,
                            titleStyleSpec: charts.TextStyleSpec(
                              color: charts.Color.black,
                            ),
                          ),
                          charts.DatumLegend(
                              position: charts.BehaviorPosition.bottom),
                        ],
                      ),
                    ));
                  },
                ),
                FutureBuilder(
                  future: Singleton().classLeaderboard.isEmpty
                      ? _sConnector.getClassLeaderboardAsync()
                      : Future.value(Singleton().classLeaderboard),
                  builder: (context,
                      AsyncSnapshot<List<ClassLeaderboardData>> snapshot) {
                    if (snapshot.connectionState != ConnectionState.done) {
                      return _getLoadingTile();
                    }
                    var list = List<ClassLeaderboardData>();
                    if (snapshot.data != null) {
                      list = snapshot.data;
                    }
                    Singleton().classLeaderboard = list;

                    var series = [
                      charts.Series<ClassLeaderboardData, String>(
                          domainFn: (ClassLeaderboardData data, _) =>
                              data.label,
                          measureFn: (ClassLeaderboardData data, _) =>
                              data.data,
                          colorFn: (ClassLeaderboardData data, _) =>
                              charts.Color.fromHex(code: data.colour),
                          data: list,
                          id: "ClassLeaderboard"),
                    ];

                    return _getTile(
                      Padding(
                        padding: EdgeInsets.all(24),
                        child: charts.BarChart(
                          series,
                          behaviors: [
                            charts.ChartTitle(
                              "Žolíci ve třídách",
                              subTitle: "Počet žolíků v jednotlivých třídách",
                              behaviorPosition: charts.BehaviorPosition.top,
                              titleOutsideJustification:
                                  charts.OutsideJustification.start,
                              innerPadding: 18,
                              titleStyleSpec: charts.TextStyleSpec(
                                color: charts.Color.black,
                              ),
                            ),
                            charts.DatumLegend(
                              position: charts.BehaviorPosition.bottom,
                              desiredMaxColumns: 5,
                            ),
                          ],
                          primaryMeasureAxis: charts.NumericAxisSpec(
                            tickProviderSpec:
                                charts.BasicNumericTickProviderSpec(
                              desiredMinTickCount: 5,
                            ),
                          ),
                        ),
                      ),
                    );
                  },
                ),
              ],
            ),
          ),
        ),
      ],
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
      void Function() onTap}) {
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
      onTap: onTap,
    );
  }
}
