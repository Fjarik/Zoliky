import 'dart:convert';

import 'package:zoliky_teachers/utils/api/connectors/MainClient.dart';
import 'package:zoliky_teachers/utils/api/enums/Projects.dart';
import 'package:zoliky_teachers/utils/api/enums/StatusCode.dart';
import 'package:zoliky_teachers/utils/api/models/Unavailability.dart';
import 'package:zoliky_teachers/utils/api/models/WebStatus.dart';
import 'package:zoliky_teachers/utils/api/models/universal/MActionResult.dart';

class PublicConnector {
  final String urlApi = "https://api.zoliky.eu/";
  // final String urlApi = "http://169.254.106.111:93/";

  String get _defaultName => "user";
  String get _defaultPassword => "It9ac8kw";
  String get _currentProject => Projects.FlutterTeacher.value.toString();

  Future<String> get loginToken async =>
      (await getToken(_defaultName, _defaultPassword)).content;

  MainClient cli;

  String usedToken;

  PublicConnector() {
    cli = new MainClient();
  }

  Future<MActionResult<String>> getToken(String name, String pwd) async {
    if (name.isEmpty || pwd.isEmpty) {
      return MActionResult<String>().ctorOnlyStatus(StatusCode.InvalidInput);
    }

    var map = {
      "grant_type": "password",
      "username": name,
      "password": pwd,
    };
    return await _getToken(map);
  }

  Future<MActionResult<String>> getFbToken(String token, String provider) async {
    if (token.isEmpty) {
      return MActionResult<String>().ctorOnlyStatus(StatusCode.InvalidInput);
    }

    var map = {
      "grant_type": provider,
      "key": token,
    };
    return await _getToken(map);
  }

  Future<MActionResult<String>> _getToken(Map<String, String> map) async {
    try {
      var res = await cli.post("$urlApi/token",
          headers: {
            "projectId": _currentProject,
          },
          body: map);

      if (res.statusCode != 200 && res.statusCode != 400) {
        return MActionResult<String>().ctorOnlyStatus(StatusCode.InternalError);
      }

      var body = res.body;

      if (body.isEmpty) {
        return MActionResult<String>().ctorOnlyStatus(StatusCode.NotFound);
      }

      var stuff = json.decode(body);
      if (res.statusCode == 400) {
        if (stuff["error"] == "invalid_grant") {
          return MActionResult<String>()
              .ctorWithexception(stuff["error_description"]);
        }
        var code = int.parse(stuff["error"]);
        return MActionResult<String>().ctorOnlyStatus(StatusCode(code));
      }

      if (stuff["access_token"] == null) {
        return MActionResult<String>().ctorOnlyStatus(StatusCode.WrongPassword);
      }
      return MActionResult<String>()
          .ctorWithContent(StatusCode.OK, stuff["access_token"]);
    } catch (ex) {
      return MActionResult<String>().ctorWithexception(ex);
    }
  }

  Future<String> getDate() async {
    String url = this.urlApi + "public/time";
    var res = await cli.get(url);
    return Future.delayed(Duration(seconds: 2), () => res.body);
  }

  Future<WebStatus> checkStatusAsync() async {
    String url = this.urlApi + "public/status";
    url += "?projectId=${Projects.Flutter.value}";
    var ws = WebStatus.unFunctional();
    try {
      var res = await cli.get(url);
      String body = res.body;
      if (body.isEmpty) {
        throw new Error();
      }
      var _json = json.decode(body);
      ws = WebStatus.fromJson(_json);
      if (ws.content != null) {
        ws.content = Unavailability.fromJson(ws.content);
      }
    } catch (ex) {
      ws.content = ex;
    }
    return Future<WebStatus>.value(ws);
  }
}
