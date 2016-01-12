﻿using System;
using System.Threading;
using System.Threading.Tasks;
using System.Data.Entity;
using Microsoft.Framework.Logging;
using StarterKit.DataAccess.Exceptions;
using StarterKit.DataAccess.Repositories.Base;

namespace StarterKit.DataAccess.Uow
{
    public abstract class UnitOfWorkBase<TContext> : IUnitOfWorkBase where TContext : DbContext
    {
        protected internal UnitOfWorkBase(ILogger logger, TContext context, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _context = context;
            _serviceProvider = serviceProvider;
        }

        protected readonly ILogger _logger;
        protected TContext _context;
        protected readonly IServiceProvider _serviceProvider;

        public int SaveChanges()
        {
            CheckDisposed();
            return _context.SaveChanges();
        }

        public Task<int> SaveChangesAsync()
        {
            CheckDisposed();
            return _context.SaveChangesAsync();
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            CheckDisposed();
            return _context.SaveChangesAsync(cancellationToken);
        }

        public TRepository GetRepository<TRepository>()
        {
            CheckDisposed();
			var repositoryType = typeof(TRepository);
            var repository = (TRepository)_serviceProvider.GetService(repositoryType);
			if ( repository == null )
			{
				throw new RepositoryNotFoundException(repositoryType.Name, String.Format("Repository {0} niet gevonden in de IOC container. Controleer of deze geregistreerd is.", repositoryType.Name));
			}

            ((IRepositoryInjection<TContext>)repository).SetContext(_context);
            return repository;
        }

        #region IDisposable Implementation

        protected bool _isDisposed;

        protected void CheckDisposed()
        {
            if ( _isDisposed ) throw new ObjectDisposedException("The UnitOfWork is already disposed and cannot be used anymore.");
        }

        protected virtual void Dispose(bool disposing)
        {
            if ( !_isDisposed )
            {
                if ( disposing )
                {
                    if ( _context != null )
                    {
                        _context.Dispose();
                        _context = null;
                    }
                }
            }
            _isDisposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~UnitOfWorkBase()
        {
            Dispose(false);
        }

        #endregion
    }
}
