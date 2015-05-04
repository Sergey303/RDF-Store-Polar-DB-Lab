using PolarDB;
using System.Collections.Generic;
using System.Linq;

namespace RDFTripleStore
{
    public static class TableViewEx
    {

        public static bool NotDeleted(object[] entry)
        {
            return !(bool)entry[0];
        }

        public static IEnumerable<object[]> ReadWritableTriples(this IEnumerable<object> source)
        {
            return source//.Select(entry => entry.Get())
                .Cast<object[]>()
                .Where(NotDeleted)
                .Select(row => row[1])
                .Cast<object[]>();
        }
        public static IEnumerable<object[]> ReadWritableTriples(this IEnumerable<PaEntry> source)
        {
            return source.Select(entry => entry.Get())
                .Cast<object[]>()
                .Where(NotDeleted)
                .Select(row => row[1])
                .Cast<object[]>();
        }
    }
}