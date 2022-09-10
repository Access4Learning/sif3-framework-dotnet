/*
 * Copyright 2022 Systemic Pty Ltd
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Tardigrade.Framework.Models.Domain;
using Tardigrade.Framework.Models.Persistence;
using Tardigrade.Framework.Persistence;

namespace Sif.Framework.NHibernate.Persistence
{
    /// <inheritdoc />
    public class GenericRepository<TEntity, TKey> : IRepository<TEntity, TKey>
        where TEntity : class, IHasUniqueIdentifier<TKey>, new()
    {
        protected IBaseSessionFactory SessionFactory;

        /// <summary>
        /// Instantiate this class using a session factory for NHibernate.
        /// </summary>
        /// <param name="sessionFactory">Session factory for NHibernate.</param>
        public GenericRepository(IBaseSessionFactory sessionFactory)
        {
            SessionFactory = sessionFactory;
        }

        /// <inheritdoc cref="IReadOnlyRepository{TEntity,TKey}.Count(Expression{Func{TEntity, bool}})" />
        public int Count(Expression<Func<TEntity, bool>> filter = null)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc cref="IReadOnlyRepository{TEntity,TKey}.CountAsync(Expression{Func{TEntity, bool}}, CancellationToken)" />
        public Task<int> CountAsync(Expression<Func<TEntity, bool>> filter = null, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc cref="IRepository{TEntity, TKey}.Create(TEntity)" />
        public TEntity Create(TEntity item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            using (ISession session = SessionFactory.OpenSession())
            {
                TKey id;

                using (ITransaction transaction = session.BeginTransaction())
                {
                    if (!EqualityComparer<TKey>.Default.Equals(item.Id, default(TKey)))
                    {
                        session.Update(item);
                        id = item.Id;
                    }
                    else
                    {
                        id = (TKey)session.Save(item);
                    }

                    transaction.Commit();
                }

                return session.Get<TEntity>(id);
            }
        }

        /// <inheritdoc cref="IRepository{TEntity, TKey}.CreateAsync(TEntity, CancellationToken)" />
        public Task<TEntity> CreateAsync(TEntity item, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc cref="IRepository{TEntity, TKey}.Delete(TEntity)" />
        public virtual void Delete(TEntity item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            using (ISession session = SessionFactory.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    session.Delete(item);
                    transaction.Commit();
                }
            }
        }

        /// <inheritdoc cref="IRepository{TEntity, TKey}.DeleteAsync(TEntity, CancellationToken)" />
        public Task DeleteAsync(TEntity item, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc cref="IRepository{TEntity,TKey}.Exists(TKey)" />
        public virtual bool Exists(TKey id)
        {
            using (ISession session = SessionFactory.OpenSession())
            {
                return session.Get<TEntity>(id) != null;
            }
        }

        /// <inheritdoc cref="IRepository{TEntity,TKey}.ExistsAsync(TKey, CancellationToken)" />
        public Task<bool> ExistsAsync(TKey id, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc cref="IReadOnlyRepository{TEntity,TKey}.Retrieve(Expression{Func{TEntity,bool}}, PagingContext, Func{IQueryable{TEntity},IOrderedQueryable{TEntity}}, Expression{Func{TEntity,object}}[])" />
        public IEnumerable<TEntity> Retrieve(
            Expression<Func<TEntity, bool>> filter = null,
            PagingContext pagingContext = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> sortCondition = null,
            params Expression<Func<TEntity, object>>[] includes)
        {
            using (ISession session = SessionFactory.OpenSession())
            {
                if (pagingContext == null)
                {
                    return session.CreateCriteria(typeof(TEntity)).List<TEntity>();
                }

                var pageIndex = (int)pagingContext.PageIndex;
                var pageSize = (int)pagingContext.PageSize;

                return session.QueryOver<TEntity>().Skip(pageIndex * pageSize).Take(pageSize).List<TEntity>();
            }
        }

        /// <inheritdoc cref="IReadOnlyRepository{TEntity,TKey}.Retrieve(TKey, Expression{Func{TEntity,object}}[])" />
        public TEntity Retrieve(TKey id, params Expression<Func<TEntity, object>>[] includes)
        {
            using (ISession session = SessionFactory.OpenSession())
            {
                return session.Get<TEntity>(id);
            }
        }

        /// <inheritdoc cref="IReadOnlyRepository{TEntity,TKey}.RetrieveAsync(Expression{Func{TEntity,bool}}, PagingContext, Func{IQueryable{TEntity},IOrderedQueryable{TEntity}}, CancellationToken, Expression{Func{TEntity,object}}[])" />
        public Task<IEnumerable<TEntity>> RetrieveAsync(
            Expression<Func<TEntity, bool>> filter = null,
            PagingContext pagingContext = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> sortCondition = null,
            CancellationToken cancellationToken = new CancellationToken(),
            params Expression<Func<TEntity, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc cref="IReadOnlyRepository{TEntity,TKey}.RetrieveAsync(TKey, CancellationToken, Expression{Func{TEntity,object}}[])" />
        public Task<TEntity> RetrieveAsync(
            TKey id,
            CancellationToken cancellationToken = new CancellationToken(),
            params Expression<Func<TEntity, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc cref="IRepository{TEntity, TKey}.Update(TEntity)" />
        public void Update(TEntity item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            using (ISession session = SessionFactory.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    if (!EqualityComparer<TKey>.Default.Equals(item.Id, default(TKey)))
                    {
                        session.Update(item);
                    }
                    else
                    {
                        _ = (TKey)session.Save(item);
                    }

                    transaction.Commit();
                }
            }
        }

        /// <inheritdoc cref="IRepository{TEntity, TKey}.UpdateAsync(TEntity, CancellationToken)" />
        public Task UpdateAsync(TEntity item, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }
    }
}