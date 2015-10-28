using System;
using System.IO;
using System.Web;
using System.Xml.Linq;
using RDFCommon;
using RDFTripleStore;


namespace SparqlEndpointForm
{
    public class RdfStores
    {
        private static StoreCascadingInt store;

        public static object locker=new object();

        public static StoreCascadingInt Store
        {
            get
            {
              
                {
                    return store;
                    
                }
            }
        }

        internal static void Create()
        {   
                store = new StoreCascadingInt(AppDomain.CurrentDomain.BaseDirectory + "../Databases/int based/");

                store.Start();
            //    store.ActivateCache();
              //  store.ClearAll();

                //store.AddFromXml(XElement.Load(@"C:\deployed\0001.xml"));
                //File.WriteAllText("C:/deployed/all.fog.ttl",store.ToTurtle());
            //    store.ReloadFrom(@"C:\deployed\1.ttl");
            
        }
    
        
    }
}