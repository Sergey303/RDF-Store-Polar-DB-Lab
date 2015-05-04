using System;

namespace RDFCommon
{
    public interface IAllNode :IComparable
    {
        dynamic Content { get; }
    }
}