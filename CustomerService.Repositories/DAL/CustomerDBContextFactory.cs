using Microsoft.EntityFrameworkCore;

namespace CustomerService.Repositories.DAL
{
    public class CustomerDBContextFactory : ICustomerDBContextFactory
    {
        private readonly DbContextOptionsBuilder<CustomerDBContext> _options;

        public CustomerDBContextFactory(DbContextOptionsBuilder<CustomerDBContext> options)
        {
            _options = options;
        }

        public ICustomerDBContext CreateDBContext()
        {
            return new CustomerDBContext(_options.Options);
        }
    }
}
