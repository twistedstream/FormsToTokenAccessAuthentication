namespace TS.FormsToTokenAccessAuthentication.Sample.Client.Models
{
    public class Authentication
    {
        public string Token { get; set; }
        public string Coverage { get; set; }

        public override string ToString()
        {
            return string.Format("token=\"{0}\", coverage=\"{1}\"", Token, Coverage);
        }
    }
}