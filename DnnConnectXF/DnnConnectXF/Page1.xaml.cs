using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace DnnConnectXF
{
    public partial class Page1
    {
        public Page1()
        {
            InitializeComponent();

            // N.B. LoadData is an async method so will run as a task,
            // not synchronously

            LoadData();
        }

        private async Task LoadData()
        {
            // Fetch the list of pages from DNN

            var client = new HttpClient();

            client.DefaultRequestHeaders.Add(
                "Authorization",
                "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes("dnnconnect:dnnconnect")));

            var pageJson =
                JObject.Parse(
                    await
                        client.GetStringAsync(
                            "http://mydnnsite/DesktopModules/InternalServices/API/ItemListService/GetPages"));

            var pages = ParsePageJson(pageJson["Tree"]);

            // Group the pages by first letter of name

            var pageGroups = pages.Select(page => page.Name.Substring(0, 1)).Distinct().OrderBy(ch => ch).Select(
                ch =>
                {
                    var pageGroup = new PageGroup
                    {
                        Title = ch.ToUpperInvariant(),
                        LongTitle = "Starts with " + ch.ToUpperInvariant()
                    };
                    pageGroup.AddRange(pages.Where(page => page.Name.Substring(0, 1) == ch).OrderBy(page => page.Name));
                    return pageGroup;
                }).ToList();

            // Bind the view XAML to the data

            BindingContext = new ViewModel { PageGroups = pageGroups };
        }

        private static List<DnnPage> ParsePageJson(JToken json)
        {
            var result = new List<DnnPage>();

            // If the page has a name (so not root for example), add it to the list

            var thisPage = json["data"];
            if (thisPage != null && thisPage.Type != JTokenType.Null && thisPage["value"] != null
                && thisPage["value"].Type == JTokenType.String)
                result.Add(
                    new DnnPage { Name = (string)thisPage["value"], Description = "Page ID " + (string)thisPage["key"] });

            // Add any child pages

            var children = json["children"] as JArray;
            if (children != null)
            {
                foreach (var child in children)
                    result.AddRange(ParsePageJson(child));
            }

            return result;
        }
    }
}