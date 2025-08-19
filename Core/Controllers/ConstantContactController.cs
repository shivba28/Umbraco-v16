// using Microsoft.AspNetCore.Mvc;
// using System.Net.Http;
// using System.Net.Http.Headers;
// using System.Threading.Tasks;
// using Newtonsoft.Json;
// using System.IO;
// using System.Collections.Generic;
// using System.Linq;
// using GardenCms.Core.Services; // Import the service
// using HtmlAgilityPack;
// using System.Text.RegularExpressions;

// namespace UmbracoV16.Core.Controllers
// {
//     [Route("api/[controller]")]
//     [ApiController]
//     public class ConstantContactController : ControllerBase
//     {
//         private readonly ConstantContactService _constantContactService;
//         private readonly IHttpClientFactory _httpClientFactory; // Use IHttpClientFactory

//         private readonly int timeoutSeconds = 1000;
//         public ConstantContactController(ConstantContactService constantContactService, IHttpClientFactory httpClientFactory)
//         {
//             _constantContactService = constantContactService;
//             _httpClientFactory = httpClientFactory;
//         }


//         [HttpGet("fetch-all-campaign-data")]
//         public async Task<IActionResult> FetchAllCampaignData()
//         {
//             string accessToken = await _constantContactService.GetAccessTokenAsync();
//             //Console.WriteLine(accessToken);
//             var client = _httpClientFactory.CreateClient();
//             client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
//             client.Timeout = TimeSpan.FromSeconds(timeoutSeconds);

//             // Step 1: Fetch campaign IDs
//             HttpResponseMessage response = await client.GetAsync("https://api.cc.email/v3/emails?limit=500");
//             if (!response.IsSuccessStatusCode)
//             {
//                 return StatusCode((int)response.StatusCode, new { success = false, message = "Error fetching campaigns." });
//             }

//             string result = await response.Content.ReadAsStringAsync();
//             var campaignsData = JsonConvert.DeserializeObject<dynamic>(result);
//             var campaignDetails = new List<dynamic>();

//             if (campaignsData?.campaigns != null)
//             {
//                 foreach (var campaign in campaignsData.campaigns)
//                 {
//                     if (campaign.current_status == "Done")
//                     {
//                         string campaignId = (string)campaign.campaign_id;
//                         string campaignName = (string)campaign.name;
//                         string createdAt = ExtractDateFromCampaignName(campaignName);
//                         string dateCreated = campaign.created_at.ToString("MMM dd, yyyy");
//                         string date = string.IsNullOrWhiteSpace(createdAt) ? dateCreated : createdAt;
                        
//                         // Step 2: Fetch campaign details
//                         string campaignUrl = $"https://api.cc.email/v3/emails/{campaignId}";
//                         response = await RetryRequestAsync(client, campaignUrl);
//                         if (!response.IsSuccessStatusCode)
//                         {
//                             Console.WriteLine(campaignId);
//                             continue; // Skip this campaign if details fail
//                         }

//                         string campaignData = await response.Content.ReadAsStringAsync();
//                         var campaignInfo = JsonConvert.DeserializeObject<dynamic>(campaignData);
//                         string campaignActivityId = "";

//                         if (campaignInfo?.campaign_activities != null)
//                         {
//                             var permalinkActivity = ((IEnumerable<dynamic>)campaignInfo.campaign_activities)
//                                 .FirstOrDefault(activity => (string)activity.role == "primary_email");

//                             if (permalinkActivity != null)
//                             {
//                                 campaignActivityId = (string)permalinkActivity.campaign_activity_id;
//                             }
//                         }

//                         if (!string.IsNullOrEmpty(campaignActivityId))
//                         {
//                             // Step 3: Fetch campaign activity details
//                             string activityUrl = $"https://api.cc.email/v3/emails/activities/{campaignActivityId}?include=permalink_url%2Chtml_content";
//                             response = await client.GetAsync(activityUrl);
//                             if (response.IsSuccessStatusCode)
//                             {
//                                 string activityData = await response.Content.ReadAsStringAsync();
//                                 var activity = JsonConvert.DeserializeObject<dynamic>(activityData);
//                                 string htmlContent = activity?.html_content!;

