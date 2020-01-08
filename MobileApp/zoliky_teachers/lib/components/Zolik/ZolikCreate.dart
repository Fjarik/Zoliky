import 'package:firebase_analytics/firebase_analytics.dart';
import 'package:firebase_analytics/observer.dart';
import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:zoliky_teachers/pages/Administration/Zolik/ZolikCreatePage.dart';
import 'package:zoliky_teachers/utils/Global.dart';
import 'package:zoliky_teachers/utils/Singleton.dart';
import 'package:zoliky_teachers/utils/api/connectors/ZolikConnector.dart';
import 'package:zoliky_teachers/utils/api/enums/ZolikType.dart';
import 'package:zoliky_teachers/utils/api/models/Class.dart';
import 'package:zoliky_teachers/utils/api/models/User.dart';
import 'package:zoliky_teachers/utils/api/models/universal/ZolikCreateModel.dart';

class ZolikCreatePageState extends State<ZolikCreatePage> {
  ZolikCreatePageState(this.analytics, this.observer);

  final FirebaseAnalytics analytics;
  final FirebaseAnalyticsObserver observer;

  final GlobalKey<ScaffoldState> _key = GlobalKey<ScaffoldState>();
  final GlobalKey<FormState> _formKey = GlobalKey<FormState>();

  final TextEditingController _txtTitle = TextEditingController();
  final FocusNode _focusTitle = FocusNode();

  User get logged => Singleton().user;

  List<Class> get _classes => Global.classes;

  List<DropdownMenuItem<ZolikType>> get _dropTypes => ZolikType.values
      .map((x) => DropdownMenuItem<ZolikType>(
            value: x,
            child: Text(x.name),
          ))
      .toList();

  List<DropdownMenuItem<int>> get _dropSubjects => Global.subjects
      .map((x) => DropdownMenuItem<int>(
            value: x.id,
            child: Text(x.name),
          ))
      .toList();

  List<DropdownMenuItem<int>> get _dropClasses => _classes
      .map((x) => DropdownMenuItem<int>(
            value: x.id,
            child: Text(x.name),
          ))
      .toList();

  List<DropdownMenuItem<int>> get _dropStudents =>
      Global.students
          .where((x) => x.id != logged.id && x.classId == classId)
          .map((x) => DropdownMenuItem<int>(
                value: x.id,
                child: Text(x.fullName),
              ))
          .toList() ??
      List<DropdownMenuItem<int>>();

  bool isLoading = false;
  int subjectId;
  int classId;
  int studentId;
  ZolikType zolikType;
  bool allowSplit = true;
  int _stepIndex = 0;

  void _setLoading(bool value) {
    setState(() {
      isLoading = value;
    });
  }

