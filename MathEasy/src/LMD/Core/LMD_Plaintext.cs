using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Documents;

namespace MathEasy.src.LMD.Core
{
    public class LMD_Plaintext: LMD_Object
    {
        public String Text { get; set; }
        public String HTML { get; set; }

        public LMD_Plaintext(String Text)
        {
            this.Text = Text;
            this.HTML = "";
        }

        public bool isEqualTo(LMD_Object cmpObj)
        {
            LMD_Plaintext cmpPt = cmpObj as LMD_Plaintext;
            return cmpPt != null && Text == cmpPt.Text;
        }

        public void build()
        {
            HTML = HTMLizer.prepareForHTML(Text);
        }
    }
}
