import 'dart:convert';

import 'package:zoliky_teachers/utils/api/connectors/PublicConnector.dart';
import 'package:zoliky_teachers/utils/api/enums/StatusCode.dart';
import 'package:zoliky_teachers/utils/api/models/Class.dart';
import 'package:zoliky_teachers/utils/api/models/universal/MActionResult.dart';

class ClassConnector extends PublicConnector {
  ClassConnector(String token) {
    this.usedToken = token;
  }

  Future<MActionResult<Class>> getAsync(int id) async {
    if (id < 1) {
      return new MActionResult<Class>().ctorOnlyStatus(StatusCode.NotValidID);
    }
    try {
      var url = "$urlApi/class/get/$id";
      var res =
          await cli.get(url, headers: {"Authorization": "Bearer $usedToken"});

      if (res.statusCode != 200) {
        return new MActionResult<Class>()
            .ctorOnlyStatus(StatusCode.InternalError);
      }

      String body = res.body;

      if (body.isEmpty) {
        return new MActionResult<Class>().ctorOnlyStatus(StatusCode.NotFound);
      }

      var _json = json.decode(body);

      _json["Content"] = Class.fromJson(_json["Content"]);

      var ws = MActionResult<Class>.fromJson(_json);
      return ws;
    } catch (ex) {
      return new MActionResult<Class>().ctorWithexception(ex);
    }
  }

  Future<List<Class>> getClassesAsync() async {
    try {
      var url = "$urlApi/class/getall";
      var res =
          await cli.get(url, headers: {"Authorization": "Bearer $usedToken"});

      if (res.statusCode != 200) {
        return new List<Class>();
      }

      String body = res.body;

      if (body.isEmpty) {
        return new List<Class>();
      }

      var content = json.decode(body);
      var classes = new List<Class>();
      if (content != null && content.length > 0) {
        content.forEach((map) => classes.add(Class.fromJson(map)));
      }
      return classes;
    } catch (ex) {
      return new List<Class>();
    }
  }
}
