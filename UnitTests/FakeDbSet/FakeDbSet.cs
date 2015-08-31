using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace FakeDbSet
{
    public class FakeDbSet<T> : IDbSet<T>, IDbAsyncEnumerable<T> where T : class
    {
        private readonly IQueryable queryable;

        public FakeDbSet()
        {
            Local = new ObservableCollection<T>();
            queryable = Local.AsQueryable();
        }

        public int Count
        {
            get { return Local.Count; }
        }

        public IDbAsyncEnumerator<T> GetAsyncEnumerator()
        {
            return new AsyncEnumeratorWrapper<T>(Local.GetEnumerator());
        }

        IDbAsyncEnumerator IDbAsyncEnumerable.GetAsyncEnumerator()
        {
            return GetAsyncEnumerator();
        }

        public virtual T Find(params object[] keyValues)
        {
            throw new NotImplementedException("Derive from FakeDbSet<T> and override Find");
        }

        public T Add(T item)
        {
            Local.Add(item);
            return item;
        }

        public T Remove(T item)
        {
            Local.Remove(item);
            return item;
        }

        public T Attach(T item)
        {
            Local.Add(item);
            return item;
        }

        public T Create()
        {
            return Activator.CreateInstance<T>();
        }

        public TDerivedEntity Create<TDerivedEntity>() where TDerivedEntity : class, T
        {
            return Activator.CreateInstance<TDerivedEntity>();
        }

        public ObservableCollection<T> Local { get; }

        Type IQueryable.ElementType
        {
            get { return queryable.ElementType; }
        }

        Expression IQueryable.Expression
        {
            get { return queryable.Expression; }
        }

        IQueryProvider IQueryable.Provider
        {
            get { return new AsyncQueryProviderWrapper<T>(queryable.Provider); }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Local.GetEnumerator();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return Local.GetEnumerator();
        }

        public Task<T> FindAsync(CancellationToken cancellationToken, params object[] keyValues)
        {
            throw new NotImplementedException();
        }

        public T Detach(T item)
        {
            Local.Remove(item);
            return item;
        }
    }
}