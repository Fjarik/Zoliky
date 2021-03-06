import 'package:expandable/expandable.dart';
import 'package:firebase_analytics/firebase_analytics.dart';
import 'package:firebase_analytics/observer.dart';
import 'package:flutter/material.dart';
import 'package:pull_to_refresh/pull_to_refresh.dart';
import 'package:zoliky_teachers/pages/Administration/Zolik/ZolikCreatePage.dart';
import 'package:zoliky_teachers/pages/Administration/Zolik/ZolikDetailPage.dart';
import 'package:zoliky_teachers/pages/Administration/Zolik/ZolikRemovePage.dart';
import 'package:zoliky_teachers/pages/Administration/ZoliksPage.dart';
import 'package:zoliky_teachers/utils/Global.dart';
import 'package:zoliky_teachers/utils/Singleton.dart';
import 'package:zoliky_teachers/utils/api/connectors/ZolikConnector.dart';
import 'package:zoliky_teachers/utils/api/models/Class.dart';
import 'package:zoliky_teachers/utils/api/models/Subject.dart';
import 'package:zoliky_teachers/utils/api/models/Zolik.dart';
import 'package:zoliky_teachers/utils/enums/ZolikSort.dart';

class _ZolikFilter {
  _ZolikFilter();

  _ZolikFilter.content({this.classId, this.onlyMe, this.subjectId});

  int classId;
  int subjectId;
  bool onlyMe;
}

class ZoliksPageState extends State<ZoliksPage> {
  ZoliksPageState(this.analytics, this.observer) {
    _classes.addAll(Global.classes);
    _classes.add(Class()
      ..id = -1
      ..name = "Všechny");
    _subjects.addAll(Global.subjects);
    _subjects.add(Subject()
      ..id = -1
      ..name = "Všechny"
      ..shortcut = "Všechny");
  }

  final FirebaseAnalytics analytics;
  final FirebaseAnalyticsObserver observer;

  final RefreshController _rController = RefreshController();

  List<Class> _classes = List<Class>();
  List<Subject> _subjects = List<Subject>();

  List<DropdownMenuItem<int>> get _dropClasses => _classes
      .map((c) => DropdownMenuItem<int>(
            value: c.id,
            child: Text(c.name),
          ))
      .toList();

  List<DropdownMenuItem<int>> get _dropSubjects => _subjects
      .map((x) => DropdownMenuItem<int>(
            value: x.id,
            child: Text(
              x.name.length > 25 ? x.name.substring(0, 25) + "..." : x.name,
            ),
          ))
      .toList();

  ZolikConnector get _zConnector => ZolikConnector(Singleton().token);

  Future<List<Zolik>> get _future =>
      Singleton().zoliks.isEmpty || Singleton().changed
          ? _zConnector.getSchoolZoliks()
          : Future.value(Singleton().zoliks);

