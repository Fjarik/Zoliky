import 'package:zoliky_teachers/utils/api/Extends.dart';
import 'package:zoliky_teachers/utils/api/enums/StatusCode.dart';

class MActionResult<T> {
  T content;
  Object exception;
  bool get isSuccess =>
      (status.value == StatusCode.OK.value && content != null);
  StatusCode status;
  DateTime date;

  MActionResult();

  MActionResult<T> ctorOnlyStatus(StatusCode status) {
    return ctorContentAndException(status, null, null);
  }

  MActionResult<T> ctorWithContent(StatusCode status, T result) {
    return ctorContentAndException(status, result, null);
  }

  MActionResult<T> ctorWithexception(Object ex) {
    return ctorContentAndException(StatusCode.SeeException, null, ex);
  }

  MActionResult<T> ctorContentAndException(
      StatusCode status, T result, Object ex) {
    this.status = status;
    this.exception = ex;
    this.content = result;
    return this;
  }

  MActionResult.fromJson(Map<String, dynamic> _json)
      : content = _json["Content"],
        exception = _json["Exception"],
        status = StatusCode(_json["Status"]),
        date = Extends.parseDateTime(_json["Date"].toString());

  MActionResult.fromJsonWthContent(Map<String, dynamic> _json)
      : exception = _json["Exception"],
        status = StatusCode(_json["Status"]),
        date = Extends.parseDateTime(_json["Date"].toString());
}
