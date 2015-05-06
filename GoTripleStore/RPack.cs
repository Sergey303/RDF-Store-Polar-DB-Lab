using System;
using System.Collections.Generic;
using System.Linq;
using RDFCommon.OVns;

namespace GoTripleStore
{
    public class RPack
    {
        //public bool result;
        public object[] row;
        private TripleStore ts;
        public TripleStore Store { get { return ts; } }
        public RPack(object[] row, TripleStore ts)
        {
            this.row = row;
            this.ts = ts;
        }
        public object Get(object si)
        {
            return si is int ? row[(int)si] : si;
        }
        public string Ges(object si)
        {
            return si is int ? (string)row[(int)si] : (string)si;
        }
        // Получаение IRI
        public string Val(int ind)
        {
            return (string)row[ind];
        }
        // Получение целых
        public int Vai(int ind)
        {
            ObjectVariants lit = (ObjectVariants)row[ind];
            if (lit.Variant != ObjectVariantEnum.Int) throw new Exception("Wrong literal vid in Vai method");
            return ((OV_int)lit).value;
        }
        // Получение СТРОК
        public string Vas(int ind)
        {
            ObjectVariants lit = (ObjectVariants)row[ind];
            if (lit.Variant != ObjectVariantEnum.Str) throw new Exception("Wrong literal vid in Vas method");
            return ((OV_string)lit).value;
        }
        public void Set(object si, object valu)
        {
            if (!(si is int)) throw new Exception("argument must be an index");
            int ind = (int)si;
            row[ind] = valu;
        }
    }
    public static class RPackExtention
    {
        // На уровне массива объектов RPack храняться (что попало или) null, если значение не вычислено,
        // строка, если это IRI, значение типа ObjectVariants если это литерал
        // на уровне параметров целые обозначают параметры по номеру
        public static IEnumerable<RPack> spo(this IEnumerable<RPack> pack, object subj, object pred, object obj)
        { // отсуствуют DatatypeProperty
            return pack.Where(pk => pk.Store.ChkOSubjPredObj(pk.Ges(subj), pk.Ges(pred), pk.Ges(obj)));
        }
        public static IEnumerable<RPack> Spo(this IEnumerable<RPack> pack, object subj, object pred, object obj)
        {
            if (!(subj is int)) throw new Exception("subject must be an index");
            return pack.SelectMany(pk => pk.Store
                .GetSubjectByObjPred(pk.Ges(obj), pk.Ges(pred))
                .Select(su =>
                {
                    pk.Set(subj, su);
                    return new RPack(pk.row, pk.Store);
                }));
        }
        public static IEnumerable<RPack> spO(this IEnumerable<RPack> pack, object subj, object pred, object obj)
        {
            if (!(obj is int)) throw new Exception("object must be an index");
            return pack.SelectMany(pk => pk.Store
                .GetObjBySubjPred(pk.Ges(subj), pk.Ges(pred))
                .Select(ob =>
                {
                    pk.Set(obj, ob);
                    return new RPack(pk.row, pk.Store);
                }));
        }
        public static IEnumerable<RPack> spD(this IEnumerable<RPack> pack, object subj, object pred, object dat)
        {
            if (!(dat is int)) throw new Exception("data must be an index");
            return pack.SelectMany(pk => pk.Store
                .GetDataBySubjPred(pk.Ges(subj), pk.Ges(pred))
                .Select(da =>
                {
                    pk.Set(dat, da); //((Text)da.value).s);
                    return new RPack(pk.row, pk.Store);
                }));
        }
    }
}
