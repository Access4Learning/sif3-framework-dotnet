/*
 * Copyright 2014 Systemic Pty Ltd
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

    public class GenericRepository<T, PK> : IGenericRepository<T, PK> where T : IPersistable<PK>, new()
    {

        /// <see cref="Sif.Framework.Persistence.IGenericRepository{T}.Delete(PK)">Delete</see>
        public virtual void Delete(PK objId)
        {

            if (objId == null)
            {
                throw new ArgumentNullException("objId");
            }

            using (ISession session = NHibernateHelper.OpenSession())
            {

                using (ITransaction transaction = session.BeginTransaction())
                {
                    T obj = session.Get<T>(objId);

                    if (obj != null)
                    {
                        session.Delete(obj);
                        transaction.Commit();
                    }

                }

            }

        }

        /// <see cref="Sif.Framework.Persistence.IGenericRepository{T}.Delete(T)">Delete</see>
        public virtual void Delete(T obj)
        {

            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            using (ISession session = NHibernateHelper.OpenSession())
            {

                using (ITransaction transaction = session.BeginTransaction())
                {
                    session.Delete(obj);
                    transaction.Commit();
                }

            }

        }

        /// <see cref="Sif.Framework.Persistence.IGenericRepository{T}.Delete(ICollection<T>)">Delete</see>
        public virtual void Delete(ICollection<T> objs)
        {

            if (objs == null)
            {
                throw new ArgumentNullException("objs");
            }

            using (ISession session = NHibernateHelper.OpenSession())
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

        /// <see cref="Sif.Framework.Persistence.IGenericRepository{T}.Retrieve(PK)">Retrieve</see>
        public virtual T Retrieve(PK objId)
        {

            using (ISession session = NHibernateHelper.OpenSession())
            {
                return session.Get<T>(objId);
            }

        }

        /// <see cref="Sif.Framework.Persistence.IGenericRepository{T}.Retrieve(T)">Retrieve</see>
        public virtual ICollection<T> Retrieve(T obj)
        {

            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            using (ISession session = NHibernateHelper.OpenSession())
            {
                Example example = Example.Create(obj).EnableLike(MatchMode.Anywhere).ExcludeZeroes().IgnoreCase();
                return session.CreateCriteria(typeof(T)).Add(example).List<T>();
            }

        }

        /// <see cref="Sif.Framework.Persistence.IGenericRepository{T}.Retrieve()">Retrieve</see>
        public virtual ICollection<T> Retrieve()
        {

            using (ISession session = NHibernateHelper.OpenSession())
            {
                return session.CreateCriteria(typeof(T)).List<T>();
            }

        }

        /// <see cref="Sif.Framework.Persistence.IGenericRepository{T}.Save(T)">Save</see>
        public virtual PK Save(T obj)
        {

            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            PK objId;

            using (ISession session = NHibernateHelper.OpenSession())
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

        /// <see cref="Sif.Framework.Persistence.IGenericRepository{T}.Save(ICollection<T>)">Save</see>
        public virtual void Save(ICollection<T> objs)
        {

            if (objs == null)
            {
                throw new ArgumentNullException("objs");
            }

            using (ISession session = NHibernateHelper.OpenSession())
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

    }

}
