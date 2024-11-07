using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace MentalTest.Utils
{
    public class AnswerOption : INotifyPropertyChanged
    {
        private bool _isSelected;

        public string AnswerText { get; set; }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}