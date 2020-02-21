import 'package:zoliky_teachers/utils/interfaces/IDbObject.dart';

class ZolikType implements IDbObject {
  int id;
  //String name;
  String friendlyName;
  bool isSplittable;
  bool isTestType;
  bool allowGive;

  ZolikType();

  ZolikType.fromJson(Map<String, dynamic> json)
      : id = json["ID"],
    //    name = json["Name"],
        friendlyName = json["FriendlyName"],
        isSplittable = json["IsSplittable"],
        isTestType = json["IsTestType"],
        allowGive = json["AllowGive"];
}
