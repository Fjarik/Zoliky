import 'package:flutter/cupertino.dart';

class ZolikCreateModel {
  int toId;
  int subjectId;
  int typeId;
  String title;
  bool allowSplit;

  bool get isValid => (this.toId > 0 &&
      this.subjectId > 0 &&
      this.title != null &&
      this.title.isNotEmpty &&
      this.typeId > 0);

  ZolikCreateModel();

  ZolikCreateModel.create({
    @required this.toId,
    @required this.subjectId,
    @required this.title,
    @required this.typeId,
    this.allowSplit = true,
  });

  Map<String, String> toJson() => {
        "ToID": toId.toString(),
        "SubjectID": subjectId.toString(),
        "Title": title ?? "",
        "Type": typeId.toString(),
        "AllowSplit": allowSplit.toString(),
      };
}
