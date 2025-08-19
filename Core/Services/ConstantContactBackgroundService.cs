// using Microsoft.Extensions.Hosting;
// using Microsoft.Extensions.Logging;
// using System;
// using System.Net.Http;
// using System.Threading;
// using System.Threading.Tasks;
// using UmbracoV16.Core.Services;
// using System.Net.Http.Headers;
// using Newtonsoft.Json;

// namespace UmbracoV16.Core.Services
// {
//     public class ConstantContactBackgroundService : BackgroundService
//     {
//         private readonly ILogger<ConstantContactBackgroundService> _logger;
//         private readonly IHttpClientFactory _httpClientFactory;
//         private readonly IServiceProvider _serviceProvider;
//         private readonly ConstantContactService _constantContactService;
//         private readonly int timeoutSeconds = 1000;

//         public ConstantContactBackgroundService(
//             ILogger<ConstantContactBackgroundService> logger,
//             IHttpClientFactory httpClientFactory,
//             IServiceProvider serviceProvider,
//             ConstantContactService constantContactService)
//         {
//             _logger = logger;
//             _httpClientFactory = httpClientFactory;
//             _serviceProvider = serviceProvider;
//             _constantContactService = constantContactService;
//         }

//         protected override async Task ExecuteAsync(CancellationToken stoppingToken)
//         {
//             _logger.LogInformation("Constant Contact Background Service is starting.");

//             while (!stoppingToken.IsCancellationRequested)
//             {
//                 try
//                 {
//                     await RunTaskAsync();
//                 }
//                 catch (Exception ex)
//                 {
//                     _logger.LogError(ex, "Error running Constant Contact Background Service.");
//                 }

//                 // Calculate delay until the next midnight execution
//                 var nextRunTime = DateTime.UtcNow.Date.AddDays(1).AddHours(0); // Run at midnight UTC
//                 var delay = nextRunTime - DateTime.UtcNow;
//                 _logger.LogInformation($"Next execution scheduled in {delay.TotalHours} hours.");

//                 await Task.Delay(delay, stoppingToken);
//             }
//         }

//         private async Task RunTaskAsync()
//         {
//             _logger.LogInformation("Fetching campaign data from Constant Contact API...");

//              string accessToken = await _constantContactService.GetAccessTokenAsync();
//             _logger.LogInformation($"✅ Retrieved Access Token: {accessToken.Substring(0, 5)}...");

//             var client = _httpClientFactory.CreateClient();
//             client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
//             client.Timeout = TimeSpan.FromSeconds(timeoutSeconds);

//             var apiUrl = "https://localhost:44355/api/constantcontact/fetch-all-campaign-data"; // Adjust your API URL

//             HttpResponseMessage response = await client.GetAsync(apiUrl);
//             if (response.IsSuccessStatusCode)
//             {
//                 _logger.LogInformation("Successfully fetched and saved campaign data.");
//             }
//             else
//             {
//                 _logger.LogError($"Failed to fetch campaign data. Status Code: {response.StatusCode}");
//             }
//         }
//     }
// }
