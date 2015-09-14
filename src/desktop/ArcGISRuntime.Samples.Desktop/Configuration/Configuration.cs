using System.Collections.Generic;

namespace ArcGISRuntime.Samples.Desktop.Configuration
{
    /// <summary>
    /// カテゴリノードクラス
    /// </summary>
    public class CategoryNode
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="name">ページ名</param>
        public CategoryNode(string name)
        {
            this.Name = name;
            ChildNodes = new List<PageNode>();
        }

        /// <summary>
        /// カテゴリ名
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 子ページノード
        /// </summary>
        public List<PageNode> ChildNodes { get; set; }
    }
}
