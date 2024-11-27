using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novaf_Dokr.posix.emulater
{
    public class alphabtx
    {
        public class InsOperater
        {
            public class Instruction
            {
                public string AFB { get; set; }
                public string BFB { get; set; }
                public string CFB { get; set; }
                public string DFB { get; set; }
                public string EFB { get; set; }
                public string FFB { get; set; }
                public string GFB { get; set; }
                public string HFB { get; set; }
                public string IFB { get; set; }
                public string JFB { get; set; }
                public string KFB { get; set; }
                public string LFB { get; set; }
                public string MFB { get; set; }
                public string NFB { get; set; }
                public string OFB { get; set; }
                public string PFB { get; set; }
                public string QFB { get; set; }
                public string RFB { get; set; }
                public string SFB { get; set; }
                public string TFB { get; set; }
                public string UFB { get; set; }
                public string VFB { get; set; }
                public string WFB { get; set; }
                public string XFB { get; set; }
                public string YFB { get; set; }
                public string ZFB { get; set; }
            }
            


            public static Instruction New(List<string> FBS)
            { 
                Instruction INST = new Instruction();

                foreach (var item in FBS)
                {
                    if (item.Length == 4)
                    {
                        if (item.ToLower() == "aaaa")
                        { 
                        
                        }
                    }
                    else
                    {
                        Console.WriteLine($"An `alphabet` mush be of four bits, like so `abcd`, not this `{item}`.");
                    }
                }

                return INST;
            }

            /////////////////////////////////////////
            // these are the set of four bits each //
            /////////////////////////////////////////
            //AFB, BFB,
            //CFB, DFB,
            //EFB, FFB,
            //GFB, HFB,
            //IFB, JFB,
            //KFB, LFB,
            //MFB, NFB,
            //OFB, PFB,
            //QFB, RFB,
            //SFB,
            //TFB, UFB,
            //VFB, WFB,
            //XFB, YFB,
            //ZFB
        }
    }
}
