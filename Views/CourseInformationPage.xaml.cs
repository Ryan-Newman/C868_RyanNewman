namespace C868_RyanNewman.Views;

public partial class CourseInformationPage : ContentPage
{
	public CourseInformationPage(CourseInformationViewModel courseInformationViewModel)
	{
		InitializeComponent();
        BindingContext = courseInformationViewModel;

    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        var viewModel = BindingContext as CourseInformationViewModel;


        if (viewModel != null)
        {
          await viewModel.CheckCourseStartDateAlertAsync();
          await viewModel.CheckCourseEndDateAlertAsync();
          await viewModel.CheckObjectiveAssessmentDatesAsync();
          await viewModel.CheckPerformanceAssessmentDatesAsync();
        }
    }
}