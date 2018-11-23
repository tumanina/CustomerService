namespace CustomerService.Repositories.DAL
{
    public interface ICustomerDBContextFactory
    {
        ICustomerDBContext CreateDBContext();
    }
}
