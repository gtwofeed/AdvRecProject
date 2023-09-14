using Crud;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GrpcService.Models
{
    public class Worker : INotifyPropertyChanged
    {
        private string lastName;
        private string firstName;
        private string middleName;
        private long birthday;
        private Sex sex = Sex.Default;
        private bool haveChildren;
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
        public string FirstName
        {
            get => firstName;
            set
            {
                firstName = value;
                OnPropertyChanged("FirstName");
            }
        }
        public string MiddleName
        {
            get => middleName;
            set
            {
                middleName = value;
                OnPropertyChanged("MiddleName");
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
        public bool HasChildren
        {
            get => haveChildren;
            set
            {
                haveChildren = value;
                OnPropertyChanged("HasChildren");
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
