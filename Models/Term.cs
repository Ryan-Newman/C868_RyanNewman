using System.Collections.ObjectModel;

namespace C868_RyanNewman.Models
{
    [Table("terms")]
    public class Term : BaseEntity
    {
        public string Name { get; set; }
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
        public string DateRange => $"{StartDate.ToString("M/d/yyyy")} - {EndDate.ToString("M/d/yyyy")}";

        // Foreign Key for UserId
        public int UserId { get; set; }

        // Add a collection of courses
        [Ignore]
        public ObservableCollection<Course> Courses { get; set; } = new ObservableCollection<Course>();

    }
}
