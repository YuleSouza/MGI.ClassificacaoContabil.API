using System;

namespace Service.Helper
{
    public class PredicateFactory<T> where T : class
    {
        public PredicateFactory(T classe)
        {
        }
        public Func<T,bool> Create() 
        {
            return new Func<T, bool>(_ => true);
        }        
    }
}
