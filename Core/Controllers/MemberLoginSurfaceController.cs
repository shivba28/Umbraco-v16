// using GardenCms.Core.Security;
// using GardenCms.Core.ViewModels;
// using Microsoft.AspNetCore.Http.Extensions;
// using Microsoft.AspNetCore.Identity;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.AspNetCore.Mvc.ModelBinding;
// using Microsoft.Extensions.Logging;
// using System.Security.Claims;
// using Umbraco.Cms.Core.Cache;
// using Umbraco.Cms.Core.Logging;
// using Umbraco.Cms.Core.Models.Membership;
// using Umbraco.Cms.Core.Routing;
// using Umbraco.Cms.Core.Security;
// using Umbraco.Cms.Core.Services;
// using Umbraco.Cms.Core.Web;
// using Umbraco.Cms.Infrastructure.Persistence;
// using Umbraco.Cms.Web.Common.Security;
// using Umbraco.Cms.Web.Website.Controllers;
// using Microsoft.AspNetCore.Mvc.Routing;

// using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

// namespace GardenCms.Core.Controllers
// {
//     public class MemberLoginSurfaceController : SurfaceController
//     {
//         private readonly IMemberSignInManager _memberSignInManager;
//         private readonly IMemberManager _memberManager;
//         private readonly IMemberService _memberService;
//         private readonly ILogger<MemberLoginSurfaceController> _logger;
//         private readonly ActiveDirectoryUserManger _adUserManager;
//         public MemberLoginSurfaceController(
//             //these are required by the base controller
//             IUmbracoContextAccessor umbracoContextAccessor, 
//             IUmbracoDatabaseFactory databaseFactory, 
//             ServiceContext services, 
//             AppCaches appCaches, 
//             IProfilingLogger profilingLogger, 
//             IPublishedUrlProvider publishedUrlProvider,
//              //these are dependencies we've added
//             IMemberSignInManager memberSignInManager,
//             IMemberManager memberManager,
//             IMemberService memberService,
//             ILogger<MemberLoginSurfaceController> logger,
//             ActiveDirectoryUserManger adUserManager
//             ) : base(umbracoContextAccessor, databaseFactory, services, appCaches, profilingLogger, publishedUrlProvider)
//             {
//                 _memberSignInManager = memberSignInManager ?? throw new ArgumentNullException(nameof(memberSignInManager));
//                 _memberManager = memberManager ?? throw new ArgumentNullException(nameof(memberManager));
//                 _memberService = memberService ?? throw new ArgumentNullException(nameof(memberService));
//                 _logger = logger ?? throw new ArgumentNullException(nameof(logger));
//                 _adUserManager =  adUserManager;
//             }

//         [HttpPost]
//         public async Task<IActionResult> Login(MemberLoginViewModel model)
//         {
//             var returnUrl = Request.GetEncodedPathAndQuery();
//             //var returnUrl2 = $"{Request.Path}{Request.QueryString}";
//             //var returnUrl3 = Request;
//             //model.ReturnUrl = returnUrl1;

//             //var returnUrl3 = Request.
//             var placeholderUmbracoMember = "ActiveDirectoryUser";
//             if (!ModelState.IsValid)
//             {
//                 return CurrentUmbracoPage();
//             }
            
//             //validate member credentials using AD. Includes User's groups from AD
//             var result = await _adUserManager.ValidateUserAsync(model.Username, model.Password, true);
//             if (!result.IsSuccess)
//             { 
//                 //Could not authenicate user because with Active Directory becuase either their username or password is incorrect.
//                 ModelState.AddModelError(string.Empty, "Username or password is incorrect.");
//                 return CurrentUmbracoPage();
//             }

//             //Get placeholder Umbraco Member so we can virtualy assigned role it based on the user's group access from AD. 
//             var member = await _memberManager.FindByNameAsync(placeholderUmbracoMember);
            
//             if (member is null)
//             {
//                 ModelState.AddModelError(string.Empty, $"{placeholderUmbracoMember} member does not exist in Umbraco backoffice.");
//                 return CurrentUmbracoPage();
//             }

//             if (result.IsSuccess && result?.Groups.Count > 0)
//             {
//                 var customRoles = new List<Claim>();
//                 foreach (var group in result.Groups)
//                 {
//                     customRoles.Add(new (ClaimTypes.Role, group.Name));
//                 }

//                 foreach (var claim in customRoles)
//                 {
//                     member.Claims.Add(new() { ClaimType = claim.Type, ClaimValue = claim.Value, UserId = member.Id});
//                 }
//             }

//             if (member is not null)
//             {
//                 //log member in using the placeholder member 
//                 await _memberSignInManager.SignInAsync(member, true);
//                 if (!string.IsNullOrEmpty(returnUrl))
//                 {
//                     return Redirect(returnUrl);
//                 }
//             }

//             return Redirect("/");
//         }

//         [HttpPost]
//         public async Task<IActionResult> Logout()
//         {
//             await _memberSignInManager.SignOutAsync();

//             return RedirectToCurrentUmbracoUrl();
//         }
//     }
// }
