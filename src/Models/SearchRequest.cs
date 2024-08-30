namespace YandexSearchApp.Models
{
    public class SearchRequest
    {
        public string? SearchKeyword { get; set; }
        public int StartIndex { get; set; } = 0;
        public int PageSize { get; set; } = 10;
        public string? SearchPattern { get; set; }
    }
}
