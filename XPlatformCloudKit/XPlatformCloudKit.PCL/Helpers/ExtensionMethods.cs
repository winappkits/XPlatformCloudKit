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
            var item = (IEnumerable<Item>)null;
            var group = (IEnumerable<IGrouping<string, Item>>)null;

            if (AppSettings.OrderByItem)
            {
                if (AppSettings.ItemByAscending)
                {
                    item = items.OrderBy(i => i.Title);
                }
                else
                {
                    item = items.OrderByDescending(i => i.Title);
                }

                group = item.GroupBy(i => i.Group);
            }
            else
            {
                group = items.GroupBy(i => i.Group);
            }

            if (AppSettings.OrderbyGroup)
            {
                if (AppSettings.GroupByAscending)
                {
                    group = group.OrderBy(g => g.Key);
                }
                else
                {
                    group = group.OrderByDescending(g => g.Key);
                }
            }

            var list = group.Select(g => new Group<Item>(g.Key, g));

            return list;
        }
    }
}
