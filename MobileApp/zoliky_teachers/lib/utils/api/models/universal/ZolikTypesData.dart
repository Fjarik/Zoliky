class ZolikTypesData {
  String label;
  int count;

  ZolikTypesData.fromJson(Map<String, dynamic> _json)
      : label = _json["Label"],
        count = _json["Count"];
}
