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
                .Configure(settings => settings.AfterCallAsync = LogRequestAsync);

            async Task LogRequestAsync(FlurlCall httpCall)
            {
                try
                {
                    var logProps = new Dictionary<string, object>
                    {
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
                        _logger.LogInformation("{requestData}", logProps);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unable to log request");
                }
            }
        }
    }
}
