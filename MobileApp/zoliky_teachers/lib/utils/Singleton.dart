import 'package:zoliky_teachers/utils/api/models/Student.dart';
import 'package:zoliky_teachers/utils/api/models/User.dart';
import 'package:zoliky_teachers/utils/api/models/universal/ClassLeaderboardData.dart';

import 'api/models/Zolik.dart';

class Singleton {
  static final Singleton _singleton = new Singleton._internal();

  bool darkmode = false;
  bool biometrics = false;
  bool changed = false;
  String token = "";
  User user;
  List<Zolik> zoliks;
  List<Student> students;
  List<ClassLeaderboardData> classLeaderboard;

  void clear() {
    token = "";
    user = null;
    zoliks = new List();
    students = new List();
    classLeaderboard = new List();
  }

  factory Singleton() {
    return _singleton;
  }

  Singleton._internal() {
    zoliks = List<Zolik>();
    students = List<Student>();
    classLeaderboard = List<ClassLeaderboardData>();
  }
}
