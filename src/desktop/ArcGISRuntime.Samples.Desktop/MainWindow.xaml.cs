using ArcGISRuntime.Samples.Desktop.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace ArcGISRuntime.Samples.Desktop
{
    public partial class MainWindow : Window
    {
        private MenuItem _currentMenuItem;          //現在選択されているメニューアイテム

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            LoadSamplePages();
        }

        /// <summary>
        /// サンプル ページの読み込み
        /// </summary>
        private void LoadSamplePages()
        {
            //ページメニュー
            List<MenuItem> menuFromPages = new List<MenuItem>();

            //App.Configからページ一覧を取得
            PageSection section = ConfigurationManager.GetSection("pageSection") as PageSection;

            if (section != null)
            {
                //ページメニュー
                foreach (PageElement page in section.Pages)
                {
                    var pageItem = new MenuItem(){
                        Header = page.Title,
                    };
                    pageItem.Click += (s, e) => { NavigateToPage(page.Url, s as MenuItem); };

                    var categoryItem = _menu.Items.Cast<MenuItem>().Where(item => item.Header == page.Category).FirstOrDefault();
                    if (categoryItem != null)
                    {
                        //すでにカテゴリが存在する場合は、カテゴリの子メニューとして追加
                        categoryItem.Items.Add(pageItem);
                    }
                    else
                    {
                        //カテゴリが存在しない場合は新規作成し、子メニューとして追加
                        categoryItem = new MenuItem(){ Header = page.Category };
                        _menu.Items.Add(categoryItem);
                        categoryItem.Items.Add(pageItem);
                    }
                }
            }
        }

        /// <summary>
        /// サンプル ページへのナビゲーション
        /// </summary>
        private void NavigateToPage(string url, MenuItem selectedMenuItem)
        {
            //現在のメニューアイテムのチェックを解除
            if (_currentMenuItem != null)
            {
                _currentMenuItem.IsChecked = false;
            }

            //サンプル ページへメイン フレームをナビゲート
            _mainFrame.NavigationService.Navigate(new Uri(url, UriKind.Relative));

            //選択されたメニュー アイテムをチェック
            _currentMenuItem = selectedMenuItem;
            _currentMenuItem.IsChecked = true;
        }
    }
}
