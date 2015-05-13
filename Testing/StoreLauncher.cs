using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RDFCommon;
using RDFTripleStore;

namespace TestingNs
{
   static class StoreLauncher
    {
        public static readonly IStore Store = new FirstIntStore("../../../Databases/int based");
        //public static readonly IStore Store = new SecondStringSore("../../../Databases/string based");
       public const int Millions = 1;
       //  public static readonly SecondStringSore ts = new SecondStringSore("../../../Databases/");
        static StoreLauncher()
       {
          if(false)
          {
              Store.ReloadFrom(Config.Source_data_folder_path + Millions + ".ttl");
          }
            Store.Warmup();
       }
    }
}
