using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using PolarDB;
using Task15UniversalIndex;

namespace GoTripleStore
{
    public class Standard3TabsString //: IGra<PaEntry> 
    {
        private string path;
        private TableView tab_person, tab_photo_doc, tab_reflection;
        private IndexHalfkeyImmutable<string> ind_arr_person, ind_arr_photo_doc, ind_arr_reflected, ind_arr_in_doc;
        private IndexDynamic<string, IndexHalfkeyImmutable<string>> index_person, index_photo_doc, index_reflected, index_in_doc;
        public Standard3TabsString(string path)
        {
            PType tp_person = new PTypeRecord(
                new NamedType("code", new PType(PTypeEnumeration.sstring)),
                new NamedType("name", new PType(PTypeEnumeration.sstring)),
                new NamedType("age", new PType(PTypeEnumeration.integer)));
            PType tp_photo_doc = new PTypeRecord(
                new NamedType("code", new PType(PTypeEnumeration.sstring)),
                new NamedType("name", new PType(PTypeEnumeration.sstring)));
            PType tp_reflection = new PTypeRecord(
                new NamedType("code", new PType(PTypeEnumeration.sstring)), // Может не быть
                new NamedType("reflected", new PType(PTypeEnumeration.sstring)), // ссылки на коды
                new NamedType("in_doc", new PType(PTypeEnumeration.sstring)));
            tab_person = new TableView(path + "person", tp_person);
            tab_photo_doc = new TableView(path + "photo_doc", tp_photo_doc);
            tab_reflection = new TableView(path + "reflection", tp_reflection);
            // Индексы: Персона
            Func<object, string> person_code_keyproducer = v => (string)((object[])((object[])v)[1])[0];
            ind_arr_person = new IndexHalfkeyImmutable<string>(path + "person_ind")
            {
                Table = tab_person,
                KeyProducer = person_code_keyproducer,
                HalfProducer = s => s.GetHashCode(),
                Scale = null
            };
            ind_arr_person.Scale = new ScaleCell(path + "person_ind") { IndexCell = ind_arr_person.IndexCell };
            index_person = new IndexDynamic<string, IndexHalfkeyImmutable<string>>(true)
            {
                Table = tab_person,
                IndexArray = ind_arr_person,
                KeyProducer = person_code_keyproducer
            };
            // Индексы - документ
            Func<object, string> photo_doc_code_keyproducer = v => (string)((object[])((object[])v)[1])[0];
            ind_arr_photo_doc = new IndexHalfkeyImmutable<string>(path + "photo_doc_ind")
            {
                Table = tab_photo_doc,
                KeyProducer = photo_doc_code_keyproducer,
                HalfProducer = s => s.GetHashCode(),
                Scale = null
            };
            ind_arr_photo_doc.Scale = new ScaleCell(path + "photo_doc_ind") { IndexCell = ind_arr_photo_doc.IndexCell };
            index_photo_doc = new IndexDynamic<string, IndexHalfkeyImmutable<string>>(true)
            {
                Table = tab_photo_doc,
                IndexArray = ind_arr_photo_doc,
                KeyProducer = photo_doc_code_keyproducer
            };
            // Индекс - reflection-reflected
            Func<object, string> reflected_keyproducer = v => (string)((object[])((object[])v)[1])[1];
            ind_arr_reflected = new IndexHalfkeyImmutable<string>(path + "reflected_ind")
            {
                Table = tab_reflection,
                KeyProducer = reflected_keyproducer,
                HalfProducer = s => s.GetHashCode(),
                Scale = null
            };
            ind_arr_reflected.Scale = new ScaleCell(path + "reflected_ind") { IndexCell = ind_arr_reflected.IndexCell };
            index_reflected = new IndexDynamic<string, IndexHalfkeyImmutable<string>>(false)
            {
                Table = tab_reflection,
                IndexArray = ind_arr_reflected,
                KeyProducer = reflected_keyproducer
            };
            // Индекс - reflection-in_doc
            Func<object, string> in_doc_keyproducer = v => (string)((object[])((object[])v)[1])[2];
            ind_arr_in_doc = new IndexHalfkeyImmutable<string>(path + "in_doc_ind")
            {
                Table = tab_reflection,
                KeyProducer = in_doc_keyproducer,
                HalfProducer = s => s.GetHashCode(),
                Scale = null
            };
            ind_arr_in_doc.Scale = new ScaleCell(path + "in_doc_ind") { IndexCell = ind_arr_in_doc.IndexCell };
            index_in_doc = new IndexDynamic<string, IndexHalfkeyImmutable<string>>(false)
            {
                Table = tab_reflection,
                IndexArray = ind_arr_in_doc,
                KeyProducer = in_doc_keyproducer
            };
        }
        public void Clear() { tab_person.Clear(); tab_photo_doc.Clear(); tab_reflection.Clear(); }
        public void Warmup() { tab_person.Warmup(); tab_photo_doc.Warmup(); tab_reflection.Warmup(); }
        public void Build(IEnumerable<XElement> records)
        {
            this.Clear();
            tab_person.Fill(new object[0]);
            tab_photo_doc.Fill(new object[0]);
            tab_reflection.Fill(new object[0]);
            foreach (XElement rec in records)
            {
                string code = rec.Attribute("id").Value;
                if (rec.Name == "person")
                {
                    string name = rec.Element("name").Value;
                    int age = Int32.Parse(rec.Element("age").Value);
                    tab_person.AppendValue(new object[] { code, name, age });
                }
                else if (rec.Name == "photo_doc")
                {
                    string name = rec.Element("name").Value;
                    tab_photo_doc.AppendValue(new object[] { code, name });
                }
                else if (rec.Name == "reflection")
                {
                    string reflected = rec.Element("reflected").Attribute("ref").Value;
                    string in_doc = rec.Element("in_doc").Attribute("ref").Value;
                    tab_reflection.AppendValue(new object[] { code, reflected, in_doc });
                }
            }
            // Построение индексов
            ind_arr_person.Build();
            ind_arr_photo_doc.Build();
            ind_arr_reflected.Build();
            ind_arr_in_doc.Build();
        }
        public object[] GetPersonById(string id)
        {
            var ob = index_person.GetAllByKey(id)
                .Select(ent => ((object[])ent.Get())[1])
                .FirstOrDefault();
            return (object[])ob;
        }
        public object[] GetPhoto_docById(string id)
        {
            var ob = index_photo_doc.GetAllByKey(id)
                .Select(ent => ((object[])ent.Get())[1])
                .FirstOrDefault();
            return (object[])ob;
        }
        /// <summary>
        /// По заданному коду персоны, выдет множество отношений, в которых код присутствует на соответстующем месте. 
        /// </summary>
        /// <param name="code"></param>
        public IEnumerable<object[]> GetReflectionsByReflected(string id)
        {
            var query = index_reflected.GetAllByKey(id)
                .Select(ent => ((object[])ent.Get())[1])
                .Select(re => (string)((object[])re)[2])
                .Select(c => this.GetPhoto_docById(c))
                ;
            return query;
        }
    }
}
