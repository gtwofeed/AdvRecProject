using Crud;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GrpcWpfClient.Models
{
    public class Worker : INotifyPropertyChanged
    {
        private string lastName;
        private long birthday;
        private Sex sex = Sex.Default;
        public int Id { get; set; }
        public string LastName
        {
            get => lastName;
            set
            {
                lastName = value;
                OnPropertyChanged("LastName");
            }
        }
        public long Birthday
        {
            get => birthday;
            set
            {
                birthday = value;
                OnPropertyChanged("Birthday");
            }
        }
        public Sex Sex
        {
            get => sex;
            set
            {
                sex = value;
                OnPropertyChanged("Sex");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
