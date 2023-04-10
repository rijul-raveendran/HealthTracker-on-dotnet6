using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthTracker.DataService.IRepository
{
    public interface IGenericRespository<T> where T : class
    {
        // Get all entities 
        Task<IEnumerable<T>> All();
        // Get single entity by id
        Task<T> GetById(Guid id);
        // Add new entity   
        Task<bool> Add(T entity);
        // Update entity    
        Task<bool> Upsert(T entity);

        // Delete entity by id      
        Task<bool> Delete(Guid id, string userId);
    }
}
