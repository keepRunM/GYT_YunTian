using System;

namespace Zer.Framework.Import.Attributes
{
    public class SortAttribute:Attribute
    {
        public int Index { get; private set; }

        public SortAttribute(int index)
        {
            Index = index;
        }
    }
}