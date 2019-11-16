import 'package:zoliky_teachers/utils/api/connectors/PublicConnector.dart';

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
}
