using System.Collections.ObjectModel;

namespace C868_RyanNewman.ViewModels
{
    [QueryProperty(nameof(TermId), "TermId")] 
    [QueryProperty(nameof(CourseId), "CourseId")] 
    public partial class CourseSelectionViewModel : BaseViewModel, INotifyPropertyChanged
    {
        private readonly CourseService _courseService;
        private ObservableCollection<Course> _courses;
        private bool _isBusy;
        private Course _course;

        // Observable collection to hold the courses
        public ObservableCollection<Course> Courses
        {
            get => _courses;
            set
            {
                _courses = value;
                OnPropertyChanged(nameof(Courses));
            }
        }

        // IsBusy property to prevent multiple requests at once
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                if (_isBusy != value)
                {
                    _isBusy = value;
                    OnPropertyChanged(nameof(IsBusy));
                }
            }
        }

        // Property to hold the Term ID
        private int _termId;
        public int TermId
        {
            get => _termId;
            set
            {
                if (_termId != value)
                {
                    _termId = value;
                    OnPropertyChanged(nameof(TermId));
                    // Trigger loading of courses whenever TermId is changed
                    LoadCoursesAsync(_termId);
                }
            }
        }
        private int _courseId;
        public int CourseId
        {
            get => _courseId;
            set
            {
                _courseId = value;
                LoadCoursesAsync(_courseId); // Load course details when CourseId is set
            }
        }
        // Constructor that takes in CourseService
        public CourseSelectionViewModel(CourseService courseService)
        {
            _courseService = courseService;
            AddCourseCommand = new Command(async () => await AddCourse());
            //GetCourseDetailsCommand = new Command(async () => await GetCourseDetails(_course));
            //GetCourseDetailsCommand = new Command<Course>(async (_course) => await GetCourseDetails(_course));
            EditCourseCommand = new Command<Course>(async (course) => await EditCourse(course));
            GetCourseInformationCommand = new Command<Course>(async (Course course) => await GetCourseInformation(course));

            Courses = new ObservableCollection<Course>();
        }
        private async Task EditCourse(Course course)
        {
            if (course == null || course.Id <= 0)
            {
                Debug.WriteLine("Invalid course or course Id is 0.");
                await Shell.Current.DisplayAlert("Error", "Invalid course selected for editing.", "OK");
                return;
            }

            if (IsBusy) return;

            // Navigate to TermDetailsPage with the selected term's ID
            await Shell.Current.GoToAsync($"{nameof(CourseDetailsPage)}?CourseId={course.Id}");
        }
        public ICommand EditCourseCommand { get; }
        public ICommand GetCourseInformationCommand { get; }
        private async Task GetCourseInformation(Course selectedCourse)
        {
            if (selectedCourse == null) return;

            Console.WriteLine($"Navigating to CourseInformationPage with Course Id: {selectedCourse.Id}");
            await Shell.Current.GoToAsync($"{nameof(CourseInformationPage)}?CourseId={selectedCourse.Id}");
        }
        private async Task GetCourseDetails(Course selectedCourse)
        {
            if (selectedCourse == null) return;

            Console.WriteLine($"Navigating to CourseDetailsPage with Course Id: {selectedCourse.Id}");
            await Shell.Current.GoToAsync($"{nameof(CourseDetailsPage)}?CourseId={selectedCourse.Id}");
        }

        // Method to load courses for the given term
        public async Task LoadCoursesAsync(int termId)
        {
            if (IsBusy) return;

            IsBusy = true;

            try
            {
                // Fetch courses for the term from the service
                var courses = await _courseService.GetCoursesForTermAndUserAsync(termId, App.CurrentUserId);

                if (courses == null || !courses.Any())
                {
                    Debug.WriteLine("No courses found for the specified term.");
                    Courses = new ObservableCollection<Course>(); // Clear or set to empty list
                }
                else
                {
                    // Populate the observable collection with courses
                    Courses = new ObservableCollection<Course>(courses);
                    Debug.WriteLine($"Successfully loaded {Courses.Count} courses for term {termId}.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to load courses: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        // Command to navigate to AddCoursePage
        public ICommand AddCourseCommand { get; }
        public ICommand GetCourseDetailsCommand { get; }

        // Navigate to AddCoursePage and pass the current TermId
        private async Task AddCourse()
        {
            Debug.WriteLine($"Navigating to AddCoursePage with TermId: {TermId}");
            await Shell.Current.GoToAsync($"AddCoursePage?TermId={TermId}");
        }

        // INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}