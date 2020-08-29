using Eto.Drawing;
using Eto.Forms;
using System.Collections.Generic;
using System.Linq;
using HB = HoneybeeSchema;
using System;
using EnergyLibrary = HoneybeeSchema.Helper.EnergyLibrary;
using HoneybeeSchema;

namespace Honeybee.UI
{
    public class Dialog_RoomRadianceProperty: Dialog<HB.RoomRadiancePropertiesAbridged>
    {
     
        public Dialog_RoomRadianceProperty(HB.RoomRadiancePropertiesAbridged roomRadianceProperties, bool updateChangesOnly = false)
        {
            try
            {
                var prop = roomRadianceProperties ?? new HB.RoomRadiancePropertiesAbridged();

                if (updateChangesOnly)
                    prop = new HB.RoomRadiancePropertiesAbridged("No Changes");


                Padding = new Padding(15);
                Title = "Room Radiance Properties - Honeybee";
                WindowStyle = WindowStyle.Default;
                Width = 450;
                this.Icon = DialogHelper.HoneybeeIcon;

                //Get constructions
                //var cSets = EnergyLibrary.DefaultConstructionSets.ToList();
                var mSets = EnergyLibrary.InModelRadianceProperties.ModifierSets
                    .OfType<ModifierSetAbridged>()
                    .ToList();

                if (updateChangesOnly)
                    mSets.Insert(0, new ModifierSetAbridged("No Changes"));


                var constructionSetDP = DialogHelper.MakeDropDown(prop.ModifierSet, (v) => prop.ModifierSet = v?.Identifier,
                    mSets, "Default Modifier Set");


                DefaultButton = new Button { Text = "OK" };
                DefaultButton.Click += (sender, e) => 
                {
                    Close(prop); 
                };

                AbortButton = new Button { Text = "Cancel" };
                AbortButton.Click += (sender, e) => Close();

                var buttons = new TableLayout
                {
                    Padding = new Padding(5, 10, 5, 5),
                    Spacing = new Size(10, 10),
                    Rows = { new TableRow(null, this.DefaultButton, this.AbortButton, null) }
                };


                var layout = new DynamicLayout();
                //layout.DefaultPadding = new Padding(10);
                layout.DefaultSpacing = new Size(5, 5);

                layout.AddSeparateRow("Room ModifierSet:");
                layout.AddSeparateRow(constructionSetDP);
                layout.AddSeparateRow(buttons);
                layout.AddSeparateRow(null);
                //Create layout
                Content = layout;
            }
            catch (Exception e)
            {
                throw new ArgumentException($"Failed to open RoomRadianceProperty dialog:\n{e.Message}");
            }
            
            
        }
    
    
    }
}
