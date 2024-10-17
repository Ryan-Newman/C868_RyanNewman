namespace C868_RyanNewman.Views;

public partial class CourseDetailsPage : ContentPage
{
	public CourseDetailsPage(CourseDetailsViewModel courseDetailsViewModel)
	{
		InitializeComponent();
        BindingContext = courseDetailsViewModel;

    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

    }
}