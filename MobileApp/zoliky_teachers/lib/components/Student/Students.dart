import 'package:expandable/expandable.dart';
import 'package:firebase_analytics/firebase_analytics.dart';
import 'package:firebase_analytics/observer.dart';
import 'package:flutter/material.dart';
import 'package:pull_to_refresh/pull_to_refresh.dart';
import 'package:zoliky_teachers/pages/Administration/Student/StudentDetailPage.dart';
import 'package:zoliky_teachers/pages/Administration/StudentsPage.dart';
import 'package:zoliky_teachers/utils/Global.dart';
import 'package:zoliky_teachers/utils/Singleton.dart';
import 'package:zoliky_teachers/utils/api/connectors/StudentConnector.dart';
import 'package:zoliky_teachers/utils/api/models/Class.dart';
import 'package:zoliky_teachers/utils/api/models/Student.dart';
import 'package:zoliky_teachers/utils/enums/StudentSort.dart';

class _StudentFilter {
  _StudentFilter();

  _StudentFilter.content({this.classId, this.onlyWithZoliks});

  int classId;
  bool onlyWithZoliks;
}

class StudentsPageState extends State<StudentsPage> {
  StudentsPageState(this.analytics, this.observer) {
    _classes.addAll(Global.classes);
    _classes.add(Class()
      ..id = -1
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

  StudentConnector get _sConnector => StudentConnector(Singleton().token);

  Future<List<Student>> get _future => Singleton().students.isEmpty
      ? _sConnector.getStudents()
      : Future.value(Singleton().students);

  int classIdOnly = -1;
  StudentSort sortBy = StudentSort.id;
  bool ascending = false;
  bool onlyWithZoliks = false;

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
    var res = await showDialog<StudentSort>(
      context: context,
      builder: (BuildContext ctx) {
        var dictionary = {
          "Výchozí": StudentSort.id,
          "Jména": StudentSort.name,
          "Příjmení": StudentSort.lastname,
          "Třídy": StudentSort.className,
          "Počtu žolíků": StudentSort.zolikCount,
        };
        return AlertDialog(
          title: Text("Řazení studentů"),
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
                      .map((map) => RadioListTile<StudentSort>(
                            groupValue: sortBy,
                            title: Text(map.key),
                            value: map.value,
                            selected: map.value == sortBy,
                            onChanged: (StudentSort val) {
                              Navigator.of(ctx).pop<StudentSort>(val);
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
                Navigator.of(ctx).pop<StudentSort>(sortBy);
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
    var res = await showDialog<_StudentFilter>(
      context: context,
      builder: (BuildContext ctx) {
        int selectedId = classIdOnly;
        bool onlyWith = onlyWithZoliks;
        return AlertDialog(
          title: Text("Filtrování studentů"),
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
                        hint: Text("Filtrování studentů podle třídy"),
                        value: selectedId,
                        onChanged: (int id) {
                          sState(() {
                            selectedId = id;
                          });
                        },
                      ),
                    ),
                    CheckboxListTile(
                      title: Text("Pouze s žolíky"),
                      subtitle: Text(
                        "Zobrazovat pouze studenty, které mají nějakého žolíka",
                      ),
                      value: onlyWith,
                      onChanged: (bool val) {
                        sState(() {
                          onlyWith = val;
                        });
                      },
                    )
                  ],
                ),
              );
            },
          ),
          actions: <Widget>[
            FlatButton(
              child: Text("Vymazat filtry"),
              onPressed: () {
                Navigator.of(ctx).pop<_StudentFilter>(_StudentFilter());
              },
            ),
            FlatButton(
              child: Text("Zrušit"),
              onPressed: () {
                Navigator.of(ctx).pop<_StudentFilter>(null);
              },
            ),
            FlatButton(
              child: Text("OK"),
              onPressed: () {
                Navigator.of(ctx).pop<_StudentFilter>(_StudentFilter.content(
                  classId: selectedId,
                  onlyWithZoliks: onlyWith,
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
      classIdOnly = res.classId;
      onlyWithZoliks = res.onlyWithZoliks ?? false;
    });
  }

  Future<void> _detailClick(Student selected) async {
    var r = MaterialPageRoute(
      builder: (BuildContext ctx) => StudentDetailPage(
        analytics: analytics,
        observer: observer,
        student: selected,
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
          builder: (BuildContext ctx, AsyncSnapshot<List<Student>> snapshot) {
            if (snapshot.connectionState != ConnectionState.done) {
              return Global.loading();
            }
            var _students = List<Student>();

            Iterable<Student> _s;
            if (snapshot.data != null) {
              Singleton().students = snapshot.data;
              _s = snapshot.data;
            }
            if (classIdOnly != null && classIdOnly != -1) {
              _s = _s.where((x) => x.classId == classIdOnly);
            }
            if (onlyWithZoliks) {
              _s = _s.where((x) => x.zolikCount > 0);
            }

            _students = _s.toList();

            switch (sortBy) {
              case StudentSort.id:
                _students.sort(
                    (Student one, Student two) => (one.id.compareTo(two.id)));
                break;
              case StudentSort.name:
                _students.sort((Student one, Student two) =>
                    (one.name.compareTo(two.name)));
                break;
              case StudentSort.lastname:
                _students.sort((Student one, Student two) =>
                    (one.lastname.compareTo(two.lastname)));
                break;
              case StudentSort.className:
                _students.sort((Student one, Student two) =>
                    (one.className.compareTo(two.className)));
                break;
              case StudentSort.zolikCount:
                _students.sort((Student one, Student two) =>
                    (one.zolikCount.compareTo(two.zolikCount)));
                break;
            }

            if (!ascending) {
              _students = _students.reversed.toList();
            }

            return SmartRefresher(
              controller: _rController,
              enablePullDown: true,
              enablePullUp: false,
              onRefresh: () async {
                Future.delayed(Duration(milliseconds: 500));
                Singleton().students.clear();
                setState(() {});
                _rController.refreshCompleted();
              },
              child: ListView.builder(
                padding: EdgeInsets.zero,
                itemCount: _students.length,
                shrinkWrap: true,
                itemBuilder: (BuildContext c, int i) {
                  return _studentWidget(_students[i]);
                },
              ),
            );
          },
        ),
      ),
    );
  }

  Widget _studentWidget(Student s) {
    return Card(
      child: Padding(
        padding: EdgeInsets.all(10),
        child: ExpandablePanel(
          header: Row(
            children: <Widget>[
              Text(
                s.fullName,
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
                  "(" + s.className + ")",
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
          collapsed: _collapsedStudent(s),
          expanded: _expandedStudent(s),
          tapHeaderToExpand: true,
          hasIcon: true,
        ),
      ),
    );
  }

  Widget _studentInfoLine(String title, String content) {
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

  Widget _collapsedStudent(Student s) {
    return Container(
      child: Column(
        children: <Widget>[
          Row(
            children: <Widget>[
              Column(
                children: <Widget>[
                  Text("Třída: "),
                ],
              ),
              Column(
                children: <Widget>[
                  Text(
                    s.className,
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
                    s.zolikCount.toString(),
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

  Widget _expandedStudent(Student s) {
    return Container(
      child: Row(
        children: <Widget>[
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              mainAxisSize: MainAxisSize.max,
              children: <Widget>[
                _studentInfoLine("Počet žolíků:", s.zolikCount.toString()),
                _studentInfoLine("Jméno:", s.fullName),
                _studentInfoLine("Třída:", s.className),
                Divider(
                  color: Colors.blueAccent,
                  thickness: 2,
                ),
                Row(
                  mainAxisAlignment: MainAxisAlignment.spaceAround,
                  children: <Widget>[
                    FlatButton.icon(
                      icon: Icon(Icons.visibility),
                      onPressed: () async => await _detailClick(s),
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
