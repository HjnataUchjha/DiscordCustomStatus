namespace DiscordCustomStatus.WebApi.Api.Models
{
    public class ConfigSelectModel
    {
        public Guid? CurrentId { get; set; }
        public List<ConfigSelectItem> SelectItems { get; set; }
    }
}
