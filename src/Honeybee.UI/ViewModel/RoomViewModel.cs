using Eto.Forms;
using HoneybeeSchema;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Honeybee.UI.ViewModel
{
    public class RoomViewModel : ViewModelBase
    {
        private Room _hbObj = new Room("InvalidRoom", new List<Face>(), new RoomPropertiesAbridged());
        public Room HoneybeeObject
        {
            get { return _hbObj; }
            private set { this.Set(() => _hbObj = value, nameof(HoneybeeObject)); }
        }

        private string _faceCount = "0";
        public string FaceCount
        {
            get { return _faceCount; }
            private set { this.Set(() => _faceCount = value, nameof(FaceCount)); }
        }

        private string _displayName = "";

        public string DisplayName
        {
            get { return _displayName; }
            private set { this.Set(() => _displayName = value, nameof(DisplayName));}
        }




        /// <summary>
        /// Action after the property is changed.
        /// </summary>
        public Action<string> ActionWhenChanged { get; private set; }
        /// <summary>
        /// Action to execute when a subsurface is selected from face list.
        /// </summary>
        public Action<string> Redraw { get; private set; }

        private View.Room Control;
        public ModelProperties ModelProperties { get; set; }
        internal RoomViewModel(View.Room roomPanel)
        {
            this.Control = roomPanel;
        }

        public void Update(ModelProperties libSource, Room honeybeeRoom, Action<string> actionWhenChanged, Action<string> redrawDisplay)
        {
            this.ModelProperties = libSource;

            ActionWhenChanged = actionWhenChanged;
            Redraw = redrawDisplay;

            HoneybeeObject = honeybeeRoom;
            //HoneybeeObject.DisplayName = honeybeeRoom.DisplayName ?? string.Empty;
            HoneybeeObject.Faces = honeybeeRoom.Faces?.Where(_ => _ != null).ToList();
            FaceCount = honeybeeRoom.Faces?.Count().ToString();
            
        }


        public void OnRoomFaceSelected(object s, EventArgs e)
        {
            var sel = this.Control.FacesGridView.SelectedItem as Face;
            if (sel != null)
                this.Redraw(sel.Identifier);
            else
                this.Redraw(null);
        }

        public void OnRoomFaceMouseDoubleClick(object s, EventArgs e)
        {
            var sel = this.Control.FacesGridView.SelectedItem as Face;
            if (sel == null)
                return;

            var dialog = new Dialog_Face(this.ModelProperties, sel);
            var dialog_rc = dialog.ShowModal(Config.Owner);
            if (dialog_rc != null)
            {
                //MessageBox.Show(dialog_rc.ToJson());
                var faces = this.HoneybeeObject.Faces;
                var index = faces.FindIndex(_ => _.Identifier == dialog_rc.Identifier);
                this.HoneybeeObject.Faces[index] = dialog_rc;

                this.ActionWhenChanged($"Set {dialog_rc.Identifier} Properties");
            }
            //MessageBox.Show(sel.ToJson());
        }

        public ICommand RoomEnergyPropertyBtnClick => new RelayCommand(() => {
            var energyProp = this.HoneybeeObject.Properties.Energy ?? new RoomEnergyPropertiesAbridged();
            energyProp = energyProp.DuplicateRoomEnergyPropertiesAbridged();
            var dialog = new Dialog_RoomEnergyProperty(this.ModelProperties.Energy, energyProp);
            var dialog_rc = dialog.ShowModal(Config.Owner);
            if (dialog_rc != null)
            {
                this.HoneybeeObject.Properties.Energy = dialog_rc;
                this.ActionWhenChanged($"Set {this.HoneybeeObject.Identifier} Energy Properties ");
            }
        });

        public ICommand RoomRadiancePropertyBtnClick => new RelayCommand(() => {
            var prop = this.HoneybeeObject.Properties.Radiance ?? new RoomRadiancePropertiesAbridged();
            prop = prop.DuplicateRoomRadiancePropertiesAbridged();
            var dialog = new Dialog_RoomRadianceProperty(this.ModelProperties.Radiance, prop);
            var dialog_rc = dialog.ShowModal(Config.Owner);
            if (dialog_rc != null)
            {
                this.HoneybeeObject.Properties.Radiance = dialog_rc;
                this.ActionWhenChanged($"Set {this.HoneybeeObject.Identifier} Radiance Properties ");
            }
        });
        public ICommand HBDataBtnClick => new RelayCommand(() => {
            Dialog_Message.Show(Config.Owner, this.HoneybeeObject.ToJson(), "Schema Data");
        });

    }



}
