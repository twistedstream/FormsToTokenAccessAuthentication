using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using TS.NUnitExtensions;

namespace TS.FormsToTokenAccessAuthentication.UnitTest
{
    [TestFixture]
    public class TokenErrorResponseMessageHandlerTests
        : TestBase
    {
        private const string Realm = "foo-realm";

        private static HttpMessageInvoker StubClient(HttpStatusCode status = HttpStatusCode.Unauthorized)
        {
            return new HttpMessageInvoker(new TokenErrorResponseMessageHandler(Realm)
                {
                    InnerHandler = new FakeInnerHandler
                        {
                            Message = new HttpResponseMessage(status)
                        }
                });
        }

        [Test]
        public void Should_add_a_WwwAuthenticate_response_header_if_response_is_401()
        {
            var client = StubClient();
            var request = new HttpRequestMessage();
            
            var response = client.SendAsync(request, new CancellationToken(false)).Result;

            var header = response.Headers.WwwAuthenticate;
            Assert.That(header, Is.Not.Null);

            var value = header.Single();
            Assert.That(value.Scheme, Is.EqualTo("Token"));
            Assert.That(value.Parameter, Is.StringMatching(
                "realm=\"" + Realm + "\", coverage=\"none\", timestamp=\"\\d+\""));
        }

        private static void AssertErrorResponse(HttpResponseMessage response, string errorMessage)
        {
            var header = response.Headers.FirstOrDefault(h => h.Key == "Authentication-Error");
            Assert.That(header, Is.Not.Null);

            var value = header.Value.Single();
            Assert.That(value, Is.EqualTo("error-message=\"" + errorMessage + "\""));

            var content = response.Content.ReadAsStringAsync().Result;
            Assert.That(content, Is.EqualTo("{\"Message\":\"" + errorMessage + "\"}"));
        }

        [Test]
        public void Should_render_a_proper_error_response_if_a_token_parsing_error_occurred()
        {
            const string errorMessage = "foo error message";

            var client = StubClient();
            var request = new HttpRequestMessage();
            request.Headers.Add("Error-Message-Temp", errorMessage);

            var response = client.SendAsync(request, new CancellationToken(false)).Result;

            AssertErrorResponse(response, errorMessage);
        }

        [Test]
        public void Should_render_a_proper_error_response_if_401_and_authorization_request_header_is_missing()
        {
            var client = StubClient();
            var request = new HttpRequestMessage();

            var response = client.SendAsync(request, new CancellationToken(false)).Result;

            AssertErrorResponse(response, "Authorization request header was missing");
        }

        [Test]
        public void Should_render_a_proper_error_response_if_401_and_token_is_invalid()
        {
            var client = StubClient();
            var request = new HttpRequestMessage();
            request.Headers.Authorization = new AuthenticationHeaderValue("foo", "bar");

            var response = client.SendAsync(request, new CancellationToken(false)).Result;

            AssertErrorResponse(response, "Token is invalid or has expired");  
        }

        [Test]
        public void Should_do_nothing_if_response_is_not_a_401_and_there_were_no_token_parsing_errors()
        {
            var client = StubClient(HttpStatusCode.OK);
            var request = new HttpRequestMessage();

            var response = client.SendAsync(request, new CancellationToken(false)).Result;

            Assert.That(response.Headers.Count(), Is.EqualTo(0));
            Assert.That(response.Content, Is.Null);
        }

        private class FakeInnerHandler
            : DelegatingHandler
        {
            public HttpResponseMessage Message { get; set; }
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                return Message == null 
                    ? base.SendAsync(request, cancellationToken) 
                    : Task.Factory.StartNew(() => Message);
            }
        }
    }
}
