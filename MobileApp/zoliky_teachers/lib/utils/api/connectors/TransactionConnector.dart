import 'dart:convert';

import 'package:zoliky_teachers/utils/api/connectors/PublicConnector.dart';
import 'package:zoliky_teachers/utils/api/models/Transaction.dart';

class TransactionConnector extends PublicConnector {
  TransactionConnector(String token) {
    this.usedToken = token;
  }

  Future<List<Transaction>> getZolikTransactionsAsync(int zolikId) async {
    if (zolikId < 1) {
      return new List<Transaction>();
    }
    try {
      var url = "$urlApi/transaction/getbyzolik?zolikId=$zolikId";
      var res =
          await cli.get(url, headers: {"Authorization": "Bearer $usedToken"});

      if (res.statusCode != 200) {
        return new List<Transaction>();
      }

      String body = res.body;

      if (body.isEmpty) {
        return new List<Transaction>();
      }

      var content = json.decode(body);

      var trans = new List<Transaction>();
      if (content != null && content.length > 0) {
        content.forEach((map) => trans.add(Transaction.fromJson(map)));
      }

      return trans;
    } catch (ex) {
      return new List<Transaction>();
    }
  }
}
