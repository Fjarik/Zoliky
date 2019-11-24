import 'dart:convert';

import 'package:zoliky_teachers/utils/api/connectors/PublicConnector.dart';
import 'package:zoliky_teachers/utils/api/models/Student.dart';

class StudentConnector extends PublicConnector {
  StudentConnector(String token) {
    this.usedToken = token;
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
      var body = res.body;
      if (body.isEmpty) {
        return new List<Student>();
      }

      List<dynamic> _json = json.decode(body);
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
