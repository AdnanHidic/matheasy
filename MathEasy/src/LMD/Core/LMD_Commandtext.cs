using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathEasy.src.LMD.Commands;

namespace MathEasy.src.LMD.Core
{
    public class LMD_Commandtext : LMD_Object
    {
        public enum TypeOfCommand
        {
            Plot,
            Eval
        }

        public String Text { get; set; }
        public String HTML { get; set; }

        public TypeOfCommand Type { get; set; }

        public LMD_Commandtext(String Text, TypeOfCommand typeOfCommand)
        {
            this.Text = Text;
            this.Type= typeOfCommand;
            this.HTML = "";
        }

        public Boolean isEqualTo(LMD_Object cmpObj)
        {
            LMD_Commandtext cmpCt = cmpObj as LMD_Commandtext;
            return cmpCt != null && Text == cmpCt.Text;
        }

        public void build()
        {
            Text = Text.Replace("\n", "");
            if (Type == TypeOfCommand.Eval)
            {
                HTML = CommandEngine.getHTMLEval(Text);
            }
            else if (Type == TypeOfCommand.Plot)
            {
                HTML = CommandEngine.getHTMLPlot(Text);
            }
            else
            {
                HTML = Text;
            }
        }
    }
}
