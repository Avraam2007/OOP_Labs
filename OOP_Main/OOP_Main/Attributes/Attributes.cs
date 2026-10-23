using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOP_Main.Attributes {
    // This attribute should be above the properties that are allowed to be edited.
    [AttributeUsage(AttributeTargets.Property)]
    public class EditableAttribute : Attribute {
        public string DisplayName { get; }

        public EditableAttribute(string displayName) {
            DisplayName = displayName;
        }
    }
}
