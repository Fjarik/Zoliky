import 'dart:convert';

import 'package:zoliky_teachers/utils/Singleton.dart';
import 'package:zoliky_teachers/utils/api/connectors/MainClient.dart';
import 'package:zoliky_teachers/utils/api/enums/Projects.dart';
import 'package:zoliky_teachers/utils/api/enums/StatusCode.dart';
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

  Future<bool> getBoolAsync(String url) async {
    if (url == null || url.isEmpty) {
      return false;
    }
    var res = await cli.get(url);
    var body = res.body;
    if (body == null || body.isEmpty) {
      return false;
    }
    return body.toLowerCase() == "true";
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

  Future<MActionResult<String>> getFbToken(
      String token, String provider) async {
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
        return MActionResult<String>().ctorOnlyStatus(StatusCode.NotFound);
      }
      var token = stuff["access_token"];
      Singleton().token = token;
      return MActionResult<String>().ctorWithContent(StatusCode.OK, token);
    } catch (ex) {
      return MActionResult<String>().ctorWithexception(ex);
    }
  }

  Future<String> getDate() async {
    String url = this.urlApi + "public/time";
    var res = await cli.get(url);
    return Future.delayed(Duration(seconds: 2), () => res.body);
  }

  Future<bool> checkConnectionAsync() {
    return getBoolAsync(this.urlApi + "public/connection");
  }

  Future<bool> checkDbConnectionAsync() {
    return getBoolAsync(this.urlApi + "public/dbconnection");
  }

  Future<bool> checkConnectionsAsync() async {
    var api = await this.checkConnectionAsync();
    var db = await this.checkDbConnectionAsync();

    return api && db;
  }
}
