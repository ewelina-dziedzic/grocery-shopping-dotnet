namespace GroceryShopping.Core;

public interface IRepository<in T>
{
    Task AddAsync(T model);
}