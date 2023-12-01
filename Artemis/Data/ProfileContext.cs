using Artemis.Model;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace Artemis.Data
{
    public class ProfileContext
    {
        private readonly IMongoDatabase _database = null;

        public ProfileContext(IConfiguration config)
        {
            var client = new MongoClient(config.GetValue<string>("Mongo_ConnectionString"));
            if (client != null)
                _database = client.GetDatabase(config.GetValue<string>("Mongo_Database"));
        }

        public IMongoCollection<CurrentUser> CurrentUser => _database.GetCollection<CurrentUser>("Profile");
    }
}