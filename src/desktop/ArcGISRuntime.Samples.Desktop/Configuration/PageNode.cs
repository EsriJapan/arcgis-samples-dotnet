namespace ArcGISRuntime.Samples.Desktop.Configuration
{
    /// <summary>
    /// ページノードクラス
    /// </summary>
    public class PageNode
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="name">ページ名</param>
        /// <param name="url">XAMLのURL</param>
        public PageNode(string name, string url)
        {
            this.Name = name;
            this.Url = url;
        }

        /// <summary>
        /// カテゴリ名
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// ナビゲーションURL
        /// </summary>
        public string Url { get; private set; }
    }
}
