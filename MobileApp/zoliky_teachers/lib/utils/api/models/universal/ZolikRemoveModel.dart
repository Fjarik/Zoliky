class ZolikRemoveModel {
  int zolikId;
  String reason;

  bool get isValid =>
      (this.zolikId > 0 && this.reason != null && this.reason.isNotEmpty);

  ZolikRemoveModel();

  Map<String, String> toJson() => {
        "ZolikID": zolikId.toString(),
        "Reason": reason ?? "",
      };
}
