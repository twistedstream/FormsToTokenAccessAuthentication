using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;
using System.Web.Security;
using NUnit.Framework;
using Rhino.Mocks;
using TS.NUnitExtensions;

namespace TS.FormsToTokenAccessAuthentication.UnitTest
{
    [TestFixture]
    public class FormsCookieGeneratorTests
        : ComponentWithInterfaceTestBase<FormsCookieGenerator, IFormsCookieGenerator>
    {
        protected override IEnumerable<object> GetDependencies()
        {
            yield break;
        }

        [Test]
        public void Generate_should_require_a_request()
        {
            var target = CreateTarget();
            AssertThrowsArgumentException<ArgumentNullException>(
                () => target.Generate(null), "request");
        }

        private static HttpRequestBase StubRequest(string authorizationHeaderValue = null)
        {
            var cookies = new HttpCookieCollection();
            
            var headers = new NameValueCollection();
            if (authorizationHeaderValue != null)
                headers.Add("Authorization", authorizationHeaderValue);

            var request = MockRepository.GenerateStub<HttpRequestBase>();
            request.Stub(
                r => r.Cookies)
                   .Return(cookies);
            request.Stub(
                r => r.Headers)
                   .Return(headers);
               
            return request;
        }

        private static void AssertEmptyCookies(HttpRequestBase request)
        {
            Assert.That(request.Cookies, Is.Empty);
        }

        private static void AssertErrorHeader(HttpRequestBase request, string message)
        {
            var value = request.Headers["Error-Message-Temp"];
            if (value == null)
                Assert.Fail("Expected error HTTP header is missing");
            else
                Assert.That(value, Is.EqualTo(message), "Unexpected error HTTP header value");
        }

        [Test]
        public void Generate_should_not_add_any_cookies_if_the_authorization_header_is_missing()
        {
            var request = StubRequest();

            var target = CreateTarget();
            target.Generate(request);

            AssertEmptyCookies(request);
            Assert.That(request.Headers, Is.Empty);
        }

        [Test]
        public void Generate_should_write_error_header_if_authentication_header_value_is_unsupported()
        {
            var request = StubRequest("foo");

            var target = CreateTarget();
            target.Generate(request);

            AssertEmptyCookies(request);
            AssertErrorHeader(request, "Unsupported authentication method: foo");
        }

        [Test]
        public void Generate_should_write_error_header_if_authentication_header_value_has_a_missing_token_parameter_value()
        {
            var request = StubRequest("Token");

            var target = CreateTarget();
            target.Generate(request);

            AssertEmptyCookies(request);
            AssertErrorHeader(request, "Missing token parameter value");
        }

        [Test]
        public void Generate_should_write_error_header_if_authentication_header_value_has_an_unsupported_scheme()
        {
            var request = StubRequest("Foo ");

            var target = CreateTarget();
            target.Generate(request);

            AssertEmptyCookies(request);
            AssertErrorHeader(request, "Unsupported authentication scheme: Foo");
        }

        [Test]
        public void Generate_should_write_error_header_if_authentication_header_value_has_an_unsupported_token_authentication_value()
        {
            var request = StubRequest("Token foo");

            var target = CreateTarget();
            target.Generate(request);

            AssertEmptyCookies(request);
            AssertErrorHeader(request, "Unsupported token authentication parameter value: foo");
        }

        [Test]
        public void Generate_should_write_error_header_if_authentication_header_value_has_an_unsupported_coverage_name()
        {
            var request = StubRequest("Token token=\"some-token\", coverage=\"foo\"");

            var target = CreateTarget();
            target.Generate(request);

            AssertEmptyCookies(request);
            AssertErrorHeader(request, "Unsupported coverage name: foo");
        }

        [Test]
        public void Generate_should_write_error_header_if_authentication_header_value_has_an_empty_token()
        {
            var request = StubRequest("Token token=\"\", coverage=\"none\"");

            var target = CreateTarget();
            target.Generate(request);

            AssertEmptyCookies(request);
            AssertErrorHeader(request, "Token is empty");
        }

        [Test]
        public void Generate_should_write_the_cookie_if_the_authentication_header_is_valid()
        {
            var request = StubRequest("Token token=\"foo-token\", coverage=\"none\"");

            var target = CreateTarget();
            target.Generate(request);

            var cookie = request.Cookies[FormsAuthentication.FormsCookieName];
            if (cookie == null)
                Assert.Fail("HTTP cookie '{0}' is missing", FormsAuthentication.FormsCookieName);
            else
                Assert.That(cookie.Value, Is.EqualTo("foo-token"));

            Assert.That(request.Headers["Error-Message-Temp"], Is.Null);
        }
    }
}
