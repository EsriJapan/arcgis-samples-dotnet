using System.Configuration;

namespace ArcGISRuntime.Samples.Desktop.Configuration
{
    public class PageGroup : ConfigurationElementCollection
    {
        /// <summary>
        /// 子エレメントのタイプ
        /// </summary>
        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.AddRemoveClearMap;
            }
        }

        /// <summary>
        /// 新しい子エレメントを作成
        /// </summary>
        protected override ConfigurationElement CreateNewElement()
        {
            return new PageElement();
        }

        /// <summary>
        /// 子エレメントのキーを返す
        /// </summary>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((PageElement)element).Url;
        }

        /// <summary>
        /// インデクサ
        /// </summary>
        public PageElement this[int index]
        {
            get { return (PageElement)BaseGet(index); }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        /// <summary>
        /// name インデクサ
        /// </summary>
        public PageElement this[string name]
        {
            get { return (PageElement)BaseGet(name); }
        }
    }
}
