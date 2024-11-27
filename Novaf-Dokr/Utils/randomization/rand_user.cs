using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novaf_Dokr.Utils.randomization
{
    public class rand_user
    {
        public static string UserName(int a)
        {
           
            string Name = "";

            int b = 1; // DateTime.Now.Year;

            int Id = UserNames.IdLoop(a + b);

            Name = UserNames.UserNmaed_FrmID[Id];

            return Name;
        }

        public class UserNames
        {
            public static Dictionary<int, string> UserNmaed_FrmID = new Dictionary<int, string>();


            public static int IdLoop(int numb)
            {
                UserNames.UserNmaed_FrmID[0] = "noobaa";
                UserNames.UserNmaed_FrmID[1] = "pro-dev";
                UserNames.UserNmaed_FrmID[2] = "hecker";
                UserNames.UserNmaed_FrmID[3] = "mike-tyson";
                UserNames.UserNmaed_FrmID[4] = "JohnH";


                int num = 0;

                for (int i = num - num; i < num; i++)
                {
                    if (num >= UserNmaed_FrmID.Count)
                    {
                        num = 0;
                    }
                    else
                    {
                        num = num + 1;
                    }

                    //Console.WriteLine($".... num: {num}");
                }

                return num;
            } 
        }


    }
}
