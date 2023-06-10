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

        protected K CheckObjID<K>(K obj, string exceptionID = "") where K: HB.IIDdBase
        {
            var id = obj.Identifier;
            if (string.IsNullOrEmpty(id))
            {
                id = Guid.NewGuid().ToString().Substring(0, 5); 
                obj.Identifier = id;
                return obj;
            }

            IEnumerable<T> alldata = _allData;
            if (!string.IsNullOrEmpty(exceptionID))
                alldata = alldata.Where(_ => _.Identifier != exceptionID);
            if (alldata.Any(_ => _.Identifier == id))
            {
                id = $"{id}_{Guid.NewGuid().ToString().Substring(0, 5)}";
                MessageBox.Show(_control, $"ID [{obj.Identifier}] is conflicting with an existing item, and now it is changed to [{id}].");
            }
            obj.Identifier = id;
            return obj;
        }

        protected ManagerBaseViewModel(Control control = default, Control gridControl = default)
        {
            _control = control;
            GridControl = gridControl;
        }

     

    }

}
