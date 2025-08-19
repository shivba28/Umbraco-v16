// using Microsoft.AspNetCore.Mvc;
// using Microsoft.Extensions.Configuration;
// using System.Threading.Tasks;
// using System.Net.Http;
// using System.Collections.Generic;
// using System.IO;
// using Newtonsoft.Json;

// namespace UmbracoV16.Core.Controllers
// {
//     public class OAuthController : Controller
//     {
//         private readonly IConfiguration _configuration;
//         private static readonly string TokenFilePath = "tokens.json"; // Path to store the tokens

//         public OAuthController(IConfiguration configuration)
//         {
//             _configuration = configuration;
//         }

//         [HttpGet("oauth/callback")]
//         public async Task<IActionResult> Callback(string code)
//         {
//             if (string.IsNullOrEmpty(code))
//             {
//                 return BadRequest("Authorization code is missing.");
//             }

//             // Now use the authorization code to get access and refresh tokens
//             var clientId = _configuration["ConstantContact:ClientId"];
//             var clientSecret = _configuration["ConstantContact:ClientSecret"];
//             var redirectUri = _configuration["ConstantContact:RedirectUri"];; // Same as in the auth URL


//             using (var client = new HttpClient())
//             {
//                 var tokenRequest = new Dictionary<string, string>
//                 {
//                     { "grant_type", "authorization_code" },
//                     { "client_id", clientId },
//                     { "client_secret", clientSecret },
//                     { "code", code },
//                     { "redirect_uri", redirectUri }
//                 };
//                 var requestContent = new FormUrlEncodedContent(tokenRequest);
//                 var response = await client.PostAsync("https://authz.constantcontact.com/oauth2/default/v1/token", requestContent);
//                 if (response.IsSuccessStatusCode)
//                 {
//                     var responseContent = await response.Content.ReadAsStringAsync();
//                     // Extract tokens from the response (store the refresh token securely)
//                     var tokenData = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseContent);

//                     if (tokenData != null && tokenData.ContainsKey("access_token") && tokenData.ContainsKey("refresh_token"))
//                     {
//                         var tokenJson = JsonConvert.SerializeObject(new
//                         {
//                             access_token = tokenData["access_token"],
//                             refresh_token = tokenData["refresh_token"]
//                         }, Formatting.Indented);

//                         // Save to a file
//                         await System.IO.File.WriteAllTextAsync(TokenFilePath, tokenJson);
//                     }
                    
//                     return Ok(responseContent);
//                 }
//                 else
//                 {
//                     var errorResponse = await response.Content.ReadAsStringAsync();
//                     return StatusCode((int)response.StatusCode, errorResponse); // Log the error
//                 }
//             }
//         }
//     }
// }
