import 'package:firebase_analytics/firebase_analytics.dart';
import 'package:firebase_analytics/observer.dart';
import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:zoliky_teachers/pages/Administration/Zolik/ZolikRemovePage.dart';
import 'package:zoliky_teachers/utils/Global.dart';
import 'package:zoliky_teachers/utils/Singleton.dart';
import 'package:zoliky_teachers/utils/api/connectors/ZolikConnector.dart';
import 'package:zoliky_teachers/utils/api/models/Zolik.dart';
import 'package:zoliky_teachers/utils/api/models/universal/ZolikRemoveModel.dart';

class ZolikRemovePageState extends State<ZolikRemovePage> {
  ZolikRemovePageState(this.analytics, this.observer, this.zolik);

  final GlobalKey<ScaffoldState> _key = GlobalKey<ScaffoldState>();
  final GlobalKey<FormState> _formKey = GlobalKey<FormState>();

  final TextEditingController _txtReason = TextEditingController();
  final FocusNode _focusReason = FocusNode();

  final FirebaseAnalytics analytics;
  final FirebaseAnalyticsObserver observer;
  final Zolik zolik;

  bool loading = false;

  @override
  void initState() {
    if (zolik.lock != null && zolik.lock.isNotEmpty) {
      _txtReason.text = zolik.lock;
    }

    super.initState();
  }

  void _setLoading(bool value) {
    setState(() {
      loading = value;
    });
  }

  Future<void> _remove() async {
    if (loading) {
      return;
    }
    if (!_formKey.currentState.validate()) {
      if (_focusReason.canRequestFocus) {
        FocusScope.of(context).requestFocus(_focusReason);
      }
      SystemChannels.textInput.invokeMethod("TextInput.show");
      return;
    }
    FocusScope.of(context).unfocus();

    _setLoading(true);

    var reason = _txtReason.text;
    if (reason == null || reason.isEmpty) {
      _setLoading(false);
      _showSnackbar("Neplatné vstupní údaje");
      return;
    }

    var package = ZolikRemoveModel();
    package.reason = reason;
    package.zolikId = zolik.id;

    if (!package.isValid) {
      _setLoading(false);
      _showSnackbar("Neplatné vstupní údaje");
      return;
    }

    var zc = ZolikConnector(Singleton().token);

    var res = await zc.removeAsync(package);
    if (!res.isSuccess) {
      _setLoading(false);
      _showSnackbar(res.getStatusMessage());
      return;
    }

    Singleton().zoliks.remove(zolik);

    if (Navigator.of(context).canPop()) {
      Navigator.of(context).popUntil(ModalRoute.withName("/dashboard"));
    }
  }

  void _showSnackbar(String content) {
    var snack = SnackBar(
      content: Text(content),
      backgroundColor: Colors.red[700],
      duration: Duration(seconds: 2),
    );
    _key.currentState.showSnackBar(snack);
  }

  @override
  void dispose() {
    _txtReason?.dispose();
    _focusReason?.dispose();

    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return loading
        ? Scaffold(
            body: Global.loading(),
          )
        : Scaffold(
            key: _key,
            resizeToAvoidBottomPadding: false,
            appBar: AppBar(
              elevation: 2,
              // backgroundColor: Colors.white,
              title: Text(
                "Odstranění žolíka",
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
              child: Padding(
                padding: EdgeInsets.symmetric(horizontal: 5),
                child: Container(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: <Widget>[
                      Padding(
                        padding: EdgeInsets.symmetric(vertical: 12),
                        child: Text(
                          "Opravdu si přejete odstranit tohoto žolíka?",
                          textAlign: TextAlign.center,
                          style: TextStyle(
                            color: Colors.orange[400],
                            fontSize: 24,
                          ),
                        ),
                      ),
                      Padding(
                        padding: EdgeInsets.only(
                          bottom: 30,
                        ),
                        child: Text(
                          "Žolík bude pouze deaktivován a předán do umělého účtu (Národní banka Žolíků).",
                          style: TextStyle(
                            color: Theme.of(context).textTheme.caption.color,
                            fontSize: 16,
                          ),
                        ),
                      ),
                      Text(
                        "Obecné informace:",
                        style: Theme.of(context).textTheme.title,
                      ),
                      Divider(
                        color: Colors.blue,
                        thickness: 2,
                      ),
                      _detailLine("Udělen za (ID):",
                          content: "${zolik.title} (${zolik.id})"),
                      _detailLine("Aktuální vlastník:",
                          content:
                              "${zolik.ownerName} (${zolik.ownerClass.name})"),
                      _detailLine("Důvod odstranění:"),
                      Padding(
                        padding: EdgeInsets.symmetric(horizontal: 8),
                        child: Form(
                          key: _formKey,
                          autovalidate: false,
                          child: TextFormField(
                            controller: this._txtReason,
                            focusNode: this._focusReason,
                            autocorrect: true,
                            autofocus: false,
                            keyboardType: TextInputType.text,
                            minLines: 1,
                            maxLines: 1,
                            maxLength: 50,
                            obscureText: false,
                            textCapitalization: TextCapitalization.sentences,
                            textInputAction: TextInputAction.done,
                            onEditingComplete: () {
                              FocusScope.of(context).unfocus();
                            },
                            validator: (String value) {
                              if (value == null || value.isEmpty) {
                                return "Musíte zadat důvod odstranění";
                              }
                              if (value.length < 4) {
                                return "Důvod musí být dlouhý minimálně 4 znaky";
                              }
                              return null;
                            },
                            onChanged: (String val) {
                              _formKey.currentState.validate();
                            },
                            decoration: InputDecoration(
                              hintText: "Zadejte důvod odstranění",
                            ),
                          ),
                        ),
                      ),
                    ],
                  ),
                ),
              ),
            ),
            floatingActionButton: FloatingActionButton.extended(
              onPressed: _remove,
              label: Text("Odstranit"),
              icon: Icon(Icons.delete),
              backgroundColor: Colors.red,
            ),
          );
  }

  Widget _detailLine(String title, {String content}) {
    return _detailLineWidget(
      title,
      content == null || content.isEmpty
          ? Container()
          : Text(
              content,
              style: TextStyle(
                fontSize: 17,
                fontWeight: FontWeight.bold,
              ),
            ),
    );
  }

  Widget _detailLineWidget(String title, Widget content) {
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
            child: content,
          ),
        ],
      ),
    );
  }
}
