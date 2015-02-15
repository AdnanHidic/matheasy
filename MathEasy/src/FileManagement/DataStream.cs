using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathEasy.src.FileManagement
{
    public class DataStream
    {
        public enum Type
        {
            TXT,
            HTML
        }

        public String Content { get; set; }
        public Type TypeProperty { get; set; }

        public DataStream(String Information, Type TypeProperty)
        {
            this.Content = Information;
            this.TypeProperty = TypeProperty;
        }
    }
}
