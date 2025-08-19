using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UmbracoV16.Core.ViewModels
{
    public class MemberLoginViewModel
    {
        public string FormTitle { get; set; } = string.Empty;
        public string? Username { get; set; }
        public string? Password { get; set; }
        public bool RememberMe { get; set; }
        //public string ReturnUrl { get; set; } = string.Empty;
    }
}
