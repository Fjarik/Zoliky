class Extends {
  static DateTime parseDateTime(String text) {
    text = text.replaceRange(text.indexOf('.'), text.indexOf("+"), "");
    return DateTime.parse(text);
  }
}
