using Eto.Forms;
using LadybugDisplaySchema;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Honeybee.UI
{
    public class ErrorData: TreeGridItem
    {
        public string ErrorCode { get; set; }
        public string ErrorType => Error?.ErrorType;
        public string DisplayMessage { get; set; }
        public string DisplayFullMessage { get; set; }
        public bool IsParent { get; set; }
        public bool HasGeometry { get; set; }
        public bool HasParentGeometry { get; set; }

        public HoneybeeSchema.ValidationError Error { get; }

        public ErrorData(HoneybeeSchema.ValidationError error)
        {
            this.Error = error;
            this.ErrorCode = error.Code;


            var parentChildMatch = error?.ElementId?.Select(
                (_, i)
                =>
                {
                    if (string.IsNullOrEmpty(_))
                        return null;

                    var m = new
                    {
                        eID = _,
                        eName = error?.ElementName?.ElementAtOrDefault(i),
                        topParent = error?.TopParents?.ElementAtOrDefault(i),
                        parents = error?.Parents?.ElementAtOrDefault(i)
                    };
                    return m;
                })
                ?.Where(_ => _ != null)
                ?.ToList();

      
            if (parentChildMatch!= null && parentChildMatch.Any())
            {
                this.HasGeometry = true;

                //create display message
                var msgs = parentChildMatch.Select(_ =>
                {
                    var parent = _?.topParent ?? _?.parents?.FirstOrDefault();
                    var hasParent = parent != null;
                    var elemName = _.eName ?? _.eID;
                    var msg = $"{error.ElementType} ({elemName})";
                    if (hasParent) 
                    {
                    
                        var pName = parent?.Name ?? parent.Id;
                        msg = $"{parent.ParentType} ({pName}) --> {msg}";
                    }
                    return msg;
                });
                this.DisplayMessage = string.Join(System.Environment.NewLine, msgs);

                this.HasParentGeometry = parentChildMatch.Any(_=>_.topParent != null || (_.parents?.Any()).GetValueOrDefault());
            }

           
            this.DisplayFullMessage = $"Error Code: {error?.Code} - {error?.ElementType}({error?.ExtensionType}){Environment.NewLine}{error?.Message}";

        }

        public ErrorData(string fatalErrorMessage)
        {
            var fatalError = new HoneybeeSchema.ValidationError("000000", "Fatal Error", HoneybeeSchema.ExtensionTypes.Core, HoneybeeSchema.ObjectTypes.Room, new List<string> { "FatalElementID" }, fatalErrorMessage);
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

            var errorCountMsg = errorCount == 1 ? $"1 Error" : $"{errorCount} Errors";
            this.DisplayMessage = $"Found {children.FirstOrDefault()?.Error?.ErrorType} ({errorCountMsg})";
            this.IsParent = true;
        }
    }
    public class ErrorViewModel : ViewModelBase
    {
        private string totalErrorCount;
        public string TotalErrorCount
        {
            get => totalErrorCount;
            set
            {
                Set(() => totalErrorCount = value, nameof(TotalErrorCount));
                TotalErrorMessage = $"Found {value} Error(s)";
            }
        }

        private string totalErrorMessage;
        public string TotalErrorMessage
        {
            get => totalErrorMessage;
            set => Set(() => totalErrorMessage = value, nameof(TotalErrorMessage));
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

        private bool _validateBtnEnabled;
        public bool ValidateBtnEnabled
        {
            get => _validateBtnEnabled;
            set => Set(() => _validateBtnEnabled = value, nameof(ValidateBtnEnabled));
        }


        //private DataStoreCollection<ErrorData> _errorColletion;

        private TreeGridItemCollection _gridViewDataCollection;
        public TreeGridItemCollection GridViewDataCollection
        {
            get => _gridViewDataCollection;
            set => this.Set(() => _gridViewDataCollection = value, nameof(_gridViewDataCollection));
        }

        private List<ErrorData> _selectedErrors;
        public List<ErrorData> SelectedErrors
        {
            get => _selectedErrors;
            set 
            {
                this.Set(() => _selectedErrors = value, nameof(SelectedErrors));

                if (_selectedErrors == null)
                {
                    SelectedError = null;
                }
                else
                {
                    if (_selectedErrors.Count == 1)
                        SelectedError = _selectedErrors.FirstOrDefault();
                    else
                    {
                        CurrentErrorMessage = String.Join(Environment.NewLine, _selectedErrors.Select(_ => _.DisplayFullMessage));
                    }

                   
                }
          
            }
        }


        private ErrorData _selectedError;
        public ErrorData SelectedError
        {
            get => _selectedError;
            set
            {
                this.Set(() => _selectedError = value, nameof(SelectedError));
                CurrentErrorMessage = value?.DisplayFullMessage ?? "Select an error in the tree list";

                if (value != null)
                {
                    if (value.Error != null)
                    {
                        var i = _errors.IndexOf(value);
                        _currentIndex = i + 1;
                    }
                    else if (value.IsParent) //parent
                    {
                        if (value.Children.FirstOrDefault() is ErrorData d)
                        {
                            var i = _errors.IndexOf(d);
                            _currentIndex = i;
                        }
                    }
                }
              
                Refresh();
            }
        }


        private List<ErrorData> _errors = new List<ErrorData>();
        private HoneybeeSchema.ValidationReport _report;
        private Dialog_Error _control;
        private Action<IEnumerable<HoneybeeSchema.ValidationError>, bool> _showAction;
        private Action _rerunValidationAction;
        public ErrorViewModel(
            HoneybeeSchema.ValidationReport report,
            Dialog_Error control)
        {
            _control = control;
            var rpt = report ?? new HoneybeeSchema.ValidationReport("0.0.0", "0.0.0", false);
            Update(rpt, null, null);
        }

        public void Update(
            HoneybeeSchema.ValidationReport report, 
            Action<IEnumerable<HoneybeeSchema.ValidationError>, bool> showAction,
            Action reRunValidation = default)
        {
            _report = report;
            _showAction = showAction;
            _rerunValidationAction = reRunValidation;

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
                .Select(_ => new ErrorData(_.Key, _));

            if (GridViewDataCollection == null)
                GridViewDataCollection = new TreeGridItemCollection(errorGroups);
            else
            {
                GridViewDataCollection.Clear();
                GridViewDataCollection.AddRange(errorGroups);
            }

            this.ValidateBtnEnabled = reRunValidation != null;
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
            this.CurrentErrorIndex = $"{_currentIndex} of {TotalErrorCount}";
            this.PreBtnEnabled = _currentIndex > 1;
            this.NextBtnEnabled = _currentIndex < _errors.Count;

            if (this.SelectedError != null)
            {
                this.ShowBtnEnabled = SelectedError.HasGeometry && _showAction != null;
                this.ShowParentBtnEnabled = SelectedError.HasParentGeometry && _showAction != null;
                this.MoreBtnEnabled = true;
            }
            else
            {
                this.ShowBtnEnabled = false;
                this.ShowParentBtnEnabled = false;
                this.MoreBtnEnabled = false;
            }

        }

       

        public Eto.Forms.RelayCommand PreBtnCommand => new Eto.Forms.RelayCommand(() =>
        {
            if (_currentIndex <= 1)
                return;

            _control.MoveToPrevious();
        });

        public Eto.Forms.RelayCommand NextBtnCommand => new Eto.Forms.RelayCommand(() =>
        {
            if (_currentIndex >= _errors.Count)
                return;

            _control.MoveToNext();

        });

      

        public Eto.Forms.RelayCommand ShowCommand => new Eto.Forms.RelayCommand(() =>
        {
            if (this.SelectedErrors == null)
                return;

            var errs = this.SelectedErrors.Select(_ => _.Error).Where(_ => _ != null);
            _showAction?.Invoke(errs, false);
        });

        public Eto.Forms.RelayCommand ShowParentCommand => new Eto.Forms.RelayCommand(() =>
        {
            if (this.SelectedErrors == null)
                return;

            var errs = this.SelectedErrors.Select(_ => _.Error).Where(_ => _ != null);
            _showAction?.Invoke(errs, true);
        });

        public Eto.Forms.RelayCommand ValidateCommand => new Eto.Forms.RelayCommand(() =>
        {
            _rerunValidationAction?.Invoke();
        });

        public Eto.Forms.RelayCommand ErrorLinkCommand => new Eto.Forms.RelayCommand(() =>
        {
            //https://docs.pollination.cloud/user-manual/get-started/troubleshooting/help-with-modeling-error-codes#000204
            var url = @"https://docs.pollination.cloud/user-manual/get-started/troubleshooting/help-with-modeling-error-codes";
            if (SelectedError != null)
                url = $"{url}#{SelectedError.ErrorCode}";
            System.Diagnostics.Process.Start(url);
        });

    }

    
}
