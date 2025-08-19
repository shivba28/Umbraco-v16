// using Microsoft.Extensions.Configuration;
// using System;
// using System.Net.Http;
// using System.Net.Http.Headers;
// using System.Text.Json;
// using System.Text;
// using System.Threading.Tasks;
// using Newtonsoft.Json;
// using Skybrud.Essentials.Strings.Extensions;

// namespace UmbracoV16.Core.Services
// {
//     public class ConstantContactService
//     {
//         private readonly HttpClient _httpClient;
//         private readonly IConfiguration _configuration;
//         private string _accessToken;
//         private DateTime _tokenExpiration;
//         private static readonly string TokenFilePath = "tokens.json"; // Path to store the tokens

//         public ConstantContactService(HttpClient httpClient, IConfiguration configuration)
//         {
//             _httpClient = httpClient;
//             _configuration = configuration;

//             string tokensJson = File.ReadAllText(TokenFilePath);

//             // Deserialize the JSON to a dynamic object
//             var tokens = JsonConvert.DeserializeObject<dynamic>(tokensJson);

//             _accessToken = tokens!.access_token;

//             //Console.WriteLine("AccessToken: " + _accessToken);
//         }

//         public async Task<string> GetAccessTokenAsync()
//         {
//             if (!string.IsNullOrEmpty(_accessToken) && DateTime.UtcNow < _tokenExpiration)
//             {
//                 return _accessToken; // Use cached token if still valid
//             }

//             return await RefreshAccessTokenAsync();
//         }

//         private async Task<string> RefreshAccessTokenAsync()
//         {
//             var clientId = _configuration["ConstantContact:ClientId"];
//             var clientSecret = _configuration["ConstantContact:ClientSecret"];
//             var refreshToken = _configuration["ConstantContact:RefreshToken"];

//             if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret) || string.IsNullOrEmpty(refreshToken))
//             {
//                 throw new Exception("ConstantContact credentials are missing in configuration.");
//             }

//             // string credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"));
//             string combinedString = $"{clientId}:{clientSecret}";
//             byte[] byteArray = Encoding.UTF8.GetBytes(combinedString);
//             string credentials = "Basic " + Convert.ToBase64String(byteArray);
            

//             // string requestUrl = $"https://authz.constantcontact.com/oauth2/default/v1/token?refresh_token={refreshToken}&grant_type=refresh_token";

//             // using var request = new HttpRequestMessage(HttpMethod.Post, requestUrl);

//             //Console.WriteLine(refreshToken);
//             // Build the request URI
//             StringBuilder requestBuilder = new StringBuilder()
//                 .Append("https://authz.constantcontact.com/oauth2/default/v1/token")
//                 .Append("?refresh_token=")
//                 .Append(refreshToken)
//                 .Append("&grant_type=refresh_token");
//             Uri requestUri = new Uri(requestBuilder.ToString());
            
//             using (HttpClient httpClient = new HttpClient())
//             {
//                 HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, requestUri)
//                 {
//                     Content = new StringContent("", Encoding.UTF8, "application/x-www-form-urlencoded")
//                 };
//                 request.Headers.Add("Accept", "application/json");
//                 request.Headers.Add("Authorization", credentials);

//                 //Console.WriteLine(request);

//                 // Send the request and get the response
//                 HttpResponseMessage httpResponse = await httpClient.SendAsync(request);
//                 string responseContent = await httpResponse.Content.ReadAsStringAsync();

//                 // Handle the response
//                 //Console.WriteLine("Response: " + responseContent);

                
//                 var jsonResponse = await httpResponse.Content.ReadAsStringAsync();

//                 if (httpResponse.IsSuccessStatusCode)
//                 {
//                     var tokenResponse = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonResponse);
//                     //Console.WriteLine("Token value:", tokenResponse["access_token"]);
                    
//                     if (tokenResponse == null && !tokenResponse!.ContainsKey("access_token") && !tokenResponse.ContainsKey("refresh_token"))
//                     {
//                         throw new Exception("Invalid token response from Constant Contact.");
//                     }

//                     _accessToken = tokenResponse["access_token"];
//                     _tokenExpiration = DateTime.UtcNow.AddSeconds(tokenResponse["expires_in"].ToDouble());

//                     var tokenJson = JsonConvert.SerializeObject(new
//                         {
//                             access_token = tokenResponse["access_token"],
//                             refresh_token = tokenResponse["refresh_token"]
//                         }, Formatting.Indented);

//                     await System.IO.File.WriteAllTextAsync(TokenFilePath, tokenJson);

//                     // Save the new refresh token
//                     // SaveToken(_accessToken, tokenResponse["refresh_token"]);

//                     return _accessToken;
//                 }
//                 else
//                 {
//                     throw new Exception($"Failed to refresh token. Status Code: {httpResponse.StatusCode}, Response: {jsonResponse}");
//                 }
//             }
//         }

//         // private void SaveToken(string newAccessToken, string newRefreshToken)
//         // {
//         //     // Store the new refresh token securely (e.g., in a database or a file)
//         //     // For now, just log it or update appsettings.json manually
//         //     //Console.WriteLine($"New Access Token: {newAccessToken}");
//         //     //Console.WriteLine($"New Refresh Token: {newRefreshToken}");
//         // }

//         // private class TokenResponse
//         // {
//         //     public string AccessToken { get; set; }
//         //     public int ExpiresIn { get; set; }
//         //     public string RefreshToken { get; set; }
//         // }
//     }
// }
