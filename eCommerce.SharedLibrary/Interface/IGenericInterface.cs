using eCommerce.SharedLibrary.Response;
using System.Linq.Expressions;

namespace eCommerce.SharedLibrary.Interface
{
    public interface IGenericInterface<T> where T : class
    {
        Task<Responses> CreateAsync(T Entity);
        Task<Responses> UpdateAsync(T Entity);
        Task<Responses> DeleteAsync(T Entity);
        Task<Responses> GetAllAsync();
        Task<T> FindByIdAsync(int id);
        Task<T> GetByAsync(Expression<Func<T, bool>> predicate);
    }
}
