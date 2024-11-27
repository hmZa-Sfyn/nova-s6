using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novaf_Dokr.posix.emulater
{

    public class compiler
    {
        public static string Convert_To_Bytecode(string codeline)
        {
            //string bytecode = "aaaaaaaaaaaaaaaa";
            string bytecode = "";
            int BitCount = 0;

            List<string> Words = codeline.Split(' ').ToList();

            for (int i = 0; i < Words.Count; i++)
            {
                if (Words[i].ToLower() == "nop")
                {
                    Words[i] = "aaaaaaaaaaaaaaaaaaaa";
                    break;
                }
                else if (Words[i].ToLower() == "rem")
                {
                    Words[i] = "0001";
                }
                else if (Words[i].ToLower() == "regs")
                {
                    Words[i] = "0010";
                }
                else if (Words[i].ToLower() == "regl")
                {
                    Words[i] = "0011";
                }
                else if (Words[i].ToLower() == "rem")
                {
                    Words[i] = "0001";
                }
                else if (Words[i].ToLower() == "rem")
                {
                    Words[i] = "0001";
                }
            } 

            return bytecode;
        }

        public static List<string> To_Bytecode(string code)
        { 
        
            List<string> BC = new List<string>();

            List<string> lINES = code.Split(';').ToList();

            foreach (string line in lINES)
            {
                Console.WriteLine(line);
                BC.Add(Convert_To_Bytecode(line));
            }

            return BC;

        }
    }
}
