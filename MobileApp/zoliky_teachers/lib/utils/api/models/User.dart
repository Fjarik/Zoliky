import 'dart:typed_data';

import 'package:intl/intl.dart';
import 'package:zoliky_teachers/utils/Global.dart';
import 'package:zoliky_teachers/utils/api/enums/UserRoles.dart';
import 'package:zoliky_teachers/utils/api/models/Class.dart';
import 'package:zoliky_teachers/utils/api/models/Image.dart';
import 'package:zoliky_teachers/utils/api/models/Rank.dart';
import 'package:zoliky_teachers/utils/api/models/Role.dart';
import 'package:zoliky_teachers/utils/api/models/Zolik.dart';
import 'package:zoliky_teachers/utils/interfaces/IDbObject.dart';

class User implements IDbObject {
  int id;
  int classId;
  int schoolId;
  String uqid;
  String username;
  String email;
  String name;
  String lastname;
  int sex;
  bool _enabled;
  DateTime memberSince;

  String className;
  String schoolName;
  DateTime lastLogin;
  bool isBanned;
  bool isPublicVisible;
  bool emailConfirmed;

  Image profilePhoto;
  List<Zolik> zoliky;
  List<Role> roles;

  String token;

  Class get trida => Global.getClassById(this.classId);
  String get fullName => this.name + " " + this.lastname;
  bool get isEnabled => this._enabled;

  bool get hasTesterRights {
    return this.isInRole(UserRole.Tester);
  }

  Uint8List profileImage;

  Uint8List getProfileImage() {
    if (profileImage == null)
      profileImage = Global.getImageFromBase64(this.profilePhoto.base64);

    return profileImage;
  }

  User();

  bool isInRole(UserRole role) {
    return _isInRole(role.value);
  }

  bool _isInRole(String role) {
    if (roles == null && roles.length < 1) {
      return false;
    }
    return roles.any((r) => r.name == role);
  }

  bool isInRolesOr(List<UserRole> roles) {
    for (UserRole r in roles) {
      if (isInRole(r)) {
        return true;
      }
    }
    return false;
  }

  bool isInRolesAnd(List<UserRole> roles) {
    for (UserRole r in roles) {
      if (!isInRole(r)) {
        return false;
      }
    }
    return true;
  }

  User.demo() {
    id = 0;
    classId = 1;
    uqid = "Ahoj";
    username = "Test";
    email = "test@test.cz";
    name = "Test";
    lastname = "Test";
    sex = 0;
    _enabled = false;
    memberSince = DateTime.now();
    roles = new List();
    zoliky = new List();
  }

  static User fromJson(Map<String, dynamic> json) {
    User u = new User();
    u.id = json["ID"];
    u.classId = json["ClassID"];
    u.schoolId = json["SchoolID"];
    u.uqid = json["UQID"];
    u.username = json["Username"];
    u.email = json["Email"];
    u.name = json["Name"];
    u.lastname = json["Lastname"];
    u.sex = json["Sex"];
    u._enabled = json["IsEnabled"];
    u.memberSince = DateTime.parse(json["MemberSince"]);

    u.className = json["ClassName"];
    u.schoolName = json["SchoolName"];
    u.lastLogin = DateTime.parse(json["LastLoginDate"]);
    u.isBanned = json["IsBanned"];
    u.isPublicVisible = json["IsVisiblePublic"];
    u.emailConfirmed = json["EmailConfirmed"];

    if (json.containsKey("ProfilePhoto") && json["ProfilePhoto"] != null) {
      u.profilePhoto = Image.fromJson(json["ProfilePhoto"]);
    }
    /* if (json.containsKey("Class") && json["Class"] != null) {
      u.trida = Class.fromJson(json["Class"]);
    }*/

    u.roles = new List<Role>();
    if (json.containsKey("Roles") && json["Roles"] != null) {
      List<Map<String, dynamic>>.from(json["Roles"])
          .forEach((map) => u.roles.add(Role.fromJson(map)));
    }

    u.zoliky = new List<Zolik>();
    if (json.containsKey("Zoliky") && json["Zoliky"] != null) {
      List<Map<String, dynamic>>.from(json["Zoliky"])
          .forEach((map) => u.zoliky.add(Zolik.fromJson(map)));
    }
    return u;
  }

  @override
  String toString() {
    String since = new DateFormat('dd.MM.yyyy').format(memberSince);
    return "Id: $id; \n$name $lastname ($username); \nEmail: $email; \nSince: $since";
  }
}
