// using System.DirectoryServices.AccountManagement;
// using UmbracoV16.Core.Constants;

// namespace UmbracoV16.Core.Security
// {
//     public class ActiveDirectoryUserManger
//     {
//         private readonly IConfiguration _configuration;
//         private readonly ILogger<ActiveDirectoryUserManger> _logger;

//         public ActiveDirectoryUserManger(IConfiguration configuration, ILogger<ActiveDirectoryUserManger> logger)
//         {
//             _configuration = configuration;
//             _logger = logger;
//         }

//         public Task<Result> ValidateUserAsync(string username, string password , bool includeUserGroup = false)
//         {
//             return Task.FromResult(IsValidADUser(username, password, includeUserGroup));
//         }

//         [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
//         private Result IsValidADUser(string userName, string password, bool includeUserGroup = false)
//         {
//             try
//             {
//                 var domain = _configuration.GetValue<string>(AppSettings.ActiveDirectoryDomain);  //get active directory domain from appsettings 
//                 using (PrincipalContext principalContext = new PrincipalContext(ContextType.Domain, domain))
//                 {
//                     var areCredentialsValid = principalContext.ValidateCredentials(userName, password);
//                     if(areCredentialsValid && includeUserGroup)
//                     {
//                         UserPrincipal userPrincipal = UserPrincipal.FindByIdentity(principalContext, userName);
//                         if(userPrincipal is not null)
//                         {
//                             //Get all direct groups, which the user belongs to.
//                             var userGroups = userPrincipal.GetGroups().ToList();
//                             if (userGroups.Count > 0)
//                             {
//                                 var groups = userGroups.Select(x => new UserGroup()
//                                 {
//                                     Name = x.Name,
//                                 }).ToList();
//                                 return new Result(true, groups);    
//                             }
//                             return new Result(true, new List<UserGroup>());
//                         }
//                     }
//                     return new Result(false, new List<UserGroup>());
//                 }
//             }
//             catch (Exception)
//             {
//                 _logger.LogError($"Unable to connect to AD and ValidateCredentials");
//                 return new Result(false, new List<UserGroup>());
//             }
//         }

//         public class Result
//         {
//             public Result(bool isSuccess, List<UserGroup> groups)
//             {
//                 IsSuccess = isSuccess;
//                 Groups = groups;
//             }
//             public bool IsSuccess { get; }
//             public bool IsFailure => !IsSuccess;
//             public List<UserGroup> Groups { get; set;} = new List<UserGroup>();
//         }

//         public class UserGroup
//         {
//             public string Name { get; set; } = string.Empty;
//         }
//     }
// }