//                                 // Extract first image URL
//                                 string imageUrl = ExtractImageUrl(htmlContent);

//                                 var completeCampaignInfo = new
//                                 {
//                                     campaign_id = campaignId,
//                                     name = campaignName,
//                                     created_at = date,
//                                     campaign_activity_id = campaignActivityId,
//                                     campaign_subject = activity?.subject,
//                                     campaign_url = activity?.permalink_url,
//                                     campaign_preheader = activity?.preheader,
//                                     campaign_image = imageUrl,
//                                     // html_content = activity?.html_content,
//                                 };

//                                 campaignDetails.Add(completeCampaignInfo);
//                             }
//                             else
//                             {
//                                 activityUrl = $"https://api.cc.email/v3/emails/activities/{campaignActivityId}?include=permalink_url";
//                                 response = await client.GetAsync(activityUrl);
//                                 if (response.IsSuccessStatusCode)
//                                 {
//                                     string activityData = await response.Content.ReadAsStringAsync();
//                                     var activity = JsonConvert.DeserializeObject<dynamic>(activityData);
//                                     string imageUrl = null!;

//                                     var completeCampaignInfo = new
//                                     {
//                                         campaign_id = campaignId,
//                                         name = campaignName,
//                                         created_at = createdAt,
//                                         campaign_activity_id = campaignActivityId,
//                                         campaign_subject = campaignName,
//                                         campaign_url = activity?.permalink_url,
//                                         campaign_preheader = activity?.preheader,
//                                         campaign_image = imageUrl,
//                                     };

//                                     campaignDetails.Add(completeCampaignInfo);
//                                 }
//                             }
//                         }
//                     }
//                 }
//             }

//             // Save the campaigns to the file
//             await SaveCampaignDataToFileAsync(campaignDetails);
//             // string outputFile = "full_campaign_data.json";
//             // System.IO.File.WriteAllText(outputFile, JsonConvert.SerializeObject(campaignDetails, Formatting.Indented));

//             return Ok(new { success = true, data = campaignDetails });
//         }

//         // Method to save new campaigns to the file
//         private async Task SaveCampaignDataToFileAsync(List<dynamic> newCampaigns)
//         {
//             string filePath = "full_campaign_data.json";
//             List<dynamic> existingCampaigns = [];

//             // Check if file exists and read the existing campaigns
//             if (System.IO.File.Exists(filePath))
//             {
//                 string existingContent = await System.IO.File.ReadAllTextAsync(filePath);
//                 existingCampaigns = JsonConvert.DeserializeObject<List<dynamic>>(existingContent) ?? new List<dynamic>();
//             }

//             // Get existing campaign IDs for quick lookup
//             HashSet<string> existingCampaignIds = new HashSet<string>(existingCampaigns.Select(c => (string)c.campaign_id));

//             // Find new campaigns that are not in the file
//             var uniqueNewCampaigns = newCampaigns
//                 .Where(newCampaign => !existingCampaignIds.Contains((string)newCampaign.campaign_id))
//                 .ToList();

//             // If there are new campaigns, add them to the top
//             if (uniqueNewCampaigns.Count != 0)
//             {
//                 existingCampaigns.InsertRange(0, uniqueNewCampaigns); // Insert at the beginning

//                 // Serialize and save the updated data
//                 string json = JsonConvert.SerializeObject(existingCampaigns, Formatting.Indented);
//                 await System.IO.File.WriteAllTextAsync(filePath, json);

//                 Console.WriteLine($"Added {uniqueNewCampaigns.Count} new campaigns at the top.");
//             }
//             else
//             {
//                 Console.WriteLine("No new campaigns to add.");
//             }
//         }

