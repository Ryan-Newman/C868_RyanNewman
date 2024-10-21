namespace C868_RyanNewman
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(TermDetailsPage), typeof(TermDetailsPage));
            Routing.RegisterRoute(nameof(AddTermPage), typeof(AddTermPage));
            Routing.RegisterRoute(nameof(CourseSelectionPage), typeof(CourseSelectionPage));
            Routing.RegisterRoute(nameof(AddCoursePage), typeof(AddCoursePage));
            Routing.RegisterRoute(nameof(CourseDetailsPage), typeof(CourseDetailsPage));
            Routing.RegisterRoute(nameof(CourseInformationPage), typeof(CourseInformationPage));
            Routing.RegisterRoute(nameof(PerformanceAssessmentPage), typeof(PerformanceAssessmentPage));
            Routing.RegisterRoute(nameof(ObjectiveAssessmentPage), typeof(ObjectiveAssessmentPage));
            Routing.RegisterRoute(nameof(UserLoginPage), typeof(UserLoginPage));
            Routing.RegisterRoute(nameof(RegisterPage), typeof(RegisterPage));

        }
    }
}
