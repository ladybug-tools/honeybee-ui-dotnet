using System;
using System.Collections.Generic;
using System.Linq;
using Eto.Drawing;
using Eto.Forms;

namespace Honeybee.UI
{
    public static class DialogHelper
    {
        public static Icon HoneybeeIcon => GetHoneybeeIcon();
        //public static Icon LadybugIcon => Icon.FromResource("Honeybee.UI.EmbeddedResources.ladybug.ico");

        static System.Drawing.Icon _icon;
        static Eto.Drawing.Icon _etoicon;

        public static System.Drawing.Icon GetHoneybeeIcon(System.Drawing.Size sizeInPixels)
        {
            if (_icon == null)
            {
                using (var stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("Honeybee.UI.EmbeddedResources.honeybee.ico"))
                {
                    if (stream != null) _icon = new System.Drawing.Icon(stream, sizeInPixels);
                }
            }
            return _icon;
        }

        public static Eto.Drawing.Icon GetHoneybeeIcon()
        {
            if (_etoicon == null)
            {
                using (var stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("Honeybee.UI.EmbeddedResources.honeybee.ico"))
                {
                    if (stream != null) _etoicon = new Eto.Drawing.Icon(stream);
                }
            }
            return _etoicon;
        }

        public static DropDown MakeDropDown<T>(T currentValue, Action<T> setAction, IEnumerable<T> valueLibrary, string defaultItemName = default) where T : HoneybeeSchema.IIdentified
        {
            return MakeDropDown(currentValue?.Identifier, setAction, valueLibrary, defaultItemName);
        }
        public static DropDown MakeDropDown<T>(string currentObjName, Action<T> setAction, IEnumerable<T> valueLibrary, string defaultItemName = default) where T : HoneybeeSchema.IIdentified
        {
            var items = valueLibrary.ToList();
            var dropdownItems = items.Select(_ => new ListItem() { Text = _.Identifier, Tag = _ }).ToList();
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
                (int i) => setAction(i == 0 ? default : items[i - 1])
                );

            return dp;

        }

        public static DropDown MakeDropDownForAnyOf<T>(string currentValueName, Action<T> setAction, IEnumerable<T> valueLibrary) where T : HoneybeeSchema.AnyOf
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

    }
}
