using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Win10UniversalClient.Entities
{
    public class DataSample : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public string Field1 { get; set; }

        [NonSerialized]
        private bool isReadOnly = true;

        private bool isEnable;

        public DataSample()
        {
            isReadOnly = true;
            isEnable = false;
        }

        public bool IsReadOnly
        {
            get { return isReadOnly; }

            set
            {
                isReadOnly = value;
                OnPropertyChanged();
            }
        }

        public bool IsEnable
        {
            get { return isEnable; }

            set
            {
                isEnable = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
