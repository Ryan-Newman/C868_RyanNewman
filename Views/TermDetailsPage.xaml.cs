namespace C868_RyanNewman.Views;

public partial class TermDetailsPage : ContentPage
{
	public TermDetailsPage(TermDetailsViewModel termDetailsViewModel)
	{
		InitializeComponent();
		BindingContext = termDetailsViewModel;
	}

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        
    }
}
