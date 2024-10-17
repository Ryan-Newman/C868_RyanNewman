namespace C868_RyanNewman.ViewModels
{
    public abstract class EntityViewModel<TEntity, TService> : BaseViewModel
     where TEntity : class, new()
     where TService : class
    {
        protected readonly TService _service;  // Generic service for Term, Course, etc.
        private TEntity _entity;

        public TEntity Entity
        {
            get => _entity;
            set
            {
                _entity = value;
                OnPropertyChanged(nameof(Entity));
            }
        }

        public ICommand SaveCommand { get; }

        protected EntityViewModel(TService service)
        {
            _service = service;
            Entity = new TEntity();
            SaveCommand = new Command(async () => await OnSaveAsync());
        }

        protected abstract Task OnSaveAsync();  // Each ViewModel will implement its save logic
    }

}
