using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using Flurl.Http.Configuration;
using Microsoft.Extensions.Logging;

namespace CryptoTax.ConsoleApp
{
    public class CustomFlurlClientFactory : PerBaseUrlFlurlClientFactory
    {
        private readonly ILogger<CustomFlurlClientFactory> _logger;

        public CustomFlurlClientFactory(ILogger<CustomFlurlClientFactory> logger)
        {
            _logger = logger;
        }

        protected override IFlurlClient Create(Url url)
        {
            return base
                .Create(url)
                .Configure(settings => settings.BeforeCall = LogBefore)
                .Configure(settings => settings.AfterCallAsync = LogAfterAsync);

            void LogBefore(FlurlCall httpCall)
            {
                var logProps = new Dictionary<string, object> { ["Console"] = true };
                using (_logger.BeginScope(logProps))
                {
                    _logger.LogInformation("{verb} {url}", httpCall.Request.Verb, httpCall.Request.Url);
                }
            }

            async Task LogAfterAsync(FlurlCall httpCall)
            {
                try
                {
                    var logProps = new Dictionary<string, object>
                    {
                        ["Http"] = true,
                        ["Request"] = httpCall,
                        ["RequestHeaders"] = httpCall.Request.Headers,
                        ["StatusCode"] = httpCall.Response?.StatusCode,
                        ["ReasonPhrase"] = httpCall.Response?.ResponseMessage.ReasonPhrase,
                        ["ResponseBody"] = await httpCall.Response?.ResponseMessage?.Content?.ReadAsStringAsync(),
                        ["Completed"] = httpCall.Completed,
                        ["Succeeded"] = httpCall.Succeeded,
                        ["Duration"] = httpCall.Duration,
                    };
                    using (_logger.BeginScope(logProps))
                    {
                        _logger.LogInformation("{props}", logProps);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error logging request");
                }
            }
        }
    }
}
