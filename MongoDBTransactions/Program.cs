using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MongoDBTransaction
{


    public static class Program
    {

        public class Product
        {
            [BsonId]
            public ObjectId Id { get; set; }
            [BsonElement("SKU")]
            public int SKU { get; set; }
            [BsonElement("Description")]
            public string Description { get; set; }
            [BsonElement("Price")]
            public Double Price { get; set; }
        }

        // replace with your connection string if it is different
        //const string MongoDBConnectionString = "mongodb://localhost:27020";
        const string MongoDBConnectionString = "mongodb://mongoset1:27020,mongoset2:27018,mongoset3:27019?replicaSet=mongodb-replicaset";

        public static async Task Main(string[] args)
        {
            OfficalMongoDBTryExample();
            if (!await UpdateProductsAsync()) { Environment.Exit(1); }
            Console.WriteLine("Finished updating the product collection");
            Console.ReadKey();
        }

        private static async Task<bool> UpdateProductsAsync()
        {
            // Create client connection to our MongoDB database
            var client = new MongoClient(MongoDBConnectionString);

            // Create the collection object that represents the "products" collection
            var database = client.GetDatabase("E21I4DAB");
            var products = database.GetCollection<Product>("products");

            // Clean up the collection if there is data in there
            await database.DropCollectionAsync("products");

            // collections can't be created inside a transaction so create it first
            await database.CreateCollectionAsync("products");

            // Create a session object that is used when leveraging transactions
            using (var session = await client.StartSessionAsync())
            {
                // Begin transaction
                session.StartTransaction();

                try
                {
                    // Create some sample data
                    var tv = new Product
                    {
                        Description = "Television",
                        SKU = 4001,
                        Price = 2000
                    };
                    var book = new Product
                    {
                        Description = "A funny book",
                        SKU = 43221,
                        Price = 19.99
                    };
                    var dogBowl = new Product
                    {
                        Description = "Bowl for Fido",
                        SKU = 123,
                        Price = 40.00
                    };

                    // Insert the sample data
                    await products.InsertOneAsync(session, tv);
                    await products.InsertOneAsync(session, book);
                    await products.InsertOneAsync(session, dogBowl);

                    var resultsBeforeUpdates = await products
                                    .Find<Product>(session, Builders<Product>.Filter.Empty)
                                    .ToListAsync();
                    Console.WriteLine("Original Prices:\n");
                    foreach (Product d in resultsBeforeUpdates)
                    {
                        Console.WriteLine(
                                    String.Format("Product Name: {0}\tPrice: {1:0.00}",
                                        d.Description, d.Price)
                        );
                    }

                    // Increase all the prices by 10% for all products
                    var update = new UpdateDefinitionBuilder<Product>()
                            .Mul<Double>(r => r.Price, 1.1);
                    await products.UpdateManyAsync(session,
                            Builders<Product>.Filter.Empty,
                            update); //,options);

                    // Made it here without error? Let's commit the transaction
                    await session.CommitTransactionAsync();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error writing to MongoDB: " + e.Message);
                    await session.AbortTransactionAsync();
                    return false;
                }

                // Let's print the new results to the console
                Console.WriteLine("\n\nNew Prices (10% increase):\n");
                var resultsAfterCommit = await products
                        .Find<Product>(session, Builders<Product>.Filter.Empty)
                        .ToListAsync();
                foreach (Product d in resultsAfterCommit)
                {
                    Console.WriteLine(
                        String.Format("Product Name: {0}\tPrice: {1:0.00}",
                                                    d.Description, d.Price)
                    );
                }

                return true;
            }
        }

        private static void OfficalMongoDBTryExample()
        {
            // For a replica set, include the replica set name and a seedlist of the members in the URI string; e.g.
            // string uri = "mongodb://mongodb0.example.com:27017,mongodb1.example.com:27017/?replicaSet=myRepl";
            // For a sharded cluster, connect to the mongos instances; e.g.
            // string uri = "mongodb://mongos0.example.com:27017,mongos1.example.com:27017/";
            var client = new MongoClient(MongoDBConnectionString);

            // Prereq: Create collections.
            var database1 = client.GetDatabase("mydb1");
            var collection1 = database1.GetCollection<BsonDocument>("foo").WithWriteConcern(WriteConcern.WMajority);
            collection1.InsertOne(new BsonDocument("abc", 0));

            var database2 = client.GetDatabase("mydb2");
            var collection2 = database2.GetCollection<BsonDocument>("bar").WithWriteConcern(WriteConcern.WMajority);
            collection2.InsertOne(new BsonDocument("xyz", 0));

            // Step 1: Start a client session.
            using (var session = client.StartSession())
            {
                // Step 2: Optional. Define options to use for the transaction.
                var transactionOptions = new TransactionOptions(
                    readPreference: ReadPreference.Primary,
                    readConcern: ReadConcern.Local,
                    writeConcern: WriteConcern.WMajority);

                // Step 3: Define the sequence of operations to perform inside the transactions
                var cancellationToken = CancellationToken.None; // normally a real token would be used
                var result = session.WithTransaction(
                    (s, ct) =>
                    {
                        collection1.InsertOne(s, new BsonDocument("abc", 1 + DateTime.Now.Second), cancellationToken: ct);
                        collection2.InsertOne(s, new BsonDocument("xyz", 999 + DateTime.Now.Second), cancellationToken: ct);
                        return "Inserted into collections in different databases";
                    },
                transactionOptions,
                cancellationToken);
            }
        }
    }
}
