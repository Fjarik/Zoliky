import 'dart:convert';

import 'package:zoliky_teachers/utils/api/connectors/PublicConnector.dart';
import 'package:zoliky_teachers/utils/api/enums/StatusCode.dart';
import 'package:zoliky_teachers/utils/api/models/User.dart';
import 'package:zoliky_teachers/utils/api/models/universal/MActionResult.dart';

class UserConnector extends PublicConnector {
  UserConnector();

  UserConnector.withToken(String token) {
    usedToken = token;
  }

  Future<MActionResult<User>> login(String username, String password) async {
    try {
      var res = await getToken(username, password);
      if (!res.isSuccess) {
        return new MActionResult<User>().ctorOnlyStatus(res.status);
      }
      this.usedToken = res.content;

      return await this.getMeAsync();
    } catch (ex) {
      return new MActionResult<User>().ctorWithexception(ex);
    }
  }

  Future<MActionResult<User>> loginExternal(
      String token, String provider) async {
    try {
      var res = await getFbToken(token, provider);
      if (!res.isSuccess) {
        return new MActionResult<User>().ctorOnlyStatus(res.status);
      }
      this.usedToken = res.content;

      return await this.getMeAsync();
    } catch (ex) {
      return new MActionResult<User>().ctorWithexception(ex);
    }
  }

  Future<MActionResult<User>> getMeAsync([bool includeImage = true]) async {
    try {
      var url = "$urlApi/user/me?includeImage=$includeImage";
      var res =
          await cli.get(url, headers: {"Authorization": "Bearer $usedToken"});

      if (res.statusCode != 200) {
        return new MActionResult<User>()
            .ctorOnlyStatus(StatusCode.InternalError);
      }

      String body = res.body;

      if (body.isEmpty) {
        return new MActionResult<User>().ctorOnlyStatus(StatusCode.NotFound);
      }

      var _json = json.decode(body);
      var content = _json['Content'];
      if (content != null && content.toString().isNotEmpty) {
        _json['Content'] = User.fromJson(content);
      }

      var ws = MActionResult<User>.fromJson(_json);
      return ws;
    } catch (ex) {
      return new MActionResult<User>().ctorWithexception(ex);
    }
  }

  Future<MActionResult<User>> getAsync(int userId,
      {bool includeImage = true}) async {
    if (userId < 1) {
      return new MActionResult<User>().ctorOnlyStatus(StatusCode.NotValidID);
    }
    try {
      var url = "$urlApi/user/get?id=$userId&includeImage=$includeImage";
      var res =
          await cli.get(url, headers: {"Authorization": "Bearer $usedToken"});

      if (res.statusCode != 200) {
        return new MActionResult<User>()
            .ctorOnlyStatus(StatusCode.InternalError);
      }

      var body = res.body;

      if (body.isEmpty) {
        return new MActionResult<User>().ctorOnlyStatus(StatusCode.NotFound);
      }

      var _json = json.decode(body);
      var content = _json['Content'];
      if (content != null && content.toString().isNotEmpty) {
        _json['Content'] = User.fromJson(content);
      }

      var ws = MActionResult<User>.fromJson(_json);
      return ws;
    } catch (ex) {
      return new MActionResult<User>().ctorWithexception(ex);
    }
  }
}
