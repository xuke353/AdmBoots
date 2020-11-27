using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using IdentityModel;
using Microsoft.AspNetCore.Http;

namespace AdmBoots.Infrastructure.Framework.Web {

    public class AdmSession : IAdmSession {

        private readonly ClaimsIdentity _claimsIdentity;
        private int? _userId;
        private string _name;

        public AdmSession(IHttpContextAccessor httpContextAccessor) {
            _claimsIdentity = httpContextAccessor?.HttpContext?.User?.Identity as ClaimsIdentity;
        }

        public int? UserId {
            get {
                if (_userId != null)
                    return _userId;
                var claim = _claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                return claim == null ? null : (_userId = Convert.ToInt32(claim.Value));
            }
        }

        public bool IsAuthenticated => _claimsIdentity.IsAuthenticated;

        public string UserName => _claimsIdentity.Name;

        public string Name {
            get {
                if (!string.IsNullOrEmpty(_name))
                    return _name;
                return _name = _claimsIdentity.FindFirst(JwtClaimTypes.Name)?.Value;
            }
        }
    }
}
