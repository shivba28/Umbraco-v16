// using Microsoft.AspNetCore.Mvc;
// using Microsoft.AspNetCore.Authorization;
// using UmbracoV16.Core.ViewModels;
// using Umbraco.Cms.Web.BackOffice.Security;
// using Our.Umbraco.TagHelpers.Services;

// namespace UmbracoV16.Core.Controllers
// {
//     [AllowAnonymous]
//     public class CustomBackOfficeLoginController : Controller
//     {
//         private readonly IBackOfficeSignInManager _signInManager;
//         private readonly ILogger<CustomBackOfficeLoginController> _logger;
//         private readonly IBackofficeUserAccessor _backofficeUserAccessor;

//         public CustomBackOfficeLoginController(
//             IBackOfficeSignInManager signInManager,
//             ILogger<CustomBackOfficeLoginController> logger,
//             IBackofficeUserAccessor backofficeUserAccessor)
//         {
//             _signInManager = signInManager;
//             _logger = logger;
//             _backofficeUserAccessor = backofficeUserAccessor;
//         }

//         [HttpGet]
//         public IActionResult Login(string? returnUrl = null)
//         {
//             var backOfficeUser = _backofficeUserAccessor.BackofficeUser.IsAuthenticated;
//             Console.WriteLine("backofficeuser: " + backOfficeUser);

//             if (_backofficeUserAccessor.BackofficeUser.IsAuthenticated)
//             {
//                 return Redirect("/umbraco"); // Already logged in
//             }
//             ViewData["ReturnUrl"] = returnUrl;
//             ViewData["Title"] = "Food Services - Admin Login"; // Custom title
            
//             var model = new BackOfficeLoginViewModel
//             {
//                 ReturnUrl = returnUrl
//             };
            
//             return View(model);
//         }

//         [HttpPost]
//         [ValidateAntiForgeryToken]
//         public async Task<IActionResult> Login(BackOfficeLoginViewModel model)
//         {
//             ViewData["Title"] = "Food Services - Admin Login"; // Custom title
            
//             if (!ModelState.IsValid)
//             {
//                 return View(model);
//             }

//             try
//             {
//                 var result = await _signInManager.PasswordSignInAsync(
//                     model.Username, 
//                     model.Password, 
//                     model.RememberMe, 
//                     lockoutOnFailure: true);

//                 if (result.Succeeded)
//                 {
//                     _logger.LogInformation("User {Username} logged in.", model.Username);
                    
//                     if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
//                     {
//                         return Redirect(model.ReturnUrl);
//                     }
                    
//                     // Redirect to Umbraco backoffice
//                     return Redirect("/umbraco");
//                 }
                
//                 if (result.IsLockedOut)
//                 {
//                     _logger.LogWarning("User {Username} account locked out.", model.Username);
//                     ModelState.AddModelError(string.Empty, "Account locked out.");
//                     return View(model);
//                 }
                
//                 ModelState.AddModelError(string.Empty, "Invalid username or password.");
//                 return View(model);
//             }
//             catch (Exception ex)
//             {
//                 _logger.LogError(ex, "Error during login attempt for user {Username}", model.Username);
//                 ModelState.AddModelError(string.Empty, "An error occurred during login. Please try again.");
//                 return View(model);
//             }
//         }

//         [HttpPost]
//         public async Task<IActionResult> Logout()
//         {
//             await _signInManager.SignOutAsync();
//             _logger.LogInformation("User logged out.");
//             return Redirect("/admin");
//         }
//     }
// }
