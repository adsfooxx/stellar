using Coravel.Invocable;
using Infrastructure.Service.MongoDB;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using System.Diagnostics.CodeAnalysis;

namespace Web
{   
    [Experimental("SKEXP0001")]
    public class Invocable : IInvocable
    {
        private readonly IServiceProvider _serviceProvider;

        public Invocable(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task Invoke()
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
            {
                //var mongoDBService = scope.ServiceProvider.GetRequiredService<MongoDBService>();
                //await mongoDBService.UpdateProduct();
                Console.WriteLine("跑");
            }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Task encountered an error: {ex.Message}");
            }
        }
    }
}
