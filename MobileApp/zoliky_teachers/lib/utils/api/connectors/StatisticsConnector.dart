import 'dart:convert';

import 'package:zoliky_teachers/utils/api/connectors/PublicConnector.dart';
import 'package:zoliky_teachers/utils/api/models/universal/ClassLeaderboardData.dart';
import 'package:zoliky_teachers/utils/api/models/universal/ZolikTypesData.dart';

class StatisticsConnector extends PublicConnector {
  StatisticsConnector(String token) : super() {
    this.usedToken = token;
  }

  Future<int> getStudentCountAsync() async {
    try {
      var url = "$urlApi/statistics/getstudentcount";
      var res =
          await cli.get(url, headers: {"Authorization": "Bearer $usedToken"});

      if (res.statusCode != 200) {
        return 0;
      }

      String body = res.body;

      if (body.isEmpty) {
        return 0;
      }

      var count = int.tryParse(body);
      if (count == null) {
        return 0;
      }
      return count;
    } catch (ex) {
      return 0;
    }
  }

  Future<int> getZolikCountAsync() async {
    try {
      var url = "$urlApi/statistics/getzolikcount";
      var res =
          await cli.get(url, headers: {"Authorization": "Bearer $usedToken"});

      if (res.statusCode != 200) {
        return 0;
      }

      String body = res.body;

      if (body.isEmpty) {
        return 0;
      }

      var count = int.tryParse(body);
      if (count == null) {
        return 0;
      }
      return count;
    } catch (ex) {
      return 0;
    }
  }

  Future<int> getTeacherCountAsync() async {
    try {
      var url = "$urlApi/statistics/getteachercount";
      var res =
          await cli.get(url, headers: {"Authorization": "Bearer $usedToken"});

      if (res.statusCode != 200) {
        return 0;
      }

      String body = res.body;

      if (body.isEmpty) {
        return 0;
      }

      var count = int.tryParse(body);
      if (count == null) {
        return 0;
      }
      return count;
    } catch (ex) {
      return 0;
    }
  }

  Future<List<ZolikTypesData>> getZolikTypesDataAsync() async {
    try {
      var url = "$urlApi/statistics/getzoliktypesdata";
      var res =
          await cli.get(url, headers: {"Authorization": "Bearer $usedToken"});

      if (res.statusCode != 200) {
        return new List();
      }

      var body = res.body;

      if (body.isEmpty) {
        return new List();
      }

      var content = json.decode(body);
      var list = new List<ZolikTypesData>();
      if (content != null && content.length > 0) {
        content.forEach((map) => list.add(ZolikTypesData.fromJson(map)));
      }

      return list;
    } catch (ex) {
      return new List();
    }
  }

  Future<List<ClassLeaderboardData>> getClassLeaderboardAsync() async {
    try {
      var url = "$urlApi/statistics/getclassleaderboard";
      var res =
          await cli.get(url, headers: {"Authorization": "Bearer $usedToken"});

      if (res.statusCode != 200) {
        return new List();
      }

      var body = res.body;

      if (body.isEmpty) {
        return new List();
      }

      var content = json.decode(body);
      var list = new List<ClassLeaderboardData>();
      if (content != null && content.length > 0) {
        content.forEach((map) => list.add(ClassLeaderboardData.fromJson(map)));
      }

      return list;
    } catch (ex) {
      return new List();
    }
  }
}
