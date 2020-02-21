import 'package:zoliky_teachers/utils/Global.dart';
import 'package:zoliky_teachers/utils/api/models/Class.dart';
import 'package:zoliky_teachers/utils/api/models/ZolikType.dart';
import 'package:zoliky_teachers/utils/interfaces/IDbObject.dart';

class Zolik implements IDbObject {
  int id;
  int ownerId;
  int subjectId;
  int teacherId;
  int originalOwnerId;
  int schoolId;
  int ownerClassId;
  int typeId;

  String title;
  bool enabled;
  DateTime ownerSince;
  DateTime created;
  String lock;
  String subjectName;
  String teacherName;
  String ownerName;
  String originalOwnerName;
  bool canBeTransfered;
  bool isLocked;
  bool isSplittable;

  Zolik();

  ZolikType get type => Global.getTypeById(this.typeId);

  String get zolikType => this.type.friendlyName;

  Class get ownerClass => Global.getClassById(this.ownerClassId);

  Zolik.fromJson(Map<String, dynamic> json)
      : id = json["ID"],
        ownerId = json["OwnerID"],
        subjectId = json["SubjectID"],
        teacherId = json["TeacherID"],
        schoolId = json["SchoolID"],
        originalOwnerId = json["OriginalOwnerID"],
        ownerClassId = json["OwnerClassId"],
        typeId = json["TypeID"],
        title = json["Title"],
        subjectName = json["SubjectName"],
        teacherName = json["TeacherName"],
        ownerName = json["OwnerName"],
        originalOwnerName = json["OriginalOwnerName"],
        enabled = json["Enabled"],
        ownerSince = DateTime.parse(json["OwnerSince"]),
        created = DateTime.parse(json["Created"]),
        lock = json["Lock"],
        canBeTransfered = json["CanBeTransfered"],
        isLocked = json["IsLocked"],
        isSplittable = json["IsSplittable"];

  Zolik.fromJsonBase(Map<String, String> json)
      : typeId = int.parse(json["Type"]),
        title = json["Title"];
}
