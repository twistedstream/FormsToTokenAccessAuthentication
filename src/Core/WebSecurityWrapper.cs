using WebMatrix.WebData;

namespace TS.FormsToTokenAccessAuthentication
{
    /// <summary>
    /// The default implmentation of the <see cref="IWebSecurityWrapper"/> interface.
    /// </summary>
    public sealed class WebSecurityWrapper 
        : IWebSecurityWrapper
    {
        bool IWebSecurityWrapper.Login(string userName, string password)
        {
            return WebSecurity.Login(userName, password);
        }
    }
}