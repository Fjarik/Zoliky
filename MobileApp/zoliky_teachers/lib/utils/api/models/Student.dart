import 'package:zoliky_teachers/utils/api/models/Image.dart';
import 'package:zoliky_teachers/utils/interfaces/IDbObject.dart';

class Student implements IDbObject {
  int id;
  int profilePhotoId;
  int classId;
  String name;
  String lastname;
  String className;
  int zolikCount;
  int xp;

  Image profilePhoto;

  String get fullName => this.name + " " + this.lastname;

  Student();

  static Student fromJson(Map<String, dynamic> json) {
    Student u = new Student();
    if (json.containsKey("ID") && json["ID"] != null) {
      u.id = json["ID"];
    }
    if (json.containsKey("ProfilePhotoID") && json["ProfilePhotoID"] != null) {
      u.profilePhotoId = json["ProfilePhotoID"];
    }
    if (json.containsKey("ClassID") && json["ClassID"] != null) {
      u.classId = json["ClassID"];
    }
    if (json.containsKey("Name") && json["Name"] != null) {
      u.name = json["Name"];
    }
    if (json.containsKey("Lastname") && json["Lastname"] != null) {
      u.lastname = json["Lastname"];
    }
    if (json.containsKey("ClassName") && json["ClassName"] != null) {
      u.className = json["ClassName"];
    }
    if (json.containsKey("ZolikCount") && json["ZolikCount"] != null) {
      u.zolikCount = json["ZolikCount"];
    }
    if (json.containsKey("XP") && json["XP"] != null) {
      u.xp = json["XP"];
    }
    if (json.containsKey("ProfilePhoto") && json["ProfilePhoto"] != null) {
      u.profilePhoto = Image.fromJson(json["ProfilePhoto"]);
    } else {
      u.profilePhoto = Image.defaultImage();
    }
    return u;
  }
}
