import 'package:zoliky_teachers/utils/api/enums/Enum.dart';

class UserRole extends Enum<String> {
  const UserRole(String value) : super(value);

  static const UserRole Teacher = const UserRole("Teacher");
  static const UserRole Administrator = const UserRole("Administrator");
  static const UserRole Student = const UserRole("Student");
  static const UserRole Tester = const UserRole("Tester");
  static const UserRole Public = const UserRole("Public");
  static const UserRole FakeStudent = const UserRole("StudentFake");
  static const UserRole Support = const UserRole("Support");
  static const UserRole HiddenStudent = const UserRole("StudentHidden");
  static const UserRole Robot = const UserRole("Robot");
  static const UserRole Developer = const UserRole("Developer");
  static const UserRole LoginOnly = const UserRole("LoginOnly");
  static const UserRole SchoolManager = const UserRole("SchoolManager");

  operator ==(Object other) => (other is UserRole && other.value == this.value);

  int get hashCode => value.hashCode;
}
