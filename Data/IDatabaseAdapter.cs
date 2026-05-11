using System.Collections.Generic;

namespace kursach
{
    public interface IDatabaseAdapter
    {
        bool TestConnection();
        List<Course> GetAllCourses();
        bool CreateApplication(int courseId, int userId);
        List<Application> GetAllApplications();
    }
}