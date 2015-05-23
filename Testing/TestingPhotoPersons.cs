using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Polar.Data;

namespace TestingNs
{
   public static class TestingPhotoPersons
   {
       private static int npersons = 40 * 1000;
       public static TestDataGenerator data = new TestDataGenerator(npersons, 2378459);
       private static Random rnd=new Random();

       public static string QDescribePerson()
       {
           return @"DESCRIBE person" + rnd.Next(npersons - 1);
       }
       public static string QGetPersonInfo()
       {
           return string.Format(@"SELECT ?property ?value WHERE {{ <person{0}> ?property ?value }}", rnd.Next(npersons - 1));
       }
       public static string QGetPersonName()
       {
           return string.Format(@"SELECT ?name WHERE {{ <person{0}> <name> ?name }}", rnd.Next(npersons - 1));
       }
       public static string QContainsPersonType()
       {
           return string.Format(@"ASK WHERE {{ <person{0}> <type> <person> }}", rnd.Next(npersons - 1));
       }
       public static string QGetPersonPhotoNames()
       {
           return string.Format(@"SELECT ?name WHERE {{ ?reflection <reflected> <person{0}>
                                                  ?reflection  <in_doc>   ?doc
                                                  ?doc <name> ?name  }}", rnd.Next(npersons - 1));
       }
    }
}
