using Eto.Forms;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Honeybee.UI
{
    public class ErrorData: TreeGridItem
    {
        public string ErrorCode { get; set; }
        public string DisplayMessage { get; set; }
        public string DisplayFullMessage { get; set; }
        public bool IsParent { get; set; }
        public bool IsGeometry { get; set; }
        public bool HasParentGeometry { get; set; }

        public HoneybeeSchema.ValidationError Error { get; }

        public ErrorData(HoneybeeSchema.ValidationError error)
        {
            this.Error = error;
            this.ErrorCode = error.Code;

            this.DisplayMessage = error.Message;
            this.DisplayFullMessage = $"Error Code: {error?.Code} - {error?.ElementType}({error?.ExtensionType}){Environment.NewLine}{error?.Message}";

            this.IsGeometry = !string.IsNullOrEmpty(error?.ElementId);
            this.HasParentGeometry = (error?.TopParents?.FirstOrDefault() ?? error?.Parents?.FirstOrDefault()) != null;

        }

        public ErrorData(string fatalErrorMessage)
        {
            var fatalError = new HoneybeeSchema.ValidationError("999999", HoneybeeSchema.ExtensionTypes.Core, HoneybeeSchema.ObjectTypes.Room, "FatalElementID", fatalErrorMessage);
            this.Error = fatalError;
            this.ErrorCode = fatalError.Code;
            this.DisplayMessage = fatalErrorMessage;
            this.DisplayFullMessage = $"Fatal Error{Environment.NewLine}{fatalErrorMessage}";
        }

        public ErrorData(string errorCode, IEnumerable<ErrorData> children)
        {
            this.ErrorCode = errorCode;
            this.Children.AddRange(children);
            var errorCount = children.Count();

            this.DisplayMessage = errorCount == 1 ? $"Error Code {errorCode} (1 Error)" : $"Error Code {errorCode} ({errorCount} Errors)";
            this.IsParent = true;
        }
    }
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

        private bool _moreBtnEnabled;
        public bool MoreBtnEnabled
        {
            get => _moreBtnEnabled;
            set => Set(() => _moreBtnEnabled = value, nameof(MoreBtnEnabled));
        }

        //private DataStoreCollection<ErrorData> _errorColletion;

        private TreeGridItemCollection _gridViewDataCollection;
        public TreeGridItemCollection GridViewDataCollection
        {
            get => _gridViewDataCollection;
            set => this.Set(() => _gridViewDataCollection = value, nameof(_gridViewDataCollection));
        }

        private ErrorData _selectedError;
        public ErrorData SelectedError
        {
            get => _selectedError;
            set
            {
                this.Set(() => _selectedError = value, nameof(SelectedError));
                CurrentErrorMessage = value?.DisplayFullMessage ?? "Select an error in the tree list";

                if (value?.Error != null)
                {
                    var i = _errors.IndexOf(value);
                    _currentIndex = i + 1;
                }
                Refresh();
            }
        }


        private List<ErrorData> _errors = new List<ErrorData>();
        private HoneybeeSchema.ValidationReport _report;
        private Eto.Forms.Control _control;
        private Action<HoneybeeSchema.ValidationError, bool> _showAction;
        public ErrorViewModel(HoneybeeSchema.ValidationReport report,  Eto.Forms.Control control, Action<HoneybeeSchema.ValidationError, bool> showAction)
        {
            _report = report;
            _control = control;
            _showAction = showAction;

            _errors = new List<ErrorData>();

            if (!string.IsNullOrEmpty(report.FatalError))
            {
                _errors.Add(new ErrorData(report.FatalError));
            }

            if (report.Errors != null && report.Errors.Any())
            {
                _errors.AddRange(report.Errors.Select(_ => new ErrorData(_)));
            }

            var errorGroups = _errors
                .GroupBy(_ => _.ErrorCode)
                .Select(_=> new ErrorData(_.Key, _));

            GridViewDataCollection = new TreeGridItemCollection(errorGroups);

            this.TotalErrorCount = _errors?.Count.ToString();
        }

        public void UILoaded()
        {
            this.SelectedError = _errors.FirstOrDefault();
            this.CurrentErrorIndex = $"{_currentIndex} of {TotalErrorCount}";

            Refresh();
        }

        public void Refresh()
        {
            if (this.SelectedError == null)
                return;

            this.CurrentErrorIndex = $"{_currentIndex} of {TotalErrorCount}";

            this.PreBtnEnabled = _currentIndex > 1;
            this.NextBtnEnabled = _currentIndex < _errors.Count;

            this.ShowBtnEnabled = SelectedError.IsGeometry && _showAction != null;
            this.ShowParentBtnEnabled = SelectedError.HasParentGeometry && _showAction != null;
            this.MoreBtnEnabled = SelectedError != null && SelectedError.Error != null;
        }

        public Eto.Forms.RelayCommand PreBtnCommand => new Eto.Forms.RelayCommand(() =>
        {
            if (_currentIndex <= 1)
                return;

            _currentIndex -= 1;
            this.PreBtnEnabled = _currentIndex > 1;
            this.NextBtnEnabled = _currentIndex < _errors.Count;
            this.CurrentErrorIndex = $"{_currentIndex} of {this.TotalErrorCount}";

            this.SelectedError = this._errors.ElementAt(_currentIndex - 1);
        });

        public Eto.Forms.RelayCommand NextBtnCommand => new Eto.Forms.RelayCommand(() =>
        {
            if (_currentIndex >= _errors.Count)
                return;

            _currentIndex += 1;
            this.PreBtnEnabled = _currentIndex > 1;
            this.NextBtnEnabled = _currentIndex < _errors.Count;
            this.CurrentErrorIndex = $"{_currentIndex} of {this.TotalErrorCount}";

            this.SelectedError = this._errors.ElementAt(_currentIndex - 1);
        });

        public Eto.Forms.RelayCommand MoreInfoCommand => new Eto.Forms.RelayCommand(() =>
        {
            if (this.SelectedError == null)
                return;

            Dialog_Message.Show(this._control, this.SelectedError.Error.ToJson(true));
        });

        public Eto.Forms.RelayCommand ShowCommand => new Eto.Forms.RelayCommand(() =>
        {
            if (this.SelectedError == null)
                return;

            var err = this.SelectedError.Error;
            _showAction?.Invoke(err, false);
        });

        public Eto.Forms.RelayCommand ShowParentCommand => new Eto.Forms.RelayCommand(() =>
        {
            if (this.SelectedError == null)
                return;

            var err = this.SelectedError.Error;
            _showAction?.Invoke(err, true);
        });


    }

    
}
