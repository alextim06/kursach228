using System;
using System.Collections.Generic;
using System.Data;
using Npgsql;

namespace kursach
{
    public class DatabaseAdapter : IDatabaseAdapter
    {
        private string connectionString;

        public DatabaseAdapter(string host, string database, string username, string password, int port = 5432)
        {
            connectionString = $"Host={host};Port={port};Database={database};Username={username};Password={password}";
        }

        private NpgsqlConnection GetConnection()
        {
            return new NpgsqlConnection(connectionString);
        }

        public List<Course> GetAllCourses()
        {
            var courses = new List<Course>();
            string query = "SELECT * FROM Courses WHERE status = 'approved'";

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        courses.Add(new Course
                        {
                            course_id = reader.GetInt32(0),
                            title = reader.GetString(1),
                            hours = reader.GetInt32(2),
                            price = reader.GetDecimal(3),
                            teacher_id = reader.GetInt32(4),
                            status = reader.GetString(5),
                            description = reader.GetString(6)
                        });
                    }
                }
            }
            return courses;
        }

        public Course GetCourseById(int courseId)
        {
            string query = "SELECT * FROM Courses WHERE course_id = @course_id";

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@course_id", courseId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Course
                            {
                                course_id = reader.GetInt32(0),
                                title = reader.GetString(1),
                                hours = reader.GetInt32(2),
                                price = reader.GetDecimal(3),
                                teacher_id = reader.GetInt32(4),
                                status = reader.GetString(5),
                                description = reader.GetString(6)
                            };
                        }
                    }
                }
            }
            return null;
        }

        public bool AddCourse(Course course)
        {
            string query = @"INSERT INTO Courses (title, hours, price, teacher_id, status, description) 
                            VALUES (@title, @hours, @price, @teacher_id, @status, @description)";

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@title", course.title);
                    cmd.Parameters.AddWithValue("@hours", course.hours);
                    cmd.Parameters.AddWithValue("@price", course.price);
                    cmd.Parameters.AddWithValue("@teacher_id", course.teacher_id);
                    cmd.Parameters.AddWithValue("@status", course.status);
                    cmd.Parameters.AddWithValue("@description", course.description);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool UpdateCourse(Course course)
        {
            string query = @"UPDATE Courses SET title=@title, hours=@hours, price=@price, 
                            teacher_id=@teacher_id, status=@status, description=@description 
                            WHERE course_id=@course_id";

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@course_id", course.course_id);
                    cmd.Parameters.AddWithValue("@title", course.title);
                    cmd.Parameters.AddWithValue("@hours", course.hours);
                    cmd.Parameters.AddWithValue("@price", course.price);
                    cmd.Parameters.AddWithValue("@teacher_id", course.teacher_id);
                    cmd.Parameters.AddWithValue("@status", course.status);
                    cmd.Parameters.AddWithValue("@description", course.description);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool DeleteCourse(int courseId)
        {
            string query = "DELETE FROM Courses WHERE course_id = @course_id";

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@course_id", courseId);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public List<Application> GetAllApplications()
        {
            var applications = new List<Application>();
            string query = "SELECT * FROM Applications ORDER BY date_created DESC";

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        applications.Add(new Application
                        {
                            application_id = reader.GetInt32(0),
                            student_id = reader.GetInt32(1),
                            course_id = reader.GetInt32(2),
                            status = reader.GetString(3),
                            date_created = reader.GetDateTime(4)
                        });
                    }
                }
            }
            return applications;
        }

        public List<Application> GetApplicationsByStatus(string status)
        {
            var applications = new List<Application>();
            string query = "SELECT * FROM Applications WHERE status = @status ORDER BY date_created DESC";

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@status", status);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            applications.Add(new Application
                            {
                                application_id = reader.GetInt32(0),
                                student_id = reader.GetInt32(1),
                                course_id = reader.GetInt32(2),
                                status = reader.GetString(3),
                                date_created = reader.GetDateTime(4)
                            });
                        }
                    }
                }
            }
            return applications;
        }

        public List<Application> GetApplicationsByStudent(int studentId)
        {
            var applications = new List<Application>();
            string query = "SELECT * FROM Applications WHERE student_id = @student_id";

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@student_id", studentId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            applications.Add(new Application
                            {
                                application_id = reader.GetInt32(0),
                                student_id = reader.GetInt32(1),
                                course_id = reader.GetInt32(2),
                                status = reader.GetString(3),
                                date_created = reader.GetDateTime(4)
                            });
                        }
                    }
                }
            }
            return applications;
        }

        public bool CreateApplication(int studentId, int courseId)
        {
            if (IsApplicationExists(studentId, courseId))
            {
                return false;
            }

            string query = @"INSERT INTO Applications (student_id, course_id, status, date_created) 
                            VALUES (@student_id, @course_id, 'pending', CURRENT_TIMESTAMP)";

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@student_id", studentId);
                    cmd.Parameters.AddWithValue("@course_id", courseId);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool UpdateApplicationStatus(int applicationId, string status)
        {
            string query = "UPDATE Applications SET status = @status WHERE application_id = @application_id";

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@application_id", applicationId);
                    cmd.Parameters.AddWithValue("@status", status);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool IsApplicationExists(int studentId, int courseId)
        {
            string query = "SELECT COUNT(*) FROM Applications WHERE student_id = @student_id AND course_id = @course_id";

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@student_id", studentId);
                    cmd.Parameters.AddWithValue("@course_id", courseId);
                    long count = (long)cmd.ExecuteScalar();
                    return count > 0;
                }
            }
        }

        public List<CourseRequest> GetAllCourseRequests()
        {
            var requests = new List<CourseRequest>();
            string query = "SELECT * FROM CourseRequests ORDER BY request_id DESC";

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        requests.Add(new CourseRequest
                        {
                            request_id = reader.GetInt32(0),
                            teacher_id = reader.GetInt32(1),
                            title = reader.GetString(2),
                            hours = reader.GetInt32(3),
                            format = reader.GetString(4),
                            schedule_file = reader.IsDBNull(5) ? null : reader.GetString(5),
                            status = reader.GetString(6)
                        });
                    }
                }
            }
            return requests;
        }

        public List<CourseRequest> GetCourseRequestsByTeacher(int teacherId)
        {
            var requests = new List<CourseRequest>();
            string query = "SELECT * FROM CourseRequests WHERE teacher_id = @teacher_id";

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@teacher_id", teacherId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            requests.Add(new CourseRequest
                            {
                                request_id = reader.GetInt32(0),
                                teacher_id = reader.GetInt32(1),
                                title = reader.GetString(2),
                                hours = reader.GetInt32(3),
                                format = reader.GetString(4),
                                schedule_file = reader.IsDBNull(5) ? null : reader.GetString(5),
                                status = reader.GetString(6)
                            });
                        }
                    }
                }
            }
            return requests;
        }

        public bool CreateCourseRequest(CourseRequest request)
        {
            string query = @"INSERT INTO CourseRequests (teacher_id, title, hours, format, schedule_file, status) 
                            VALUES (@teacher_id, @title, @hours, @format, @schedule_file, 'pending')";

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@teacher_id", request.teacher_id);
                    cmd.Parameters.AddWithValue("@title", request.title);
                    cmd.Parameters.AddWithValue("@hours", request.hours);
                    cmd.Parameters.AddWithValue("@format", request.format);
                    cmd.Parameters.AddWithValue("@schedule_file", (object)request.schedule_file ?? DBNull.Value);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool UpdateCourseRequestStatus(int requestId, string status)
        {
            string query = "UPDATE CourseRequests SET status = @status WHERE request_id = @request_id";

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@request_id", requestId);
                    cmd.Parameters.AddWithValue("@status", status);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public User AuthenticateUser(string login, string password)
        {
            string query = "SELECT * FROM Users WHERE login = @login AND password = @password";

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@login", login);
                    cmd.Parameters.AddWithValue("@password", password);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new User
                            {
                                user_id = reader.GetInt32(0),
                                login = reader.GetString(1),
                                password = reader.GetString(2),
                                role = reader.GetString(3),
                                full_name = reader.IsDBNull(4) ? null : reader.GetString(4),
                                email = reader.IsDBNull(5) ? null : reader.GetString(5)
                            };
                        }
                    }
                }
            }
            return null;
        }

        public Student GetStudentByUserId(int userId)
        {
            string query = "SELECT * FROM Students WHERE user_id = @user_id";

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@user_id", userId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Student
                            {
                                student_id = reader.GetInt32(0),
                                user_id = reader.GetInt32(1)
                            };
                        }
                    }
                }
            }
            return null;
        }

        public Teacher GetTeacherByUserId(int userId)
        {
            string query = "SELECT * FROM Teachers WHERE user_id = @user_id";

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@user_id", userId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Teacher
                            {
                                teacher_id = reader.GetInt32(0),
                                user_id = reader.GetInt32(1)
                            };
                        }
                    }
                }
            }
            return null;
        }

        public int GetStudentCountForCourse(int courseId)
        {
            string query = "SELECT COUNT(*) FROM Applications WHERE course_id = @course_id AND status = 'approved'";

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@course_id", courseId);
                    long count = (long)cmd.ExecuteScalar();
                    return (int)count;
                }
            }
        }

        public bool IsCourseAvailable(int courseId)
        {
            var course = GetCourseById(courseId);
            return course != null && course.status == "approved";
        }

        public bool ValidateApplication(int studentId, int courseId)
        {
            string studentQuery = "SELECT COUNT(*) FROM Students WHERE student_id = @student_id";
            string courseQuery = "SELECT COUNT(*) FROM Courses WHERE course_id = @course_id AND status = 'approved'";

            using (var conn = GetConnection())
            {
                conn.Open();

                using (var cmd = new NpgsqlCommand(studentQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@student_id", studentId);
                    long studentCount = (long)cmd.ExecuteScalar();
                    if (studentCount == 0) return false;
                }

                using (var cmd = new NpgsqlCommand(courseQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@course_id", courseId);
                    long courseCount = (long)cmd.ExecuteScalar();
                    if (courseCount == 0) return false;
                }
            }

            return !IsApplicationExists(studentId, courseId);
        }

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
    }
}