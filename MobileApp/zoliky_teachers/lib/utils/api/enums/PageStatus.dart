import 'package:zoliky_teachers/utils/api/enums/Enum.dart';

class PageStatus extends Enum<int> {
  const PageStatus(int value) : super(value);

  static const PageStatus Functional = const PageStatus(0);
  static const PageStatus Limited = const PageStatus(1);
  static const PageStatus Unfunctional = const PageStatus(2);
  static const PageStatus NotAvailable = const PageStatus(3);

  operator ==(Object other) =>
      (other is PageStatus && other.value == this.value);

  int get hashCode => value.hashCode;
}
