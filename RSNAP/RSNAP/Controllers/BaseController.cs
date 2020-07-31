using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using RSNAP.Models;

namespace RSNAP.Controllers
{
    public class BaseController : Controller
    {
        public string _Email { get; set; }
        public string _ROLE { get; set; }
        public string _Name { get; set; }
        public string _RoleText { get; set; }

        public void FillSessionInfo()
        {
            if (!HttpContext.User.Identity.IsAuthenticated)
            {
                return;
            }

            IEnumerable<Claim> claims = HttpContext.User.Claims;


            foreach (Claim claim in claims)
            {
                switch (claim.Type)
                {
                    case ClaimTypes.NameIdentifier:
                        _Name = claim.Value;
                        break;
                    case ClaimTypes.Role:
                        _ROLE = claim.Value;
                        break;
                    case ClaimTypes.Email:

                        _Email = claim.Value;
                        break;
                }
            }

            // Disable All fields and Modification buttons, only search fields and button enabled
            if (_ROLE.ToUpper() == RoleEnum.FO.GetDescription())
            {
                _RoleText = "FO";
            }
            else if (_ROLE.ToUpper() == RoleEnum.CO.GetDescription())
            {
                _RoleText = "CO";
            }
            else if (_ROLE.ToUpper() == RoleEnum.RO.GetDescription())
            {
                _RoleText = "RO";
            }
        }
    }
}