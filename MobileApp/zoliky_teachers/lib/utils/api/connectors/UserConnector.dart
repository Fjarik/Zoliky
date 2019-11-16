import 'dart:convert';

import 'package:http/http.dart';
import 'package:http_parser/http_parser.dart';
import 'package:zoliky_teachers/utils/api/connectors/PublicConnector.dart';
import 'package:zoliky_teachers/utils/api/enums/StatusCode.dart';
import 'package:zoliky_teachers/utils/api/models/Student.dart';
import 'package:zoliky_teachers/utils/api/models/User.dart';
import 'package:zoliky_teachers/utils/api/models/universal/MActionResult.dart';

class UserConnector extends PublicConnector {
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
      [bool includeImage = true]) async {
    if (userId < 1) {
      return new MActionResult<User>().ctorOnlyStatus(StatusCode.NotValidID);
    }
    try {
      var url = "$urlApi/user/get/$userId&includeImage=$includeImage";
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

  Future<bool> setMobileToken(String token) async {
    if (token.isEmpty || this.usedToken.isEmpty) {
      return false;
    }
    try {
      var url = "$urlApi/user/mobiletoken";
      var res = await cli.post(url,
          headers: {"Authorization": "Bearer $usedToken"},
          body: {"Token": token});

      if (res.statusCode != 200) {
        return false;
      }
      return res.body.toLowerCase() == "true";
    } catch (ex) {
      return false;
    }
  }

  Future<bool> resetMobileToken() async {
    if (this.usedToken.isEmpty) {
      return false;
    }
    try {
      var url = "$urlApi/user/resettoken";
      var res =
          await cli.post(url, headers: {"Authorization": "Bearer $usedToken"});
      if (res.statusCode != 200) {
        return false;
      }
      return res.body.toLowerCase() == "true";
    } catch (ex) {
      return false;
    }
  }

  Future<bool> changeProfilePhoto(List<int> bytes, String mimeString) async {
    if (this.usedToken.isEmpty) {
      return false;
    }
    try {
      var mime = MediaType.parse(mimeString);
      var url = "$urlApi/user/changeprofilephoto";
      var uri = Uri.parse(url);
      var request = new MultipartRequest("POST", uri);
      request.headers.putIfAbsent("Authorization", () => "Bearer $usedToken");
      request.files.add(MultipartFile.fromBytes("file", bytes,
          filename: 'fajl', contentType: mime));
      var res = await request.send();

      /*var res =
          await cli.post(url, headers: {"Authorization": "Bearer $usedToken"});*/
      if (res.statusCode != 200) {
        return false;
      }
      var stream = res.stream.toStringStream();
      var body = "";
      await for (var value in stream) {
        body += value;
      }
      return body.toLowerCase() == "true";
    } catch (ex) {
      return false;
    }
  }

  Future<List<Student>> getStudents({int classId, int imageMaxSize}) async {
    try {
      var url = "$urlApi/student/getstudents";
      if (classId != null && classId > 0) {
        url += "?classId=$classId";
        if (imageMaxSize != null && imageMaxSize > 0) {
          url += "&imageMaxSize=$imageMaxSize";
        }
      } else {
        if (imageMaxSize != null && imageMaxSize > 0) {
          url += "?imageMaxSize=$imageMaxSize";
        }
      }

      var res =
          await cli.get(url, headers: {"Authorization": "Bearer $usedToken"});
      if (res.statusCode != 200) {
        return new List<Student>();
      }
      String body = res.body;
      if (body.isEmpty) {
        return new List<Student>();
      }
      var _json = json.decode(body);
      var students = new List<Student>();
      if (_json != null && _json.length > 0) {
        _json.forEach((map) => students.add(Student.fromJson(map)));
      }

      return students;
    } catch (ex) {
      return new List<Student>();
    }
  }

  Future<List<Student>> getTopStudents({int classId, int top}) async {
    try {
      var imageMaxSize = 1;
      var url = "$urlApi/student/gettop?imageMaxSize=$imageMaxSize";
      if (classId != null && classId > 0) {
        url += "&classId=$classId";
      }
      if (top != null && top > 0) {
        url += "&top=$top";
      }
      var res =
          await cli.get(url, headers: {"Authorization": "Bearer $usedToken"});

      if (res.statusCode != 200) {
        return new List<Student>();
      }
      String body = res.body;
      if (body.isEmpty) {
        return new List<Student>();
      }
      var content = json.decode(body);
      var students = new List<Student>();
      if (content != null && content.length > 0) {
        content.forEach((map) => students.add(Student.fromJson(map)));
      }
      return students;
    } catch (ex) {
      return new List<Student>();
    }
  }
}
