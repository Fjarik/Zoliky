import 'package:flutter/material.dart';
import 'package:zoliky_teachers/pages/Administration/Zolik/ZolikDetailPage.dart';
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
      body: Padding(
        padding: EdgeInsets.all(10),
        child: Container(
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: <Widget>[
              Text(
                "Obecné informace:",
                style: Theme.of(context).textTheme.headline,
              ),
              Divider(
                color: Colors.blue,
                thickness: 2,
              ),
              _detailLine("Udělen za:", zolik.title),
              _detailLine("Druh:", zolik.zolikType),
              _detailLine("Aktuální vlastník:",
                  "${zolik.ownerName} (${zolik.ownerClass.name})"),
              _detailLine("Předmět:", zolik.subjectName),
              _detailLine("Vyučující:", zolik.teacherName),
              _detailLine("Původní vlastník:", zolik.originalOwnerName),
              _detailLine(
                  "Datum vytvoření:", Global.getDateString(zolik.created)),
              Padding(
                padding: EdgeInsets.only(top: 20),
                child: Text(
                  "Pokročilé:",
                  style: Theme.of(context).textTheme.headline,
                ),
              ),
              Divider(
                color: Colors.blue,
                thickness: 2,
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
                  var list = _getTableHeader();
                  var i = 0;
                  list.addAll(this._transactions.map((Transaction t) {
                    i++;
                    return _transactionWidget(t, isAlternate: i % 2 == 0);
                  }).toList());
                  return Table(
                    columnWidths: {
                      0: FractionColumnWidth(0.13),
                      2: FractionColumnWidth(0.1),
                      4: FractionColumnWidth(0.22),
                      5: FractionColumnWidth(0),
                    },
                    defaultVerticalAlignment: TableCellVerticalAlignment.middle,
                    children: list,
                  );
                },
              ),
            ],
          ),
        ),
      ),
      floatingActionButton: FloatingActionButton.extended(
        onPressed: () {},
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
          child: Text(t.id.toString()),
        ),
        Text(t.from),
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
              "ID",
              style: TextStyle(
                fontWeight: FontWeight.bold,
              ),
            ),
          ),
          Text(
            "Odesílatel",
            style: TextStyle(
              fontWeight: FontWeight.bold,
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
