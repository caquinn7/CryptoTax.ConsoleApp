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
                .Configure(settings => settings.AfterCall = LogRequest)
                .Configure(settings => settings.OnErrorAsync = LogFailedRequestAsync);

            void LogRequest(FlurlCall httpCall)
            {
                try
                {
                    // only log successful calls b/c unsuccessful ones will be logged in other method
                    if (httpCall.HttpResponseMessage.IsSuccessStatusCode)
                    {
                        _logger.LogInformation("{httpCall}", httpCall);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unable to log request");
                }
            }

            async Task LogFailedRequestAsync(FlurlCall httpCall)
            {
                try
                {
                    var logProps = new Dictionary<string, object>
                    {
                        ["Request"] = httpCall,
                        ["RequestHeaders"] = httpCall.Request.Headers,
                        ["StatusCode"] = httpCall.Response?.StatusCode,
                        ["ReasonPhrase"] = httpCall.Response?.ResponseMessage.ReasonPhrase,
                        ["ResponseBody"] = await httpCall.Response?.ResponseMessage.Content.ReadAsStringAsync(),
                        ["Completed"] = httpCall.Completed,
                        ["Succeeded"] = httpCall.Succeeded,
                        ["Duration"] = httpCall.Duration,
                    };
                    using (_logger.BeginScope(logProps))
                    {
                        _logger.LogError(httpCall.Exception, "{requestData}", logProps);
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
