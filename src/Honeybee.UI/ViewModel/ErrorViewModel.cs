using System;
using System.Collections.Generic;
using System.Linq;

namespace Honeybee.UI
{
    public class ErrorViewModel : ViewModelBase
    {
        private string totalErrorCount;
        public string TotalErrorCount
        {
            get => totalErrorCount;
            set => Set(() => totalErrorCount = value, nameof(TotalErrorCount));
        }

        //private string fatalError;
        //public string FatalError
        //{
        //    get => fatalError;
        //    set => Set(() => fatalError = value, nameof(FatalError));
        //}

        private int _currentIndex = 1;
        private string currentErrorIndex = "XX of XX";
        public string CurrentErrorIndex
        {
            get => currentErrorIndex;
            set => Set(() => currentErrorIndex = value, nameof(CurrentErrorIndex));
        }

        private string currentErrorMessage;
        public string CurrentErrorMessage
        {
            get => currentErrorMessage;
            set => Set(() => currentErrorMessage = value, nameof(CurrentErrorMessage));
        }

        private bool _preBtnEnabled;
        public bool PreBtnEnabled
        {
            get => _preBtnEnabled;
            set => Set(() => _preBtnEnabled = value, nameof(PreBtnEnabled));
        }

        private bool _nextBtnEnabled;
        public bool NextBtnEnabled
        {
            get => _nextBtnEnabled;
            set => Set(() => _nextBtnEnabled = value, nameof(NextBtnEnabled));
        }

        private bool _showBtnEnabled;
        public bool ShowBtnEnabled
        {
            get => _showBtnEnabled;
            set => Set(() => _showBtnEnabled = value, nameof(ShowBtnEnabled));
        }

        private bool _showParentBtnEnabled;
        public bool ShowParentBtnEnabled
        {
            get => _showParentBtnEnabled;
            set => Set(() => _showParentBtnEnabled = value, nameof(ShowParentBtnEnabled));
        }

        private HoneybeeSchema.ValidationError _currentError;

        public HoneybeeSchema.ValidationError CurrentError
        {
            get => _currentError;
            set 
            { 
                _currentError = value;
                CurrentErrorMessage = $"Error Code: {value?.Code} - {value?.ElementType}({value?.ExtensionType}){Environment.NewLine}{value?.Message}" ;
            }
        }

        private List<HoneybeeSchema.ValidationError> _errors;

        private HoneybeeSchema.ValidationReport _report;
        private Eto.Forms.Control _control;
        private Action<HoneybeeSchema.ValidationError, bool> _showAction;
        public ErrorViewModel(HoneybeeSchema.ValidationReport report,  Eto.Forms.Control control, Action<HoneybeeSchema.ValidationError, bool> showAction)
        {
            _report = report;
            _control = control;
            _showAction = showAction;

            _errors = new List<HoneybeeSchema.ValidationError>();
          
            //this.FatalError = report.FatalError;

            if (!string.IsNullOrEmpty(report.FatalError))
            {
                var fatalError = new HoneybeeSchema.ValidationError("999999", HoneybeeSchema.ExtensionTypes.Core, HoneybeeSchema.ObjectTypes.Room, "FatalElementID", report.FatalError);
                _errors.Add(fatalError);
            }

            if (report.Errors != null && report.Errors.Any())
            {
                _errors.AddRange(report.Errors);
            }

        }

        public void Refresh()
        {
            CurrentError = _errors.First();
            this.TotalErrorCount = _errors?.Count.ToString();
            this.CurrentErrorIndex = $"1 of {TotalErrorCount}";

            this.PreBtnEnabled = false;
            this.NextBtnEnabled = _errors.Count > 1;

            this.ShowBtnEnabled = !string.IsNullOrEmpty(CurrentError.ElementId);
            this.ShowParentBtnEnabled = (CurrentError?.TopParents?.FirstOrDefault() ?? CurrentError?.Parents?.FirstOrDefault()) != null;
        }

        public Eto.Forms.RelayCommand PreBtnCommand => new Eto.Forms.RelayCommand(() =>
        {
            if (_currentIndex <= 1)
                return;

            _currentIndex -= 1;
            this.PreBtnEnabled = _currentIndex > 1;
            this.NextBtnEnabled = _currentIndex < _errors.Count;
            this.CurrentErrorIndex = $"{_currentIndex} of {this.TotalErrorCount}";

            this.CurrentError = this._errors.ElementAt(_currentIndex - 1);
        });

        public Eto.Forms.RelayCommand NextBtnCommand => new Eto.Forms.RelayCommand(() =>
        {
            if (_currentIndex >= _errors.Count)
                return;

            _currentIndex += 1;
            this.PreBtnEnabled = _currentIndex > 1;
            this.NextBtnEnabled = _currentIndex < _errors.Count;
            this.CurrentErrorIndex = $"{_currentIndex} of {this.TotalErrorCount}";

            this.CurrentError = this._errors.ElementAt(_currentIndex - 1);
        });

        public Eto.Forms.RelayCommand MoreInfoCommand => new Eto.Forms.RelayCommand(() =>
        {
            if (this.CurrentError == null)
                return;

            Dialog_Message.Show(this._control, this.CurrentError.ToJson(true));
        });

        public Eto.Forms.RelayCommand ShowCommand => new Eto.Forms.RelayCommand(() =>
        {
            if (this.CurrentError == null)
                return;

            var err = this.CurrentError;
            _showAction?.Invoke(err, false);
        });

        public Eto.Forms.RelayCommand ShowParentCommand => new Eto.Forms.RelayCommand(() =>
        {
            if (this.CurrentError == null)
                return;

            var err = this.CurrentError;
            _showAction?.Invoke(err, true);
        });


    }

    
}
