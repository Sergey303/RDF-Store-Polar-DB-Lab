using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sema2012m;
using TestingNs;
using Testing = TestingNs.Testing;

namespace VirtuosoBigData
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Start");
            EngineVirtuoso engine = new EngineVirtuoso("HOST=localhost:1550;UID=dba;PWD=dba;Charset=UTF-8;Connection Timeout=500", "g");

            string graph = "berlin100m";
         //  LoadGraph(engine, graph, @"..\..\..\Databases\string based\photoPersons.ttl");
            //return;

            //RunTests(engine);
           // RunMany(engine);
       //     RunBerlinsWithConstants(engine);
//            ViruosoBSBmParameters(engine);
        //    OneParametred(engine, 12, 100);
            foreach (var row in engine.Query("sparql "+TestingPhotoPersons.testing.QGetPerson3123Info()))
            {
                foreach (var o in row)
                {
                    Console.WriteLine(o);
                }
            }
          //  Console.WriteLine("Total duration=" + (DateTime.Now - tt0).Ticks / 10000L); tt0 = DateTime.Now;
            TestingPhotoPersons.testing.Run(q =>
            {
                var qu = "sparql " + q;

                if (qu.Contains("SELECT "))
                {                                  
                   return engine.Query(qu).Count();
                }
                else
                {
                    //IEnumerable<object[]> enumerable = engine.Query(qu);
                    var execute = engine.ExecuteScalar(qu);
                    return true;
                }
            });
        }
        private static string GetEntity(Dictionary<string, string> namespaces, string line)
        {
            string subject = null;
            int colon = line.IndexOf(':');
            if (colon == -1) { Console.WriteLine("Err in line: " + line); goto End; }
            string prefix = line.Substring(0, colon + 1);
            if (!namespaces.ContainsKey(prefix)) { Console.WriteLine("Err in line: " + line); goto End; }
            subject = namespaces[prefix] + line.Substring(colon + 1);
        End:
            return subject;
        }
        private static void LoadGraph(EngineVirtuoso engine, string graph, string datafile)
        {
            BufferForInsertCommand buffer = new BufferForInsertCommand(engine, graph);
            buffer.InitBuffer();

            engine.Execute("sparql clear graph <" + graph + ">");

            int ntriples = 0;
            string subject = null;
            Dictionary<string, string> namespaces = new Dictionary<string, string>();
            StreamReader sr = new StreamReader(datafile);
            int count = 200000000;
            for (int i = 0; i < count; i++)
            {
                string line = sr.ReadLine();
                if (i % 10000 == 0) { Console.Write("{0} ", i / 10000); }
                if (line == null) break;
                if (line == "") continue;
                if (line[0] == '@')
                { // namespace
                    string[] parts = line.Split(' ');
                    if (parts.Length != 4 || parts[0] != "@prefix" || parts[3] != ".")
                    {
                        Console.WriteLine("Err: strange line: " + line);
                        continue;
                    }
                    string pref = parts[1];
                    string nsname = parts[2];
                    if (nsname.Length < 3 || nsname[0] != '<' || nsname[nsname.Length - 1] != '>')
                    {
                        Console.WriteLine("Err: strange nsname: " + nsname);
                        continue;
                    }
                    nsname = nsname.Substring(1, nsname.Length - 2);
                    namespaces.Add(pref, nsname);
                }
                else if (line[0] != ' ')
                { // Subject
                    line = line.Trim();
                    subject = line.Substring(1, line.Length - 2);//GetEntity(namespaces, line);
                    if (subject == null) continue;
                }
                else
                { // Predicate and object
                    string line1 = line.Trim();
                    int first_blank = line1.IndexOf(' ');
                    if (first_blank == -1) { Console.WriteLine("Err in line: " + line); continue; }
                    string pred_line = line1.Substring(0, first_blank);
                    string predicate = pred_line.Substring(1, pred_line.Length - 2); //GetEntity(namespaces, pred_line);
                    string rest_line = line1.Substring(first_blank + 1).Trim();
                    // Уберем последний символ
                    rest_line = rest_line.Substring(0, rest_line.Length - 1).Trim();
                    bool isPrefixed = true;
                    if (rest_line[0] == '\"' || rest_line[0] == '<') isPrefixed = false;
                    string command = "<" + subject + "> <" + predicate + "> ";
                    if (!isPrefixed)
                    {
                        // Тип данных может быть "префиксным"
                        int pp = rest_line.IndexOf("^^");
                        if (pp != -1 && rest_line[pp + 2] != '<')
                        {
                            string data_part = rest_line.Substring(0, pp + 2);
                            string qname = rest_line.Substring(pp + 2);
                            string tp = qname;// GetEntity(namespaces, qname);
                            // Надо бы проверить...
                            command += data_part + "<" + tp + ">";
                        }
                        else
                        {
                            command += rest_line;
                        }
                    }
                    else
                    {
                        command += "<" + rest_line + "> ";
                    }
                    buffer.AddCommandToBuffer(command);
                    ntriples++;
                }
            }
            buffer.FlushBuffer();
            Console.WriteLine();
            Console.WriteLine("ntriples={0}", ntriples);
        }
      


    }
}
