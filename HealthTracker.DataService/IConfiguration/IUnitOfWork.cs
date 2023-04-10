using HealthTracker.DataService.DataService;
using HealthTracker.DataService.IRepository;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthTracker.DataService.IConfiguration
{
    public interface IUnitOfWork
    {
        IUsersRepository Users{ get; }
        Task CompleteAsync(); 
    }
}
