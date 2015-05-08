using System;
using System.Collections.Generic;
using System.Linq;
using PolarDB;
using RDFCommon.OVns;

namespace GoTripleStore
{
    public class Triple
    {
        private ObjectVariants _subj, _pred, _obj;
        public Triple(ObjectVariants subj, ObjectVariants pred, ObjectVariants obj) { _subj = subj; _pred = pred; _obj = obj; }
        public ObjectVariants Subj { get { return _subj; } }
        public ObjectVariants Pred { get { return _pred; } }
        public ObjectVariants Obj { get { return _obj; } }
    }
    public interface IGraph
    {
        bool Contains(ObjectVariants subj, ObjectVariants pred, ObjectVariants obj);
        IEnumerable<Triple> GetTriples();
        IEnumerable<Triple> GetTriplesWithSubject(ObjectVariants subj);
        IEnumerable<Triple> GetTriplesWithSubjectPredicate(ObjectVariants subj, ObjectVariants pred);
        IEnumerable<Triple> GetTriplesWithPredicate(ObjectVariants pred);
        IEnumerable<Triple> GetTriplesWithPredicateObject(ObjectVariants pred, ObjectVariants obj);
        IEnumerable<Triple> GetTriplesWithObject(ObjectVariants obj);

    }
    public class TPack
    {
        public ObjectVariants[] row;
        private IGraph g;
        public IGraph G { get { return g; } }
        public TPack(ObjectVariants[] row, IGraph g)
        {
            this.row = row;
            this.g = g;
        }
        // Существенным добавлением в ObjectVariants является индекс переменной (!) Пока используется целый, что неправильно 
        // Метод по значению si (часть шаблона триплета) выдает или si как литерал, или берет значение в массиве
        public ObjectVariants Get(ObjectVariants si)
        {

            return si.Variant == ObjectVariantEnum.Double ? row[((OV_integer)si).value] : si;
        }
        // Запись значения в массив
        public void Set(ObjectVariants si, ObjectVariants valu)
        {
            if (si.Variant == ObjectVariantEnum.Double) throw new Exception("argument must be an index");
            int ind = ((OV_integer)si).value;
            row[ind] = valu;
        }
    }
    public static class TPackExtention
    {
        // На уровне массива объектов TPack хранятся IRI, литералы или null, означающее, что значение не вычислено,
        // на уровне параметров специальный индексный вариант обозначает параметр по номеру

        // Универсальный Match: все параметры могуть быть индексами, IRI, литералами (только для объекта)
        public static IEnumerable<TPack> spo(this IEnumerable<TPack> packs, ObjectVariants subj, ObjectVariants pred, ObjectVariants obj)
        {
            var query = packs.SelectMany(pk => ProcessTripleTemplate(pk, subj, pred, obj));
            return query;
        }
        public static IEnumerable<TPack> ProcessTripleTemplate(TPack onepack, ObjectVariants subj, ObjectVariants pred, ObjectVariants obj)
        {
            // Значения для обработки
            ObjectVariants[] values = new ObjectVariants[3];
            values[0] = onepack.Get(subj); values[1] = onepack.Get(pred); values[2] = onepack.Get(obj);
            // Маска
            int mask = (values[0] != null ? 1 : 0) << 2 | (values[1] != null ? 1 : 0) << 1 | (values[0] != null ? 1 : 0);
            // Значения: 0-spo, 1-spO, 2-sPo, 3-sPO, 4-Spo, 5-SpO, 6-SPo, 7-SPO
            if (mask == 0)
            {
                bool ok = onepack.G.Contains(values[0], values[1], values[2]);
                if (ok) return Enumerable.Repeat<TPack>(onepack, 1);
                else Enumerable.Empty<TPack>();
            }
            // Далее могут быть 0, 1, много решений
            Triple[] solutions = new Triple[0];
            switch (mask)
            {
                case 1: { solutions = onepack.G.GetTriplesWithSubjectPredicate(subj, pred).ToArray(); break; }
                case 2: { solutions = onepack.G.GetTriplesWithSubject(subj).Where(t => t.Obj == obj).ToArray(); break; }
                case 3: { solutions = onepack.G.GetTriplesWithSubject(subj).ToArray(); break; }
                case 4: { solutions = onepack.G.GetTriplesWithPredicateObject(pred, obj).ToArray(); break; }
                case 5: { solutions = onepack.G.GetTriplesWithPredicate(pred).ToArray(); break; }
                case 6: { solutions = onepack.G.GetTriplesWithObject(obj).ToArray(); break; }
                case 7: { solutions = onepack.G.GetTriples().ToArray(); break; }
            }
            if (solutions.Length == 0) return Enumerable.Empty<TPack>();
            // Контекст (пустые)
            bool[] isEmpty = null;
            if (solutions.Length > 1)
            {
                // Надо сохранить контекст
                isEmpty = onepack.row.Select(ov => ov == null).ToArray();
            }
            // Теперь надо сформировать поток ТPack-ов, построенных на одном и том же массиве rows, но перед каждым
            // элементом потока мы восстанавливаем контекст через isEmpty и фиксируем результаты из очередного
            // триплета в solutions.
            ObjectVariants[] row = onepack.row;
            bool notfirst = false;
            var packFlow = solutions
                .Select(t =>
                {
                    // Восстановление контекста если не первый
                    if (notfirst)
                    {
                        for (int i = 0; i < row.Length; i++) if (isEmpty[i]) row[i] = null;
                    }
                    notfirst = true;
                    // Запись вычисленных полей
                    if ((mask & 4) != 0) onepack.Set(subj, t.Subj);
                    if ((mask & 2) != 0) onepack.Set(pred, t.Pred);
                    if ((mask & 1) != 0) onepack.Set(obj, t.Obj);
                    return new TPack(row, onepack.G);
                });
            return packFlow; //Enumerable.Empty<TPack>();
        }
    }
}
