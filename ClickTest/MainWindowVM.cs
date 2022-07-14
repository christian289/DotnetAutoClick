namespace ClickTest
{
    public class MainWindowVM : BaseVM
    {
        private object? _Context;

        public object? Context
        {
            get { return _Context; }
            set
            {
                if (value == _Context) return;

                _Context = value;
                OnPropertyChagned();
            }
        }

    }
}
