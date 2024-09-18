namespace NIHR.NotificationService.Context;

public class Notification
{
    public int Id { get; set; }
    public string PrimaryIdentifier { get; set; }
    public ICollection<NotificationData> NotificationDatas { get; set; } = new List<NotificationData>();
    public bool IsProcessed  { get; set; }

}
