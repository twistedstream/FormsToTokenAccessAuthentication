using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;

namespace TS.FormsToTokenAccessAuthentication
{
    /// <summary>
    /// The default implementation of the <see cref="IFormsCookieGenerator"/> interface.
    /// </summary>
    public sealed class FormsCookieGenerator 
        : IFormsCookieGenerator
    {
        void IFormsCookieGenerator.Generate(HttpRequestBase request)
        {
            if (request == null) throw new ArgumentNullException("request");

            // exit if request doesn't contain the auth header
            if (request.Headers.AllKeys.All(k => k != Constants.AuthorizationHeaderName))
                return;

            var authValue = request.Headers.Get(Constants.AuthorizationHeaderName);

            // detect scheme
            const string schemeName = "scheme";
            const string parameterName = "parameter";
            var schemePattern = string.Format(
                "^(?<{0}>\\S+)\\s+(?<{1}>.*)",
                schemeName,
                parameterName);
            var schemeMatchs = Regex.Matches(authValue, schemePattern);
            if (schemeMatchs.Count == 1)
            {
                var schemeMatch = schemeMatchs[0];
                var scheme = schemeMatch.Groups[schemeName].Value;
                if (scheme == Constants.TokenSchemeName)
                {
                    var tokenAuthParameter = schemeMatch.Groups[parameterName].Value;

                    var tokenAuthPattern = string.Format(
                        "{0}=\"(?<{0}>.*)\",\\s*{1}=\"(?<{1}>.*)\"",
                        Constants.TokenAttributeName,
                        Constants.CoverageAttributeName);
                    var tokenAuthMatches = Regex.Matches(authValue, tokenAuthPattern);
                    if (tokenAuthMatches.Count == 1)
                    {
                        var tokenAuthMatch = tokenAuthMatches[0];
                        var coverage = tokenAuthMatch.Groups[Constants.CoverageAttributeName].Value;
                        if (coverage == Constants.CoverageNoneName)
                        {
                            var token = tokenAuthMatch.Groups[Constants.TokenAttributeName].Value;
                            if (!string.IsNullOrWhiteSpace(token))
                            {
                                var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, token);
                                request.Cookies.Add(cookie);
                            }
                            else
                                WriteError(request, "Token is empty");
                        }
                        else
                            WriteError(request, "Unsupported coverage name: " + coverage);
                    }
                    else
                        WriteError(request,
                                   "Unsupported token authentication parameter value: " + tokenAuthParameter);
                }
                else
                    WriteError(request, "Unsupported authentication scheme: " + scheme);
            }
            else
            {
                if (authValue == Constants.TokenSchemeName)
                    WriteError(request, "Missing token parameter value");
                else
                    WriteError(request, "Unsupported authentication method: " + authValue);
            }   
        }

        private static void WriteError(HttpRequestBase request, string message)
        {
            // write error message to throw-away request header so it can be picked up by message handler later in pipeline
            request.Headers.Add(Constants.ErrorMessageTempHeaderName, message);
        }
    }
}