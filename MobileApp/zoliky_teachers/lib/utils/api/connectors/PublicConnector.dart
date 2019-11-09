import 'dart:convert';

import 'package:zoliky_teachers/utils/api/connectors/MainClient.dart';
import 'package:zoliky_teachers/utils/api/enums/Projects.dart';
import 'package:zoliky_teachers/utils/api/models/Unavailability.dart';
import 'package:zoliky_teachers/utils/api/models/WebStatus.dart';

class PublicConnector {
  final String urlApi = "https://api.zoliky.eu/";

  String get _defaultName => "user";
  String get _defaultPassword => "It9ac8kw";

  Future<String> get loginToken async =>
      await getToken(_defaultName, _defaultPassword);

  MainClient cli;

  String usedToken;

  PublicConnector() {
    cli = new MainClient();
  }

  Future<String> getToken(String name, String pwd) async {
    if (name.isEmpty || pwd.isEmpty) {
      name = _defaultName;
      pwd = _defaultPassword;
    }

    var map = {
      //"Content-type": "x-www-form-urlencoded",
      "grant_type": "password",
      "username": name,
      "password": pwd,
    };

    try {
      var res = await cli.post("$urlApi/token", body: map);

      if (res.statusCode != 200) {
        return "";
      }

      String body = res.body;

      if (body.isEmpty) {
        return "";
      }

      var stuff = json.decode(body);
      if (stuff["access_token"] == null) {
        return "";
      }
      return stuff["access_token"];
    } catch (ex) {
      return "";
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
