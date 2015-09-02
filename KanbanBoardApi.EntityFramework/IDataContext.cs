using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Threading.Tasks;

namespace KanbanBoardApi.EntityFramework
{
    public interface IDataContext
    {
        IDbSet<TEntity> Set<TEntity>() where TEntity : class;

        Task<int> SaveChangesAsync();

        void SetModified(object entity);
    }
}