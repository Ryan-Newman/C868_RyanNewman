namespace C868_RyanNewman.Views;

public partial class RegisterPage : ContentPage
{
	public RegisterPage(RegisterViewModel registerViewModel)
	{
		InitializeComponent();
        BindingContext = registerViewModel;
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

    }
}