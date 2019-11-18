class ClassLeaderboardData {
  String label;
  double data;
  String colour;

  ClassLeaderboardData.fromJson(Map<String, dynamic> _json)
      : label = _json["Label"],
        colour = _json["Colour"],
        data = _json["Data"];
}