  int classIdOnly = -1;
  ZolikSort sortBy = ZolikSort.id;
  bool onlyMe = false;
  int subjectId = -1;
  bool ascending = false;

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
    var res = await showDialog<ZolikSort>(
      context: context,
      builder: (BuildContext ctx) {
        var dictionary = {
          "Výchozí": ZolikSort.id,
          "Data vytvoření": ZolikSort.created,
          "Třídy": ZolikSort.className,
          "Poslední pohyby": ZolikSort.lastTransfer
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
    if (res == null) {
      return;
    }
    setState(() {
      sortBy = res ?? this.sortBy;
    });
  }

  Future<void> _filterClick() async {
    var res = await showDialog<_ZolikFilter>(
      context: context,
      builder: (BuildContext ctx) {
        int selectedId = classIdOnly;
        bool onlyMee = onlyMe;
        int sId = subjectId;
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
                    Padding(
                      padding: EdgeInsets.only(left: 16),
                      child: Text("Podle třídy:"),
                    ),
                    Padding(
                      padding: EdgeInsets.only(left: 16),
                      child: DropdownButton<int>(
                        items: _dropClasses,
                        hint: Text("Filtrování žolíků podle třídy"),
                        value: selectedId,
                        onChanged: (int id) {
                          sState(() {
                            selectedId = id;
                          });
                        },
                      ),
                    ),
                    Padding(
                      padding: EdgeInsets.only(left: 16),
                      child: Text("Podle předmětu:"),
                    ),
                    Padding(
                      padding: EdgeInsets.only(left: 16),
                      child: DropdownButton<int>(
                        items: _dropSubjects,
                        hint: Text("Filtrování podle předmětu"),
                        value: sId,
                        onChanged: (int id) {
                          sState(() {
                            sId = id;
                          });
                        },
                      ),
                    ),
                    CheckboxListTile(
                      title: Text("Pouze mnou přidělené"),
                      subtitle: Text(
                        "Zobrazovat pouze žolíky, kteří byli přiděleni Vámi",
                      ),
                      value: onlyMee,
                      onChanged: (bool val) {
                        sState(() {
                          onlyMee = val;
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
              child: Text("Vymazat filtry"),
              onPressed: () {
                Navigator.of(ctx).pop<_ZolikFilter>(_ZolikFilter());
              },
            ),
            FlatButton(
              child: Text("Zrušit"),
              onPressed: () {
                Navigator.of(ctx).pop<_ZolikFilter>(null);
              },
            ),
            FlatButton(
              child: Text("OK"),
              onPressed: () {
                Navigator.of(ctx).pop<_ZolikFilter>(_ZolikFilter.content(
                  classId: selectedId,
                  onlyMe: onlyMee,
                  subjectId: sId,
                ));
              },
            ),
          ],
        );
      },
    );
    if (res == null) {
      return;
    }
    setState(() {
      classIdOnly = res?.classId;
      onlyMe = res?.onlyMe ?? false;
      subjectId = res?.subjectId;
    });
  }

  Future<void> _detailClick(Zolik selected) async {
    var r = MaterialPageRoute(
      builder: (BuildContext ctx) => ZolikDetailPage(
        analytics: analytics,
        observer: observer,
        zolik: selected,
      ),
    );
    await Navigator.push(context, r);
  }

  Future<void> _removeClick(Zolik selected) async {
    var r = MaterialPageRoute(
      builder: (BuildContext ctx) => ZolikRemovePage(
        analytics: analytics,
        observer: observer,
        zolik: selected,
      ),
    );
    await Navigator.push(context, r);
  }

  Future<void> _newClick() async {
    var r = MaterialPageRoute(
      builder: (BuildContext ctx) => ZolikCreatePage(
        analytics: analytics,
        observer: observer,
      ),
    );
    await Navigator.push(context, r);
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
          IconButton(
            icon: Icon(
              Icons.filter_list,
              color: Colors.white,
            ),
            onPressed: _filterClick,
          )
        ],
      ),
      body: Padding(
        padding: EdgeInsets.symmetric(horizontal: 5),
        child: FutureBuilder(
          future: _future,
          builder: (BuildContext ctx, AsyncSnapshot<List<Zolik>> snapshot) {
            if (snapshot.connectionState != ConnectionState.done) {
              return Global.loading();
            }
            var _zoliks = List<Zolik>();
            Singleton().changed = false;
            Iterable<Zolik> _z;
            if (snapshot.data != null) {
              Singleton().zoliks = snapshot.data;
              _z = snapshot.data;
            }
            if (classIdOnly != null && classIdOnly != -1) {
              _z = _z.where((x) => x.ownerClassId == classIdOnly);
            }
            if (onlyMe) {
              _z = _z.where((x) => x.teacherId == Singleton().user.id);
            }
            if (subjectId != null && subjectId != -1) {
              _z = _z.where((x) => x.subjectId == subjectId);
            }
            _zoliks = _z.toList();

            switch (sortBy) {
              case ZolikSort.id:
                _zoliks
                    .sort((Zolik one, Zolik two) => (one.id.compareTo(two.id)));
                break;
              case ZolikSort.created:
                _zoliks.sort((Zolik one, Zolik two) =>
                    (one.created.compareTo(two.created)));
                break;
              case ZolikSort.className:
                _zoliks.sort((Zolik one, Zolik two) =>
                    (one.ownerClass.name.compareTo(two.ownerClass.name)));
                break;
              case ZolikSort.lastTransfer:
                _zoliks.sort((Zolik one, Zolik two) =>
                    (one.ownerSince.compareTo(two.ownerSince)));
                break;
            }

            if (!ascending) {
              _zoliks = _zoliks.reversed.toList();
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
      floatingActionButton: FloatingActionButton(
        child: Icon(Icons.add),
        backgroundColor: Colors.blue,
        onPressed: _newClick,
      ),
    );
  }

  Widget _zolikWidget(Zolik z) {
    return Card(
      child: Padding(
        padding: EdgeInsets.all(10),
        child: ExpandablePanel(
          header: Row(
            children: <Widget>[
              Text(
                z.title,
                softWrap: true,
                maxLines: 1,
                overflow: TextOverflow.ellipsis,
                style: TextStyle(
                  fontSize: 20,
                  color: Colors.blueAccent,
                  fontWeight: FontWeight.w500,
                ),
              ),
              Padding(
                padding: EdgeInsets.only(left: 6),
                child: Text(
                  "(" + z.ownerClass.name + ")",
                  softWrap: true,
                  maxLines: 1,
                  overflow: TextOverflow.ellipsis,
                  style: TextStyle(
                    fontSize: 20,
                    color: Colors.blueAccent,
                    fontWeight: FontWeight.w500,
                  ),
                ),
              ),
            ],
          ),
          collapsed: _collapsedZolik(z),
          expanded: _expandedZolik(z),
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

  Widget _expandedZolik(Zolik z) {
    return Container(
      child: Row(
        children: <Widget>[
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              mainAxisSize: MainAxisSize.max,
              children: <Widget>[
                _zolikInfoLine("Typ žolíka:", z.zolikType),
                _zolikInfoLine("Udělen za:", z.title),
                _zolikInfoLine("Předmět:", z.subjectName),
                _zolikInfoLine("Vyučující:", z.teacherName),
                Divider(
                  color: Colors.blueAccent,
                  thickness: 2,
                ),
                Row(
                  mainAxisAlignment: MainAxisAlignment.spaceAround,
                  children: <Widget>[
                    FlatButton.icon(
                      icon: Icon(Icons.delete),
                      onPressed: () async => await _removeClick(z),
                      label: Text("Použít/Odstranit"),
                    ),
                    FlatButton.icon(
                      icon: Icon(Icons.visibility),
                      onPressed: () async => await _detailClick(z),
                      label: Text("Detaily"),
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
}
