using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace GraphQlHelper
{
    public interface IRepository<TType, in TId , in TModel> : IRepository<TType, TId>
    {
        Task<TType> Create(TModel model);

        Task<TType> Update(TType obj, TModel model);

        void Delete(TType obj, bool saveChanges);
    }
    
    public interface IRepository<TType, in TId>
    {
        Task<TType> Get(TId id, params Expression<Func<TType, object>>[] navigationPropertyPaths);

        Task<TType[]> Get(IEnumerable<TId> ids, params Expression<Func<TType, object>>[] navigationPropertyPaths);

        Task<TType[]> Get(string name = null, int? start = null, int? count = null, params Expression<Func<TType, object>>[] navigationPropertyPaths);
    }
}
