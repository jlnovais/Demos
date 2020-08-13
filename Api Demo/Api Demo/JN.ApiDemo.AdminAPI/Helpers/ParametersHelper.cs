using System;
using System.Collections.Generic;
using System.Linq;

namespace JN.ApiDemo.AdminAPI.Helpers
{
    public static class ParametersHelper
    {

        public static IEnumerable<string> GetRoles(string rolesStr)
        {
            if (rolesStr == null)
                return null;

            var roles = string.IsNullOrWhiteSpace(rolesStr)
                ? new string[0]
                : rolesStr.Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries);

            return roles.Select(x => x.Trim());
        }

    }
}