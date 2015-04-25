namespace RDFCommon
{
    public interface ILiteralNode :IObjectNode
    {  
        dynamic Content { get;  }
        string DataType { get; }
    }
}