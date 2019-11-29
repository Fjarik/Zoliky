import 'dart:developer';

import 'package:http/http.dart' as http;
import 'package:zoliky_teachers/utils/api/enums/Projects.dart';

class MainClient extends http.BaseClient {
  static String get _version => "2.0";
  static String get _userAgent =>
      "ZolikApiConnector/$_version (https://www.zoliky.eu; Flutter - Teachers)";
  static String get _currentProject => Projects.FlutterTeacher.value.toString();

  final http.Client _inner = new http.Client();
  Duration get defaultTimeout => const Duration(seconds: 30);

  MainClient();

  @override
  Future<http.StreamedResponse> send(http.BaseRequest request) {
    request.headers.putIfAbsent("User-Agent", () => _userAgent);
    request.headers.putIfAbsent("api-version", () => _version);
    request.headers.putIfAbsent("projectId", () => _currentProject);
    return _inner.send(request).timeout(defaultTimeout).catchError((error) {
      log(error);
    });
  }
}
