using System.Net.Http;

namespace TS.FormsToTokenAccessAuthentication
{
    /// <summary>
    /// Generates a security token from the specified credentials.
    /// </summary>
    public interface ITokenGenerator
    {
        /// <summary>
        /// Performs a login to generate an authentication token.
        /// </summary>
        /// <returns>
        /// An <see cref="AuthenticationToken"/> instance if the login was successful; 
        /// otherwise null.
        /// </returns>
        AuthenticationToken Login(HttpRequestMessage request, string userName, string password);
    }
}