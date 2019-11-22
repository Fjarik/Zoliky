import 'package:zoliky_teachers/utils/api/enums/TransactionAssignment.dart';
import 'package:zoliky_teachers/utils/api/enums/ZolikType.dart';
import 'package:zoliky_teachers/utils/interfaces/IDbObject.dart';

class Transaction implements IDbObject {
  int id;
  int fromID;
  int toId;
  int zolikId;
  String from;
  String to;
  DateTime date;
  String message;
  TransactionAssignment type;
  String zolikTitle;
  ZolikType zolikType;

  Transaction();

  Transaction.fromJson(Map<String, dynamic> json)
      : id = json["ID"],
        fromID = json["FromID"],
        toId = json["ToID"],
        zolikId = json["ZolikID"],
        from = json["From"],
        to = json["To"],
        date = DateTime.parse(json["Date"]),
        message = json["Message"],
        type = TransactionAssignment.fromId(json["Typ"]),
        zolikTitle = json["ZolikTitle"],
        zolikType = ZolikType.fromId(json["ZolikType"]);
}
