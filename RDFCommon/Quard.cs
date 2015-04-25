namespace RDFCommon
{
    public struct Quard<Ts, Tp, To, Tg>
    {
        private readonly Ts subject;
        private readonly Tp predicate;
        private readonly To @object;
        private readonly Tg g;

        public Quard(Ts s, Tp p, To o, Tg g) 
        {
            subject = s;
            predicate = p;
            @object = o;
            this.g = g;
        }
        public Ts Subject { get { return subject; } }
        public Tp Predicate { get { return predicate; } }
        public To Object { get { return @object; } }

        public Tg G
        {
            get { return g; }
        }
    }
}
