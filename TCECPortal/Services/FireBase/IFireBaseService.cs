using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TCECPortal.Services.FireBase
{
    public interface IFirebaseService
    {
        Task<object> GetAsync(string child, string propertyName, object obj);
        Task PutAsync(string child, object obj);
        Task<object> PostAsync(string child, object obj);
        Task<object> DeleteAsync(string child, object obj);
    }
}
