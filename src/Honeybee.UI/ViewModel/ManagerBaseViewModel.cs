using Eto.Forms;
using System;
using HB = HoneybeeSchema;
using System.Collections.Generic;
using System.Linq;
using HoneybeeSchema;
using System.Text.RegularExpressions;

namespace Honeybee.UI
{
    internal class ManagerBaseViewModel<T> : ViewModelBase where T: ManagerViewDataBase
    {
        private string _filterKey;
        public string FilterKey
        {
            get => _filterKey;
            set
            {
                this.Set(() => _filterKey = value, nameof(FilterKey));
                ApplyFilter();
                this.Counts = this.GridViewDataCollection.Count.ToString();
            }
        }

        private string _counts;
        public string Counts
        {
            get => $"Count: {_counts}";
            set => this.Set(() => _counts = value, nameof(Counts));
        }

        private DataStoreCollection<T> _gridViewDataCollection = new DataStoreCollection<T>();
        public DataStoreCollection<T> GridViewDataCollection
        {
            get => _gridViewDataCollection;
            set => this.Set(() => _gridViewDataCollection = value, nameof(_gridViewDataCollection));
        }

        protected List<T> _userData { get; set; }
        protected List<T> _systemData { get; set; }
        protected List<T> _allData { get; set; }
        public T SelectedData { get; set; }
        protected Control _control;
        internal Control GridControl { get; set; }

        protected void ApplyFilter(bool forceRefresh = true)
        {
            var filter = this.FilterKey;
            var allData = this._allData;

            // list is not filtered
            if (string.IsNullOrWhiteSpace(filter))
            {
                if (!forceRefresh) return;
                // reset
                ResetDataCollection();
                return;
            }

            // do nothing if user only type in one key
            if (filter.Length <= 1) return;

            // filter
            var regexPatten = ".*" + filter.Replace(" ", "(.*)") + ".*";
            var filtered = allData.Where(_ => Regex.IsMatch(_.SearchableText, regexPatten, RegexOptions.IgnoreCase));
            GridViewDataCollection.Clear();
            GridViewDataCollection.AddRange(filtered);
        }

        protected void ResetDataCollection()
        {
            if (!string.IsNullOrEmpty(this.FilterKey))
                this.FilterKey = string.Empty;

            GridViewDataCollection.Clear();
            GridViewDataCollection.AddRange(_allData);
            this.Counts = this.GridViewDataCollection.Count.ToString();
        }

        public void SortList(Func<T, string> sortFunc, bool isNumber, bool descend = false)
        {
            var c = new StringComparer(isNumber);
            var newOrder = descend ? _allData.OrderByDescending(sortFunc, c) : _allData.OrderBy(sortFunc, c);
            _allData = newOrder.ToList();
            ResetDataCollection();
        }

        protected K CheckObjName<K>(K obj, string exceptionName = "") where K: HB.IIDdBase
        {
            var name = obj.DisplayName ?? obj.Identifier;

            IEnumerable<T> alldata = _allData;
            if (!string.IsNullOrEmpty(exceptionName))
                alldata = alldata.Where(_ => _.Name != exceptionName);
            if (alldata.Any(_ => _.Name == name))
            {
                name = $"{name} {Guid.NewGuid().ToString().Substring(0, 5)}";
                MessageBox.Show(_control, $"Name [{obj.DisplayName}] is conflicting with an existing item, and now it is changed to [{name}].");
            }
            obj.Identifier = name;
            obj.DisplayName = name;
            return obj;
        }

        protected ManagerBaseViewModel(Control control = default, Control gridControl = default)
        {
            _control = control;
            GridControl = gridControl;
        }

     
        public void DialogSizeChanged()
        {
            if (GridControl == null || GridControl.Height <= 0)
                return;

            var h = this._control.Height - 200;
            h = System.Math.Max(h, 250);
            GridControl.Height = h;

        }

    }

}