  Future<void> create() async {
    if (isLoading) {
      return;
    }
    if (!_formKey.currentState.validate()) {
      return;
    }
    FocusScope.of(context).unfocus();

    _setLoading(true);

    var title = _txtTitle.text;
    if (title == null || title.isEmpty) {
      _setLoading(false);
      _showSnackbar("Neplatné vstupní údaje");
      return;
    }

    var allow = this.zolikType.isSplittable ? this.allowSplit : true;

    var model = ZolikCreateModel.create(
      toId: this.studentId,
      subjectId: this.subjectId,
      title: title,
      type: this.zolikType,
      allowSplit: allow,
    );

    var zc = ZolikConnector(Singleton().token);

    var res = await zc.createAsync(model);
    if (!res.isSuccess) {
      _setLoading(false);
      _showSnackbar(res.getStatusMessage());
      return;
    }

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
  void initState() {
    this.classId = _classes.first.id;
    super.initState();
  }

  @override
  void dispose() {
    _txtTitle?.dispose();
    _focusTitle?.dispose();

    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return isLoading
        ? Scaffold(
            body: Global.loading(text: "Vytváří se žolík"),
          )
        : Scaffold(
            key: _key,
            resizeToAvoidBottomPadding: false,
            appBar: AppBar(
              elevation: 2,
              // backgroundColor: Colors.white,
              title: Text(
                "Vytvoření žolíka",
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
              padding: EdgeInsets.only(bottom: 50),
              child: Container(
                child: Form(
                  key: _formKey,
                  child: Stepper(
                    steps: [
                      Step(
                        title: Text("Důvod udělení"),
                        subtitle: Text("Za co je žolík udělen"),
                        content: TextFormField(
                          controller: _txtTitle,
                          focusNode: _focusTitle,
                          autocorrect: true,
                          autofocus: false,
                          keyboardType: TextInputType.text,
                          textCapitalization: TextCapitalization.sentences,
                          textInputAction: TextInputAction.done,
                          maxLength: 200,
                          maxLines: 1,
                          minLines: 1,
                          onEditingComplete: () {
                            FocusScope.of(context).unfocus();
                          },
                          decoration: InputDecoration(
                            hintText: "Zadejte důvod udělení",
                          ),
                          onChanged: (String val) {
                            _formKey.currentState.validate();
                          },
                          validator: (String value) {
                            if (value == null || value.isEmpty) {
                              return "Musíte zadat důvod udělení";
                            }
                            if (value.length < 4) {
                              return "Důvod udělení musí být dlouhý minimálně 4 znaky";
                            }
                            return null;
                          },
                        ),
                      ),
                      Step(
                        title: Text("Nastavení"),
                        subtitle: Text("Typ žolíka"),
                        content: Column(
                          crossAxisAlignment: CrossAxisAlignment.start,
                          children: <Widget>[
                            DropdownButtonFormField<ZolikType>(
                              items: _dropTypes,
                              value: zolikType,
                              hint: Text("Vyberte druh žolíka"),
                              onChanged: (ZolikType value) {
                                _formKey.currentState.validate();
                                setState(() {
                                  zolikType = value;
                                  allowSplit = value.isSplittable;
                                });
                              },
                              validator: (ZolikType value) {
                                if (value == null) {
                                  return "Musíte vybrat druh žolíka";
                                }
                                return null;
                              },
                            ),
                            Visibility(
                              visible: zolikType?.isSplittable ?? false,
                              child: _detailLineWidget(
                                "Povolit rozdělení",
                                Switch(
                                  value: allowSplit,
                                  onChanged: (bool value) {
                                    setState(() {
                                      allowSplit = value;
                                    });
                                  },
                                ),
                                mainAxisAlignment:
                                    MainAxisAlignment.spaceBetween,
                              ),
                            ),
                          ],
                        ),
                      ),
                      Step(
                        title: Text("Předmět"),
                        content: DropdownButtonFormField<int>(
                          items: _dropSubjects,
                          value: subjectId,
                          hint: Text("Vyberte předmět"),
                          onChanged: (int value) {
                            _formKey.currentState.validate();
                            setState(() {
                              subjectId = value;
                            });
                          },
                          validator: (int value) {
                            if (value == null) {
                              return "Musíte vybrat předmět";
                            }
                            if (value < 1) {
                              return "Musíte vybrat platný předmět";
                            }
                            return null;
                          },
                        ),
                      ),
                      Step(
                        title: Text("Příjemce"),
                        content: Column(
                          crossAxisAlignment: CrossAxisAlignment.start,
                          children: <Widget>[
                            _detailLineWidget("Třída:", Container()),
                            DropdownButtonFormField<int>(
                              items: _dropClasses,
                              value: classId,
                              hint: Text("Vyberte třídu"),
                              onChanged: (int value) {
                                _formKey.currentState.validate();
                                setState(() {
                                  classId = value;
                                  studentId = null;
                                });
                              },
                              validator: (int value) {
                                if (value == null) {
                                  return "Musíte vybrat třídu";
                                }
                                if (value < 1) {
                                  return "Musíte vybrat platnou třídu";
                                }
                                return null;
                              },
                            ),
                            _detailLineWidget("Vlastník:", Container()),
                            DropdownButtonFormField<int>(
                              items: _dropStudents,
                              value: studentId,
                              hint: Text("Vyberte studenta"),
                              onChanged: (int value) {
                                _formKey.currentState.validate();
                                setState(() {
                                  studentId = value;
                                });
                              },
                              validator: (int value) {
                                if (value == null) {
                                  return "Musíte vybrat studenta";
                                }
                                if (value < 1) {
                                  return "Musíte vybrat platného studenta";
                                }
                                return null;
                              },
                            ),
                          ],
                        ),
                      ),
                    ],
                    currentStep: _stepIndex,
                    onStepTapped: (index) {
                      _focusTitle?.unfocus();
                      setState(() {
                        _stepIndex = index;
                      });
                    },
                    onStepContinue: () {
                      _focusTitle?.unfocus();
                      setState(() {
                        _stepIndex++;
                      });
                    },
                    onStepCancel: () {
                      _focusTitle?.unfocus();
                      setState(() {
                        _stepIndex = _stepIndex == 0 ? 0 : _stepIndex - 1;
                      });
                    },
                    controlsBuilder: (BuildContext context,
                        {VoidCallback onStepContinue,
                        VoidCallback onStepCancel}) {
                      return Row(
                        children: <Widget>[
                          _stepIndex != 0
                              ? MaterialButton(
                                  child: Text("Zpět"),
                                  onPressed: onStepCancel,
                                )
                              : Container(),
                          _stepIndex != 3
                              ? MaterialButton(
                                  child: Text("Pokračovat"),
                                  onPressed: onStepContinue,
                                )
                              : Container(),
                        ],
                      );
                    },
                  ),
                ),
              ),
            ),
            floatingActionButton: FloatingActionButton.extended(
              onPressed: create,
              label: Text("Vytvořit"),
              icon: Icon(Icons.check),
              backgroundColor: Colors.blue,
            ),
          );
  }

  Widget _detailLineWidget(String title, Widget content,
      {MainAxisAlignment mainAxisAlignment = MainAxisAlignment.start}) {
    return Padding(
      padding: EdgeInsets.symmetric(vertical: 8),
      child: Row(
        mainAxisAlignment: mainAxisAlignment,
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
