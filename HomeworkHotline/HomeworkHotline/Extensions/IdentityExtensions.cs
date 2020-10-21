using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Principal;
using System.Security.Claims;

namespace HomeworkHotline.Extensions
{
    public static class IdentityExtensions
    {
        public static string GetFirstName(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("FirstName");
            // Test for null to avoid issues during local testing
            return (claim != null) ? claim.Value : string.Empty;
        }

        public static string GetLastName(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("LastName");
            // Test for null to avoid issues during local testing
            return (claim != null) ? claim.Value : string.Empty;
        }

        public static string GetHireDate(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("HireDate");
            // Test for null to avoid issues during local testing
            return (claim != null) ? claim.Value : string.Empty;
        }

        public static string GetMNPSEmpoyeeNo(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("MNPSEmployeeNo");
            // Test for null to avoid issues during local testing
            return (claim != null) ? claim.Value : string.Empty;
        }


    }
}