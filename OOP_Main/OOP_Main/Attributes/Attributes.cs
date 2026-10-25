using System;

namespace OOP_Main.Attributes {
    /// <summary>
    /// This attribute should be above the properties that are allowed to be edited.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class EditableAttribute : Attribute {
        public string DisplayName { get; }

        public EditableAttribute(string displayName) {
            DisplayName = displayName;
        }
    }
}
