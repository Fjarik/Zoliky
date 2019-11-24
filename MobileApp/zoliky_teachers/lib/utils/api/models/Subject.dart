import 'package:zoliky_teachers/utils/interfaces/IDbObject.dart';

class Subject implements IDbObject {
  int id;
  String name;
  String shortcut;

  Subject();

  Subject.fromJson(Map<String, dynamic> json)
      : id = json["ID"],
        name = json["Name"],
        shortcut = json["Shortcut"];
}
