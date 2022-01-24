using Eto.Forms;
using System;
using HoneybeeSchema;
using System.Collections.Generic;
using System.Linq;

namespace Honeybee.UI
{
    public class UserDataItem
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public UserDataItem(string key, string value)
        {
            this.Key = key;
            this.Value = value;
        }

        public static List<UserDataItem> FromJObject(object obj)
        {
            var uds = new List<UserDataItem>();
            Newtonsoft.Json.Linq.JObject jObj = null;
            if (obj is string) 
                jObj = Newtonsoft.Json.Linq.JObject.Parse(obj?.ToString());
            else if (obj is Newtonsoft.Json.Linq.JObject j)
                jObj = j;

            if (jObj != null)
            {
                uds = jObj.Children()
                   .OfType<Newtonsoft.Json.Linq.JProperty>()
                   .Where(_=> !_.Name.StartsWith("__")) // hide reserved items, such as "__layer__"
                   .Select(_ => new UserDataItem(_.Name, _.Value.ToString()))
                   .ToList();
            }
            return uds;
            
        }
    }


    public class UserDataViewModel : ViewModelBase
    {
        private List<UserDataItem> refObjProperty = new List<UserDataItem>();

        private bool _isPanelEnabled = true;

        public bool IsPanelEnabled
        {
            get => _isPanelEnabled;
            private set { this.Set(() => _isPanelEnabled = value, nameof(IsPanelEnabled)); }
        }

        private bool _isCheckboxChecked;

        public bool IsCheckboxChecked
        {
            get => _isCheckboxChecked;

            protected set
            {
                this.Set(() => _isCheckboxChecked = value, nameof(IsCheckboxChecked));
                IsPanelEnabled = !value;
                if (_isCheckboxChecked)
                    SetHBProperty(null);
                else
                {
                    SetHBProperty(UserItemsToDic(refObjProperty));
                }
            }
        }

        private DataStoreCollection<UserDataItem> _gridViewDataCollection = new DataStoreCollection<UserDataItem>();
        public DataStoreCollection<UserDataItem> GridViewDataCollection
        {
            get => _gridViewDataCollection;
            set => this.Set(() => _gridViewDataCollection = value, nameof(_gridViewDataCollection));
        }

        public UserDataItem _selectedItem;
        public UserDataItem SelectedItem
        {
            get => _selectedItem;
            set => this.Set(() => _selectedItem = value, nameof(SelectedItem));
        }
        public Action<Dictionary<string, string>> SetHBProperty { get; private set; }

        private Control control;

        public UserDataViewModel( List<object> allUserData, Action<Dictionary<string, string>> setAction, Control control)
        {
            this.control = control;
            this.SetHBProperty = setAction;

            this.refObjProperty = UserDataItem.FromJObject(allUserData.FirstOrDefault()) ?? new List<UserDataItem>();
            

            if (allUserData.Distinct().Count() == 1 && allUserData.FirstOrDefault() == null)
            {
                this.IsCheckboxChecked = true;
            }

            if (allUserData.Distinct().Count() > 1)
            {
                //multiple objects
                // set refObj to varies
                this.refObjProperty = new List<UserDataItem>() { new UserDataItem(this.Varies, this.Varies) };
            }

            this.GridViewDataCollection = new DataStoreCollection<UserDataItem>(this.refObjProperty);
        }

        public object MatchObj(object obj)
        {
            // by room program type
            if (this.IsCheckboxChecked)
                return null;

            if (this.refObjProperty.Any(_=> _?.Key == this.Varies))
                return obj;

            var dic = UserItemsToDic(this.refObjProperty);
            var s = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
            return s;
        }

        private void UpdateRefObj()
        {
            this.refObjProperty = this.GridViewDataCollection.Select(_ => _).ToList();
            var dic = UserItemsToDic(this.refObjProperty);
            SetHBProperty(dic);
        }

        private Dictionary<string,string> UserItemsToDic(List<UserDataItem> userItems)
        {
            var dic = userItems.ToDictionary(_ => _.Key, _ => _.Value);
            return dic;
        }

        public System.Windows.Input.ICommand AddDataCommand => new RelayCommand(() =>
        {
            var dialog = new Dialog_AddUserData(this.GridViewDataCollection, null);
            var rc = dialog.ShowModal(this.control);
            if (rc != null)
            {
                this.GridViewDataCollection.Add(rc);
                UpdateRefObj();
                //gd.DataStore = this.GridViewDataCollection;
            }

        });
        public System.Windows.Input.ICommand EditDataCommand => new RelayCommand(() =>
        {
            var sel = this.SelectedItem;
            if (sel == null)
            {
                MessageBox.Show($"Nothing is selected!");
                return;
            }

            var dialog = new Dialog_AddUserData(this.GridViewDataCollection, sel);
            var rc = dialog.ShowModal(this.control);
            if (rc != null)
            {
                var i = this.refObjProperty.FindIndex(_=>_.Key == sel.Key);
                this.GridViewDataCollection.RemoveAt(i);
                this.GridViewDataCollection.Insert(i, rc);
                UpdateRefObj();
            }

        });

        public System.Windows.Input.ICommand RemoveDataCommand => new RelayCommand(() =>
        {
            var sel = this.SelectedItem;
            if (sel == null)
            {
                MessageBox.Show($"Nothing is selected!");
                return;
            }

            this.GridViewDataCollection.Remove(sel);
            UpdateRefObj();
        });
    }


}
