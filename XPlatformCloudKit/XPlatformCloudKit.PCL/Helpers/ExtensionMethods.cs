using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XPlatformCloudKit.Models;

namespace XPlatformCloudKit.Helpers
{
    public static class ExtensionMethods
    {
        //returns a string by which the Groups are ordered in the ItemsShowcaseView
        //public static string GetOrderPreference(this IGrouping<string, Item> itemGroup)
        //{
        //    //if (itemGroup.Key.Contains("Azure"))
        //    //    return "1";
        //    //if (itemGroup.Key == "Dog")
        //    //    return "2";
        //    //if (itemGroup.Key == "Local Data")
        //    //    return "3";
        //    //if (itemGroup.Key == "HTML")
        //    //    return "4";
        //    //else
        //        return itemGroup.Key; //Sort all groups alphabetically
        //}

        //returns a LINQ query by which the Groups are ordered in the ItemsShowcaseView
        public static IEnumerable<Group<Item>> BuildQuery(this List<Item> items)
        {
            var query = items.GroupBy(i => i.Group);

            if (AppSettings.UseOrderByClause)
            {

                if (AppSettings.ByAscendingOrder)
                {
                    query = query.OrderBy(g => g.Key);
                }
                else
                {
                    query = query.OrderByDescending(g => g.Key);
                }
            }

            var group = query.Select(g => new Group<Item>(g.Key, g));

            return group;
        }
    }
}
