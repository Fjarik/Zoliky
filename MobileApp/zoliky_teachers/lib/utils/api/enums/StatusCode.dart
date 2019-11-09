
import 'package:zoliky_teachers/utils/api/enums/Enum.dart';

class StatusCode extends Enum<int> {
  const StatusCode(int value) : super(value);

  static const StatusCode OK = const StatusCode(200);
  static const StatusCode SeeException = const StatusCode(303);
  static const StatusCode Forbidden = const StatusCode(403);
  static const StatusCode NotFound = const StatusCode(404);
  static const StatusCode Timeout = const StatusCode(408);
  static const StatusCode InternalError = const StatusCode(500);
  static const StatusCode NotValidID = const StatusCode(600);
  static const StatusCode AlreadyExists = const StatusCode(603);
  static const StatusCode InvalidInput = const StatusCode(605);
  static const StatusCode NotEnabled = const StatusCode(607);
  static const StatusCode WrongPassword = const StatusCode(610);
  static const StatusCode NoPassword = const StatusCode(611);
  static const StatusCode ExpiredPassword = const StatusCode(613);
  static const StatusCode InsufficientPermissions = const StatusCode(615);
  static const StatusCode CannotTransfer = const StatusCode(617);
  static const StatusCode CannotSplit = const StatusCode(618);
  static const StatusCode JustALittleError = const StatusCode(620);

  static StatusCode getStatus() {
    return StatusCode.OK;
  }

  operator ==(Object other) =>
      (other is StatusCode && other.value == this.value);

  int get hashCode => value.hashCode;

}
