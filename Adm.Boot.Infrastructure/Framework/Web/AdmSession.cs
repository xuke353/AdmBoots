using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using IdentityModel;
using Microsoft.AspNetCore.Http;

namespace AdmBoots.Infrastructure.Framework.Web {
    public class AdmSession {
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly ClaimsPrincipal _user;

        private int _userId = 0;
        private string _name;

        public AdmSession(IHttpContextAccessor httpContextAccessor) {
            _httpContextAccessor = httpContextAccessor;
            _user = _httpContextAccessor.HttpContext.User;
        }

        public int UserId {
            get {
                var nameIdentifier = _user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                if (nameIdentifier == null)
                    return _userId;
                return _userId = Convert.ToInt32(nameIdentifier.Value);
            }
        }


        public bool IsAuthenticated => _user.Identity.IsAuthenticated;

        public string UserName => _user.Identity.Name;

        public string Name {
            get {
                if (!string.IsNullOrEmpty(_name))
                    return _name;
                return _name = _user.Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.Name)?.Value;
            }
        }


    }
}
