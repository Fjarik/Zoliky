import 'package:zoliky_teachers/utils/api/enums/PageStatus.dart';

class WebStatus {
  PageStatus status;
  String message;
  Object content;
  bool get canAccess => (status == PageStatus.Functional);

  WebStatus();

  WebStatus.fromJson(Map<String, dynamic> json)
      : status = PageStatus(json['Status']),
        message = json['Message'],
        content = json['Content'];

  WebStatus.unFunctional({Object content})
      : status = PageStatus.Unfunctional,
        message = "Nezdařilo se načíst stav serverů",
        content = content ?? null;

  @override
  String toString() {
    return "Status: $status, \nmsg: $message, \nContent: $content\n";
  }
}
