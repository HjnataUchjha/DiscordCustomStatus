using Microsoft.AspNetCore.Mvc;

namespace DiscordCustomStatus.WebApi.Api
{
    public class DcsConfigController(WebApiConfig webApiConfig) : Controller
    {
        private readonly WebApiConfig _webApiConfig = webApiConfig;
    }
}
