using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace TS.FormsToTokenAccessAuthentication
{
    /// <summary>
    /// A ASP.NET Web Api message handler that generates an appropriate HTTP error responses,
    /// that conform to the Token Access Authentication spec 
    /// (http://tools.ietf.org/html/draft-hammer-http-token-auth-01).
    /// </summary>
    public sealed class TokenErrorResponseMessageHandler
        : DelegatingHandler
    {
        private readonly string _realm;

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenErrorResponseMessageHandler"/> class 
        /// with the specified realm.
        /// </summary>
        public TokenErrorResponseMessageHandler(string realm)
        {
            if (realm == null) throw new ArgumentNullException("realm");

            _realm = realm;
        }

        /// <summary>
        /// Overrides the <see cref="DelegatingHandler.SendAsync"/> method in order to generate the 
        /// appropriate HTTP response.
        /// </summary>
        protected async override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // call the inner handler.
            var response = await base.SendAsync(request, cancellationToken);

            var is401 = response.StatusCode == HttpStatusCode.Unauthorized;
            var tokenParsingErrorExists = request.Headers.Contains(Constants.ErrorMessageTempHeaderName);


            // write header for client to know the supported authentication mechanism
            if (is401)
            {
                var origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                var diff = DateTime.Now.ToUniversalTime() - origin;
                var timestamp = Math.Round(Math.Floor(diff.TotalSeconds));

                var authValue = string.Format(
                    "{0}=\"{1}\", {2}=\"{3}\", {4}=\"{5}\"",
                    Constants.RealmAttributeName,
                    _realm,
                    Constants.CoverageAttributeName,
                    Constants.CoverageNoneName,
                    Constants.TimestampAttributeName,
                    timestamp);

                response.Headers.WwwAuthenticate.Add(
                    new AuthenticationHeaderValue(Constants.TokenSchemeName, authValue));
            }

            // build potential error message to send back in the response
            string errorMessage = null;
            if (tokenParsingErrorExists)
                errorMessage = request.Headers.GetValues(Constants.ErrorMessageTempHeaderName).First();
            else if (is401)
                if (request.Headers.Authorization == null)
                    errorMessage = string.Format("{0} request header is missing",
                                                 Constants.AuthorizationHeaderName);
                else
                    errorMessage = "Token is invalid or has expired";

            // write error message in the header and body
            if (errorMessage != null)
            {
                var headerValue = string.Format("{0}=\"{1}\"",
                                                Constants.ErrorMessageAttributeName,
                                                errorMessage);
                response.Headers.Add(Constants.AuthenticationErrorHeaderName, headerValue);

                response.Content = new ObjectContent<HttpError>(new HttpError(errorMessage),
                                                                new JsonMediaTypeFormatter());
            }

            return response;
        }
    }
}