import 'package:zoliky_teachers/utils/api/enums/Enum.dart';

class ZolikType extends Enum<int> {
  final String name;

  const ZolikType(int value, this.name) : super(value);

  static const ZolikType Normal = const ZolikType(0, "Normální žolík");
  static const ZolikType Joker = const ZolikType(1, "Jokér");
  static const ZolikType Black = const ZolikType(2, "Černý Petr");
  static const ZolikType Debug = const ZolikType(5, "Testovací žolík");
  static const ZolikType DebugJoker = const ZolikType(6, "Testovací jokér");

  static ZolikType fromId(int id) {
    switch (id) {
      case 0:
        return ZolikType.Normal;
      case 1:
        return ZolikType.Joker;
      case 2:
        return ZolikType.Black;
      case 5:
        return ZolikType.Debug;
      case 6:
        return ZolikType.DebugJoker;
    }
    return ZolikType(id, "Neznámý typ");
  }

  operator ==(Object other) =>
      (other is ZolikType && other.value == this.value);

  int get hashCode => value.hashCode;
}
