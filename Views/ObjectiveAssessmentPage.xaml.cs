namespace C868_RyanNewman.Views;

public partial class ObjectiveAssessmentPage : ContentPage
{
	public ObjectiveAssessmentPage(ObjectiveAssessmentViewModel objectiveAssessmentViewModel)
	{
		InitializeComponent(); 
        BindingContext = objectiveAssessmentViewModel;

    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        var viewModel = BindingContext as ObjectiveAssessmentViewModel;

    }
}