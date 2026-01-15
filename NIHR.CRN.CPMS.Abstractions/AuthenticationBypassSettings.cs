namespace NIHR.CRN.CPMS.Abstractions
{
    public class AuthenticationBypassSettings
    {
        public bool Bypass { get; set; }

        public string BypassEmail { get; set; }
    }
}