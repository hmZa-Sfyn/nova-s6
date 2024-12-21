using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nova_s6.shells.hqsh.mind
{
    public class attr
    {
        public static Dictionary<int,string> MindStorage = new Dictionary<int, string>();

        public static bool storeAdd(List<string> data_points)
        {
            for (int i = 0; i < data_points.Count; i++) 
            {
                Console.WriteLine($"{MindStorage.Count+i,-4} ==== {data_points[i],-20}");
                MindStorage[MindStorage.Count+i] = data_points[i];
            }
            
            return true; 
        }

        public static bool store(List<string> data_points)
        {
            for (int i = 0; i < data_points.Count; i++)
            {
                Console.WriteLine($"{i,-4} ==== {data_points[i],-20}");
                MindStorage[i].Replace(MindStorage[i], data_points[i]);
            }

            return true;
        }

        public static bool show()
        {
            foreach (var (k, v) in MindStorage)
            {
                Console.WriteLine($"{k,-4} ==== {v,-20}");
            }

            return true;
        }

        public static string get(int k)
        {
            if (MindStorage[k] == null)
                return 0.ToString();

            return MindStorage[k].ToString();
        }
    }
}
