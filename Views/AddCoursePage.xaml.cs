namespace C868_RyanNewman.Views;

public partial class AddCoursePage : ContentPage
{
    private readonly CourseSelectionViewModel _courseSelectionViewModel;

    public AddCoursePage(AddCourseViewModel viewModel, CourseSelectionViewModel courseSelectionViewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _courseSelectionViewModel = courseSelectionViewModel;
    }
}