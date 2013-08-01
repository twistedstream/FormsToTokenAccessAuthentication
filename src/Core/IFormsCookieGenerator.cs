using System.Web;

namespace TS.FormsToTokenAccessAuthentication
{
    /// <summary>
    /// Examines an HTTP request and converts HTTP headers containing authentication tokens
    /// that conform to the Token Access Authentication spec 
    /// (http://tools.ietf.org/html/draft-hammer-http-token-auth-01) and converts them 
    /// to HTTP cookies that will get picked up by ASP.NET Forms Authentication.
    /// </summary>
    public interface IFormsCookieGenerator
    {
        /// <summary>
        /// Performs the cookie generation using the specified <see cref="HttpRequestBase"/>.
        /// </summary>
        void Generate(HttpRequestBase request);
    }
}