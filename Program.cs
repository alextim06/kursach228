using Npgsql;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography.X509Certificates;

namespace kursach
{
    public class Program
    {
        public static bool Run(IDatabaseAdapter db)
        {
            if (!db.TestConnection())
                return false;

            var courses = db.GetAllCourses();
            var result = db.CreateApplication(1, 1);
            var apps = db.GetAllApplications();
           

            return true;
        }

        static void Main(string[] args)
        {

            DatabaseAdapter db = new DatabaseAdapter(
                host: "localhost",
                database: "education_db",
                username: "postgres",
                password: "1234"
            );

            Run(db);

            Console.ReadKey();


            List<Course> courses = db.GetAllCourses();

            Console.WriteLine("=== Список курсов ===");

            db.

            foreach (var course in courses)
            {
                Console.WriteLine($"ID: {course.course_id}");
                Console.WriteLine($"Название: {course.title}");
                Console.WriteLine($"Часы: {course.hours}");
                Console.WriteLine($"Цена: {course.price}");
                Console.WriteLine("------------------------");
            }

            Console.ReadKey(); 
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
      