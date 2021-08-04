using System;

namespace Honeybee.UI
{
    internal abstract class ManagerViewDataBase : IEquatable<ManagerViewDataBase>
    {
        public string Name { get; protected set; }
        public string SearchableText { get; protected set; }
        public bool Equals(ManagerViewDataBase other)
        {
            return other?.Name == this?.Name;
        }

    }

}
