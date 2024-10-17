namespace C868_RyanNewman.Views;
[QueryProperty(nameof(TermId), "TermId")]
public partial class CourseSelectionPage : ContentPage
{
    private readonly CourseSelectionViewModel _viewModel;
    public int TermId { get; set; }
    // Modify the constructor to accept CourseService and termId
    public CourseSelectionPage(CourseService courseService)
    {
        InitializeComponent();
        BindingContext = new CourseSelectionViewModel(courseService);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is CourseSelectionViewModel viewModel)
        {
            await viewModel.LoadCoursesAsync(TermId);
        }
    }
}
