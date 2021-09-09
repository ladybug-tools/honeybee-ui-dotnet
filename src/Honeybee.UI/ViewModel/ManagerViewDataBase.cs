using System;
using System.Collections.Generic;

namespace Honeybee.UI
{
    internal abstract class ManagerViewDataBase : IEquatable<ManagerViewDataBase>
    {
        public string Name { get; protected set; }
        public string SearchableText { get; protected set; }
        public bool Equals(ManagerViewDataBase other)
        {
            if (Object.ReferenceEquals(this, other)) return true;

            if (Object.ReferenceEquals(this, null) || Object.ReferenceEquals(other, null))
                return false;

            return this.Name == other.Name;
        }

        public override int GetHashCode()
        {
            if (Object.ReferenceEquals(this, null)) return 0;
            int hashProductName = this.Name == null ? 0 : this.Name.GetHashCode();
            return hashProductName;
        }

    }

    internal class ManagerItemComparer<T> : EqualityComparer<T> where T: ManagerViewDataBase
    {
        public override bool Equals(T x, T y)
        {

            if (Object.ReferenceEquals(x, y)) return true;
            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;

            return x.Name == y.Name;
        }


        public override int GetHashCode(T other)
        {
            if (Object.ReferenceEquals(other, null)) return 0;
            return other.Name == null ? 0 : other.Name.GetHashCode();
        }
    }

}
