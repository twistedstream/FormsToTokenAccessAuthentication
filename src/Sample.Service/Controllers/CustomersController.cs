using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TS.FormsToTokenAccessAuthentication.Sample.Service.Models;

namespace TS.FormsToTokenAccessAuthentication.Sample.Service.Controllers
{
    [Authorize]
    public class CustomersController 
        : ApiController
    {
        // simple, in-memory data
        private static readonly IDictionary<int, Customer> Data = new Dictionary<int, Customer>();

        // GET api/customers
        public IEnumerable<Customer> Get()
        {
            return Data.Values;
        }

        // GET api/customers/5
        public Customer Get(int id)
        {
            EnsureCustomerExists(id);

            return Data[id];
        }

        // POST api/customers
        [Authorize(Roles = "admin")]
        public HttpResponseMessage Post([FromBody]Customer customer)
        {
            customer.Id = Data.Count + 1;
            Data.Add(customer.Id, customer);

            var response = Request.CreateResponse(HttpStatusCode.Created, customer);
            var url = Url.Link("DefaultApi", new {id = customer.Id});
            response.Headers.Location = new Uri(url);

            return response;
        }

        // PUT api/customers/5
        [Authorize(Roles = "admin")]
        public void Put(int id, [FromBody]Customer customer)
        {
            EnsureCustomerExists(id);

            customer.Id = id;
            Data[id] = customer;
        }

        // DELETE api/customers/5
        [Authorize(Roles = "admin")]
        public void Delete(int id)
        {
            EnsureCustomerExists(id);

            Data.Remove(id);
        }

        private void EnsureCustomerExists(int id)
        {
            if (!Data.ContainsKey(id))
                throw new HttpResponseException(
                    Request.CreateErrorResponse(
                        HttpStatusCode.NotFound,
                        "Customer with ID " + id + " does not exist."));
        }
    }
}
