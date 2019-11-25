import 'package:zoliky_teachers/utils/api/models/User.dart';
import 'package:zoliky_teachers/utils/api/models/universal/ClassLeaderboardData.dart';

import 'api/models/Zolik.dart';

class Singleton {
  static final Singleton _singleton = new Singleton._internal();

  bool darkmode = false;
  bool biometrics = false;
  String token = "";
  User user;
  List<Zolik> zoliks;
  List<ClassLeaderboardData> classLeaderboard;

  factory Singleton() {
    return _singleton;
  }

  Singleton._internal() {
    zoliks = List<Zolik>();
    classLeaderboard = List<ClassLeaderboardData>();
  }
}
