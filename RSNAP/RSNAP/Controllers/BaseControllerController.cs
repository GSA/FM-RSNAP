using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace RSNAP.Controllers
{
    public class BaseControllerController : Controller
    {
        public string _Email { get; set; }
        public string _ROLE { get; set; }
        public string _Name { get; set; }

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
        }
    }
}