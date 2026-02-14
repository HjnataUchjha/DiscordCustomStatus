using DiscordCustomStatus.AppConfig;
using DiscordCustomStatus.WebApi.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace DiscordCustomStatus.WebApi.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class DcsConfigController : Controller
    {
        [HttpGet("configs")]
        public ConfigSelectModel GetConfigs()
        {
            var config = AppConfigHelper.Config;

            return new ConfigSelectModel
            {
                CurrentId = config.CurrentDcsConfigId,
                SelectItems = [..config.DcsConfigs
                    .Select(x => new ConfigSelectItem
                    {
                        Id = x.Id,
                        Name = x.Name
                    })
                    .OrderBy(x => x.Name)]
            };
        }

        [HttpPost("set-current-config/{configId}")]
        public void SetCurrentConfig(Guid configId)
        {
            AppConfigHelper.Config.CurrentDcsConfigId = configId;
            AppConfigHelper.SaveConfig();
        }

        [HttpGet("config/{id}")]
        public ConfigEditModel GetConfig(Guid id)
        {
            var config = AppConfigHelper.Config.DcsConfigs.First(x => x.Id == id);

            return new ConfigEditModel
            {
                Id = id,
                Name = config.Name,
                ApiKey = config.ApiKey,
                GameDetails = config.GameDetails,
                State = config.State,
                ImageKey = config.ImageKey,
                ImageText = config.ImageText
            };
        }

        [HttpPost("config")]
        public void UpdateConfig(ConfigEditModel model)
        {
            var config = AppConfigHelper.Config;

            if (config.DcsConfigs.Any(x => x.Id != model.Id && x.Name == model.Name))
            {
                throw new Exception("Конфиг с таким именем уже существует");
            }

            DcsConfig updatingConfig = null;
            if (model.Id is null)
            {
                updatingConfig = new DcsConfig
                {
                    Id = Guid.NewGuid(),
                };
                config.DcsConfigs.Add(updatingConfig);
            }
            else
            {
                updatingConfig = config.DcsConfigs.First(x => x.Id == model.Id);
            }

            updatingConfig.Name = model.Name;
            updatingConfig.ApiKey = model.ApiKey;
            updatingConfig.GameDetails = model.GameDetails;
            updatingConfig.State = model.State;
            updatingConfig.ImageKey = model.ImageKey;
            updatingConfig.ImageText = model.ImageText;

            AppConfigHelper.SaveConfig();
        }

        [HttpDelete("config/{id}")]
        public void DeleteConfig(Guid id)
        {
            var config = AppConfigHelper.Config;

            DcsConfig deletingConfig = config.DcsConfigs.First(x => x.Id == id);
            config.DcsConfigs.Remove(deletingConfig);

            if (config.CurrentDcsConfigId == id)
            {
                config.CurrentDcsConfigId = null;
            }

            AppConfigHelper.SaveConfig();
        }
    }
}
