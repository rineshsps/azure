//references : 

//1) https://docs.microsoft.com/en-us/azure/storage/storage-dotnet-how-to-use-tables
//2) https://docs.microsoft.com/en-us/rest/api/storageservices/querying-tables-and-entities


using System;
using Microsoft.Azure; // Namespace for CloudConfigurationManager
using Microsoft.WindowsAzure.Storage; // Namespace for CloudStorageAccount
using Microsoft.WindowsAzure.Storage.Table; // Namespace for table 


namespace TableStroage
{
    public class CustomerEntity : TableEntity
    {
        public CustomerEntity(string lastName, string firstName)
        {
            this.PartitionKey = lastName;
            this.RowKey = firstName;
        }
        public CustomerEntity()
        {
        }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {

            // Retrieve the storage account from the connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("StorageConnectionString"));
            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Create the CloudTable that represents the "people" table.
            CloudTable tableDelete = tableClient.GetTableReference("people");
            // Delete the table it if exists.
            tableDelete.DeleteIfExists();

            // Retrieve a reference to the table.
            CloudTable table = tableClient.GetTableReference("people");

            // Create the table if it doesn't exist.
            table.CreateIfNotExists();

           

            //var ce = new CustomerEntity("ri", "ka")
            //{
            //    Email = "rika@mail.com",
            //    PhoneNumber = "1234"
            //};

            var customer = new CustomerEntity()
            {

                PartitionKey = "par-1",
                RowKey = "row-5",
                Email = "rika@mail.com",
                PhoneNumber = "1234"
            };

            // Create the TableOperation object that inserts the customer entity.
            TableOperation insertOperation = TableOperation.Insert(customer);
            // Execute the insert operation.
            table.Execute(insertOperation);
            //Insert a batch of entities
            //You can insert a batch of entities into a table in one write operation.Some other notes on batch operations
            TableBatchInsertion(table);

            //Retrieve all entities in a partition
            RetriveRecord(table);

        }
        private static void RetriveRecord(CloudTable table)
        {
            // Construct the query operation for all customer entities where PartitionKey="Smith".
            TableQuery<CustomerEntity> query = new TableQuery<CustomerEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "par-1"));
           
            // Print the fields for each customer.
            foreach (CustomerEntity entity in table.ExecuteQuery(query))
            {
                Console.WriteLine("{0}, {1}\t{2}\t{3}", entity.PartitionKey, entity.RowKey,
                    entity.Email, entity.PhoneNumber);
                Console.WriteLine($"{entity.PartitionKey }\t{ entity.PhoneNumber}");
            }
        }

        private static void TableBatchInsertion(CloudTable table)
        {
            // Create the batch operation.
            TableBatchOperation batchOperation = new TableBatchOperation();

            // Create a customer entity and add it to the table.
            CustomerEntity customer1 = new CustomerEntity("Smith1", "Jeff1");
            customer1.Email = "Jeff@contoso.com";
            customer1.PhoneNumber = "425-555-0104";

            // Create another customer entity and add it to the table.
            CustomerEntity customer2 = new CustomerEntity("Smith1", "Ben1");
            customer2.Email = "Ben@contoso.com";
            customer2.PhoneNumber = "425-555-0102";

            // Add both customer entities to the batch insert operation.
            batchOperation.Insert(customer1);
            batchOperation.Insert(customer2);

            // Execute the batch operation.
            table.ExecuteBatch(batchOperation);
        }
    }
}
