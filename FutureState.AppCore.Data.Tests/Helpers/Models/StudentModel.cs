using System.Collections.Generic;
using FutureState.AppCore.Data.Attributes;

namespace FutureState.AppCore.Data.Tests.Helpers.Models
{
    public class StudentModel : ModelBase
    {
        public StudentModel()
        {
            Courses = new List<CourseModel>();
            Books = new List<BookModel>();
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }

        [ManyToMany]
        public IList<CourseModel> Courses { get; set; }

        [OneToMany]
        public IList<BookModel> Books { get; set; }
    }
}