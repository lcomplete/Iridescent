using System;

namespace Iridescent.OrmExpress
{
    public class TableAttribute:Attribute
    {
        public string Name { get; set; }

        public TableAttribute()
        {
            
        }

        public TableAttribute(string name)
        {
            Name = name;
        }
    }
}