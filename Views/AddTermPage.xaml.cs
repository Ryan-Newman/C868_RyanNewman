namespace C868_RyanNewman.Views;

[QueryProperty(nameof(DbPath), "dbPath")]
public partial class AddTermPage : ContentPage
{
    private string dbPath;

    public string DbPath
    {
        get => dbPath;
        set
        {
            dbPath = value;
            BindingContext = new AddTermViewModel(dbPath);
        }
    }
    public AddTermPage()
	{
		InitializeComponent();

	}
    private async void OnAddTermsButtonClicked(object sender, EventArgs e)
    {
        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "C868.db3");
        await Shell.Current.GoToAsync($"{nameof(AddTermPage)}?dbPath={dbPath}");
    }
}