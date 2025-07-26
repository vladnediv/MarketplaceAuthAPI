namespace DAL.Repository.Interface;

public interface IGenericRepository<T> where T : class
{
    public Task<T> Create(T entity);
    public Task<T> GetById(int id);
    public Task Update(T entity);
    public Task Delete(T entity);
    public Task SaveChangesAsync();
}