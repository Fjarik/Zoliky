class TeacherSubject {
  int teacherId;
  int classId;
  int subjectId;

  TeacherSubject();

  TeacherSubject.fromJson(Map<String, dynamic> json)
      : teacherId = json["TeacherID"],
        classId = json["ClassID"],
        subjectId = json["SubjectID"];
}
