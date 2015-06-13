using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using RDFCommon.OVns;
using RDFCommon;

namespace RDFTripleStore
{
    public class NamedGraphsByFolders : RdfNamedGraphs
    {

        public NamedGraphsByFolders(DirectoryInfo directory, NodeGenerator ng, Func<DirectoryInfo, IGraph> graphCtor)
            : base(ng, s => graphCtor(new DirectoryInfo(directory + CodeGraphName2DirName(s))))
        {
            foreach (var graphDir in directory.EnumerateDirectories())
                named.Add(DecodeDirName2GraphName(graphDir.Name), graphCtor(graphDir));
        }

        private static string CodeGraphName2DirName(string name)
        {
            return Convert.ToBase64String(Encoding.UTF32.GetBytes(name));
        }

        private static string DecodeDirName2GraphName(string fileName)
        {
            return Encoding.UTF32.GetString(Convert.FromBase64String(fileName));

        }
    }
}