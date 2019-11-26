import 'package:flutter/material.dart';
import 'package:zoliky_teachers/pages/Administration/Zolik/ZolikDetailPage.dart';
import 'package:zoliky_teachers/pages/Administration/Zolik/ZolikRemovePage.dart';
import 'package:zoliky_teachers/utils/Global.dart';
import 'package:zoliky_teachers/utils/Singleton.dart';
import 'package:zoliky_teachers/utils/api/connectors/TransactionConnector.dart';
import 'package:zoliky_teachers/utils/api/models/Transaction.dart';
import 'package:zoliky_teachers/utils/api/models/Zolik.dart';

class ZolikDetailState extends State<ZolikDetailPage> {
  ZolikDetailState(Zolik z) {
    zolik = z;
  }

  TransactionConnector get _tConnector =>
      TransactionConnector(Singleton().token);

  Future<List<Transaction>> get _future =>
      _tConnector.getZolikTransactionsAsync(zolik.id);

  Zolik zolik;

  List<Transaction> _transactions;

  Future<void> _removeZolikClick(Zolik selected) async {
    var r = MaterialPageRoute(
      builder: (BuildContext ctx) => ZolikRemovePage(
        zolik: selected,
      ),
    );
    await Navigator.push(context, r);
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        elevation: 2,
        backgroundColor: Colors.white,
        title: Text(
          "Detaily žolíka",
          style: TextStyle(
            color: Colors.black,
            fontWeight: FontWeight.w700,
            fontSize: 30.0,
          ),
        ),
        leading: IconButton(
          icon: Icon(
            Icons.arrow_back,
            color: Colors.black,
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
              _detailLine("Udělen za:", zolik.title),
              _detailLine("Druh:", zolik.zolikType),
              _detailLine("Aktuální vlastník:",
                  "${zolik.ownerName} (${zolik.ownerClass.name})"),
              _detailLine("Předmět:", zolik.subjectName),
              _detailLine("Vyučující:", zolik.teacherName),
              _detailLine("Původní vlastník:", zolik.originalOwnerName),
              _detailLine(
                  "Datum vytvoření:", Global.getDateString(zolik.created)),
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
                  "Transakce:",
                  style: TextStyle(
                    fontSize: 18,
                  ),
                ),
              ),
              FutureBuilder(
                future: _future,
                builder: (BuildContext ctx,
                    AsyncSnapshot<List<Transaction>> snapshot) {
                  if (snapshot.connectionState != ConnectionState.done) {
                    return Global.loading();
                  }

                  if (snapshot.data != null) {
                    this._transactions = snapshot.data;
                  }

                  /// Duplikování
                  // var a = snapshot.data.asMap();
                  // this._transactions.addAll(List.from(a.values));

                  var list = _getTableHeader();
                  var i = 0;
                  list.addAll(this._transactions.map((Transaction t) {
                    i++;
                    return _transactionWidget(t, isAlternate: i % 2 == 0);
                  }).toList());
                  return Padding(
                    padding: EdgeInsets.only(bottom: 70),
                    child: Table(
                      columnWidths: {
                        0: FlexColumnWidth(3),
                        1: FlexColumnWidth(1),
                        2: FlexColumnWidth(3),
                        3: FlexColumnWidth(2),
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
      floatingActionButton: FloatingActionButton.extended(
        onPressed: () => _removeZolikClick(zolik),
        label: Text("Odstranit"),
        icon: Icon(Icons.delete),
        backgroundColor: Colors.red,
      ),
    );
  }

  TableRow _transactionWidget(Transaction t, {bool isAlternate = false}) {
    return TableRow(
      decoration: BoxDecoration(
        color: isAlternate ? Colors.blue[100] : Colors.transparent,
      ),
      children: <Widget>[
        Padding(
          padding: EdgeInsets.only(left: 5),
          child: Text(t.from),
        ),
        Text(
          "→",
          textAlign: TextAlign.center,
        ),
        Text(t.to),
        Padding(
          padding: EdgeInsets.only(right: 5),
          child: Text(Global.getDateString(t.date)),
        ),
        Container(
          height: 50,
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
              "Odesílatel",
              style: TextStyle(
                fontWeight: FontWeight.bold,
              ),
            ),
          ),
          Text(
            "→",
            textAlign: TextAlign.center,
            style: TextStyle(
              fontWeight: FontWeight.bold,
            ),
          ),
          Text(
            "Příjemce",
            style: TextStyle(
              fontWeight: FontWeight.bold,
            ),
          ),
          Text(
            "Datum",
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
