import 'dart:convert';

import 'package:zoliky_teachers/utils/api/connectors/PublicConnector.dart';
import 'package:zoliky_teachers/utils/api/enums/StatusCode.dart';
import 'package:zoliky_teachers/utils/api/models/Transaction.dart';
import 'package:zoliky_teachers/utils/api/models/Zolik.dart';
import 'package:zoliky_teachers/utils/api/models/universal/MActionResult.dart';
import 'package:zoliky_teachers/utils/api/models/universal/ZolikCreateModel.dart';
import 'package:zoliky_teachers/utils/api/models/universal/ZolikRemoveModel.dart';

class ZolikConnector extends PublicConnector {
  ZolikConnector(String token) {
    this.usedToken = token;
  }

  Future<MActionResult<Zolik>> getAsync(int zolikId) async {
    if (zolikId < 1) {
      return new MActionResult<Zolik>().ctorOnlyStatus(StatusCode.NotValidID);
    }
    try {
      var url = "$urlApi/zolik/get/$zolikId";
      var res =
          await cli.get(url, headers: {"Authorization": "Bearer $usedToken"});

      if (res.statusCode != 200) {
        return new MActionResult<Zolik>()
            .ctorOnlyStatus(StatusCode.InternalError);
      }

      String body = res.body;

      if (body.isEmpty) {
        return new MActionResult<Zolik>().ctorOnlyStatus(StatusCode.NotFound);
      }

      var _json = json.decode(body);

      _json["Content"] = Zolik.fromJson(_json["Content"]);

      MActionResult<Zolik> ws = MActionResult.fromJson(_json);
      return Future<MActionResult<Zolik>>.value(ws);
    } catch (ex) {
      return new MActionResult<Zolik>().ctorWithexception(ex);
    }
  }

  Future<MActionResult<Transaction>> removeAsync(ZolikRemoveModel p) async {
    if (p == null || !p.isValid) {
      return new MActionResult<Transaction>()
          .ctorOnlyStatus(StatusCode.InvalidInput);
    }
    try {
      var packageMap = p.toJson();

      var url = "$urlApi/zolik/delete";
      var res = await cli.post(url,
          headers: {"Authorization": "Bearer $usedToken"}, body: packageMap);

      if (res.statusCode != 200) {
        return new MActionResult<Transaction>()
            .ctorOnlyStatus(StatusCode.InternalError);
      }

      var body = res.body;

      if (body.isEmpty) {
        return new MActionResult<Transaction>()
            .ctorOnlyStatus(StatusCode.NotFound);
      }

      Map<String, dynamic> _json = json.decode(body);

      _json["Content"] = Transaction.fromJson(_json["Content"]);

      var ws = MActionResult<Transaction>.fromJson(_json);
      return ws;
    } catch (ex) {
      return new MActionResult<Transaction>().ctorWithexception(ex);
    }
  }

    Future<MActionResult<Transaction>> createAsync(ZolikCreateModel p) async {
    if (p == null || !p.isValid) {
      return new MActionResult<Transaction>()
          .ctorOnlyStatus(StatusCode.InvalidInput);
    }
    try {
      var packageMap = p.toJson();

      var url = "$urlApi/zolik/create";
      var res = await cli.post(url,
          headers: {"Authorization": "Bearer $usedToken"}, body: packageMap);

      if (res.statusCode != 200) {
        return new MActionResult<Transaction>()
            .ctorOnlyStatus(StatusCode.InternalError);
      }

      var body = res.body;

      if (body.isEmpty) {
        return new MActionResult<Transaction>()
            .ctorOnlyStatus(StatusCode.NotFound);
      }

      Map<String, dynamic> _json = json.decode(body);

      _json["Content"] = Transaction.fromJson(_json["Content"]);

      var ws = MActionResult<Transaction>.fromJson(_json);
      return ws;
    } catch (ex) {
      return new MActionResult<Transaction>().ctorWithexception(ex);
    }
  }

  Future<MActionResult<List<Zolik>>> getUserZoliksAsync(int userId,
      [bool isTester = false]) async {
    if (userId < 1) {
      return new MActionResult<List<Zolik>>()
          .ctorOnlyStatus(StatusCode.NotValidID);
    }
    try {
      var url = "$urlApi/zolik/getuserzoliks?userId=$userId";
      if (isTester == true) {
        url += "&isTester=$isTester";
      }
      var res =
          await cli.get(url, headers: {"Authorization": "Bearer $usedToken"});

      if (res.statusCode != 200) {
        return new MActionResult<List<Zolik>>()
            .ctorOnlyStatus(StatusCode.InternalError);
      }

      String body = res.body;

      if (body.isEmpty) {
        return new MActionResult<List<Zolik>>()
            .ctorOnlyStatus(StatusCode.NotFound);
      }

      var _json = json.decode(body);
      List<Map<String, dynamic>> content =
          List<Map<String, dynamic>>.from(_json['Content']);
      List<Zolik> zoliks = new List<Zolik>();
      if (content != null && content.length > 0) {
        content.forEach((map) => zoliks.add(Zolik.fromJson(map)));
      }
      MActionResult<List<Zolik>> ws = MActionResult.fromJsonWthContent(_json);
      ws.content = zoliks;
      return ws;
    } catch (ex) {
      return new MActionResult<List<Zolik>>().ctorWithexception(ex);
    }
  }

  Future<List<Zolik>> getSchoolZoliks([bool onlyEnabled = true]) async {
    try {
      var url = "$urlApi/zolik/getSchoolZoliks?onlonlyEnabled=$onlyEnabled";
      var res =
          await cli.get(url, headers: {"Authorization": "Bearer $usedToken"});

      if (res.statusCode != 200) {
        return List<Zolik>();
      }

      String body = res.body;

      if (body.isEmpty) {
        return List<Zolik>();
      }

      var _json = json.decode(body);

      var content = List<Map<String, dynamic>>.from(_json);
      var zoliks = new List<Zolik>();
      if (content != null && content.length > 0) {
        content.forEach((map) => zoliks.add(Zolik.fromJson(map)));
      }
      return zoliks;
    } catch (ex) {
      return List<Zolik>();
    }
  }
}
