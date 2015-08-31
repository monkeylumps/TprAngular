using System.Data.Entity;

namespace KanbanBoardApi.EntityFramework
{
    public class DataContext : DbContext, IDataContext
    {
        public new IDbSet<TEntity> Set<TEntity>() where TEntity : class
        {
            return base.Set<TEntity>();
        }
    }
}