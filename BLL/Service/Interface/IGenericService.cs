using BLL.Model.ResponseModel;

namespace BLL.Service.Interface;

public interface IGenericService<T> where T : class
{
    public Task<ServiceResponse<T>>  GetAsync(int id);
    public Task<ServiceResponse<T>> CreateAsync(T entity);
    public Task<ServiceResponse<T>> UpdateAsync(T entity);
    public Task<ServiceResponse<T>> DeleteAsync(int id);
}