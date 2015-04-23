﻿using System;

namespace RDFTripleStore
{
    /// <summary>
    /// Альтернатива <see cref="System.Collections.IEnumerable<T>"/> 
    /// Применимо, где нельзя порождать поток.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IGenerator<T>
    {
        void Start(Action<T> onGenerate);
    }
}