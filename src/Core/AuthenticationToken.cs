namespace TS.FormsToTokenAccessAuthentication
{
    /// <summary>
    /// Contains authenticated token information.
    /// </summary>
    public class AuthenticationToken
    {
        /// <summary>
        /// The value of the token.
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// The coverage of the token.
        /// </summary>
        public string Coverage { get; set; }
    }
}