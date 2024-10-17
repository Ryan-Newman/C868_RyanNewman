namespace C868_RyanNewman.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        string _title;
        bool _isBusy;
        public bool IsNotBusy => !IsBusy;


        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                if (_isBusy != value)
                    return;
                _isBusy = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsNotBusy));
            }
        }

        public string Title
        {
            get => _title;
            set
            {
                if (_title != value)
                    return;
                _title = value;
                OnPropertyChanged();

            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
