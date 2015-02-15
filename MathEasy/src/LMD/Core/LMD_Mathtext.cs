using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathEasy.src.LMD.Math;

namespace MathEasy.src.LMD.Core
{
    public class LMD_Mathtext: LMD_Object
    {
        public String Text { get; set; }
        public String HTML { get; set; }

        public LMD_Mathtext(String Text)
        {
            this.Text = Text;
            this.HTML = "";
        }

        public bool isEqualTo(LMD_Object cmpObj)
        {
            LMD_Mathtext cmpMt = cmpObj as LMD_Mathtext;
            return cmpMt != null && Text == cmpMt.Text;
        }

        public void build()
        {
            Text = Text.Replace("\n", "");
            HTML = MathEngine.getHTML(Text);
        }

        public String buildAndReturnResultIfPossible()
        {
            return MathEngine.getCommandResult(Text.Replace("\n",""));
        }
    }
}
