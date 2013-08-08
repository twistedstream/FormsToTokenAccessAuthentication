using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using TS.FormsToTokenAccessAuthentication.Sample.Client.Model;
using TS.FormsToTokenAccessAuthentication.Sample.Client.Models;

namespace TS.FormsToTokenAccessAuthentication.Sample.Client
{
    static class Program
    {
        private const string CustomersUri = "customers/";
        private static HttpClient _client;

        static void Main(string[] args)
        {
            Console.WriteLine("FormsToTokenAccessAuthentication Sample Client");
            Console.WriteLine();

            // init client
            _client = new HttpClient
                {
                    BaseAddress = new Uri("http://localhost.fiddler:8080/api/"),
                };
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            try
            {
                AuthenticateClient();
                ShowCustomers();
                AddCustomer("Bob Customer");
                var id = AddCustomer("Joe Customer");
                AddCustomer("Sue Customer");
                ShowCustomers();
                UpdateCustomer(id, "Joseph Customer");
                ShowCustomer(id);
                DeleteCustomer(id);
                ShowCustomers();

            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred calling the API:");
                Console.WriteLine(ex.Message);
                Console.WriteLine();
            }

            Console.WriteLine("Press enter to quit.");
            Console.ReadLine();
        }

        private static void AuthenticateClient()
        {
            Console.WriteLine("NOTE:");
            Console.WriteLine("See file 'src\\Sample.Service\\Filters\\SimpleMembershipInitializer.cs' for existing user credentials.");
            Console.WriteLine();

            Console.Write("Username: ");
            var userName = Console.ReadLine();
            Console.Write("Password: ");
            var password = Console.ReadLine();
            Console.WriteLine();

            // get auth token
            var login = new Login
                {
                    Username = userName,
                    Password = password
                };
            var response = _client.PostAsJsonAsync("authentication", login).Result;
            EnsureSuccess(response);
            var auth = response.Content.ReadAsAsync<Authentication>().Result;

            // set Authorization header for secure calls
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", auth.ToString());
        }

        private static void ShowCustomers()
        {
            var response = _client.GetAsync(CustomersUri).Result;
            EnsureSuccess(response);
            
            Console.WriteLine("Customers:");
            var customers = response.Content.ReadAsAsync<IEnumerable<Customer>>().Result.ToList();
            if (customers.Count == 0)
                Console.WriteLine("(none)");
            else
                foreach (var customer in customers)
                    ShowCustomer(customer);
            Console.WriteLine();
        }

        private static void ShowCustomer(Customer customer)
        {
            Console.WriteLine("{0}: {1}", customer.Id, customer.Name);
        }

        private static void ShowCustomer(int id)
        {
            Console.WriteLine("Fetching customer with ID {0}:", id);
            var response = _client.GetAsync(CustomersUri + id).Result;
            EnsureSuccess(response);

            var customer = response.Content.ReadAsAsync<Customer>().Result;
            ShowCustomer(customer);
            Console.WriteLine();
        }

        private static int AddCustomer(string name)
        {
            var response = _client.PostAsJsonAsync(CustomersUri, new Customer { Name = name }).Result;
            EnsureSuccess(response);

            Console.WriteLine("New customer created: {0}", response.Headers.Location);
            var newCustomer = response.Content.ReadAsAsync<Customer>().Result;
            ShowCustomer(newCustomer);
            Console.WriteLine();

            return newCustomer.Id;
        }

        private static void UpdateCustomer(int id, string newName)
        {
            var customer = new Customer {Name = newName};
            var response = _client.PutAsJsonAsync(CustomersUri + id, customer).Result;
            EnsureSuccess(response);

            Console.WriteLine("Customer with ID {0} updated to have new name '{1}'.", id, newName);
            Console.WriteLine();
        }

        private static void DeleteCustomer(int id)
        {
            var response = _client.DeleteAsync(CustomersUri + id).Result;
            EnsureSuccess(response);

            Console.WriteLine("Customer with ID {0} deleted.", id);
            Console.WriteLine();
        }

        private static void EnsureSuccess(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode) return;

            var rawError = response.Content.ReadAsStringAsync().Result;
            var error = JsonConvert.DeserializeObject(rawError);
            var formattedError = JsonConvert.SerializeObject(error, Formatting.Indented);
            var message = string.Format("ERROR: {0}:{1}\n{2}",
                                        (int) response.StatusCode,
                                        response.ReasonPhrase,
                                        formattedError);

            throw new ApplicationException(message);
        }
    }
}
