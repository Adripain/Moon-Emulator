using System;

namespace Moon.Database.Interfaces
{
    public interface IQueryAdapter : IRegularQueryAdapter, IDisposable
    {
        long InsertQuery();
        void RunQuery();
    }
}