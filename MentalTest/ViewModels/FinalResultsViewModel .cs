using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace MentalTest.ViewModels
{
    public class FinalResultsViewModel : INotifyPropertyChanged
    {
        private string _finalResultMessage;
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
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }


}
