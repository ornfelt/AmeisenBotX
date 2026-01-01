using System;

namespace AmeisenBotX.Core.Engines.Combat.Classes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class CombatClassMetadataAttribute : Attribute
    {
        public string DisplayName { get; }
        public string Author { get; }

        public CombatClassMetadataAttribute(string displayName, string author)
        {
            DisplayName = displayName;
            Author = author;
        }
    }

    public class CombatClassDescriptor
    {
        public string DisplayName { get; set; }
        public string Author { get; set; }
        public string TypeName { get; set; }
        public string FullTypeName { get; set; }

        public override string ToString()
        {
            return $"{DisplayName} ({Author})";
        }
    }
}
