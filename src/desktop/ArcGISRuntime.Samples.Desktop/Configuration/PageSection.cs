using System.Configuration;

namespace ArcGISRuntime.Samples.Desktop.Configuration
{
    /// <summary>
    /// ページ設定セクション
    /// </summary>
    public class PageSection : ConfigurationSection
    {
        [ConfigurationProperty("pages", IsRequired = true)]
        [ConfigurationCollection(typeof(PageGroup), AddItemName = "addpage", ClearItemsName = "clearpage", RemoveItemName = "removepage")]
        public PageGroup Pages
        {
            get { return (PageGroup)base["pages"]; }
            set { base["pages"] = value; }
        }
    }
}
