using Dapper;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace kursach
{
    public class DatabaseAdapter : IDatabaseAdapter
    {
        private readonly string connectionString;

        public DatabaseAdapter(string host, string database, string username, string password, int port = 5432)
        {
            connectionString =
                $"Host={host};Port={port};Database={database};Username={username};Password={password}";
        }

        private IDbConnection GetConnection()
        {
            return new NpgsqlConnection(connectionString);
        }

        // ---------------- CORE ----------------

        public bool TestConnection()
        {
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public DataTable QueryTable(string sql, object param = null)
        {
            using (var conn = GetConnection())
            {
                conn.Open();

                var result = conn.Query(sql, param);

                DataTable table = new DataTable();

                if (result.Any())
                {
                    foreach (var prop in ((IDictionary<string, object>)result.First()).Keys)
                        table.Columns.Add(prop);

                    foreach (IDictionary<string, object> row in result)
                        table.Rows.Add(row.Values.ToArray());
                }

                return table;
            }
        }

        public int Execute(string sql, object param = null)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                return conn.Execute(sql, param);
            }
        }

        // ---------------- AUTH ----------------

        public User AuthenticateUser(string login, string password)
        {
            using (var conn = GetConnection())
            {
                conn.Open();

                return conn.QueryFirstOrDefault<User>(
                    "SELECT * FROM Users WHERE login = @login AND password = @password",
                    new { login, password }
                );
            }
        }

        // ---------------- COURSES ----------------

        public List<Course> GetAvailablePrograms()
        {
            using (var conn = GetConnection())
            {
                conn.Open();

                return conn.Query<Course>(
                    @"SELECT * FROM Courses 
                      WHERE status = 'approved' 
                      ORDER BY course_id"
                ).ToList();
            }
        }

        public Course GetProgramById(int id)
        {
            using (var conn = GetConnection())
            {
                conn.Open();

                return conn.QueryFirstOrDefault<Course>(
                    "SELECT * FROM Courses WHERE course_id = @id",
                    new { id }
                );
            }
        }

        public List<Course> GetAllCourses()
        {
            using (var conn = GetConnection())
            {
                conn.Open();

                return conn.Query<Course>(
                    "SELECT * FROM Courses ORDER BY course_id"
                ).ToList();
            }
        }

        public Course GetCourseById(int id)
        {
            using (var conn = GetConnection())
            {
                conn.Open();

                return conn.QueryFirstOrDefault<Course>(
                    "SELECT * FROM Courses WHERE course_id = @id",
                    new { id }
                );
            }
        }

        public bool AddCourse(Course course)
        {
            using (var conn = GetConnection())
            {
                conn.Open();

                string sql = @"
                    INSERT INTO Courses
                    (title, hours, price, teacher_id, status, description)
                    VALUES
                    (@title, @hours, @price, @teacher_id, @status, @description)";

                return conn.Execute(sql, course) > 0;
            }
        }

        public bool UpdateCourse(Course course)
        {
            using (var conn = GetConnection())
            {
                conn.Open();

                string sql = @"
                    UPDATE Courses SET
                        title = @title,
                        hours = @hours,
                        price = @price,
                        teacher_id = @teacher_id,
                        status = @status,
                        description = @description
                    WHERE course_id = @course_id";

                return conn.Execute(sql, course) > 0;
            }
        }

        public bool DeleteCourse(int id)
        {
            using (var conn = GetConnection())
            {
                conn.Open();

                return conn.Execute(
                    "DELETE FROM Courses WHERE course_id = @id",
                    new { id }
                ) > 0;
            }
        }

        // ---------------- STUDENTS ----------------

        public Student GetStudentByUserId(int userId)
        {
            using (var conn = GetConnection())
            {
                conn.Open();

                return conn.QueryFirstOrDefault<Student>(
                    @"SELECT s.*, u.full_name, u.email 
                      FROM Students s
                      JOIN Users u ON s.user_id = u.user_id
                      WHERE s.user_id = @userId",
                    new { userId }
                );
            }
        }

        // ---------------- APPLICATIONS ----------------

        public List<Application> GetAllApplications()
        {
            using (var conn = GetConnection())
            {
                conn.Open();

                return conn.Query<Application>(
                    @"SELECT a.*, c.title as course_title, u.full_name as student_name
                      FROM Applications a
                      JOIN Students s ON a.student_id = s.student_id
                      JOIN Users u ON s.user_id = u.user_id
                      JOIN Courses c ON a.course_id = c.course_id
                      ORDER BY a.date_created DESC"
                ).ToList();
            }
        }

        public List<Application> GetApplicationsByStudent(int studentId)
        {
            using (var conn = GetConnection())
            {
                conn.Open();

                return conn.Query<Application>(
                    @"SELECT a.*, c.title as course_title
                      FROM Applications a
                      JOIN Courses c ON a.course_id = c.course_id
                      WHERE a.student_id = @studentId
                      ORDER BY a.date_created DESC",
                    new { studentId }
                ).ToList();
            }
        }

        public bool CreateApplication(int studentId, int courseId)
        {
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();

                    // Проверяем, существует ли уже заявка
                    int count = conn.ExecuteScalar<int>(
                        "SELECT COUNT(*) FROM Applications WHERE student_id = @studentId AND course_id = @courseId",
                        new { studentId, courseId }
                    );

                    if (count > 0)
                        return false;

                    string sql = @"
                        INSERT INTO Applications (student_id, course_id, status, date_created)
                        VALUES (@studentId, @courseId, 'pending', CURRENT_TIMESTAMP)";

                    int result = conn.Execute(sql, new { studentId, courseId });
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in CreateApplication: {ex.Message}");
                return false;
            }
        }

        public bool HasApplication(int userId, int courseId)
        {
            using (var conn = GetConnection())
            {
                conn.Open();

                var student = GetStudentByUserId(userId);
                if (student == null)
                    return false;

                int count = conn.ExecuteScalar<int>(
                    "SELECT COUNT(*) FROM Applications WHERE student_id = @studentId AND course_id = @courseId",
                    new { studentId = student.student_id, courseId }
                );

                return count > 0;
            }
        }

        public bool UpdateApplicationStatus(int applicationId, string status)
        {
            using (var conn = GetConnection())
            {
                conn.Open();

                return conn.Execute(
                    "UPDATE Applications SET status = @status WHERE application_id = @applicationId",
                    new { applicationId, status }
                ) > 0;
            }
        }

        public bool IsApplicationExists(int studentId, int courseId)
        {
            using (var conn = GetConnection())
            {
                conn.Open();

                int count = conn.ExecuteScalar<int>(
                    "SELECT COUNT(*) FROM Applications WHERE student_id = @studentId AND course_id = @courseId",
                    new { studentId, courseId }
                );

                return count > 0;
            }
        }

        // ---------------- TEACHERS ----------------

        public Teacher GetTeacherByUserId(int userId)
        {
            using (var conn = GetConnection())
            {
                conn.Open();

                return conn.QueryFirstOrDefault<Teacher>(
                    @"SELECT t.*, u.full_name, u.email 
                      FROM Teachers t
                      JOIN Users u ON t.user_id = u.user_id
                      WHERE t.user_id = @userId",
                    new { userId }
                );
            }
        }

        // ---------------- STATS ----------------

        public int GetStudentCountForCourse(int courseId)
        {
            using (var conn = GetConnection())
            {
                conn.Open();

                return conn.ExecuteScalar<int>(
                    "SELECT COUNT(*) FROM Applications WHERE course_id = @courseId AND status = 'approved'",
                    new { courseId }
                );
            }
        }

        public bool IsCourseAvailable(int courseId)
        {
            var course = GetCourseById(courseId);
            return course != null && course.status == "approved";
        }

        public bool ValidateApplication(int studentId, int courseId)
        {
            using (var conn = GetConnection())
            {
                conn.Open();

                int studentCount = conn.ExecuteScalar<int>(
                    "SELECT COUNT(*) FROM Students WHERE student_id = @studentId",
                    new { studentId }
                );

                int courseCount = conn.ExecuteScalar<int>(
                    "SELECT COUNT(*) FROM Courses WHERE course_id = @courseId AND status = 'approved'",
                    new { courseId }
                );

                if (studentCount == 0 || courseCount == 0)
                    return false;
            }

            return !IsApplicationExists(studentId, courseId);
        }

        // Получить заявки студента с деталями курса
        public List<MyApplicationsForm.ApplicationWithDetails> GetApplicationsByStudentWithDetails(int studentId)
        {
            using (var conn = GetConnection())
            {
                conn.Open();

                string sql = @"
            SELECT 
                a.application_id,
                a.student_id,
                a.course_id,
                c.title as course_title,
                a.status,
                a.date_created
            FROM Applications a
            JOIN Courses c ON a.course_id = c.course_id
            WHERE a.student_id = @studentId
            ORDER BY a.date_created DESC";

                return conn.Query<MyApplicationsForm.ApplicationWithDetails>(sql, new { studentId }).ToList();
            }
        }

        // Удалить заявку
        public bool DeleteApplication(int applicationId)
        {
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();

                    // Проверяем, что заявка в статусе 'pending'
                    var application = conn.QueryFirstOrDefault<Application>(
                        "SELECT * FROM Applications WHERE application_id = @applicationId",
                        new { applicationId }
                    );

                    if (application == null)
                        return false;

                    if (application.status != "pending")
                        return false;

                    // Удаляем заявку
                    int result = conn.Execute(
                        "DELETE FROM Applications WHERE application_id = @applicationId",
                        new { applicationId }
                    );

                    return result > 0;
                }
            }
            catch
            {
                return false;
            }
        }

        // ---------------- TEACHER METHODS ----------------

        // Получить курсы преподавателя
        public List<Course> GetCoursesByTeacher(int teacherId)
        {
            using (var conn = GetConnection())
            {
                conn.Open();

                return conn.Query<Course>(
                    "SELECT * FROM Courses WHERE teacher_id = @teacherId ORDER BY course_id",
                    new { teacherId }
                ).ToList();
            }
        }

        // Получить расписание по курсу
        public List<Schedule> GetScheduleByCourse(int courseId)
        {
            using (var conn = GetConnection())
            {
                conn.Open();

                return conn.Query<Schedule>(
                    "SELECT * FROM Schedule WHERE group_id = @courseId ORDER BY lesson_date, lesson_time",
                    new { courseId }
                ).ToList();
            }
        }

        // Получить студентов курса для посещаемости
        public DataTable GetStudentsForAttendance(int scheduleId)
        {
            using (var conn = GetConnection())
            {
                conn.Open();

                string sql = @"
            SELECT 
                s.student_id,
                u.full_name as student_name,
                a.status,
                a.attendance_id
            FROM Students s
            JOIN Users u ON s.user_id = u.user_id
            LEFT JOIN Attendance a ON a.student_id = s.student_id AND a.schedule_id = @scheduleId
            WHERE s.student_id IN (
                SELECT DISTINCT student_id FROM Applications 
                WHERE course_id = (SELECT group_id FROM Schedule WHERE schedule_id = @scheduleId)
                AND status = 'approved'
            )
            ORDER BY u.full_name";

                return QueryTable(sql, new { scheduleId });
            }
        }

        // Сохранить посещаемость
        public bool SaveAttendance(int studentId, int scheduleId, string status)
        {
            using (var conn = GetConnection())
            {
                conn.Open();

                // Проверяем, существует ли запись
                int count = conn.ExecuteScalar<int>(
                    "SELECT COUNT(*) FROM Attendance WHERE student_id = @studentId AND schedule_id = @scheduleId",
                    new { studentId, scheduleId }
                );

                if (count > 0)
                {
                    // Обновляем существующую
                    string sql = "UPDATE Attendance SET status = @status WHERE student_id = @studentId AND schedule_id = @scheduleId";
                    return conn.Execute(sql, new { studentId, scheduleId, status }) > 0;
                }
                else
                {
                    // Добавляем новую
                    string sql = "INSERT INTO Attendance (student_id, schedule_id, status) VALUES (@studentId, @scheduleId, @status)";
                    return conn.Execute(sql, new { studentId, scheduleId, status }) > 0;
                }
            }
        }

        // ---------------- COURSE REQUESTS METHODS ----------------

        // Получить заявки на курсы преподавателя
        public DataTable GetCourseRequestsByTeacher(int teacherId)
        {
            using (var conn = GetConnection())
            {
                conn.Open();

                return QueryTable(
                    "SELECT * FROM CourseRequests WHERE teacher_id = @teacherId ORDER BY request_id DESC",
                    new { teacherId }
                );
            }
        }

        // Получить заявку по ID
        public DataRow GetCourseRequestById(int requestId)
        {
            using (var conn = GetConnection())
            {
                conn.Open();

                DataTable dt = QueryTable(
                    "SELECT * FROM CourseRequests WHERE request_id = @requestId",
                    new { requestId }
                );

                if (dt.Rows.Count > 0)
                    return dt.Rows[0];

                return null;
            }
        }

        // Добавить заявку на курс
        public bool AddCourseRequest(int teacherId, string title, int hours, string format, string scheduleFile)
        {
            using (var conn = GetConnection())
            {
                conn.Open();

                string sql = @"
            INSERT INTO CourseRequests (teacher_id, title, hours, format, schedule_file, status)
            VALUES (@teacherId, @title, @hours, @format, @scheduleFile, 'pending')";

                return conn.Execute(sql, new { teacherId, title, hours, format, scheduleFile }) > 0;
            }
        }



        // Обновить заявку на курс
        public bool UpdateCourseRequest(int requestId, string title, int hours, string format, string scheduleFile)
        {
            using (var conn = GetConnection())
            {
                conn.Open();

                string sql = @"
            UPDATE CourseRequests 
            SET title = @title, 
                hours = @hours, 
                format = @format, 
                schedule_file = @scheduleFile,
                status = 'pending'
            WHERE request_id = @requestId AND status = 'pending'";

                return conn.Execute(sql, new { requestId, title, hours, format, scheduleFile }) > 0;
            }
        }



        // Удалить заявку на курс
        public bool DeleteCourseRequest(int requestId)
        {
            using (var conn = GetConnection())
            {
                conn.Open();

                DataRow request = GetCourseRequestById(requestId);
                if (request == null || request["status"].ToString() != "pending")
                    return false;

                return conn.Execute(
                    "DELETE FROM CourseRequests WHERE request_id = @requestId",
                    new { requestId }
                ) > 0;
            }
        }

        // Получить все заявки на курсы (с именем преподавателя)
        public DataTable GetAllCourseRequests()
        {
            using (var conn = GetConnection())
            {
                conn.Open();

                string sql = @"
            SELECT 
                cr.request_id,
                cr.teacher_id,
                u.full_name as teacher_name,
                cr.title,
                cr.hours,
                cr.format,
                cr.schedule_file,
                cr.status
            FROM CourseRequests cr
            JOIN Teachers t ON cr.teacher_id = t.teacher_id
            JOIN Users u ON t.user_id = u.user_id
            ORDER BY 
                CASE cr.status 
                    WHEN 'pending' THEN 1 
                    WHEN 'approved' THEN 2 
                    WHEN 'rejected' THEN 3 
                END,
                cr.request_id DESC";

                return QueryTable(sql);
            }
        }

        // Одобрить заявку на курс и добавить в Courses
        public bool ApproveCourseRequest(int requestId, int teacherId, string title, int hours, string format)
        {
            using (var conn = GetConnection())
            {
                conn.Open();

                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // 1. Обновляем статус заявки
                        string updateRequest = "UPDATE CourseRequests SET status = 'approved' WHERE request_id = @requestId";
                        conn.Execute(updateRequest, new { requestId }, transaction);

                        // 2. Добавляем курс в таблицу Courses
                        string insertCourse = @"
                    INSERT INTO Courses (title, hours, price, teacher_id, status, description)
                    VALUES (@title, @hours, 0, @teacherId, 'approved', @description)";

                        conn.Execute(insertCourse, new { title, hours, teacherId, description = $"Курс по {title}" }, transaction);

                        transaction.Commit();
                        return true;
                    }
                    catch
                    {
                        transaction.Rollback();
                        return false;
                    }
                }
            }
        }

        // Отклонить заявку
        public bool RejectCourseRequest(int requestId)
        {
            using (var conn = GetConnection())
            {
                conn.Open();

                string sql = "UPDATE CourseRequests SET status = 'rejected' WHERE request_id = @requestId";
                return conn.Execute(sql, new { requestId }) > 0;
            }
        }

        // ---------------- SCHEDULE METHODS ----------------

        // Сгенерировать CSV файл с расписанием преподавателя
        public string GenerateTeacherSchedule(int teacherId)
        {
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();

                    string sql = @"
                SELECT 
                    c.title as course_title,
                    s.lesson_date,
                    s.lesson_time,
                    s.topic,
                    COUNT(DISTINCT a.student_id) as students_count
                FROM Schedule s
                JOIN Courses c ON s.group_id = c.course_id
                LEFT JOIN Applications a ON a.course_id = c.course_id AND a.status = 'approved'
                WHERE c.teacher_id = @teacherId
                GROUP BY s.schedule_id, c.title
                ORDER BY s.lesson_date, s.lesson_time";

                    var schedule = conn.Query(sql, new { teacherId }).ToList();

                    if (schedule.Count == 0)
                        return null;

                    // Создаем CSV файл
                    string tempPath = System.IO.Path.GetTempPath();
                    string fileName = $"Schedule_Teacher_{teacherId}_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
                    string filePath = System.IO.Path.Combine(tempPath, fileName);

                    using (var writer = new System.IO.StreamWriter(filePath, false, System.Text.Encoding.UTF8))
                    {
                        writer.WriteLine("Курс;Дата;Время;Тема;Количество студентов");

                        foreach (var item in schedule)
                        {
                            writer.WriteLine($"{item.course_title};{item.lesson_date:dd.MM.yyyy};{item.lesson_time};{item.topic};{item.students_count}");
                        }
                    }

                    return filePath;
                }
            }
            catch
            {
                return null;
            }
        }
    }
}