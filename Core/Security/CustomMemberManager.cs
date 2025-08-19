// using Microsoft.AspNetCore.Http;
// using Microsoft.AspNetCore.Identity;
// using Microsoft.Extensions.Logging;
// using Microsoft.Extensions.Options;
// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Security.Claims;
// using System.Text;
// using System.Threading.Tasks;
// using Umbraco.Cms.Core.Configuration.Models;
// using Umbraco.Cms.Core.Net;
// using Umbraco.Cms.Core.Security;
// using Umbraco.Cms.Core.Services;
// using Umbraco.Cms.Web.Common.Security;

// namespace UmbracoV16.Core.Security
// {
//     public class CustomMemberManager : MemberManager
//     {
//         public CustomMemberManager(IIpResolver ipResolver, IMemberUserStore store, IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<MemberIdentityUser> passwordHasher, IEnumerable<IUserValidator<MemberIdentityUser>> userValidators, IEnumerable<IPasswordValidator<MemberIdentityUser>> passwordValidators, IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<MemberIdentityUser>> logger, IOptionsSnapshot<MemberPasswordConfigurationSettings> passwordConfiguration, IPublicAccessService publicAccessService, IHttpContextAccessor httpContextAccessor) : base(ipResolver, store, optionsAccessor, passwordHasher, userValidators, passwordValidators, errors, services, logger, passwordConfiguration, publicAccessService, httpContextAccessor)
//         {
//         }
//         public override async Task<MemberIdentityUser> GetUserAsync(ClaimsPrincipal principal)
//         {
//             var baseUser = await base.GetUserAsync(principal);
//             var customUser = new MemberIdentityUser();
//             if (baseUser is not null) 
//             {
//                 customUser.Id = baseUser.Id;
//                 customUser.UserName = baseUser.UserName;
//                 customUser.Email = baseUser.Email;
//                 customUser.IsApproved = true;

//                 //virtually assign role to the signed in member.
//                 //https://www.youtube.com/watch?v=aouFfym_7Zs
//                 foreach(var claim in principal.Claims.Where(x => x.Type == ClaimTypes.Role))
//                 {
//                     customUser.AddRole(claim.Value);
//                 }
//             }
//             return customUser;
//         }
//     }
// }
