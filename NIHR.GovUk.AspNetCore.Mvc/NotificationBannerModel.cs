using Microsoft.AspNetCore.Mvc;
using NIHR.Infrastructure.AspNetCore;

namespace NIHR.GovUk.AspNetCore.Mvc;

public class NotificationBannerModel
{
    public bool IsSuccess { get; set; }
    public string? Title { get; set; }
    public string? Heading { get; set; }
    public string? Body { get; set; }
    public string? LinkText { get; set; }
    public string? LinkUrl { get; set; }
}

public static class NotificationBannerExtensions
{
    public static void AddSuccessNotification(this Controller controller, string heading)
    {
        controller.AddNotification(new NotificationBannerModel
        {
            IsSuccess = true,
            Title = "Success",
            Heading = heading
        });
    }

    public static void AddNotification(this Controller controller, NotificationBannerModel notification)
    {
        controller.TempData.Put("Notification", notification);
    }
}
