using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReflectorTest.Entities
{
    internal struct MyStructures
    {
    }

    internal struct PropertiesAndFieldsStructure
    {
        public int MyIntProperty { get; set; }
        public string MyStringProperty { get; set; } = string.Empty;
        public int? MyNulableIntProperty { get; set; }
        public string? MyNulableStringProperty { get; set; } = null;
        public int MyPrivateSetIntProperty { get; private set; }
        public string MyPrivateSetStringProperty { get; private set; } = string.Empty;
        public int MyPrivateInitSetIntProperty { get; private init; }
        public string MyPrivateInitSetStringProperty { get; private init; } = string.Empty;
        public int MyInternalInitSetIntProperty { get; internal init; }
        public string MyInternalInitSetStringProperty { get; internal init; } = string.Empty;
        public int MyInitSetIntProperty { get; init; }
        public string MyInitSetStringProperty { get; init; } = string.Empty;
        public int MyPrivateGetIntProperty { private get; init; }
        public string MyPrivateGetStringProperty { private get; init; } = string.Empty;
        private int MyPrivateIntProperty { get; set; }
        private string MyPrivateStringProperty { get; set; } = string.Empty;
        internal int MyInternalIntProperty { get; set; }
        internal string MyInternalStringProperty { get; set; } = string.Empty;

        public int MyInternalGetIntProperty { internal get; init; }
        public string MyInternalGetStringProperty { internal get; init; } = string.Empty;

        public int MyIntField;
        public string MyStringField = string.Empty;
        public int? MyNullableIntField;
        public string? MyNullableStringField = null;

        private int MyPrivateIntField;
        private string MyPrivateStringField = string.Empty;

        internal int MyInternalIntField;
        internal string MyInternalStringField = string.Empty;

        public const int MyConstIntField = 10;
        public const string MyConstStringField = "Empty";

        public readonly int MyReadonlyIntField;
        public readonly string MyReadonlyStringField = string.Empty;

        public static int MyStaticIntField;
        public static string MyStaticStringField = string.Empty;

        public static readonly int MyStaticReadonlyIntField;
        public static readonly string MyStaticReadonlyStringField = string.Empty;
    }
}
