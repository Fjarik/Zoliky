import 'dart:convert';

import 'package:zoliky_teachers/utils/api/connectors/PublicConnector.dart';
import 'package:zoliky_teachers/utils/api/enums/Projects.dart';
import 'package:zoliky_teachers/utils/api/enums/StatusCode.dart';
import 'package:zoliky_teachers/utils/api/models/ProjectSettings.dart';
import 'package:zoliky_teachers/utils/api/models/universal/MActionResult.dart';

class ProjectSettingsConnector extends PublicConnector {
  ProjectSettingsConnector(String token) : super() {
    this.usedToken = token;
  }

  Future<MActionResult<ProjectSettings>> getAsync(String key,
      [Projects project]) async {
    try {
      var url = "$urlApi/projectSettings/get?key=$key&project=$project";
      var res =
          await cli.get(url, headers: {"Authorization": "Bearer $usedToken"});

      if (res.statusCode != 200) {
        return new MActionResult<ProjectSettings>()
            .ctorOnlyStatus(StatusCode.InternalError);
      }

      String body = res.body;

      if (body.isEmpty) {
        return new MActionResult<ProjectSettings>()
            .ctorOnlyStatus(StatusCode.NotFound);
      }

      var _json = json.decode(body);
      var content = _json['Content'];
      if (content != null && content.toString().isNotEmpty) {
        _json['Content'] = ProjectSettings.fromJson(content);
      }

      var ws = MActionResult<ProjectSettings>.fromJson(_json);
      return ws;
    } catch (ex) {
      return new MActionResult<ProjectSettings>().ctorWithexception(ex);
    }
  }

  Future<List<ProjectSettings>> getAllAsync() async {
    try {
      var url = "$urlApi/projectSettings/getall";
      var res =
          await cli.get(url, headers: {"Authorization": "Bearer $usedToken"});

      if (res.statusCode != 200) {
        return new List<ProjectSettings>();
      }

      String body = res.body;

      if (body.isEmpty) {
        return new List<ProjectSettings>();
      }

      var content = json.decode(body);
      var settings = new List<ProjectSettings>();
      if (content != null && content.length > 0) {
        content.forEach((map) => settings.add(ProjectSettings.fromJson(map)));
      }
      return settings;
    } catch (ex) {
      return new List<ProjectSettings>();
    }
  }
}
