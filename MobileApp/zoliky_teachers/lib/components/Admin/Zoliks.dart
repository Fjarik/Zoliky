import 'package:expandable/expandable.dart';
import 'package:firebase_analytics/firebase_analytics.dart';
import 'package:firebase_analytics/observer.dart';
import 'package:flutter/material.dart';
import 'package:pull_to_refresh/pull_to_refresh.dart';
import 'package:zoliky_teachers/components/Admin/ZolikDetails.dart';
import 'package:zoliky_teachers/pages/Administration/ZoliksPage.dart';
import 'package:zoliky_teachers/utils/Global.dart';
import 'package:zoliky_teachers/utils/Singleton.dart';
import 'package:zoliky_teachers/utils/api/connectors/ZolikConnector.dart';
import 'package:zoliky_teachers/utils/api/models/Class.dart';
import 'package:zoliky_teachers/utils/api/models/Zolik.dart';
import 'package:zoliky_teachers/utils/enums/ZolikSort.dart';

class ZoliksPageState extends State<ZoliksPage> {
  ZoliksPageState(this.analytics, this.observer) {
    _classes = Global.classes;
    _classes.add(Class()
      ..id = null
      ..name = "Všechny");
  }

  final FirebaseAnalytics analytics;
  final FirebaseAnalyticsObserver observer;

  final RefreshController _rController = RefreshController();

  List<Class> _classes = List<Class>();

  List<DropdownMenuItem<int>> get _dropClasses => _classes
      .map((c) => DropdownMenuItem<int>(
            value: c.id,
            child: Text(c.name),
          ))
      .toList();

  ZolikConnector get _zConnector => ZolikConnector(Singleton().token);

  Future<List<Zolik>> get _future => Singleton().zoliks.isEmpty
      ? _zConnector.getSchoolZoliks()
      : Future.value(Singleton().zoliks);

  int classIdOnly;
  ZolikSort sortBy = ZolikSort.id;

  @override
  void initState() {
    super.initState();
  }

  Future<void> _orderBy() async {
    var res = await showDialog<ZolikSort>(
      context: context,
      builder: (BuildContext ctx) {
        var dictionary = {
          "ID": ZolikSort.id,
          "Data vytvoření": ZolikSort.created,
          "Třídy": ZolikSort.className,
          "Poslední transakce": ZolikSort.lastTransfer
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
                      .map((map) => RadioListTile<ZolikSort>(
                            groupValue: sortBy,
                            title: Text(map.key),
                            value: map.value,
                            selected: map.value == sortBy,
                            onChanged: (ZolikSort val) {
                              Navigator.of(ctx).pop<ZolikSort>(val);
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
                Navigator.of(ctx).pop<ZolikSort>(sortBy);
              },
            ),
          ],
        );
      },
    );
    setState(() {
      sortBy = res;
    });
  }

  Future<void> _filterClick() async {
    var res = await showDialog<int>(
      context: context,
      builder: (BuildContext ctx) {
        int selectedId = classIdOnly;
        return AlertDialog(
          title: Text("Filtrování žolíků"),
          content: StatefulBuilder(
            builder: (BuildContext c, StateSetter sState) {
              return Container(
                child: Column(
                  mainAxisAlignment: MainAxisAlignment.start,
                  crossAxisAlignment: CrossAxisAlignment.start,
                  mainAxisSize: MainAxisSize.min,
                  children: <Widget>[
                    Text("Podle třídy:"),
                    DropdownButton<int>(
                      items: _dropClasses,
                      hint: Text("Filtrování žolíků podle třídy"),
                      value: selectedId,
                      onChanged: (int id) {
                        sState(() {
                          selectedId = id;
                        });
                      },
                    ),
                  ],
                ),
              );
            },
          ),
          actions: <Widget>[
            FlatButton(
              child: Text("Zrušit"),
              onPressed: () {
                Navigator.of(ctx).pop<int>(classIdOnly);
              },
            ),
            FlatButton(
              child: Text("OK"),
              onPressed: () {
                Navigator.of(ctx).pop<int>(selectedId);
              },
            ),
          ],
        );
      },
    );
    setState(() {
      classIdOnly = res;
    });
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        backgroundColor: Colors.white70,
        title: Text(
          "Řazení/Filtrování",
          style: TextStyle(
            color: Colors.black87,
          ),
        ),
        actions: <Widget>[
          IconButton(
            icon: Icon(
              Icons.sort_by_alpha,
              color: Colors.black87,
            ),
            onPressed: _orderBy,
          ),
          IconButton(
            icon: Icon(
              Icons.filter_list,
              color: Colors.black87,
            ),
            onPressed: _filterClick,
          )
        ],
      ),
      body: Padding(
        padding: EdgeInsets.all(10),
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

            switch (sortBy) {
              case ZolikSort.id:
                _zoliks.sort((CustomZolik one, CustomZolik two) =>
                    (one.original.id.compareTo(two.original.id)));
                break;
              case ZolikSort.created:
                _zoliks.sort((CustomZolik one, CustomZolik two) =>
                    (one.original.created.compareTo(two.original.created)));
                break;
              case ZolikSort.className:
                _zoliks.sort((CustomZolik one, CustomZolik two) => (one
                    .original.ownerClass.name
                    .compareTo(two.original.ownerClass.name)));
                break;
              case ZolikSort.lastTransfer:
                _zoliks.sort((CustomZolik one, CustomZolik two) => (one
                    .original.ownerSince
                    .compareTo(two.original.ownerSince)));
                break;
            }

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
    );
  }

  Widget _zolikWidget(CustomZolik z) {
    return Card(
      child: Padding(
        padding: EdgeInsets.all(10),
        child: ExpandablePanel(
          header: Row(
            children: <Widget>[
              Text(
                z.original.title,
                softWrap: true,
                maxLines: 1,
                overflow: TextOverflow.ellipsis,
                style: TextStyle(
                  fontSize: 20,
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
                    fontSize: 20,
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
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: <Widget>[
                    _zolikInfoLine("Typ žolíka:", z.original.zolikType),
                    _zolikInfoLine("Udělen za:", z.original.title),
                    _zolikInfoLine("Předmět:", z.original.subjectName),
                    _zolikInfoLine("Vyučující:", z.original.teacherName),
                    _zolikInfoLine(
                        "Původní vlastník:", z.original.originalOwnerName),
                  ],
                ),
              ],
            ),
          ),
          tapHeaderToExpand: true,
          hasIcon: true,
        ),
      ),
    );
  }

  Widget _zolikInfoLine(String title, String content) {
    return Padding(
      padding: EdgeInsets.symmetric(vertical: 5),
      child: Row(
        children: <Widget>[
          Text(
            title,
            maxLines: 1,
            softWrap: false,
            style: TextStyle(
              fontSize: 16,
            ),
          ),
          Padding(
            padding: EdgeInsets.only(left: 10),
            child: Text(
              content,
              maxLines: 1,
              softWrap: true,
              overflow: TextOverflow.ellipsis,
              style: TextStyle(
                fontSize: 16,
                fontWeight: FontWeight.w500,
              ),
            ),
          ),
        ],
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
