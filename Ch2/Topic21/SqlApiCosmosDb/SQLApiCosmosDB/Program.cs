using Microsoft.Azure.Cosmos;
using SQLApiCosmosDB.Models;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SQLApiCosmosDB
{
    class Program
    {
        public const string EndpointUrl = "https://kairias97-sqlapi.documents.azure.com:443/";
        public const string Key = "4rD6TH5jT2k24CDH81T1Ph2MU013H2ZtRWVIqx8JosiJ4r4CIrmsgwinJ4Isd6R6nKD5Den98pjgSFX8JU9NAw==";
        private CosmosClient client;
        private Database database;
        private Container container;

        static void Main(string[] args)
        {
            try
            {
                Program demo = new Program();
                demo.StartDemo().Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error ocurred: {ex.Message}");
            }
        }


        private async Task StartDemo()
        {
            // Setup a new database
            string databaseName = "demoDB_" + Guid.NewGuid().ToString().Substring(0,5);
            this.client = new CosmosClient(EndpointUrl, Key);
            this.database = await this.client.CreateDatabaseIfNotExistsAsync(databaseName);
            //Create a new collection within the database
            string containerName = "collection_" + Guid.NewGuid().ToString().Substring(0, 5);
            this.container = await this.database.CreateContainerIfNotExistsAsync(containerName, "/LastName");

            // Prepare data for the new collection
            Person person1 = new Person
            {
                Id = "Person.1",
                FirstName = "Santiago",
                LastName = "Fernandez",
                Devices = new Device[]
                {
                new Device { OperatingSystem = "iOS", CameraMegaPixels = 7,
                Ram = 16, Usage = "Personal"},
                new Device { OperatingSystem = "Android", CameraMegaPixels = 12,
                Ram = 64, Usage = "Work"}
                },
                Gender = "Male",
                Address = new Address
                {
                    City = "Seville",
                    Country = "Spain",
                    PostalCode = "28973",
                    Street = "Diagonal",
                    State = "Andalucia"
                },
                IsRegistered = true
            };
            await this.CreateDocumentIfNotExistsAsync(databaseName, containerName, person1);
            Person person2 = new Person
            {
                Id = "Person.2",
                FirstName = "Santiago",
                LastName = "Perez",
                Devices = new Device[]
                {
                new Device { OperatingSystem = "iOS", CameraMegaPixels = 7,
                Ram = 16, Usage = "Work"},
                new Device { OperatingSystem = "Android", CameraMegaPixels = 12,
                Ram = 32, Usage = "Personal"}
                },
                Gender = "Female",
                Address = new Address
                {
                    City = "Barcelona",
                    Country = "Spain",
                    PostalCode = "28973",
                    Street = "Diagonal",
                    State = "Barcelona"
                },
                IsRegistered = true
            };

            await this.CreateDocumentIfNotExistsAsync(databaseName, containerName, person2);


            // Make queries on the collection

            IQueryable<Person> queryablePeople = this.container.GetItemLinqQueryable<Person>(true)
                .Where(p => p.Gender == "Male");

            foreach (var person in queryablePeople)
            {
                Console.Write($"\t Person: {person}");

            }

            // Make query using SQL

            var sqlQuery = "SELECT * FROM Person WHERE Person.Gender = 'Female'";

            QueryDefinition queryDefinition = new QueryDefinition(sqlQuery);
            FeedIterator<Person> peopleResultSetIterator = this.container.GetItemQueryIterator<Person>(queryDefinition);
            while(peopleResultSetIterator.HasMoreResults)
            {
                FeedResponse<Person> currentResultSet = await peopleResultSetIterator.ReadNextAsync();
                foreach (var p in currentResultSet)
                {
                    Console.WriteLine($"\tPerson: {p}");
                }
            }

            Console.WriteLine("Press any key to continue before updating...");
            Console.ReadKey();

            person2.FirstName = "Esteban";
            person2.Gender = "Male";
            await this.container.UpsertItemAsync(person2);

            //Delete a single document
            PartitionKey partitionKey = new PartitionKey(person1.LastName);
            await this.container.DeleteItemAsync<Person>(person1.Id, partitionKey);

            await this.database.DeleteAsync();

        }

        private async Task CreateDocumentIfNotExistsAsync(string database, string collection, Person person)
        {
            try
            {
                await this?.container.ReadItemAsync<Person>(person.Id,
                new PartitionKey(person.LastName));
                
            }
            catch (CosmosException dce)
            {
                if (dce.StatusCode == HttpStatusCode.NotFound)
                {
                    await this?.container.CreateItemAsync<Person>(person,
                    new PartitionKey(person.LastName));
                    
                }
            }
        }


    }
}
