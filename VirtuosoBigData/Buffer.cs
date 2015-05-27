using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenLink.Data.Virtuoso;
using VirtuosoTest;

namespace VirtuosoBigData
{
    public class BufferForInsertCommand
    {
        private AdapterVirtuoso engine;
        private string graph;
        public BufferForInsertCommand(AdapterVirtuoso engine, string graph)
        {
            this.engine = engine;
            this.graph = graph;
        }
        private sema2012m.BufferredProcessing<string> b_entities;
        public void InitBuffer()
        {
            // размер буфера
            int bufferportion = 10000;
            // размер порции для внедрения данных
            int portion = 20;

            b_entities = new sema2012m.BufferredProcessing<string>(bufferportion, flow =>
            {
                var query = flow.Select((ent, i) => new { e = ent, i = i }).GroupBy(ei => ei.i / portion, ei => ei);
                VirtuosoCommand trcommand = engine.RunStart();
                foreach (var q in query)
                {
                    string data = q.Select(ei => ei.e + " . ")
                        .Aggregate((sum, s) => sum + " " + s);
                    //bool found = q.Any(ei => ei.e.s == "Gury_Marchuk");
                    trcommand.CommandText = "SPARQL INSERT INTO GRAPH <" + graph + "> {" + data + "}\n";
                    try
                    {
                        trcommand.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                    }
                }
                engine.RunStop(trcommand);
            });
        }
        public void AddCommandToBuffer(string comm)
        {
            b_entities.Add(comm);
        }
        public void FlushBuffer()
        {
            b_entities.Flush();
        }
    }
}
