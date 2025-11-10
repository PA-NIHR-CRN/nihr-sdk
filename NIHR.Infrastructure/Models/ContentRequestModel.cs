namespace NIHR.Infrastructure.Models
{
    public class ContentRequestModel
    {
        public string Id { get; set; }
        public string Locale { get; set; } = "en-GB";
        public int ContentTreeDepth { get; set; } = 5;
        public bool UsePreviewApi { get; set; } = false;
    }
}
