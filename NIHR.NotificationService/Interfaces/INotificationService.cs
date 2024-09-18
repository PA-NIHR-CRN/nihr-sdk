

using NIHR.NotificationService.Context;
using NIHR.NotificationService.Models;
using Notify.Models.Responses;

namespace NIHR.NotificationService.Interfaces;

public interface INotificationService
{
    Task SendPreviewEmailAsync(SendEmailRequest request, CancellationToken cancellationToken);

    Task<EmailNotificationResponse> SendBatchEmailAsync(List<Notification> notifications,
        CancellationToken cancellationToken);

    Task<TemplateList> GetTemplatesAsync(CancellationToken cancellationToken);
}
