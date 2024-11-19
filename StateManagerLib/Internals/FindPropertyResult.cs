using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace StateManagerLib.Internals
{
    internal class FindPropertyResult
    {
        internal PropertyInfo Property {  get;  }

        internal string[] Path { get; }=[];

        internal int NestLevel {  get; }

        internal FindPropertyResult(PropertyInfo property, string[] path, int nestLevel)
        {
            Property = property;
            Path = path;
            NestLevel = nestLevel;
        }
    }
}
