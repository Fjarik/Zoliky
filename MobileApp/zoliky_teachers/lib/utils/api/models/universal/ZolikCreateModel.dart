import 'package:flutter/cupertino.dart';
import 'package:zoliky_teachers/utils/api/enums/ZolikType.dart';

class ZolikCreateModel {
  int toId;
  int subjectId;
  String title;
  ZolikType type;
  bool allowSplit;

  bool get isValid => (this.toId > 0 &&
      this.subjectId > 0 &&
      this.title != null &&
      this.title.isNotEmpty &&
      this.type != null);

  ZolikCreateModel();

  ZolikCreateModel.create({
    @required this.toId,
    @required this.subjectId,
    @required this.title,
    @required this.type,
    this.allowSplit = true,
  });

  Map<String, String> toJson() => {
        "ToID": toId.toString(),
        "SubjectID": subjectId.toString(),
        "Title": title ?? "",
        "Type": type.value.toString(),
        "AllowSplit": allowSplit.toString(),
      };
}
