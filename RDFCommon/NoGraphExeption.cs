using System;

namespace RDFCommon
{
    public class NoGraphExeption : Exception
    {
        private readonly string name;

        public NoGraphExeption(string name)
        {
            this.name = name;
        }
    }
}