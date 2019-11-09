import 'package:zoliky_teachers/utils/interfaces/IDbObject.dart';

class Class implements IDbObject {
  int id;
  String name;
  DateTime since;
  DateTime graduation;

  Class();

  Class.fromJson(Map<String, dynamic> json)
      : id = json["ID"],
        name = json["Name"],
        since = DateTime.parse(json["Since"]),
        graduation = DateTime.parse(json["Graduation"]);
}
