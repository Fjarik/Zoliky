import 'package:zoliky_teachers/utils/api/enums/Enum.dart';

class Genders extends Enum<int> {
  final String name;

  const Genders(int value, this.name) : super(value);

  static const Genders NotKnown =
      const Genders(0, "Nechci sdělovat své pohlaví");
  static const Genders Male = const Genders(1, "Muž");
  static const Genders Female = const Genders(2, "Žena");
  static const Genders NotApplicable = const Genders(9, "Jiné");

  operator ==(Object other) => (other is Genders && other.value == this.value);

  static Genders fromId(int id) {
    switch (id) {
      case 0:
        return Genders.NotKnown;
      case 1:
        return Genders.Male;
      case 2:
        return Genders.Female;
      case 9:
        return Genders.NotApplicable;
    }
    return Genders(id, "Neznámý typ");
  }

  int get hashCode => value.hashCode;
}
