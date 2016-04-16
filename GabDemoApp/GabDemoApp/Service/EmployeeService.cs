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
    /// <summary>
    /// A business service class to demonstrate document db
    /// </summary>
    public class EmployeeService : IDisposable
    {

        /// <summary>
        /// Retrieve the Database ID to use from the Web Config
        /// </summary>
        private  string _databaseId;
        private  String DatabaseId
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
        private  string _collectionId;
        private  String CollectionId
        {
            get
            {
                if (string.IsNullOrEmpty(_collectionId))
                {
                    _collectionId = ConfigurationManager.AppSettings["collection"];
                }

                return _collectionId;
            }
        }


        private  DocumentClient _client;
        private  DocumentClient Client
        {
            get
            {
                if (_client == null)
                {
                    string endpoint = ConfigurationManager.AppSettings["endpoint"];
                    string authKey = ConfigurationManager.AppSettings["authKey"];
                    Uri endpointUri = new Uri(endpoint);
                    _client = new DocumentClient(endpointUri, authKey);
                }

                return _client;
            }
        }

        private  Database _database;
        private  Database Database => _database ?? (_database = ReadOrCreateDatabase());

        private  Database ReadOrCreateDatabase()
        {

            var db = Client.CreateDatabaseQuery()
                .Where(d => d.Id == DatabaseId)
                .AsEnumerable()
                .FirstOrDefault() ?? Client.CreateDatabaseAsync(new Database { Id = DatabaseId }).Result;

            return db;
        }


        private  DocumentCollection _collection;
        private  DocumentCollection Collection => _collection ?? (_collection = ReadOrCreateCollection(Database.SelfLink));

        private  DocumentCollection ReadOrCreateCollection(string databaseLink)
        {

            var col = Client.CreateDocumentCollectionQuery(databaseLink)
                .Where(c => c.Id == CollectionId)
                .AsEnumerable()
                .FirstOrDefault() ??
                      Client.CreateDocumentCollectionAsync(databaseLink, new DocumentCollection { Id = CollectionId }).Result;

            return col;
        }

        
        public  async Task<Document> CreateEmployeeAsync(Employee employee)
        {
            return await Client.CreateDocumentAsync(Collection.SelfLink, employee);
        }


        public  List<Employee> GetEmployees()
        {
            return Client.CreateDocumentQuery<Employee>(Collection.DocumentsLink)
                    .AsEnumerable()
                    .ToList<Employee>();
        }


        
        public  Employee GetEmployee(string id)
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

        public void Dispose()
        {
           
        }
    }
}