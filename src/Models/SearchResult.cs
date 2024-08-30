namespace YandexSearchApp.Models
{
    public class SearchResult
    {
        public string? RequestedUrl { get; set; }
        public string? PageTitle { get; set; }
        public double TimeTaken { get; set; }
        public int SearchPatternCount { get; set; }
    }
}
