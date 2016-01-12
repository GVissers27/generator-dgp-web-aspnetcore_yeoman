﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace StarterKit.DataAccess.Uow
{
    public interface IUnitOfWorkBase : IDisposable
    {
        int SaveChanges();
        Task<int> SaveChangesAsync();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);

        TRepository GetRepository<TRepository>();
    }
}
