import 'package:zoliky_teachers/utils/api/enums/Enum.dart';

class ZolikType extends Enum<int> {
  final String name;
  final bool isSplittable;

  const ZolikType(int value, this.name, this.isSplittable) : super(value);

  static const ZolikType Normal = const ZolikType(0, "Normální žolík", false);
  static const ZolikType Joker = const ZolikType(1, "Jokér", true);
  static const ZolikType Black = const ZolikType(2, "Černý Petr", false);
  static const ZolikType Debug = const ZolikType(5, "Testovací žolík", false);
  static const ZolikType DebugJoker =
      const ZolikType(6, "Testovací jokér", true);

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
    return ZolikType(id, "Neznámý typ", false);
  }

  static const List<ZolikType> values = const [
    ZolikType.Normal,
    ZolikType.Joker,
    ZolikType.Black,
    // ZolikType.Debug,
    // ZolikType.DebugJoker
  ];

  operator ==(Object other) =>
      (other is ZolikType && other.value == this.value);

  int get hashCode => value.hashCode;
}
