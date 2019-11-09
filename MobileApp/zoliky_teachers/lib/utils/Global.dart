import 'dart:convert';
import 'dart:typed_data';

import 'package:zoliky_teachers/utils/api/models/Class.dart';
import 'package:zoliky_teachers/utils/api/models/Rank.dart';

class Global {
  static List<Class> _classes = new List<Class>();
  static List<Rank> _ranks = new List<Rank>();

  static List<Class> get classes => _classes;
  static List<Rank> get ranks => _ranks;

  static Class getClassById(int id) {
    if (classes.length < 1 || id == null) {
      return getFakeClass();
    }
    var c = classes.firstWhere((x) => x.id == id);
    if (c == null) {
      return getFakeClass();
    }
    return c;
  }

  static Class getFakeClass() {
    return new Class()
      ..id = 0
      ..name = "0.A";
  }

  static Rank getRankByXp(int xp) {
    if (ranks.isNotEmpty) {
      return ranks.firstWhere(
              (x) => x.fromXP <= xp && xp <= (x.toXP ?? 2147483647)) ??
          ranks.first;
    }
    return null;
  }

  static bool get isInDebugMode {
    bool inDebugMode = false;
    assert(inDebugMode = true);
    return inDebugMode;
  }

  static Uint8List getImageFromBase64(String _base64) {
    return base64Decode(_base64);
  }
}
