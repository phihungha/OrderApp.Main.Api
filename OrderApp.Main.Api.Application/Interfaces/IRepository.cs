namespace OrderApp.Main.Api.Application.Interfaces
{
    public interface IRepository<T>
        where T : class
    {
        void Add(T entity);
        void Delete(T entity);
    }
}
