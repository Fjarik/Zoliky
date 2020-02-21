import 'package:flutter/material.dart';

class ThemeChanger with ChangeNotifier {
  ThemeChanger(this._theme);

  ThemeData _theme;

  ThemeData getTheme() => this._theme;

  void setTheme(ThemeData newTheme) {
    this._theme = newTheme;

    notifyListeners();
  }
}
