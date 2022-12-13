using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TCECPortal.Models;

namespace TCECPortal.Services
{
    public interface IRequestService
    {
        Task<TResult> GetAsync<TResult>(string uri, string token = "", List<RequestHeaders> requestHeaders = null);
        Task<TResult> PostAsync<TResult>(string uri, TResult data, string token = "", List<RequestHeaders> requestHeaders = null);
        Task<TResult> PostAsync<TRequest, TResult>(string uri, TRequest data, string token = "", List<RequestHeaders> requestHeaders = null);
        Task<TResult> PostTokenAuthAsync<TRequest, TResult>(string uri, TRequest data);
        Task<TResult> PatchAsync<TResult>(string uri, TResult data, string token = "", List<RequestHeaders> requestHeaders = null);
        Task<TResult> PatchAsync<TRequest, TResult>(string uri, TRequest data, string token = "", List<RequestHeaders> requestHeaders = null);
    }
}
