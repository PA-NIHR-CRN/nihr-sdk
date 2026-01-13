namespace NIHR.CRN.CPMS.Abstractions
{
    public abstract class AuthenticationBypassSettings
    {
        public bool Bypass { get; set; }

        public string BypassEmail { get; set; }
    }
}