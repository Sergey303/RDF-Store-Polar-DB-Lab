namespace RDFCommon
{
    public struct Triple<Ts, Tp, To>
    {
        private readonly Ts subject;
        private readonly Tp predicate;
        private readonly To @object;

        public Triple(Ts s, Tp p, To o) 
        {
            subject = s;
            predicate = p;
            @object = o;
        }
        public Ts Subject { get { return subject; } }
        public Tp Predicate { get { return predicate; } }
        public To Object { get { return @object; } }
    }
}
