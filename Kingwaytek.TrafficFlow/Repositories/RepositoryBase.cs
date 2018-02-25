using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Kingwaytek.TrafficFlow
{
    public abstract class RepositoryBase<TDbContext, TModel> where TModel : class, new()
    {
        protected DbContext Context;
        protected DbSet<TModel> DbSet;

        private readonly bool _isIEditColumn;
        private readonly bool _isIDeleted;
        private readonly bool _isIEnabled;
        private readonly bool _isIdentityGuid;

        protected RepositoryBase()
        {
            var factory = new DbContextFactory();
            Context = factory.GetDbContext<TDbContext>();
            DbSet = Context.Set<TModel>();
            var interfaces = typeof(TModel).GetInterfaces();
            _isIEditColumn = interfaces.Contains(typeof(IEditColumn));
            _isIDeleted = interfaces.Contains(typeof(IDeleted));
            _isIEnabled = interfaces.Contains(typeof(IEnabled));
            _isIdentityGuid = interfaces.Contains(typeof(IIdentity<Guid>));
        }

        public virtual IQueryable<TModel> GetAll()
        {
            var query = DbSet;

            return query;
        }

        public virtual IQueryable<TModel> GetAvailable(bool ignoreEnabled = false)
        {
            var query = DbSet.AsQueryable();

            if (_isIDeleted)
            {
                query = ((IQueryable<IDeleted>)query)
                            .Where(p => p.Deleted == false)
                            .Cast<TModel>();
            }

            if (_isIEnabled && ignoreEnabled == false)
            {
                query = ((IQueryable<IEnabled>)query)
                            .Where(p => p.Enabled == true)
                            .Cast<TModel>();
            }

            return query;
        }

        public virtual TModel Add(TModel model)
        {
            if (_isIEditColumn)
            {
                var editModel = (IEditColumn)model;
                editModel.CreateTime = DateTime.Now;
            }
            if (_isIdentityGuid)
            {
                var identityModel = (IIdentity<Guid>)model;
                identityModel.Id = (identityModel.Id == Guid.Empty ? Guid.NewGuid() : identityModel.Id);
            }

            model = DbSet.Add(model);
            Context.SaveChanges();

            return model;
        }

        public virtual IEnumerable<TModel> AddRange(IEnumerable<TModel> models)
        {
            if (_isIEditColumn)
            {
                models = models.Select(p =>
                {
                    var editModel = (IEditColumn)p;
                    editModel.CreateTime = DateTime.Now;
                    return (TModel)editModel;
                });
            }

            models = DbSet.AddRange(models);
            Context.SaveChanges();

            return models;
        }

        public virtual void Update(TModel model)
        {
            if (_isIEditColumn)
            {
                var editModel = (IEditColumn)model;
                editModel.LastEditTime = DateTime.Now;
            }

            Context.Entry(model).State = EntityState.Modified;
            Context.SaveChanges();
        }

        public virtual void UpdateRange(IEnumerable<TModel> models)
        {
            foreach (var model in models)
            {
                Context.Entry(model).State = EntityState.Modified;
            }

            Context.SaveChanges();
        }

        public virtual void Delete(TModel model)
        {
            if (_isIDeleted)
            {
                var deleteModel = (IDeleted)model;
                deleteModel.Deleted = true;
                Context.Entry(model).State = EntityState.Modified;
            }
            else
            {
                Context.Entry(model).State = EntityState.Deleted;
            }

            Context.SaveChanges();
        }

        public virtual void DeleteRange(IEnumerable<TModel> models)
        {
            if (_isIDeleted)
            {
                throw new Exception("Use \"Update\" or \"BatchUpdate\" to delete the data when class has IDeleted flag.");
            }

            DbSet.RemoveRange(models);
            Context.SaveChanges();
        }

        public virtual List<T> ExecuteQuery<T>(string sqlcmd, params object[] parameters)
        {
            var dbEntities = Context.Database
                                     .SqlQuery<T>(sqlcmd, parameters)
                                     .ToList<T>();

            return dbEntities;
        }
    }
}