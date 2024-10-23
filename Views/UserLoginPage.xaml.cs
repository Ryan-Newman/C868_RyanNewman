namespace C868_RyanNewman.Views;

public partial class UserLoginPage : ContentPage
{
	public UserLoginPage(UserLoginViewModel userLoginViewModel)
	{
		InitializeComponent();
        BindingContext = userLoginViewModel;

        // Check if the command is correctly set
        if (userLoginViewModel.NavigateToRegisterCommand == null)
        {
            Console.WriteLine("NavigateToRegisterCommand is null");
        }
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

    }
}