
namespace NIHR.GovUk
{
    public class Navigation
    {
        public IList<NavigationItem> Items { get; set; } = [];

        public void Add(string? url, string label)
        {
            var item = new NavigationItem { Label = label };

            if (!string.IsNullOrEmpty(url))
            {
                item.Uri = new Uri(url, UriKind.RelativeOrAbsolute);
            }

            Add(item);
        }

        public void Add(NavigationItem item)
        {
            Items.Add(item);
        }
    }

    public class NavigationItem
    {
        public string Label { get; set; } = "#";
        public Uri Uri { get; set; } = new Uri("#", UriKind.Relative);
    }
}