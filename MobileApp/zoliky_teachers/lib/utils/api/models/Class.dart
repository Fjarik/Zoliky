import 'package:zoliky_teachers/utils/Global.dart';
import 'package:zoliky_teachers/utils/Singleton.dart';
import 'package:zoliky_teachers/utils/interfaces/IDbObject.dart';

class Class implements IDbObject {
  int id;
  String name;
  DateTime since;
  DateTime graduation;

  int get studentsCount =>
      Global.students.where((x) => x.classId == this.id).length;

  double get zolikCount =>
      Singleton()
          .classLeaderboard
          ?.firstWhere((x) => x.label == this.name)
          ?.data ??
      0.0;

  Class();

  Class.fromJson(Map<String, dynamic> json)
      : id = json["ID"],
        name = json["Name"],
        since = DateTime.parse(json["Since"]),
        graduation = DateTime.parse(json["Graduation"]);
}
