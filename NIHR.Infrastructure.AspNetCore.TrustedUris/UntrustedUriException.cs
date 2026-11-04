namespace NIHR.Infrastructure.AspNetCore.TrustedUris;

public class UntrustedUriException(string message) : Exception(message);