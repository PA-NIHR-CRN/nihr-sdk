namespace NIHR.NotificationService.Context;

public class NotificationData
{
    public int Id { get; set; }
    public int NotificationId { get; set; }
    public string Key { get; set; } = null!;
    public string Value { get; set; } = null!;
}
