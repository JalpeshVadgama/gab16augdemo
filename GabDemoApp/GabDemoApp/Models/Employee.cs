using Newtonsoft.Json;

namespace GabDemoApp.Models
{
    public class Employee
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "firstName")]
        public string FirstName { get; set; }

        [JsonProperty(PropertyName = "lastName")]
        public string LastName { get; set; }

        [JsonProperty(PropertyName = "designation")]
        public string Designation { get; set; }
    }
}