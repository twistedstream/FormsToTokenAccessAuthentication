using WebMatrix.WebData;

namespace TS.FormsToTokenAccessAuthentication
{
    /// <summary>
    /// Wraps static methods in the <see cref="WebSecurity"/> class.
    /// </summary>
    public interface IWebSecurityWrapper
    {
        /// <summary>
        /// Invokes the <see cref="WebSecurity.Login"/> method with the specied credentials.
        /// </summary>
        bool Login(string userName, string password);
    }
}