//         // Function to retry HTTP requests
//         private async Task<HttpResponseMessage> RetryRequestAsync(HttpClient client, string url, int maxRetries = 3, int delayMilliseconds = 2000)
//         {
//             for (int i = 0; i < maxRetries; i++)
//             {
//                 var response = await client.GetAsync(url);
//                 if (response.IsSuccessStatusCode)
//                 {
//                     return response; // Return success response
//                 }

//                 Console.WriteLine($"Retry {i + 1} for {url} failed. Retrying in {delayMilliseconds}ms...");
//                 await Task.Delay(delayMilliseconds);
//                 delayMilliseconds *= 2; // Exponential backoff
//             }
//             return new HttpResponseMessage(System.Net.HttpStatusCode.ServiceUnavailable); // Return failure response
//         }


//         private string ExtractImageUrl(string htmlContent)
//         {
//             if (string.IsNullOrEmpty(htmlContent))
//                 return null!;

//             var doc = new HtmlDocument();
//             doc.LoadHtml(htmlContent);

//             var imgNodes = doc.DocumentNode.SelectNodes("//img");
//             if (imgNodes != null)
//             {
//                 int mediaCount = 0;
//                 foreach (var img in imgNodes)
//                 {
//                     string src = img.GetAttributeValue("src", "");
//                     // Check if the src starts with the Constant Contact image URL pattern
//                     if (src.StartsWith("https://files.constantcontact.com/") || src.StartsWith("http://files.constantcontact.com/"))
//                     {
//                         src = src.Replace("http://", "https://");
//                         mediaCount++;
//                         if (mediaCount == 2 || mediaCount == 3) // Take the second or third image
//                         {
//                             return src; // Return the second image URL
//                         }
//                     }

//                     // Check if the src is a YouTube thumbnail served through Constant Contact
//                     if (src.StartsWith("https://web-extract.constantcontact.com/v1/thumbnail") || src.StartsWith("http://web-extract.constantcontact.com/v1/thumbnail"))
//                     {
//                         mediaCount++;
//                         if (mediaCount == 2) // Take the second YouTube thumbnail image
//                         {
//                             // Extract the actual YouTube thumbnail URL from the src parameter
//                             var youtubeThumbnailUrl = ExtractYouTubeThumbnailUrl(src);
//                             return youtubeThumbnailUrl; // Return the YouTube thumbnail URL
//                         }
//                     }
//                 }
//             }

//             return null!; // No matching image found
//         }

//         private string ExtractYouTubeThumbnailUrl(string constantContactSrc)
//         {
//             var match = Regex.Match(constantContactSrc, @"url=(https?%3A%2F%2Fi\.ytimg\.com%2Fvi%2F[\w-]+%2F(?:hqdefault|maxresdefault)\.jpg|https?://i\.ytimg\.com/vi/[\w-]+/(?:hqdefault|maxresdefault)\.jpg)");
//             if (match.Success)
//             {
//                 // Decode the URL
//                 string decodedUrl = Uri.UnescapeDataString(match.Groups[1].Value);
//                 // Ensure the URL is HTTPS
//                 if (decodedUrl.StartsWith("http://"))
//                 {
//                     decodedUrl = "https://" + decodedUrl.Substring(7); // Replace "http" with "https"
//                 }

//                 return decodedUrl;
//             }
//             return null!;
//         }

//         private string ExtractDateFromCampaignName(string campaignName)
//         {
//             if (string.IsNullOrEmpty(campaignName))
//                 return null!;

//             // Regular expression to match the date pattern (Month Day, Year)
//             var match = Regex.Match(campaignName, @"\b([A-Za-z]+)\s(\d{1,2}),\s(\d{4})\b");

//             if (match.Success)
//             {
//                 return $"{match.Groups[1].Value} {match.Groups[2].Value}, {match.Groups[3].Value}"; 
//             }

//             return null!; // Return null if no valid date is found
//         }
//     }
// }
