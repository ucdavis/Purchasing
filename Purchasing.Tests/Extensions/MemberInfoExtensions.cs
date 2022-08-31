using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Purchasing.Tests.Extensions
{
    public static class MemberInfoExtensions
    {
        public static object[] GetFilteredCustomAttributes(this MemberInfo memberInfo, bool inherit)
        {
            return memberInfo.GetCustomAttributes(inherit)
                .Where(a => a.GetType().Namespace != "System.Diagnostics")
                .ToArray();
        }

        public static IList<CustomAttributeData> GetFilteredCustomAttributeData(this MemberInfo memberInfo)
        {
            return CustomAttributeData.GetCustomAttributes(memberInfo)
                .Where(a => a.AttributeType.Namespace != "System.Diagnostics")
                .ToList();
        }
    }
}
