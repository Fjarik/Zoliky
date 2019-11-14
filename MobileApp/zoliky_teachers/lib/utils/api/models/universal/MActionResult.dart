import 'package:zoliky_teachers/utils/api/Extends.dart';
import 'package:zoliky_teachers/utils/api/enums/StatusCode.dart';

class MActionResult<T> {
  T content;
  Object exception;
  bool get isSuccess =>
      (status.value == StatusCode.OK.value && content != null);
  StatusCode status;
  DateTime date;

  MActionResult();

  MActionResult<T> ctorOnlyStatus(StatusCode status) {
    return ctorContentAndException(status, null, null);
  }

  MActionResult<T> ctorWithContent(StatusCode status, T result) {
    return ctorContentAndException(status, result, null);
  }

  MActionResult<T> ctorWithexception(Object ex) {
    return ctorContentAndException(StatusCode.SeeException, null, ex);
  }

  MActionResult<T> ctorContentAndException(
      StatusCode status, T result, Object ex) {
    this.status = status;
    this.exception = ex;
    this.content = result;
    return this;
  }

  String getStatusMessage() {
    if (this.status == StatusCode.OK) {
      return "Vše v pořádku";
    } else if (this.status == StatusCode.SeeException) {
      String s = "Vyskytla se chyba";
      if (this.exception != null) {
        s += ", podrobnosti){ ${this.exception.toString()}";
      }
      return s;
    } else if (this.status == StatusCode.NoPassword) {
      return "Nemáte vytvořené heslo. Použijte funkci zapomenuté heslo";
    } else if (this.status == StatusCode.Banned) {
      return "Váš účet je zabanován";
    } else if (this.status == StatusCode.Forbidden) {
      return "Zakázáno";
    } else if (this.status == StatusCode.NotFound) {
      return "Nenalezeno";
    } else if (this.status == StatusCode.Timeout) {
      return "Vypršel časový limit na danou operaci";
    } else if (this.status == StatusCode.InternalError) {
      return "Vyskytla se chyba na straně serveru";
    } else if (this.status == StatusCode.NotValidID) {
      return "Poskytnuté ID není platné";
    } else if (this.status == StatusCode.AlreadyExists) {
      return "Tento záznam již existuje";
    } else if (this.status == StatusCode.InvalidInput) {
      return "Neplatné vstupní parametry";
    } else if (this.status == StatusCode.NotEnabled) {
      return "Uživatel není aktivovaný";
    } else if (this.status == StatusCode.EmailNotConfirmed) {
      return "Email není potvrzený";
    } else if (this.status == StatusCode.WrongPassword) {
      return "Neplatné jméno nebo  heslo";
    } else if (this.status == StatusCode.ExpiredPassword) {
      return "Zadané heslo již není platné";
    } else if (this.status == StatusCode.InsufficientPermissions) {
      return "Na tutu akci nemáte dostatečné oprávnění";
    } else if (this.status == StatusCode.CannotTransfer) {
      return "Vybraného žolíka nelze přenést";
    } else if (this.status == StatusCode.CannotSplit) {
      return "Vybraného žolík nelze rozdělit";
    } else if (this.status == StatusCode.JustALittleError) {
      return "Vyskytla se chyba, ale pouze malá :)";
    }
    return "Neznámá chyba";
  }

  MActionResult.fromJson(Map<String, dynamic> _json)
      : content = _json["Content"],
        exception = _json["Exception"],
        status = StatusCode(_json["Status"]),
        date = Extends.parseDateTime(_json["Date"].toString());

  MActionResult.fromJsonWthContent(Map<String, dynamic> _json)
      : exception = _json["Exception"],
        status = StatusCode(_json["Status"]),
        date = Extends.parseDateTime(_json["Date"].toString());
}
