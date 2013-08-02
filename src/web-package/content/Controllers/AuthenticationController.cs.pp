using System.Net;
using System.Net.Http;
using System.Web.Http;
using $rootnamespace$.Models;
using TS.FormsToTokenAccessAuthentication;

namespace $rootnamespace$.App_Start
{
    public class AuthenticationController
    	: ApiController
    {
    	//NOTE: if you can, use dependency injection to inject the ITokenGenerator instance through the constructor instead
        readonly ITokenGenerator _tokenGenerator = new FormsTokenGenerator(new WebSecurityWrapper());

        public Authentication Post(Login login)
        {
            if (ModelState.IsValid)
            {
                var token = _tokenGenerator.Login(Request, login.Username, login.Password);
                if (token != null)
                {
                    return new Authentication
                        {
                            Token = token.Value,
                            Coverage = token.Coverage
                        };
                }
            }

            throw new HttpResponseException(Request.CreateErrorResponse(
                HttpStatusCode.BadRequest, "Bad username or password"));
        }
    }
}