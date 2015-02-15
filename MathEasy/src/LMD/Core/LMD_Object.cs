using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;

namespace MathEasy.src.LMD.Core
{
    public interface LMD_Object
    {
        String Text { get; set; }
        String HTML { get; set; }
        Boolean isEqualTo(LMD_Object cmpObj);
        void build();
    }
}
