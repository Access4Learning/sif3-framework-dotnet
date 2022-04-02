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
using NHibernate.Criterion;
using Sif.Framework.Persistence;
using System;
using System.Collections.Generic;
using Tardigrade.Framework.Models.Domain;

namespace Sif.Framework.NHibernate.Persistence
{
    /// <inheritdoc />
    public class GenericRepository<TEntity, TKey> : IGenericRepository<TEntity, TKey>
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

        /// <inheritdoc cref="IGenericRepository{TEntity, TKey}.Delete(TKey)" />
        public virtual void Delete(TKey id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));

            using (ISession session = SessionFactory.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    var item = session.Get<TEntity>(id);

                    if (item != null)
                    {
                        session.Delete(item);
                        transaction.Commit();
                    }
                }
            }
        }

        /// <inheritdoc cref="IGenericRepository{TEntity, TKey}.Delete(TEntity)" />
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

        /// <inheritdoc cref="IGenericRepository{TEntity, TKey}.Delete(IEnumerable{TEntity})" />
        public virtual void Delete(IEnumerable<TEntity> items)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));

            using (ISession session = SessionFactory.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    foreach (TEntity item in items)
                    {
                        session.Delete(item);
                    }

                    transaction.Commit();
                }
            }
        }

        /// <inheritdoc cref="IGenericRepository{TEntity, TKey}.Retrieve(TKey)" />
        public virtual TEntity Retrieve(TKey id)
        {
            using (ISession session = SessionFactory.OpenSession())
            {
                return session.Get<TEntity>(id);
            }
        }

        /// <inheritdoc cref="IGenericRepository{TEntity, TKey}.Retrieve()" />
        public virtual ICollection<TEntity> Retrieve()
        {
            using (ISession session = SessionFactory.OpenSession())
            {
                return session.CreateCriteria(typeof(TEntity)).List<TEntity>();
            }
        }

        /// <inheritdoc cref="IGenericRepository{TEntity, TKey}.Retrieve(TEntity)" />
        public virtual ICollection<TEntity> Retrieve(TEntity item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            using (ISession session = SessionFactory.OpenSession())
            {
                Example example = Example.Create(item).EnableLike(MatchMode.Anywhere).ExcludeZeroes().IgnoreCase();

                return session.CreateCriteria(typeof(TEntity)).Add(example).List<TEntity>();
            }
        }

        /// <inheritdoc cref="IGenericRepository{TEntity, TKey}.Retrieve(int, int)" />
        public virtual ICollection<TEntity> Retrieve(int pageIndex, int pageSize)
        {
            using (ISession session = SessionFactory.OpenSession())
            {
                return session.QueryOver<TEntity>().Skip(pageIndex * pageSize).Take(pageSize).List<TEntity>();
            }
        }

        /// <inheritdoc cref="IGenericRepository{TEntity, TKey}.Save(TEntity)" />
        public virtual TKey Save(TEntity item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            TKey id;

            using (ISession session = SessionFactory.OpenSession())
            {
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
            }

            return id;
        }

        /// <inheritdoc cref="IGenericRepository{TEntity, TKey}.Save(IEnumerable{TEntity})" />
        public virtual void Save(IEnumerable<TEntity> items)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));

            using (ISession session = SessionFactory.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    foreach (TEntity item in items)
                    {
                        session.SaveOrUpdate(item);
                    }

                    transaction.Commit();
                }
            }
        }

        /// <inheritdoc cref="IGenericRepository{TEntity,TKey}.Exists" />
        public virtual bool Exists(TKey id)
        {
            using (ISession session = SessionFactory.OpenSession())
            {
                return session.Get<TEntity>(id) != null;
            }
        }
    }
}