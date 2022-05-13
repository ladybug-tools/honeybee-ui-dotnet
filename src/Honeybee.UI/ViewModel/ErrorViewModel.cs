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
        public ErrorViewModel(HoneybeeSchema.ValidationReport report, Eto.Forms.Control control)
        {
            _report = report;
            _control = control;

            _errors = new List<HoneybeeSchema.ValidationError>();
            var errorCount = (report.Errors?.Count).GetValueOrDefault();
          
            //this.FatalError = report.FatalError;

            if (!string.IsNullOrEmpty(report.FatalError))
            {
                var fatalError = new HoneybeeSchema.ValidationError("999999", HoneybeeSchema.ExtensionTypes.Core, HoneybeeSchema.ObjectTypes.Room, "FatalElementID", report.FatalError);
                _errors.Add(fatalError);

                errorCount += 1;

            }

            if (report.Errors != null && report.Errors.Any())
            {
                _errors.AddRange(report.Errors);
            }

            CurrentError = _errors.First();

            this.TotalErrorCount = errorCount.ToString();
            this.CurrentErrorIndex = $"1 of {TotalErrorCount}";

            this.PreBtnEnabled = false;
            this.NextBtnEnabled = _errors.Count > 1;

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

        public Eto.Forms.RelayCommand<Action<HoneybeeSchema.ValidationError>> ShowCommand => new Eto.Forms.RelayCommand<Action<HoneybeeSchema.ValidationError>>((Action<HoneybeeSchema.ValidationError> action) =>
        {
            if (this.CurrentError == null)
                return;

            var err = this.CurrentError;
            if (IsGeometryType(err.ElementType))
            {
                action?.Invoke(err);
            }
            else
            {
                Eto.Forms.MessageBox.Show($"{err.ElementType} is not a geometry element type, which cannot be shown!");
            }

        });


        private static bool IsGeometryType(HoneybeeSchema.ObjectTypes type)
        {
            switch (type)
            {
                case HoneybeeSchema.ObjectTypes.Shade:
                case HoneybeeSchema.ObjectTypes.Aperture:
                case HoneybeeSchema.ObjectTypes.Door:
                case HoneybeeSchema.ObjectTypes.Face:
                case HoneybeeSchema.ObjectTypes.Room:
                case HoneybeeSchema.ObjectTypes.SensorGrid:
                case HoneybeeSchema.ObjectTypes.View:
                    return true;
                case HoneybeeSchema.ObjectTypes.Modifier:
                case HoneybeeSchema.ObjectTypes.ModifierSet:
                case HoneybeeSchema.ObjectTypes.Material:
                case HoneybeeSchema.ObjectTypes.Construction:
                case HoneybeeSchema.ObjectTypes.ConstructionSet:
                case HoneybeeSchema.ObjectTypes.ScheduleTypeLimit:
                case HoneybeeSchema.ObjectTypes.Schedule:
                case HoneybeeSchema.ObjectTypes.ProgramType:
                case HoneybeeSchema.ObjectTypes.HVAC:
                case HoneybeeSchema.ObjectTypes.SHW:
                    return false;
                default:
                    throw new System.ArgumentException($"{type} is not supported, please contact developers!");
            }
        }
    }

    
}
