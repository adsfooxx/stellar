using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Interfaces
{
    public interface IUnitOfWork :IDisposable
    {
        //取得Repository
        IRepository<T> GetRepository<T>() where T : class;
        //交易Start
        Task BeginAsync();
        //交易變更送出
        Task CommitAsync();
        //交易回溯，什麼事都沒發生
        Task RollbackAsync();
    }
}
