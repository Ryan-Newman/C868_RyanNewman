namespace C868_RyanNewman.Models
{
    public class Course : BaseEntity
    {
        public string Title { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime DueDate { get; set; }
        public string DueDateText => $"Due Date: {DueDate.ToString("M/d/yyyy")}";
        public string DateRange => $"Start: {StartDate.ToString("M/d/yyyy")} - End: {EndDate.ToString("M/d/yyyy")}";
        public string Status { get; set; }
        public string? OptionalNotes { get; set; }
        // Create a foreign key for Instructor
        public int InstructorId { get; set; }
        // Create a foreign key for TermId
        public int TermId { get; set; }
        // Create a foreign key for UserId
        public int UserId { get; set; }

        public string InstructorName { get; set; }
        public string InstructorPhoneNumber { get; set; }
        public string InstructorEmail { get; set; }

        // Read-only property to return a formatted phone number
        public string FormattedPhoneNumber
        {
            get
            {
                if (string.IsNullOrWhiteSpace(InstructorPhoneNumber) || InstructorPhoneNumber.Length < 10)
                {
                    // Return as is if invalid or too short
                    return InstructorPhoneNumber; 
                }

                // Ensure only digits are used for formatting
                string digits = new string(InstructorPhoneNumber.Where(char.IsDigit).ToArray());

                // Apply the (123) 456-7890 format
                return string.Format("({0}) {1}-{2}",
                    digits.Substring(0, 3),  // Area code
                    digits.Substring(3, 3),  // First 3 digits
                    digits.Substring(6));    // Last 4 digits
            }
        }
        // Toggle properties
        public bool IsCourseStartDateAlertEnabled { get; set; }
        public bool IsCourseEndDateAlertEnabled { get; set; }
        public bool IsPerformanceAssessmentAlertEnabled { get; set; }
        public bool IsObjectiveAssessmentAlertEnabled { get; set; }
        public Course()
        {
            IsCourseStartDateAlertEnabled = false;
            IsCourseEndDateAlertEnabled = false;
            IsPerformanceAssessmentAlertEnabled = false;
            IsObjectiveAssessmentAlertEnabled = false;
        }

        public string PerformanceAssessmentName { get; set; }
        public DateTime? PerformanceAssessmentStartDate { get; set; }
        public DateTime? PerformanceAssessmentEndDate { get; set; }
        public string PerformanceAssessmentStatus { get; set; }
        public string ObjectiveAssessmentName { get; set; }
        public DateTime? ObjectiveAssessmentStartDate { get; set; }
        public DateTime? ObjectiveAssessmentEndDate { get; set; }
        public string ObjectiveAssessmentStatus { get; set; }
    }

}
