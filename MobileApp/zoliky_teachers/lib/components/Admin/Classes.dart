import 'package:expandable/expandable.dart';
import 'package:firebase_analytics/firebase_analytics.dart';
import 'package:firebase_analytics/observer.dart';
import 'package:flutter/material.dart';
import 'package:pull_to_refresh/pull_to_refresh.dart';
import 'package:zoliky_teachers/pages/Administration/ClassesPages.dart';
import 'package:zoliky_teachers/utils/Global.dart';
import 'package:zoliky_teachers/utils/Singleton.dart';
import 'package:zoliky_teachers/utils/api/models/Class.dart';
import 'package:zoliky_teachers/utils/enums/ClassSort.dart';

class ClassPageState extends State<ClassesPage> {
  ClassPageState(this.analytics, this.observer);

  final FirebaseAnalytics analytics;
  final FirebaseAnalyticsObserver observer;

  final RefreshController _rController = RefreshController();

  String get token => Singleton().token;

  Future<List<Class>> get _future => (this.refresh || Global.classes.isEmpty)
      ? Global.loadAndSetClasses(token)
      : Future.value(Global.classes);

  bool refresh = false;
  ClassSort sortBy = ClassSort.name;
  bool ascending = true;

  @override
  void initState() {
    super.initState();
  }

  @override
  void dispose() {
    super.dispose();
    _rController?.dispose();
  }

  void _ascDesc() {
    setState(() {
      ascending = !ascending;
    });
  }

  Future<void> _orderBy() async {
    var res = await showDialog<ClassSort>(
      context: context,
      builder: (BuildContext ctx) {
        var dictionary = {
          "Výchozí": ClassSort.id,
          "Datum nástupu": ClassSort.created,
          "Název": ClassSort.name,
          "Datum ukončení studia": ClassSort.graduation
        };
        return AlertDialog(
          title: Text("Řazení žolíků"),
          content: Container(
            child: Column(
              mainAxisAlignment: MainAxisAlignment.start,
              crossAxisAlignment: CrossAxisAlignment.start,
              mainAxisSize: MainAxisSize.min,
              children: <Widget>[
                Text("Seřadit podle:"),
                Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: dictionary.entries
                      .map((map) => RadioListTile<ClassSort>(
                            groupValue: sortBy,
                            title: Text(map.key),
                            value: map.value,
                            selected: map.value == sortBy,
                            onChanged: (ClassSort val) {
                              Navigator.of(ctx).pop<ClassSort>(val);
                            },
                          ))
                      .toList(),
                ),
              ],
            ),
          ),
          actions: <Widget>[
            FlatButton(
              child: Text("Zrušit"),
              onPressed: () {
                Navigator.of(ctx).pop<ClassSort>(sortBy);
              },
            ),
          ],
        );
      },
    );
    setState(() {
      sortBy = res ?? this.sortBy;
    });
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        backgroundColor: Colors.blue,
        title: Text(
          "Řazení/Filtrování",
          style: TextStyle(
            color: Colors.white,
          ),
        ),
        actions: <Widget>[
          IconButton(
            icon: Icon(
              ascending ? Icons.arrow_drop_down : Icons.arrow_drop_up,
              color: Colors.white,
            ),
            onPressed: _ascDesc,
          ),
          IconButton(
            icon: Icon(
              Icons.sort_by_alpha,
              color: Colors.white,
            ),
            onPressed: _orderBy,
          ),
        ],
      ),
      body: Padding(
        padding: EdgeInsets.all(10),
        child: FutureBuilder(
          future: _future,
          builder: (BuildContext ctx, AsyncSnapshot<List<Class>> snapshot) {
            if (snapshot.connectionState != ConnectionState.done) {
              return Global.loading();
            }
            this.refresh = false;

            var _classes = List<Class>();

            if (snapshot.data != null) {
              _classes = snapshot.data;
            }

            switch (sortBy) {
              case ClassSort.id:
                _classes
                    .sort((Class one, Class two) => (one.id.compareTo(two.id)));
                break;
              case ClassSort.created:
                _classes.sort(
                    (Class one, Class two) => (one.since.compareTo(two.since)));
                break;
              case ClassSort.graduation:
                _classes.sort((Class one, Class two) =>
                    (one.graduation.compareTo(two.graduation)));
                break;
              case ClassSort.name:
                _classes.sort(
                    (Class one, Class two) => (one.name.compareTo(two.name)));
                break;
            }

            if (!ascending) {
              _classes = _classes.reversed.toList();
            }

            return SmartRefresher(
              controller: _rController,
              enablePullDown: true,
              enablePullUp: false,
              onRefresh: () async {
                Future.delayed(Duration(milliseconds: 500));
                setState(() {});
                this.refresh = true;
                _rController.refreshCompleted();
              },
              child: ListView.builder(
                padding: EdgeInsets.zero,
                itemCount: _classes.length,
                shrinkWrap: true,
                itemBuilder: (BuildContext c, int i) {
                  return _classWidget(_classes[i]);
                },
              ),
            );
          },
        ),
      ),
    );
  }

  Widget _classWidget(Class c) {
    return Card(
      child: Padding(
        padding: EdgeInsets.all(10),
        child: ExpandablePanel(
          header: Text(
            c.name,
            softWrap: true,
            maxLines: 1,
            overflow: TextOverflow.ellipsis,
            style: TextStyle(
              fontSize: 20,
              color: Colors.blueAccent,
              fontWeight: FontWeight.w500,
            ),
          ),
          collapsed: _collapsedClass(c),
          expanded: _expandedClass(c),
          tapHeaderToExpand: true,
          hasIcon: true,
        ),
      ),
    );
  }

  Widget _collapsedClass(Class c) {
    var count = Global.students.where((x) => x.classId == c.id).length;
    var zoliks = Singleton()
            .classLeaderboard
            ?.firstWhere((x) => x.label == c.name)
            ?.data ??
        0.0;
    // TODO: Předělat
    return Container(
      child: Column(
        children: <Widget>[
          Row(
            children: <Widget>[
              Column(
                children: <Widget>[
                  Text("Počet studentů: "),
                ],
              ),
              Column(
                children: <Widget>[
                  Text(
                    count.toString(),
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
                  Text("Počet žolíků: "),
                ],
              ),
              Column(
                children: <Widget>[
                  Text(
                    zoliks.toStringAsFixed(0),
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

  Widget _expandedClass(Class c) {
    return Container(
      child: Column(
        children: <Widget>[
          Row(
            children: <Widget>[
              Column(
                children: <Widget>[
                  Text("Nástup: "),
                ],
              ),
              Column(
                children: <Widget>[
                  Text(
                    Global.getDateString(c.since),
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
                  Text("Ukončení studia: "),
                ],
              ),
              Column(
                children: <Widget>[
                  Text(
                    Global.getDateString(c.graduation),
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
}
