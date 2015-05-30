using System.Collections.Generic;

namespace DnnConnectXF
{
    public class ViewModel
    {
        public List<PageGroup> PageGroups { get; set; }
    }

    public class PageGroup : List<DnnPage>
    {
        public string Title { get; set; }
        public string LongTitle { get; set; }
    }

    public class DnnPage
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
