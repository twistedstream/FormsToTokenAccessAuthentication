using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web;
using System.Web.Security;
using NUnit.Framework;
using Rhino.Mocks;
using TS.NUnitExtensions;

namespace TS.FormsToTokenAccessAuthentication.UnitTest
{
    [TestFixture]
    public class FormsTokenGeneratorTests
        : ComponentWithInterfaceTestBase<FormsTokenGenerator, ITokenGenerator>
    {
        private const string UserName = "foo";
        private const string Password = "bar";

        private IWebSecurityWrapper _webSecurity;

        public override void Setup()
        {
            base.Setup();

            _webSecurity = MockRepository.GenerateStub<IWebSecurityWrapper>();
        }

        protected override IEnumerable<object> GetDependencies()
        {
            yield return _webSecurity;
        }

        [Test]
        public void Generate_should_require_a_request()
        {
            var target = CreateTarget();
            AssertThrowsArgumentException<ArgumentNullException>(
                () => target.Login(null, UserName, Password), "request");
        }

        [Test]
        public void Generate_should_require_a_user_name()
        {
            var target = CreateTarget();
            AssertThrowsArgumentException<ArgumentNullException>(
                () => target.Login(new HttpRequestMessage(), null, Password), "userName");
        }

        [Test]
        public void Generate_should_require_a_password()
        {
            var target = CreateTarget();
            AssertThrowsArgumentException<ArgumentNullException>(
                () => target.Login(new HttpRequestMessage(), "foo", null), "password");
        }

        [Test]
        public void Generate_should_not_return_a_token_if_the_credentials_were_bad()
        {
            _webSecurity
                .Stub(w => w.Login(UserName, Password))
                .Return(false);

            var target = CreateTarget();
            var result = target.Login(new HttpRequestMessage(), UserName, Password);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void Generate_should_require_the_Forms_Authentication_cookie_to_exist()
        {
            _webSecurity
                .Stub(w => w.Login(UserName, Password))
                .Return(true);

            var response = MockRepository.GenerateStub<HttpResponseBase>();
            response
                .Stub(r => r.Cookies)
                .Return(new HttpCookieCollection());
            var context = MockRepository.GenerateStub<HttpContextBase>();
            context
                .Stub(c => c.Response)
                .Return(response);
            var request = new HttpRequestMessage();
            request.Properties.Add("MS_HttpContext", context);

            var target = CreateTarget();

            Assert.Throws<InvalidOperationException>(
                () => target.Login(request, UserName, Password));
        }

        [Test]
        public void Generate_should_return_a_token_from_the_Forms_Authentication_cookie()
        {
            _webSecurity
                .Stub(w => w.Login(UserName, Password))
                .Return(true);

            var cookies = new HttpCookieCollection
                {
                    new HttpCookie(FormsAuthentication.FormsCookieName, "foo-token")
                };
            var response = MockRepository.GenerateStub<HttpResponseBase>();
            response
                .Stub(r => r.Cookies)
                .Return(cookies);
            var context = MockRepository.GenerateStub<HttpContextBase>();
            context
                .Stub(c => c.Response)
                .Return(response);
            var request = new HttpRequestMessage();
            request.Properties.Add("MS_HttpContext", context);
          
            var target = CreateTarget();
            var result = target.Login(request, UserName, Password);

            Assert.That(result, ContainsState.With(
                new
                    {
                        Value = "foo-token",
                        Coverage = "none"
                    }));
        }
    }
}
