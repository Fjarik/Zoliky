import 'package:expandable/expandable.dart';
import 'package:firebase_analytics/firebase_analytics.dart';
import 'package:firebase_analytics/observer.dart';
import 'package:flutter/material.dart';
import 'package:pull_to_refresh/pull_to_refresh.dart';
import 'package:zoliky_teachers/components/Admin/ZolikDetails.dart';
import 'package:zoliky_teachers/pages/Administration/ZoliksPage.dart';
import 'package:zoliky_teachers/utils/Singleton.dart';
import 'package:zoliky_teachers/utils/api/connectors/ZolikConnector.dart';
import 'package:zoliky_teachers/utils/api/models/Zolik.dart';

class ZoliksPageState extends State<ZoliksPage> {
  ZoliksPageState(this.analytics, this.observer);

  final FirebaseAnalytics analytics;
  final FirebaseAnalyticsObserver observer;

  final RefreshController _rController = RefreshController();

  ZolikConnector get _zConnector => ZolikConnector(Singleton().token);

  Future<List<Zolik>> get _future => Singleton().zoliks.isEmpty
      ? _zConnector.getSchoolZoliks()
      : Future.value(Singleton().zoliks);

  int classIdOnly;

  @override
  void initState() {
    super.initState();
    
    // classIdOnly = 4;
  }

  @override
  Widget build(BuildContext context) {
    return Stack(
      children: <Widget>[
        Padding(
          padding: EdgeInsets.all(0),
          child: FutureBuilder(
            future: _future,
            builder: (BuildContext ctx, AsyncSnapshot<List<Zolik>> snapshot) {
              if (snapshot.connectionState != ConnectionState.done) {
                return _loading();
              }
              var _zoliks = List<CustomZolik>();

              Iterable<CustomZolik> _z;
              if (snapshot.data != null) {
                Singleton().zoliks = snapshot.data;
                _z = snapshot.data.map((Zolik z) {
                  return CustomZolik(z);
                });
              }
              if (classIdOnly != null) {
                _z = _z.where((x) => x.original.ownerClassId == classIdOnly);
              }

              _zoliks = _z.toList();
              return SmartRefresher(
                controller: _rController,
                enablePullDown: true,
                enablePullUp: false,
                onRefresh: () async {
                  Future.delayed(Duration(milliseconds: 500));
                  Singleton().zoliks.clear();
                  setState(() {});
                  _rController.refreshCompleted();
                },
                child: ListView.builder(
                  padding: EdgeInsets.zero,
                  itemCount: _zoliks.length,
                  shrinkWrap: true,
                  itemBuilder: (BuildContext c, int i) {
                    return _zolikWidget(_zoliks[i]);
                  },
                ),
              );
            },
          ),
        ),
      ],
    );
  }

  Widget _zolikWidget(CustomZolik z) {
    return Card(
      child: Padding(
        padding: EdgeInsets.only(right: 10, left: 10, top: 10, bottom: 10),
        child: ExpandablePanel(
          header: Row(
            children: <Widget>[
              Text(
                z.original.title,
                softWrap: true,
                maxLines: 1,
                overflow: TextOverflow.ellipsis,
                style: TextStyle(
                  fontSize: 18,
                  fontWeight: FontWeight.w500,
                ),
              ),
              Padding(
                padding: EdgeInsets.only(left: 6),
                child: Text(
                  "(" + z.original.ownerClass.name + ")",
                  softWrap: true,
                  maxLines: 1,
                  overflow: TextOverflow.ellipsis,
                  style: TextStyle(
                    fontSize: 18,
                    fontWeight: FontWeight.w500,
                  ),
                ),
              ),
            ],
          ),
          collapsed: _collapsedZolik(z.original),
          expanded: Container(
            child: Row(
              children: <Widget>[
                Column(
                  children: <Widget>[
                    Text("Test"),
                    Text("Test"),
                    Text("Test"),
                    Text("Test"),
                    Text("Test"),
                    Text("Test"),
                    Text("Test"),
                  ],
                )
              ],
            ),
          ),
          tapHeaderToExpand: true,
          hasIcon: true,
        ),
      ),
    );
  }

  Widget _collapsedZolik(Zolik z) {
    return Container(
      child: Column(
        children: <Widget>[
          Row(
            children: <Widget>[
              Column(
                children: <Widget>[
                  Text("Vlastník: "),
                ],
              ),
              Column(
                children: <Widget>[
                  Text(
                    z.ownerName,
                    softWrap: true,
                    maxLines: 1,
                    overflow: TextOverflow.ellipsis,
                  ),
                ],
              ),
            ],
          ),
          Row(
            children: <Widget>[
              Column(
                children: <Widget>[
                  Text("Předmět: "),
                ],
              ),
              Column(
                children: <Widget>[
                  Text(
                    z.subjectName,
                    softWrap: true,
                    maxLines: 1,
                    overflow: TextOverflow.ellipsis,
                  ),
                ],
              ),
            ],
          ),
        ],
      ),
    );
  }

  Widget _loading() {
    return Container(
      child: Row(
        mainAxisAlignment: MainAxisAlignment.center,
        children: <Widget>[
          Column(
            mainAxisAlignment: MainAxisAlignment.center,
            children: <Widget>[
              CircularProgressIndicator(),
            ],
          ),
        ],
      ),
    );
  }
}
