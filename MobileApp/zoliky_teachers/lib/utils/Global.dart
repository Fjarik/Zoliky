import 'dart:convert';
import 'dart:typed_data';

import 'package:flutter/material.dart';
import 'package:intl/intl.dart';
import 'package:zoliky_teachers/utils/api/connectors/ClassConnector.dart';
import 'package:zoliky_teachers/utils/api/connectors/StudentConnector.dart';
import 'package:zoliky_teachers/utils/api/connectors/SubjectConnector.dart';
import 'package:zoliky_teachers/utils/api/models/Class.dart';
import 'package:zoliky_teachers/utils/api/models/Rank.dart';
import 'package:zoliky_teachers/utils/api/models/Student.dart';
import 'package:zoliky_teachers/utils/api/models/Subject.dart';

class Global {
  static List<Class> _classes = new List<Class>();
  static List<Student> _students = new List<Student>();
  static List<Subject> _subjects = new List<Subject>();
  static List<Rank> _ranks = new List<Rank>();

  static List<Class> get classes => _classes;
  static List<Student> get students => _students;
  static List<Subject> get subjects => _subjects;
  static List<Rank> get ranks => _ranks;

  static Class getClassById(int id) {
    if (classes.length < 1 || id == null) {
      return getFakeClass();
    }
    var c = classes.firstWhere((x) => x.id == id);
    if (c == null) {
      return getFakeClass();
    }
    return c;
  }

  static Class getFakeClass() {
    return new Class()
      ..id = 0
      ..name = "0.A";
  }

  static Rank getRankByXp(int xp) {
    if (ranks.isNotEmpty) {
      return ranks.firstWhere(
              (x) => x.fromXP <= xp && xp <= (x.toXP ?? 2147483647)) ??
          ranks.first;
    }
    return null;
  }

  static Future loadApp(String token) async {
    _classes = await loadClasses(token);
    _students = await _loadStudents(token);
    _subjects = await _loadSubjects(token);
  }

  static Future<List<Class>> loadClasses(String token) async {
    if (token == null || token.isEmpty) {
      return new List<Class>();
    }
    var mgr = new ClassConnector(token);
    var res = await mgr.getClassesAsync();
    return res ?? new List<Class>();
  }

  static Future<List<Student>> _loadStudents(String token) async {
    if (token == null || token.isEmpty) {
      return new List<Student>();
    }
    var mgr = new StudentConnector(token);
    var res = await mgr.getStudents(imageMaxSize: 1);
    return res ?? new List<Student>();
  }

  static Future<List<Subject>> _loadSubjects(String token) async {
    if (token == null || token.isEmpty) {
      return new List<Subject>();
    }
    var mgr = new SubjectConnector(token);
    var res = await mgr.getSubjectsAsync();
    return res ?? new List<Subject>();
  }

  static bool get isInDebugMode {
    bool inDebugMode = false;
    assert(inDebugMode = true);
    return inDebugMode;
  }

  static Uint8List getImageFromBase64(String _base64) {
    return base64Decode(_base64);
  }

  static String getDateString(DateTime date) {
    return getSpecificDateString(date, "dd.MM.yyyy");
  }

  static String getSpecificDateString(DateTime date, String format) {
    return DateFormat(format).format(date);
  }

  static Widget loading() {
    return Container(
      constraints: BoxConstraints(
        minHeight: 70,
      ),
      child: Row(
        mainAxisAlignment: MainAxisAlignment.center,
        children: <Widget>[
          Column(
            mainAxisAlignment: MainAxisAlignment.center,
            children: <Widget>[
              CircularProgressIndicator(),
            ],
          ),
        ],
      ),
    );
  }
}
