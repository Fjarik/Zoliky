import 'dart:convert';

import 'package:zoliky_teachers/utils/api/connectors/PublicConnector.dart';
import 'package:zoliky_teachers/utils/api/models/Subject.dart';
import 'package:zoliky_teachers/utils/api/models/universal/TeacherSubject.dart';

class SubjectConnector extends PublicConnector {
  SubjectConnector(String token) {
    this.usedToken = token;
  }

  Future<List<Subject>> getSubjectsAsync() async {
    try {
      var url = "$urlApi/subject/getall";
      var res =
          await cli.get(url, headers: {"Authorization": "Bearer $usedToken"});

      if (res.statusCode != 200) {
        return new List<Subject>();
      }

      var body = res.body;

      if (body.isEmpty) {
        return new List<Subject>();
      }

      List<dynamic> content = json.decode(body);

      var subjects = new List<Subject>();
      if (content != null && content.length > 0) {
        content.forEach((map) => subjects.add(Subject.fromJson(map)));
      }
      return subjects;
    } catch (ex) {
      return new List<Subject>();
    }
  }

  Future<List<TeacherSubject>> getTeacherSubjectsAsync() async {
    try {
      var url = "$urlApi/subject/getbyteacher";
      var res =
          await cli.get(url, headers: {"Authorization": "Bearer $usedToken"});

      if (res.statusCode != 200) {
        return new List<TeacherSubject>();
      }

      var body = res.body;

      if (body.isEmpty) {
        return new List<TeacherSubject>();
      }

      List<dynamic> content = json.decode(body);

      var s = new List<TeacherSubject>();
      if (content != null && content.length > 0) {
        content.forEach((map) => s.add(TeacherSubject.fromJson(map)));
      }
      return s;
    } catch (ex) {
      return new List<TeacherSubject>();
    }
  }
}
