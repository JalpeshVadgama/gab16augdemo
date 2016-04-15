using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using GabDemoApp.Models;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;

namespace GabDemoApp.Service
{
    public class EmployeeService
    {

        /// <summary>
        /// Retrieve the Database ID to use from the Web Config
        /// </summary>
        private static string _databaseId;
        private static String DatabaseId
        {
            get
            {
                if (string.IsNullOrEmpty(_databaseId))
                {
                    _databaseId = ConfigurationManager.AppSettings["database"];
                }

                return _databaseId;
            }
        }

        /// <summary>
        /// Retrieves the Collection to use from Web Config
        /// </summary>
        private static string collectionId;
        private static String CollectionId
        {
            get
            {
                if (string.IsNullOrEmpty(collectionId))
                {
                    collectionId = ConfigurationManager.AppSettings["collection"];
                }

                return collectionId;
            }
        }


        private static DocumentClient client;
        private static DocumentClient Client
        {
            get
            {
                if (client == null)
                {
                    string endpoint = ConfigurationManager.AppSettings["endpoint"];
                    string authKey = ConfigurationManager.AppSettings["authKey"];
                    Uri endpointUri = new Uri(endpoint);
                    client = new DocumentClient(endpointUri, authKey);
                }

                return client;
            }
        }

        private static Database _database;
        private static Database Database
        {
            get
            {
                if (_database == null)
                {
                    _database = ReadOrCreateDatabase();
                }

                return _database;
            }
        }

        private static Database ReadOrCreateDatabase()
        {

            var db = Client.CreateDatabaseQuery()
                            .Where(d => d.Id == DatabaseId)
                            .AsEnumerable()
                            .FirstOrDefault();

            if (db == null)
            {
                db = Client.CreateDatabaseAsync(new Database { Id = DatabaseId }).Result;
            }

            return db;
        }


        private static DocumentCollection _collection;
        private static DocumentCollection Collection
        {
            get
            {
                if (_collection == null)
                {
                    _collection = ReadOrCreateCollection(Database.SelfLink);
                }

                return _collection;
            }
        }

        private static DocumentCollection ReadOrCreateCollection(string databaseLink)
        {

            var col = Client.CreateDocumentCollectionQuery(databaseLink)
                              .Where(c => c.Id == CollectionId)
                              .AsEnumerable()
                              .FirstOrDefault();

            if (col == null)
            {
                col = Client.CreateDocumentCollectionAsync(databaseLink, new DocumentCollection { Id = CollectionId }).Result;
            }

            return col;
        }

        
        public static async Task<Document> CreateEmployeeAsync(EmployeeService employee)
        {
            return await Client.CreateDocumentAsync(Collection.SelfLink, employee);
        }


        public static List<Employee> GetEmployees()
        {
            return Client.CreateDocumentQuery<Employee>(Collection.DocumentsLink)
                    .AsEnumerable()
                    .ToList<Employee>();
        }


        
        public static Employee GetEmployee(string id)
        {
            return Client.CreateDocumentQuery<Employee>(Collection.DocumentsLink)
                        .Where(d => d.Id == id)
                        .AsEnumerable()
                        .FirstOrDefault();
        }

        public  async Task<Document> UpdateEmployeeAsync(Employee employee)
        {
            Document doc = Client.CreateDocumentQuery(Collection.DocumentsLink)
                                .Where(d => d.Id == employee.Id)
                                .AsEnumerable()
                                .FirstOrDefault();

            return await Client.ReplaceDocumentAsync(doc.SelfLink, employee);
        }

        
        public  async Task DeleteEmployeeAsyc(Employee employee)
        {
            Document doc = Client.CreateDocumentQuery(Collection.DocumentsLink)
                                .Where(d => d.Id == employee.Id)
                                .AsEnumerable()
                                .FirstOrDefault();

            await Client.DeleteDocumentAsync(doc.SelfLink);

        }





    }
}