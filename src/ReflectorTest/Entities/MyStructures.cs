using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReflectorTest.Entities
{
    public struct MyStructures
    {
    }

    public struct PropertiesAndFieldsStructure
    {

        public int MyIntProperty { get; set; }
        public string MyStringProperty { get; set; }  
        public int? MyNulableIntProperty { get; set; }
        public string? MyNulableStringProperty { get; set; }  
        public int MyPrivateSetIntProperty { get; private set; }
        public string MyPrivateSetStringProperty { get; private set; } 
        public int MyPrivateInitSetIntProperty { get; private init; }
        public string MyPrivateInitSetStringProperty { get; private init; }  
        public int MyInternalInitSetIntProperty { get; internal init; }
        public string MyInternalInitSetStringProperty { get; internal init; }  
        public int MyInitSetIntProperty { get; init; }
        public string MyInitSetStringProperty { get; init; }  
        public int MyPrivateGetIntProperty { private get; init; }
        public string MyPrivateGetStringProperty { private get; init; }  
        private int MyPrivateIntProperty { get; set; }
        private string MyPrivateStringProperty { get; set; }  
        internal int MyInternalIntProperty { get; set; }
        internal string MyInternalStringProperty { get; set; }  

        public int MyInternalGetIntProperty { internal get; init; }
        public string MyInternalGetStringProperty { internal get; init; } 

        public int MyIntField;
        public string MyStringField;
        public int? MyNullableIntField;
        public string? MyNullableStringField;

        private int MyPrivateIntField;
        private string MyPrivateStringField;

        internal int MyInternalIntField;
        internal string MyInternalStringField;

        public const int MyConstIntField = 10;
        public const string MyConstStringField = "Empty";

        public readonly int MyReadonlyIntField;
        public readonly string MyReadonlyStringField;
        public static int MyStaticIntField;
        public static string MyStaticStringField;

        public static readonly int MyStaticReadonlyIntField;
        public static readonly string MyStaticReadonlyStringField;
    }
}
