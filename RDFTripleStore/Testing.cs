using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDFTripleStore
{
    /// <summary>
    /// Расширения для интерфейсов    <see cref="IGraph<string,string,ObjectVariants>"/> и <see cref="IGraph<Ts,Tp,To>"/>
    /// </summary>
    public static class Testing
    {
        /// <summary>
        /// запускает Build и  замеряет время.
        /// </summary>
        /// <param name="graph"> тестируемый граф должен реализовать интерфейс <see cref="IGraph<string,string,ObjectVariants>"/></param>
        /// <param name="millions">в данных пока предполагаются варианты: 1, 10, 100, 1000</param>
        public static void TestBuild(this IGraph<string, string, ObjectVariants> graph, int millions)
        {
            Perfomance.ComputeTime(() =>
                graph.Build(
                    ReadTripleStringsFromTurtle.LoadGraph(
                        Config.Source_data_folder_path + millions + ".ttl")), "build " + millions + ".ttl ", true);
        }

        /// <summary>
        /// запускает Build и  замеряет время.
        /// </summary>
        /// <param name="graph"> тестируемый граф должен реализовать интерфейс <seealso cref="IGraph<string,string,ObjectVariants>"/></param>
        /// <param name="turtleFileName"> путь к внешнему файлу ttl</param>
        public static void TestBuild(this IGraph<string, string, ObjectVariants> graph, string turtleFileName)
        {
            Perfomance.ComputeTime(() =>
                graph.Build(
                    ReadTripleStringsFromTurtle.LoadGraph(turtleFileName)),
                "build " + turtleFileName + " ", true);
        }

        /// <summary>
        /// Замеряет время:
        ///  1) поток всех триплетов ограничен 100 триплетами;
        ///  2) заменяет субъекты объектами, если они uri и проводит поиск; 
        ///  3) поиск только по предикаьам взятым из первых 100 триплетов; 
        /// </summary>
        /// <typeparam name="Ts"></typeparam>
        /// <typeparam name="Tp"></typeparam>
        /// <typeparam name="To"></typeparam>
        /// <param name="graph"></param>
        public static void TestSearch(this IGraph<string, string, ObjectVariants> graph)
        {
            var all = graph.Search();
            Triple<string, string, ObjectVariants>[] ts100 = null;
            Perfomance.ComputeTime(() =>
            {
                ts100 = all.Take(100).ToArray();
            }, "get first's 100 triples ", true);
            Perfomance.ComputeTime(() =>
            {
                foreach (var t in ts100)
                {
                    if (t.Object.Variant == ObjectVariantEnum.Iri)
                        graph.Search(((OV_iri) t.Object).full_id).ToArray();

                }
            }, "search by object as subject from first's 100 triples ", true);
            Perfomance.ComputeTime(() =>
            {
                foreach (var t in ts100)
                {
                    graph.Search(predicate: t.Predicate).ToArray();

                }
            }, "search by predicate from first's 100 triples ", true);
            Perfomance.ComputeTime(() =>
            {
                foreach (var t in ts100)
                {
                    var triples = graph.Search(t.Subject, t.Predicate, t.Object).ToArray();
                    if (!triples.All(
                        tt => tt.Subject == t.Subject && tt.Predicate == t.Predicate && tt.Object == t.Object))
                        throw new Exception();
                }
            }, "search by subject predicate and object from first's 100 triples, compare correctness ", true);
            
        }
    }
}
