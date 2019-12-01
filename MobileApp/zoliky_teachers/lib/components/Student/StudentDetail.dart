import 'package:flutter/material.dart';
import 'package:zoliky_teachers/pages/Administration/Student/StudentDetailPage.dart';
import 'package:zoliky_teachers/pages/Administration/Zolik/ZolikDetailPage.dart';
import 'package:zoliky_teachers/utils/Global.dart';
import 'package:zoliky_teachers/utils/Singleton.dart';
import 'package:zoliky_teachers/utils/api/connectors/UserConnector.dart';
import 'package:zoliky_teachers/utils/api/connectors/ZolikConnector.dart';
import 'package:zoliky_teachers/utils/api/models/Student.dart';
import 'package:zoliky_teachers/utils/api/models/User.dart';
import 'package:zoliky_teachers/utils/api/models/Zolik.dart';
import 'package:zoliky_teachers/utils/api/models/universal/MActionResult.dart';

class StudentDetailPageState extends State<StudentDetailPage> {
  StudentDetailPageState(this.student) {
    this._zoliks = List<Zolik>();
  }

  Student student;

  User user;

  ZolikConnector get _zConnector => ZolikConnector(Singleton().token);
  UserConnector get _uConnector => UserConnector.withToken(Singleton().token);

  Future<MActionResult<List<Zolik>>> get _future =>
      _zConnector.getUserZoliksAsync(student.id, isTester: false);

  Future<MActionResult<User>> get _userFuture =>
      _uConnector.getAsync(student.id, includeImage: false);

  List<Zolik> _zoliks;

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        elevation: 2,
        // backgroundColor: Colors.white,
        title: Text(
          "Detaily studenta",
          style: TextStyle(
            // color: Colors.black,
            fontWeight: FontWeight.w700,
            fontSize: 30.0,
          ),
        ),
        leading: IconButton(
          icon: Icon(
            Icons.arrow_back,
            // color: Colors.black,
          ),
          onPressed: () {
            if (Navigator.of(context).canPop()) {
              Navigator.of(context).pop();
            }
          },
        ),
      ),
      body: SingleChildScrollView(
        controller: ScrollController(),
        padding: EdgeInsets.zero,
        scrollDirection: Axis.vertical,
        child: Container(
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: <Widget>[
              Global.title("Obecné informace:"),
              FutureBuilder(
                future: _userFuture,
                builder: (BuildContext ctx,
                    AsyncSnapshot<MActionResult<User>> snapshot) {
                  if (snapshot.connectionState != ConnectionState.done) {
                    return Global.loading();
                  }

                  if (snapshot.data == null || !snapshot.data.isSuccess) {
                    return Container();
                  }
                  var res = snapshot.data;
                  this.user = res.content;

                  return _userDetails(this.user);
                },
              ),
              Global.title(
                "Pokročilé:",
                topPadding: 15,
                bottomPadding: 0,
              ),
              Padding(
                padding: EdgeInsets.only(
                  top: 15,
                  right: 10,
                  bottom: 15,
                  left: 8,
                ),
                child: Text(
                  "Žolíci:",
                  style: TextStyle(
                    fontSize: 18,
                  ),
                ),
              ),
              FutureBuilder(
                future: _future,
                builder: (BuildContext ctx,
                    AsyncSnapshot<MActionResult<List<Zolik>>> snapshot) {
                  if (snapshot.connectionState != ConnectionState.done) {
                    return Global.loading();
                  }

                  if (snapshot.data != null) {
                    var res = snapshot.data;
                    if (res.isSuccess) {
                      this._zoliks = res.content;

                      /// Duplikování
                      // var a = snapshot.data.content.asMap();
                      // this._zoliks.addAll(List.from(a.values));

                    }
                  }

                  var list = _getTableHeader();
                  var i = 0;
                  list.addAll(this._zoliks.map((Zolik z) {
                    i++;
                    return _zolikWidget(z, isAlternate: i % 2 == 0);
                  }).toList());
                  return Padding(
                    padding: EdgeInsets.only(bottom: 70),
                    child: Table(
                      columnWidths: {
                        0: FlexColumnWidth(3),
                        1: FlexColumnWidth(2),
                        2: FlexColumnWidth(3),
                        3: FlexColumnWidth(1),
                        4: FlexColumnWidth(0.001),
                      },
                      defaultVerticalAlignment:
                          TableCellVerticalAlignment.middle,
                      children: list,
                    ),
                  );
                },
              ),
            ],
          ),
        ),
      ),
    );
  }

  TableRow _zolikWidget(Zolik z, {bool isAlternate = false}) {
    return TableRow(
      decoration: BoxDecoration(
        color: isAlternate
            ? (Singleton().darkmode ? Colors.blue[900] : Colors.blue[100])
            : Colors.transparent,
      ),
      children: <Widget>[
        Padding(
          padding: EdgeInsets.only(left: 5),
          child: Text(z.title),
        ),
        Text(z.subjectName),
        Text(z.teacherName),
        Padding(
          padding: EdgeInsets.only(right: 5),
          child: IconButton(
            icon: Icon(Icons.visibility),
            onPressed: () async {
              var r = MaterialPageRoute(
                builder: (BuildContext ctx) => ZolikDetailPage(
                  zolik: z,
                ),
              );
              await Navigator.push(context, r);
            },
          ),
        ),
        Container(
          height: 60,
        ),
      ],
    );
  }

  List<TableRow> _getTableHeader() {
    return <TableRow>[
      TableRow(
        children: <Widget>[
          Padding(
            padding: EdgeInsets.only(left: 5),
            child: Text(
              "Udělen za",
              style: TextStyle(
                fontWeight: FontWeight.bold,
              ),
            ),
          ),
          Text(
            "Předmět",
            style: TextStyle(
              fontWeight: FontWeight.bold,
            ),
          ),
          Text(
            "Vyučující",
            style: TextStyle(
              fontWeight: FontWeight.bold,
            ),
          ),
          Text(
            "Akce",
            textAlign: TextAlign.center,
            style: TextStyle(
              fontWeight: FontWeight.bold,
            ),
          ),
          Container(),
        ],
      ),
      TableRow(
        children: <Widget>[
          Divider(
            color: Colors.blue,
          ),
          Divider(
            color: Colors.blue,
          ),
          Divider(
            color: Colors.blue,
          ),
          Divider(
            color: Colors.blue,
          ),
          Container(),
        ],
      ),
    ];
  }

  Widget _userDetails(User u) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: <Widget>[
        _detailLine("Jméno:", u.fullName),
        _detailLine("Třída:", u.className),
        _detailLine("Datum registrace:", Global.getDateString(u.memberSince)),
        u.lastLogin != null
            ? _detailLine(
                "Poslední přihlášení:", Global.getDateString(u.lastLogin))
            : Container(),
        _detailLine("Email:", u.email),
        _detailLine("Pohlaví:", u.sex.name),
      ],
    );
  }

  Widget _detailLine(String title, String content) {
    return Padding(
      padding: EdgeInsets.symmetric(vertical: 8, horizontal: 8),
      child: Row(
        mainAxisAlignment: MainAxisAlignment.start,
        children: <Widget>[
          Text(
            title,
            style: TextStyle(
              fontSize: 17,
            ),
          ),
          Padding(
            padding: EdgeInsets.only(
              left: 10,
            ),
            child: Text(
              content,
              style: TextStyle(
                fontSize: 17,
                fontWeight: FontWeight.bold,
              ),
            ),
          ),
        ],
      ),
    );
  }
}
