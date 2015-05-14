using System;
using System.IO;
using System.Linq;
using RDFCommon;
using RDFCommon.OVns;

namespace TestingNs
{
    public static class BSBmParams
    {
        private static readonly ObjectVariants[] _products = null;//StoreLauncher.Store.GetTriplesWithPredicateObject(new OV_iri("http://www.w3.org/1999/02/22-rdf-syntax-ns#type"), 
            //new OV_iri("http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/vocabulary/Product")).ToArray();

        private static readonly int _productCount = 0;// _products.Count();
        private static readonly Random _random = new Random();

        private static string Read(string readLine)
        {
            if (readLine.StartsWith("http://"))
                return "<" + readLine + ">";
            var splitPrefixed = Prologue.SplitPrefixed(readLine);
            if (splitPrefixed.prefix == "bsbm-inst:")
                return "<http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/>";
            return readLine;
        }

        private static void QueryWriteParameters(string parameteredQuery, StreamWriter output)
        {
            var product = _products.ElementAt(_random.Next(0, _productCount));
            //Millions == 1000 ? 2855260 : Millions == 100 ? 284826 : Millions == 10 ? 284826 : 2785;
            //int productFeatureCount;
            //switch (Millons)
            //{
            //    case 1000:
            //        productFeatureCount = 478840;
            //        break;
            //    case 100:
            //        productFeatureCount = 47884;
            //        break;
            //    case 10:
            //        productFeatureCount = 47450;
            //        break;
            //    default:
            //        productFeatureCount = 4745;
            //        break;
            //}
            //int productTypesCount = Millons == 1000 ? 20110 : Millons == 100 ? 2011 : Millons == 10 ? 1510 : 151;
            //var review = random.Next(1, productCount*10);
            ////var product = random.Next(1, productCount);
            ////var productProducer = product/ProductsPerProducer + 1; 
            //var offer = random.Next(1, productCount*OffersPerProduct);
            //var vendor = offer/OffersPerVendor + 1;
            //  var offersCodes = ts.gettri(
            //   ts.CodeEntityFullName("<http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/vocabulary/Offer>"),
            //ts.CodePredicateFullName("<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>"));
            //  codes = offersCodes as int[] ?? offersCodes.ToArray();
            //  var offer = ts.DecodeEntityFullName(codes[random.Next(0, codes.Length)]);
            //  var reviewsCodes = ts.GetSubjectByObjPred(
            // ts.CodeEntityFullName("<http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/vocabulary/Review>"),
            // ts.CodePredicateFullName("<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>"));
            //  codes = reviewsCodes as int[] ?? reviewsCodes.ToArray();
            //  var review = ts.DecodePredicateFullName(codes[random.Next(0, codes.Length)]);
            //if (parameteredQuery.Contains("%ProductType%"))
            //    output.WriteLine("bsbm-inst:ProductType" + random.Next(1, productTypesCount));
            //if (parameteredQuery.Contains("%ProductFeature1%"))
            //    output.WriteLine("bsbm-inst:ProductFeature" + random.Next(1, productFeatureCount));
            //if (parameteredQuery.Contains("%ProductFeature2%"))
            //    output.WriteLine("bsbm-inst:ProductFeature" + random.Next(1, productFeatureCount));
            //if (parameteredQuery.Contains("%ProductFeature3%"))
            //    output.WriteLine("bsbm-inst:ProductFeature" + random.Next(1, productFeatureCount));
            if (parameteredQuery.Contains("%x%")) output.WriteLine(_random.Next(1, 500).ToString());
            if (parameteredQuery.Contains("%y%")) output.WriteLine(_random.Next(1, 500).ToString());
            if (parameteredQuery.Contains("%ProductXYZ%"))
                output.WriteLine(product);
            //"<http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromProducer{0}/Product{1}>",productProducer, product);
            //if (parameteredQuery.Contains("%word1%")) output.WriteLine(words[random.Next(0, words.Length)]);
            //if (parameteredQuery.Contains("%currentDate%"))
            //    output.WriteLine("\"" + DateTime.Today.AddYears(-6) + "\"^^<http://www.w3.org/2001/XMLSchema#dateTime>");
            //if (parameteredQuery.Contains("%ReviewXYZ%"))
            //    output.WriteLine(review);//"<http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromRatingSite{0}/Review{1}>",review/10000 + 1, review);
            //if (parameteredQuery.Contains("%OfferXYZ%"))
            //    output.WriteLine(offer);
            ////"<http://www4.wiwiss.fu-berlin.de/bizer/bsbm/v01/instances/dataFromVendor{0}/Offer{1}>", vendor, offer);
        }

        public static string QueryReadParameters(string parameteredQuery, StreamReader input)
        {
            if (parameteredQuery.Contains("%ProductType%"))
                parameteredQuery = parameteredQuery.Replace("%ProductType%", Read(input.ReadLine()) + ">");
            if (parameteredQuery.Contains("%ProductFeature1%"))
                parameteredQuery = parameteredQuery.Replace("%ProductFeature1%", Read(input.ReadLine()));
            if (parameteredQuery.Contains("%ProductFeature2%"))
                parameteredQuery = parameteredQuery.Replace("%ProductFeature2%", Read(input.ReadLine()));
            if (parameteredQuery.Contains("%ProductFeature3%"))
                parameteredQuery = parameteredQuery.Replace("%ProductFeature3%", Read(input.ReadLine()));
            if (parameteredQuery.Contains("%x%"))
                parameteredQuery = parameteredQuery.Replace("%x%", input.ReadLine());
            if (parameteredQuery.Contains("%y%"))
                parameteredQuery = parameteredQuery.Replace("%y%", input.ReadLine());
            if (parameteredQuery.Contains("%ProductXYZ%"))
                parameteredQuery = parameteredQuery.Replace("%ProductXYZ%", "<" + input.ReadLine() + ">");
            if (parameteredQuery.Contains("%word1%"))
                parameteredQuery = parameteredQuery.Replace("%word1%", input.ReadLine());
            if (parameteredQuery.Contains("%currentDate%"))
                parameteredQuery = parameteredQuery.Replace("%currentDate%", input.ReadLine());
            if (parameteredQuery.Contains("%ReviewXYZ%"))
                parameteredQuery = parameteredQuery.Replace("%ReviewXYZ%", "<" + input.ReadLine() + ">");
            if (parameteredQuery.Contains("%OfferXYZ%"))
                parameteredQuery = parameteredQuery.Replace("%OfferXYZ%", "<" + input.ReadLine() + ">");
            return parameteredQuery;
        }

        public static void CreateParameters(int query, int count, int millions)
        {
            var paramvaluesFilePath = string.Format(@"..\..\examples\bsbm\queries\parameters\param values for{0}m {1} query.txt", millions, query);
            var paramvaluesFilePath2 = string.Format(@"..\..\examples\bsbm\queries\parameters\{0}.rq", query);
            using (StreamWriter streamParameters = new StreamWriter(paramvaluesFilePath, true))
            using (StreamReader streamQuery = new StreamReader(paramvaluesFilePath2))
            {
                string q = streamQuery.ReadToEnd();
                for (int j = 0; j < count; j++)
                {
                    QueryWriteParameters(q, streamParameters);
                }
            }
        }
    }
}