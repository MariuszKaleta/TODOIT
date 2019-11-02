using System.Collections.Generic;
using System.Threading.Tasks;

namespace GraphQlHelper
{
    public interface IRepository<TType, TId , in TModel> : IRepository<TType, TId>
    where TType : IBaseElement<TId>
    {
        Task<TType> Create(TModel model);

        Task<TType> Update(TType obj, TModel model);

        void Delete(TType obj, bool saveChanges);
    }
    
    public interface IRepository<TType, in TId>
    where TType : IBaseElement<TId>
    {
        Task<TType> Get(TId id);
        Task<TType[]> Get(IEnumerable<TId> ids);

        Task<TType[]> Get(string name = null, int? start = null, int? count = null);
    }
}
