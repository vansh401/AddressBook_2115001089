using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Interface
{
    public interface IRedisCacheService
    {
        Task<T?> GetCachedData<T>(string key);
        Task SetCachedData<T>(string key, T value, TimeSpan expiration); 
        Task<bool> RemoveCachedData(string key);
    }
}
