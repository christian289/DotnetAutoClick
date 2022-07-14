using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ClickTest
{
    public class BaseVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChagned([CallerMemberName] string? propertyName = null)
        {
            if (PropertyChanged is not null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
