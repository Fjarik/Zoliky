import 'package:zoliky_teachers/utils/interfaces/IDbObject.dart';

class Role implements IDbObject {
  int id;
  String name;
  String friendlyName;
  String description;

  Role.fromJson(Map<String, dynamic> json)
      : id = json["ID"],
        name = json["Name"],
        friendlyName = json["FriendlyName"],
        description = json["Description"];
}
