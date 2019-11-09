class Logins {
  String uName;
  String password;
  final int project = 6;
  final String _ver = "1.0.1";

  Logins();

  Logins.fromLogins(this.uName, this.password);

  Map<String, dynamic> toJson() => {
        "UName": uName,
        "Password": password,
        "Project": project.toString(),
        "Ver": _ver,
      };
}
