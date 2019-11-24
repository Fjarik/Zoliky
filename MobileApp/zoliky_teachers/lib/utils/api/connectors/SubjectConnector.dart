import 'dart:convert';

import 'package:zoliky_teachers/utils/api/connectors/PublicConnector.dart';
import 'package:zoliky_teachers/utils/api/models/Subject.dart';

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

      var classes = new List<Subject>();
      if (content != null && content.length > 0) {
        content.forEach((map) => classes.add(Subject.fromJson(map)));
      }
      return classes;
    } catch (ex) {
      return new List<Subject>();
    }
  }
}
