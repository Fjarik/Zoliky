import 'package:zoliky_teachers/utils/api/enums/Projects.dart';
import 'package:zoliky_teachers/utils/interfaces/IDbObject.dart';

class ProjectSettings implements IDbObject {
  int projectId;
  String key;
  String value;
  DateTime changed;

  Projects get project => new Projects(this.projectId);

  ProjectSettings.fromJson(Map<String, dynamic> json)
      : projectId = json["ProjectID"],
        key = json["Key"],
        value = json["Value"],
        changed = DateTime.parse(json["Changed"]);
}
