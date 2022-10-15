using System;
using System.Collections.Generic;
using System.Linq;
using Eto.Drawing;
using Eto.Forms;
using HoneybeeSchema;

namespace Honeybee.UI
{
    public static class DialogHelper
    {
        public static Icon HoneybeeIcon => Config.HoneybeeIcon ?? GetHoneybeeIcon();
        public static string PluginName => Config.PluginName ?? "Honeybee";
        //public static Icon LadybugIcon => Icon.FromResource("Honeybee.UI.EmbeddedResources.ladybug.ico");

        //static System.Drawing.Icon _icon;
        static Eto.Drawing.Icon _etoicon;

        //public static System.Drawing.Icon GetHoneybeeIcon(System.Drawing.Size sizeInPixels)
        //{
        //    if (_icon == null)
        //    {
        //        using (var stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("Honeybee.UI.EmbeddedResources.honeybee.ico"))
        //        {
        //            if (stream != null) _icon = new System.Drawing.Icon(stream, sizeInPixels);
        //        }
        //    }
        //    return _icon;
        //}

        public static Eto.Drawing.Icon GetHoneybeeIcon()
        {
            if (_etoicon == null)
            {

                // var rs = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceNames();
                using (var stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("Honeybee.UI.EmbeddedResources.honeybee.ico"))
                {
                    if (stream != null) _etoicon = new Eto.Drawing.Icon(stream);
                }
            }
            
            return _etoicon;
        }

        public static DropDown MakeDropDown<T>(T currentValue, Action<T> setAction, IEnumerable<T> valueLibrary, string defaultItemName = default) where T : HoneybeeSchema.IIDdBase
        {
            return MakeDropDown(currentValue?.Identifier, setAction, valueLibrary, defaultItemName);
        }
        public static DropDown MakeDropDown<T>(string currentObjName, Action<T> setAction, IEnumerable<T> valueLibrary, string defaultItemName = default) where T : HoneybeeSchema.IIDdBase
        {
            var items = valueLibrary.ToList();
            var dropdownItems = items.Select(_ => new ListItem() { Text = _.DisplayName ?? _.Identifier, Key = _.DisplayName??_.Identifier, Tag = _ }).ToList();
            var dp = new DropDown();

            if (!string.IsNullOrEmpty(defaultItemName))
            {
                var foundIndex = dropdownItems.FindIndex(_ => _.Text == defaultItemName);

                if (foundIndex > -1)
                {
                    //Add exist item from list
                    dp.SelectedIndex = foundIndex;
                }
                else
                {
                    //Add a default None item with a name
                    dp.Items.Add(defaultItemName);
                    dp.SelectedIndex = 0;
                }

            }

            dp.Items.AddRange(dropdownItems);

            dp.SelectedIndexBinding.Bind(
                () => items.FindIndex(_ => _.Identifier == currentObjName) + 1,
                (int i) => setAction(i <= 0 ? default : items[i - 1])
                );

            return dp;

        }

        public static DropDown MakeDropDownForAnyOfType<T>(string currentValueName, Action<T> setAction, IEnumerable<T> valueLibrary) where T : HoneybeeSchema.AnyOf
        {
            var items = valueLibrary.ToList();
            var dropdownItems = items.Select(_ => new ListItem() { Text = _.Obj.GetType().Name, Tag = _ }).ToList();
            var dp = new DropDown();

            dp.Items.AddRange(dropdownItems);
            //dp.SelectedIndex = items.FindIndex(_ => _.Obj.GetType().Name == currentValueName);
            dp.SelectedIndexBinding.Bind(
                () => items.FindIndex(_ => _.Obj.GetType().Name == currentValueName),
                (int i) => setAction(items[i])
                );

            return dp;

        }
        public static DropDown MakeDropDownForAnyOfValue<T>(string currentIdentifier, Action<T> setAction, IEnumerable<T> valueLibrary) where T : HoneybeeSchema.AnyOf
        {
            var items = valueLibrary.ToList();
            var dropdownItems = items.Select(_ => {
                var hbObj = _.Obj as HoneybeeSchema.IIDdBase;
                return new ListItem() { Text = hbObj.DisplayName ?? hbObj.Identifier, Tag = _ };
            }).ToList();
            var dp = new DropDown();

            dp.Items.AddRange(dropdownItems);
            //dp.SelectedIndex = items.FindIndex(_ => _.Obj.GetType().Name == currentValueName);
            dp.SelectedIndexBinding.Bind(
                () => items.FindIndex(_ => 
                {
                    var hbObj = _.Obj as HoneybeeSchema.IIDdBase;
                    return hbObj.Identifier == currentIdentifier;
                }),
                (int i) => setAction(items[i])
                );

            return dp;

        }


        public static HoneybeeSchema.ScheduleRuleset CreateSchedule(Control parent = default)
        {
            var dialog = new Dialog_ScheduleTypeLimit();
            var typeLimit = dialog.ShowModal(parent);
            
            var dayId = Guid.NewGuid().ToString();
            var dayName = $"New Schedule Day {dayId.Substring(0, 5)}";
            var newDay = new ScheduleDay(
                dayId,
                new List<double> { 0.3 },
                dayName,
                new List<List<int>>() { new List<int> { 0, 0 } }
                );

            var id = Guid.NewGuid().ToString();
            var newSch = new ScheduleRuleset(
                id,
                new List<ScheduleDay> { newDay },
                dayId,
                $"New Schedule Ruleset {id.Substring(0, 5)}"
                );


            newSch.ScheduleTypeLimit = typeLimit;

            var dialog_sch = new Honeybee.UI.Dialog_Schedule(newSch);
            var dialog_sch_rc = dialog_sch.ShowModal(parent);

            return dialog_sch_rc;
        }

        public static HoneybeeSchema.Radiance.IModifier EditModifier(HoneybeeSchema.Radiance.IModifier modifier, bool locked = false, Control parent = default)
        {
            var dup = modifier.Duplicate();
            HoneybeeSchema.Radiance.IModifier dialog_rc = null;
            switch (dup)
            {
                case Plastic obj:
                    dialog_rc = new Dialog_Modifier<Plastic>(obj, locked).ShowModal(parent);
                    break;
                case Glass obj:
                    dialog_rc = new Dialog_Modifier<Glass>(obj, locked).ShowModal(parent);
                    break;
                case Trans obj:
                    dialog_rc = new Dialog_Modifier<Trans>(obj, locked).ShowModal(parent);
                    break;
                case Metal obj:
                    dialog_rc = new Dialog_Modifier<Metal>(obj, locked).ShowModal(parent);
                    break;
                case Mirror obj:
                    dialog_rc = new Dialog_Modifier<Mirror>(obj, locked).ShowModal(parent);
                    break;
                case Glow obj:
                    dialog_rc = new Dialog_Modifier<Glow>(obj, locked).ShowModal(parent);
                    break;
                case Light obj:
                    dialog_rc = new Dialog_Modifier<Light>(obj, locked).ShowModal(parent);
                    break;
                case BSDF obj:
                    dialog_rc = new Dialog_Modifier<BSDF>(obj, locked).ShowModal(parent);
                    break;
                default:
                    break;
            }

            return dialog_rc;
        }
    }
}
