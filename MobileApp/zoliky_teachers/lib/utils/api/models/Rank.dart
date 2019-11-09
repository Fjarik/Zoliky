import 'package:zoliky_teachers/utils/interfaces/IDbObject.dart';

class Rank implements IDbObject {
  int id;
  String title;
  int fromXP;
  int toXP;
  String colour;

  Rank.fromJson(Map<String, dynamic> json)
      : id = json["ID"],
        title = json["Title"],
        fromXP = json["FromXP"],
        toXP = json["ToXP"],
        colour = json["Colour"];
}
