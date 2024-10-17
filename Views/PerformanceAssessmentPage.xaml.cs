namespace C868_RyanNewman.Views;

public partial class PerformanceAssessmentPage : ContentPage
{
	public PerformanceAssessmentPage(PerformanceAssessmentViewModel performanceAssessmentViewModel)
	{
		InitializeComponent();
        BindingContext = performanceAssessmentViewModel;

    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        var viewModel = BindingContext as PerformanceAssessmentViewModel;

    }
}