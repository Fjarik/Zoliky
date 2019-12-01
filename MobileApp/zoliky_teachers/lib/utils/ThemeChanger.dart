import 'package:flutter/material.dart';

class ThemeChanger with ChangeNotifier {
  ThemeChanger(this._theme);

  ThemeData _theme;

  getTheme() => this._theme;

  setTheme(ThemeData newTheme) {
    this._theme = newTheme;

    notifyListeners();
  }
}
