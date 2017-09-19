using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Win10UniversalClient
{
    public class PropertyBinding
    {
        private bool _buttonEnabled;
        public bool ButtonEnabled
        {
            get
            {
                return _buttonEnabled;
            }
            set
            {
                _buttonEnabled = value;
                OnPropertyChanged();
            }
        }

        public void SetButtonEnabled(bool isEnable)
        {
            ButtonEnabled = isEnable;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName]string caller = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(caller));
            }
        }
    }
}
