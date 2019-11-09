import 'package:zoliky_teachers/components/Main/MainDashboard.dart';
import 'package:zoliky_teachers/utils/api/models/User.dart';

class Singleton {
  static final Singleton _singleton = new Singleton._internal();

  bool darkmode = false;
  bool biometrics = false;
  bool shakeForSupport = true;
  User user;
  DashboardPageState dashboardPageState;

  factory Singleton() {
    return _singleton;
  }

  Singleton._internal();
}
