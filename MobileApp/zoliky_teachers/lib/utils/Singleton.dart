import 'package:zoliky_teachers/utils/api/models/User.dart';

import 'api/models/Zolik.dart';

class Singleton {
  static final Singleton _singleton = new Singleton._internal();

  bool darkmode = false;
  bool biometrics = false;
  bool shakeForSupport = true;
  String token = "";
  User user;
  List<Zolik> zoliks;

  factory Singleton() {
    return _singleton;
  }

  Singleton._internal() {
    zoliks = List<Zolik>();
  }
}
