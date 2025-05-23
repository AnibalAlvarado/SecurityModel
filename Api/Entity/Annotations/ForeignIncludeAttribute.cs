using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Annotations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ForeignIncludeAttribute : Attribute
    {
        public string? SelectPath { get; }

        public ForeignIncludeAttribute(string? selectPath = null)
        {
            SelectPath = selectPath;
        }
    }
}
