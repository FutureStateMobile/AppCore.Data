using System;

namespace FutureState.AppCore.Data.Config
{
    public class DbConfiguration
    {
        private readonly IDbProvider _provider;

        internal DbConfiguration(IDbProvider provider)
        {
            _provider = provider;
        }

        public void Configure<TModel>(Action<TableOptions<TModel>> func) where TModel : class, new()
        {
            var tableOptions = new TableOptions<TModel>(_provider);
            func(tableOptions);
        }
    }
}