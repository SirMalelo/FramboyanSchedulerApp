namespace FramboyanSchedulerApi.Models
{
    public class SiteCustomization
    {
        public int Id { get; set; }
        public string WelcomeText { get; set; } = string.Empty;
        public string BackgroundImageUrl { get; set; } = string.Empty;
        public string StudioWebsiteUrl { get; set; } = string.Empty;
    }
}
