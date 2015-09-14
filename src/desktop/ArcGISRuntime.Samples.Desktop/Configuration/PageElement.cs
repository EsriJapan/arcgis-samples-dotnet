using System.Configuration;

namespace ArcGISRuntime.Samples.Desktop.Configuration
{
    /// <summary>
    /// ページ エレメント
    /// </summary>
    public class PageElement : ConfigurationElement
    {
        /// <summary>
        /// ページのURL
        /// </summary>
        [ConfigurationProperty("url", IsRequired = true)]
        public string Url
        {
            get { return (string)base["url"]; }
            set { base["url"] = value; }
        }

        /// <summary>
        /// ページのカテゴリ
        /// </summary>
        [ConfigurationProperty("category", IsRequired = true)]
        public string Category
        {
            get { return (string)base["category"]; }
            set { base["category"] = value; }
        }

        /// <summary>
        /// ページのタイトル
        /// </summary>
        [ConfigurationProperty("title", IsRequired = true)]
        public string Title
        {
            get { return (string)base["title"]; }
            set { base["title"] = value; }
        }
    }
}
