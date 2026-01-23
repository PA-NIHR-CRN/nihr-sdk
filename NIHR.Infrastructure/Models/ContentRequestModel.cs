namespace NIHR.Infrastructure.Models
{
    public class ContentRequestModel
    {
        // Contentful defaults
        public string ContentKey { get; set; } = "sys.id";
        public string ContentValue { get; set; }
        public string Locale { get; set; } = "en-GB";
        public int ContentTreeDepth { get; set; } = 5;
        public bool UsePreviewApi { get; set; } = false;
        public int limit { get; set; }
        public int skip { get; set; }

        // orderby can be a comma serperated list for multiple field sort.
        public string orderBy { get; set; }

        public string contentType {  get; set; }
    }
}
