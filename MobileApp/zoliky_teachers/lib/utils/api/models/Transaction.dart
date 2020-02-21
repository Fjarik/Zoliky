import 'package:zoliky_teachers/utils/Global.dart';
import 'package:zoliky_teachers/utils/api/enums/TransactionAssignment.dart';
import 'package:zoliky_teachers/utils/api/models/ZolikType.dart';
import 'package:zoliky_teachers/utils/interfaces/IDbObject.dart';

class Transaction implements IDbObject {
  int id;
  int fromID;
  int toId;
  int zolikId;
  int zolikTypeId;
  String from;
  String to;
  DateTime date;
  String message;
  TransactionAssignment type;
  String zolikTitle;

  ZolikType get zolikType => Global.getTypeById(this.zolikTypeId);

  Transaction();

  Transaction.fromJson(Map<String, dynamic> json)
      : id = json["ID"],
        fromID = json["FromID"],
        toId = json["ToID"],
        zolikId = json["ZolikID"],
        zolikTypeId = json["ZolikTypeID"],
        from = json["From"],
        to = json["To"],
        date = DateTime.parse(json["Date"]),
        message = json["Message"],
        type = TransactionAssignment.fromId(json["Typ"]),
        zolikTitle = json["ZolikTitle"];
}
