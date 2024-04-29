using System.ComponentModel;
using Xamarin.Forms;

namespace MentalTest.ViewModels
{
    //Result test page
    public class FinalResultsViewModel : INotifyPropertyChanged
    {
        private string _finalResultMessage;

        public Command ReturnHomeCommand { get; private set; }

        public string FinalResultMessage
        {
            get { return _finalResultMessage; }
            set
            {
                if (_finalResultMessage != value)
                {
                    _finalResultMessage = value;
                    OnPropertyChanged(nameof(FinalResultMessage));
                }
            }
        }

        public FinalResultsViewModel(string resultText)
        {
            FinalResultMessage = resultText;
            ReturnHomeCommand = new Command(ReturnHomeExecute);
        }

        private async void ReturnHomeExecute()
        {
            await Application.Current.MainPage.Navigation.PopToRootAsync();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}