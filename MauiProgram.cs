using Microsoft.Extensions.Logging;

namespace C868_RyanNewman
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "C868.db3");

            //Resgister Veiws
            builder.Services.AddSingleton<MainPage>();
            builder.Services.AddTransient<TermDetailsPage>();
            builder.Services.AddTransient<CourseDetailsPage>();
            builder.Services.AddTransient<AddTermPage>();
            builder.Services.AddTransient<CourseSelectionPage>();
            builder.Services.AddTransient<AddCoursePage>();
            builder.Services.AddTransient<CourseInformationPage>();
            builder.Services.AddTransient<PerformanceAssessmentPage>();
            builder.Services.AddTransient<ObjectiveAssessmentPage>();
            builder.Services.AddTransient<UserLoginPage>();
            builder.Services.AddTransient<RegisterPage>();


            //Register ViewModels
            builder.Services.AddSingleton<TermViewModel>();
            builder.Services.AddTransient<TermDetailsViewModel>();
            builder.Services.AddTransient<CourseDetailsViewModel>();
            builder.Services.AddTransient<AddTermViewModel>();
            builder.Services.AddTransient<CourseSelectionViewModel>();
            builder.Services.AddTransient<AddCourseViewModel>();
            builder.Services.AddTransient<CourseInformationViewModel>();
            builder.Services.AddTransient<PerformanceAssessmentViewModel>();
            builder.Services.AddTransient<ObjectiveAssessmentViewModel>();
            builder.Services.AddTransient<UserLoginViewModel>();
            builder.Services.AddTransient<RegisterViewModel>();


            // Register Services
            builder.Services.AddSingleton<TermService>(s => ActivatorUtilities.CreateInstance<TermService>(s, dbPath));
            builder.Services.AddSingleton<CourseService>(s => ActivatorUtilities.CreateInstance<CourseService>(s, dbPath));
            builder.Services.AddSingleton<UserService>(s => ActivatorUtilities.CreateInstance<UserService>(s, dbPath));

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }

    }
}
