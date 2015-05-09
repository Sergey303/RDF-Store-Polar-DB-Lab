using Task15UniversalIndex;

namespace RDFCommon.OVns
{
    public class NodeGeneratorInt:NodeGenerator
    {
        public NameTableUniversal coding_table;
        public NodeGeneratorInt(string path, bool empty)
        {     
            coding_table=new NameTableUniversal(path);
            if (empty)
            {
                Build();
            }
            else
            {
                SpecialTypes = new SpecialTypesClass(this);
            }
        }
        public void Build()
        {
            coding_table.Clear();
            coding_table.Fill(new string[0]);
            coding_table.BuildIndexes();
            coding_table.InsertPortion(SpecialTypesClass.GetAll());
            coding_table.BuildScale();
            SpecialTypes = new SpecialTypesClass(this);
        }
  
        public override ObjectVariants GetUri(string uri)
        {
           
            int code=coding_table.GetCodeByString(uri);
            if (code == -1)
                return new OV_iri(uri);
            else return new OV_iriint(code, coding_table.GetStringByCode);
        }

        public override ObjectVariants AddIri(string iri)
        {
            return new OV_iriint(coding_table.Add(iri), coding_table.GetStringByCode); ;
        }


        public override ObjectVariants CreateLiteralOtherType(string p, string typeUriNode)
        {
            return new OV_typedint(p, coding_table.Add(typeUriNode), coding_table.GetStringByCode);
        }

       
           public ObjectVariants GetCoded(int code)
        {
            return new OV_iriint(code, coding_table.GetStringByCode);
        }

     

     
    }
    
}
