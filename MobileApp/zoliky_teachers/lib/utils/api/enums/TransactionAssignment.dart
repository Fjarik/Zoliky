import 'package:zoliky_teachers/utils/api/enums/Enum.dart';

class TransactionAssignment extends Enum<int> {
  final String name;

  const TransactionAssignment(int value, this.name) : super(value);

  static const TransactionAssignment Gift =
      const TransactionAssignment(0, "Darování");
  static const TransactionAssignment QR =
      const TransactionAssignment(1, "QR kód");
  static const TransactionAssignment NewAssignment =
      const TransactionAssignment(2, "Přiřazení");
  static const TransactionAssignment ZerziRemoval =
      const TransactionAssignment(3, "Odebrání");
  static const TransactionAssignment NFC =
      const TransactionAssignment(4, "NFC");
  static const TransactionAssignment Split =
      const TransactionAssignment(5, "Rozklad jokéra");

  static TransactionAssignment fromId(int id) {
    switch (id) {
      case 0:
        return TransactionAssignment.Gift;
      case 1:
        return TransactionAssignment.QR;
      case 2:
        return TransactionAssignment.NewAssignment;
      case 3:
        return TransactionAssignment.ZerziRemoval;
      case 4:
        return TransactionAssignment.NFC;
      case 5:
        return TransactionAssignment.Split;
    }
    return TransactionAssignment(id, "Neznámý typ");
  }

  operator ==(Object other) =>
      (other is TransactionAssignment && other.value == this.value);

  int get hashCode => value.hashCode;
}
