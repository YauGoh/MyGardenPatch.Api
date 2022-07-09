using System;

namespace ${Namespace}
{
    public partial record struct ${Name}
    {
        public ${Name} (Guid value) { Value = value; }

        public ${Name} () : this(Guid.NewGuid()) { }

        public Guid Value { get; }

        public static implicit operator ${Name}(Guid value) => new ${Name} (value);
    }
}
