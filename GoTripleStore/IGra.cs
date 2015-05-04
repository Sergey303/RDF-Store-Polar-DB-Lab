using System;
using System.Collections.Generic;
using System.Linq;
using RDFCommon;
using RDFCommon.OVns;
//using PolarDB;

namespace GoTripleStore
{
    public interface IGra<TPointer> // Вместо IGraph
    {
        // Конструкции триплета:
        //   Tuple<string, string, ObjectVariants>, 
        //   указатель TPointer, 
        //   объектное представление object[] в котором три элемента: субъект, предикат, объект
        
        // Неуказанный в интерфейсе конструктор делает коннектор с сущестущей базой данных или создает ее пусто вариант
        
        // Метод Build(...) строит новую базу или (полностью) перестраивает старую базу на основе потока триплетов  
        void Build(IEnumerable<Tuple<string, string, ObjectVariants>> triples);
        // Функция преобразует указатель в триплет в объектной форме
        Func<TPointer, object[]> Dereference { get; } // выдает триплет в объектной форме, поля могут быть кодированными
        
        // Кодирование и декодирование IRI и ObjectVariant. Если не требуется, производится приведение 
        object CodeIRI(string iri);
        string DecodeIRI(object oiri);
        object CodeOV(ObjectVariants ov);
        ObjectVariants DecodeOV(object oov);

        // Главное:
        IEnumerable<TPointer> GetTriples();
        IEnumerable<TPointer> GetTriplesWithSubject(object subj);
        IEnumerable<TPointer> GetTriplesWithSubjectPredicate(object subj, object pred);
        bool Contains(object subj, object pred, object obj);
        IEnumerable<TPointer> GetTriplesWithPredicate(object pred);
        IEnumerable<TPointer> GetTriplesWithPredicateObject(object pred, object obj);
        IEnumerable<TPointer> GetTriplesWithObject(object obj);
        
    }
}
