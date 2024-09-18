namespace NIHR.NotificationService.Models;

public class SendEmailRequest
{
    public string EmailAddress { get; set; }
    public string EmailTemplateId { get; set; }
    public Dictionary<string, string> Personalisation { get; set; }
    public string Reference { get; set; }
}
