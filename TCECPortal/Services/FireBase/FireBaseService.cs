using Firebase.Database;
using Firebase.Database.Query;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TCECPortal.Services.FireBase
{
    public class FirebaseService : IFirebaseService
    {
        private readonly IConfiguration _configuration;
        private readonly FirebaseClient firebase;
        public FirebaseService(IConfiguration configuration)
        {
            _configuration = configuration;

            firebase = new FirebaseClient(_configuration.GetConnectionString("FirebaseConnection"), new FirebaseOptions
            {
                AuthTokenAsyncFactory = () => Task.FromResult(_configuration.GetConnectionString("FirebaseAuth"))
            });
        }

        public Task<object> DeleteAsync(string child, object obj)
        {
            throw new NotImplementedException();
        }

        public async Task<object> GetAsync(string child, string propertyName, object obj)
        {
            return (FirebaseObject<object>)await firebase
                  .Child(child)
                  .OrderBy(propertyName)
                  .StartAt(Convert.ToInt32(obj))
                  .EndAt(Convert.ToInt32(obj))
                  .OnceAsync<object>();
        }

        public async Task<object> PostAsync(string child, object obj)
        {
            var json = JsonConvert.SerializeObject(obj);
            var a = await firebase
              .Child(child)
              .PostAsync(json);

            return a;
        }

        public async Task PutAsync(string child, object obj)
        {
            await firebase
                .Child(child)
                .PutAsync(JsonConvert.SerializeObject(obj));
        }
    }
}
