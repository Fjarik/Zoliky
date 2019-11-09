import 'package:zoliky_teachers/utils/interfaces/IDbObject.dart';

class Unavailability extends IDbObject {
  int id;
  int projectID;
  String reason;
  String description;
  DateTime from;
  DateTime to;

  Unavailability();

  Unavailability.fromJson(Map<String, dynamic> json)
      : id = json['ID'],
        projectID = json['ProjectID'],
        reason = json['Reason'],
        description = json['Description'],
        from = DateTime.parse(json['From']),
        to = DateTime.parse(json['To']);
}
