using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace NIHR.GovUk
{
    public static class NavigationExtensions
    {
        public static Navigation Navigation(this ViewDataDictionary viewData)
        {
            if (!viewData.TryGetValue("_Navigation", out var value)){
                value = new Navigation();
                viewData.Add("_Navigation", value);
            }

            return (value as Navigation)!;
        }

        public static bool HasNavigation(this ViewDataDictionary viewData) => viewData.Navigation().Items.Any();
    }
}
