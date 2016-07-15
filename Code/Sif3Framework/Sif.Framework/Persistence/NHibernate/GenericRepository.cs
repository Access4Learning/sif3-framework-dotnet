/*
 * Copyright 2015 Systemic Pty Ltd
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
using Sif.Framework.Model.Persistence;
using System;
using System.Collections.Generic;

namespace Sif.Framework.Persistence.NHibernate
{

    /// <see cref="Sif.Framework.Persistence.IGenericRepository{T,PK}">IGenericRepository</see>
    public class GenericRepository<T, PK> : IGenericRepository<T, PK> where T : class, IPersistable<PK>, new()
    {
        protected IBaseSessionFactory sessionFactory;

        /// <summary>
        /// Instantiate this class using a session factory for NHibernate.
        /// </summary>
        /// <param name="sessionFactory">Session factory for NHibernate.</param>
        public GenericRepository(IBaseSessionFactory sessionFactory)
        {
            this.sessionFactory = sessionFactory;
        }

        /// <see cref="Sif.Framework.Persistence.IGenericRepository{T,PK}.Delete(PK)">Delete</see>
        public virtual void Delete(PK id)
        {

            if (id == null)
            {
                throw new ArgumentNullException("id");
            }

            using (ISession session = sessionFactory.OpenSession())
            {

                using (ITransaction transaction = session.BeginTransaction())
                {
                    T obj = session.Get<T>(id);

                    if (obj != null)
                    {
                        session.Delete(obj);
                        transaction.Commit();
                    }

                }

            }

        }

        /// <see cref="Sif.Framework.Persistence.IGenericRepository{T,PK}.Delete(T)">Delete</see>
        public virtual void Delete(T obj)
        {

            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            using (ISession session = sessionFactory.OpenSession())
            {

                using (ITransaction transaction = session.BeginTransaction())
                {
                    session.Delete(obj);
                    transaction.Commit();
                }

            }

        }

        /// <see cref="Sif.Framework.Persistence.IGenericRepository{T,PK}.Delete(IEnumerable<T>)">Delete</see>
        public virtual void Delete(IEnumerable<T> objs)
        {

            if (objs == null)
            {
                throw new ArgumentNullException("objs");
            }

            using (ISession session = sessionFactory.OpenSession())
            {

                using (ITransaction transaction = session.BeginTransaction())
                {

                    foreach (T obj in objs)
                    {
                        session.Delete(obj);
                    }

                    transaction.Commit();
                }

            }

        }

        /// <see cref="Sif.Framework.Persistence.IGenericRepository{T,PK}.Retrieve(PK)">Retrieve</see>
        public virtual T Retrieve(PK id)
        {

            using (ISession session = sessionFactory.OpenSession())
            {
                return session.Get<T>(id);
            }

        }

        /// <see cref="Sif.Framework.Persistence.IGenericRepository{T,PK}.Retrieve()">Retrieve</see>
        public virtual ICollection<T> Retrieve()
        {

            using (ISession session = sessionFactory.OpenSession())
            {
                return session.CreateCriteria(typeof(T)).List<T>();
            }

        }

        /// <see cref="Sif.Framework.Persistence.IGenericRepository{T,PK}.Retrieve(T)">Retrieve</see>
        public virtual ICollection<T> Retrieve(T obj)
        {

            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            using (ISession session = sessionFactory.OpenSession())
            {
                Example example = Example.Create(obj).EnableLike(MatchMode.Anywhere).ExcludeZeroes().IgnoreCase();
                return session.CreateCriteria(typeof(T)).Add(example).List<T>();
            }

        }

        /// <see cref="Sif.Framework.Persistence.IGenericRepository{T,PK}.Retrieve(System.Int32, System.Int32)">Retrieve</see>
        public virtual ICollection<T> Retrieve(int pageIndex, int pageSize)
        {

            using (ISession session = sessionFactory.OpenSession())
            {
                return session.QueryOver<T>().Skip(pageIndex * pageSize).Take(pageSize).List<T>();
            }

        }

        /// <see cref="Sif.Framework.Persistence.IGenericRepository{T,PK}.Save(T)">Save</see>
        public virtual PK Save(T obj)
        {

            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            PK objId;

            using (ISession session = sessionFactory.OpenSession())
            {

                using (ITransaction transaction = session.BeginTransaction())
                {

                    if (!EqualityComparer<PK>.Default.Equals(obj.Id, default(PK)))
                    {
                        session.Update(obj);
                        objId = obj.Id;
                    }
                    else
                    {
                        objId = (PK)session.Save(obj);
                    }

                    transaction.Commit();
                }

            }

            return objId;
        }

        /// <see cref="Sif.Framework.Persistence.IGenericRepository{T,PK}.Save(IEnumerable<T>)">Save</see>
        public virtual void Save(IEnumerable<T> objs)
        {

            if (objs == null)
            {
                throw new ArgumentNullException("objs");
            }

            using (ISession session = sessionFactory.OpenSession())
            {

                using (ITransaction transaction = session.BeginTransaction())
                {

                    foreach (T obj in objs)
                    {
                        session.SaveOrUpdate(obj);
                    }

                    transaction.Commit();
                }

            }

        }

        public virtual bool Exists(PK id)
        {
            using (ISession session = sessionFactory.OpenSession())
            {
                return session.Get<T>(id) != null;
            }
        }

    }

}
