using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace FutureState.AppCore.Data
{
    public interface IDbChange
    {
        void Create<TModel>(TModel model) where TModel : class, new();
        void Create<TModel>(TModel model, IDbMapper<TModel> dbMapper) where TModel : class, new();
        void CreateOrUpdate<TModel>(TModel model) where TModel : class, new();
        void CreateOrUpdate<TModel>(TModel model, IDbMapper<TModel> dbMapper) where TModel : class, new();
        void Delete<TModel>(Expression<Func<TModel, bool>> expression) where TModel : class, new();
        void ExecuteNonQuery(string commandText);
        void ExecuteNonQuery(string commandText, IDictionary<string, object> parameters);
        void Update<TModel>(TModel model) where TModel : class, new();
        void Update<TModel>(TModel model, IDbMapper<TModel> dbMapper) where TModel : class, new();
    }
}