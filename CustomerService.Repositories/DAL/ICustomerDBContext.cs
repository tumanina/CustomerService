using CustomerService.Repositories.Entities;
using Microsoft.EntityFrameworkCore;
using System;

namespace CustomerService.Repositories.DAL
{
    public interface ICustomerDBContext: IDisposable
    {
        DbSet<Client> Client { get; set; }
        DbSet<Session> Session { get; set; }
        DbSet<Token> Token { get; set; }
        DbSet<TEntity> Set<TEntity>() where TEntity : class;
        int SaveChanges();
    }
}
