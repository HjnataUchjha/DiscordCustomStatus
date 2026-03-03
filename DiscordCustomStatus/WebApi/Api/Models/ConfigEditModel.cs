namespace DiscordCustomStatus.WebApi.Api.Models
{
    public class ConfigEditModel
    {
        public Guid? Id { get; set; }

        public string Name { get; set; }
        public string ApiKey { get; set; }
        public string GameDetails { get; set; }
        public string State { get; set; }
        public string ImageKey { get; set; }
        public string ImageText { get; set; }
    }
}
