import 'package:zoliky_teachers/utils/api/enums/Enum.dart';

class Projects extends Enum<int> {
  const Projects(int value) : super(value);

  static const Projects Web = const Projects(1);
  static const Projects Xamarin = const Projects(2);
  static const Projects UWP = const Projects(3);
  static const Projects WPF = const Projects(4);
  static const Projects ApiTest = const Projects(5);
  static const Projects Flutter = const Projects(6);
  static const Projects Support = const Projects(7);
  static const Projects Api = const Projects(8);
  static const Projects Unknown = const Projects(9);
  static const Projects WebNew = const Projects(10);

  operator ==(Object other) => (other is Projects && other.value == this.value);

  int get hashCode => value.hashCode;
}
