using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

//“空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 上有介绍

namespace PCBeta3
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            this.listView.SelectedIndex = 1;
            SystemNavigationManager.GetForCurrentView().BackRequested += MainPage_BackRequested;

            var appView = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView();
            var titleBar = appView.TitleBar;
            titleBar.BackgroundColor = Color.FromArgb(0, 59, 59, 59); // Black
            titleBar.ButtonBackgroundColor = Color.FromArgb(0, 59, 59, 59); // Black
            titleBar.ForegroundColor = Color.FromArgb(0, 255, 255, 255); // White
            titleBar.ButtonForegroundColor = Color.FromArgb(0, 255, 255, 255); // White
        }

        private void News_Click()
        {
            this.webView.Navigate(new Uri("http://www.pcbeta.com"));
            this.splitView.IsPaneOpen = false;
        }

        private void Forum_Click()
        {
            this.webView.Navigate(new Uri("http://bbs.pcbeta.com"));
            this.splitView.IsPaneOpen = false;
        }

        private void Notify_Click()
        {
            this.webView.Navigate(new Uri("http://i.pcbeta.com/home.php?mod=space&do=notice"));
            this.splitView.IsPaneOpen = false;
        }

        private void Message_Click()
        {
            this.webView.Navigate(new Uri("http://i.pcbeta.com/home.php?mod=space&do=pm"));
            this.splitView.IsPaneOpen = false;
        }

        private async void inject()
        {
            string[] arguments = {
                    "var qr = document.getElementById('weixinqr');" +
                    "if (qr != null)" +
                        "qr.parentNode.removeChild(qr);" +
                    "var nv = document.getElementById('nv');" +
                    "if (nv != null)" +
                        "nv.parentNode.removeChild(nv);" +
                    "var lk = document.getElementById('category_lk');" +
                    "if (lk != null)" +
                        "lk.parentNode.removeChild(lk);" +
                    "var all = document.all;" +
                    "for (var i = 0; i < all.length; i++) {" +
                        "if (all[i].id.indexOf('BAIDU_DUP_wrapper_') != -1)" +
                            "all[i].parentNode.removeChild(all[i]);" +
                        "else if (all[i].id.indexOf('cproIframe') != -1)" +
                            "all[i].parentNode.removeChild(all[i]);" +
                    "}" +
                    "var block1 = document.getElementsByClassName('pb_tan x_l');" +
                    "if (block1[0] != null) {" +
                        "var p1 = block1[0].parentNode;" +
                        "if (p1 != null)" +
                            "p1.parentNode.removeChild(p1);" +
                    "}" +
                    "var block2 = document.getElementById('wp');" +
                    "if (block2 != null) {" +
                        "var s2 = block2.childNodes;" +
                        "var i = 0;" +
                        "for (i = s2.length - 1; i >= 0; i--) {" +
                            "if (s2[i].id != null && s2[i].id.indexOf('scbar') != -1)" +
                                "break;" +
                        "}" +
                        "for (i -= 1; i >= 0; i--)" +
                            "block2.removeChild(s2[i]);" +
                    "}" +

                    // 检查是否有消息、提醒
                    "var notify = document.getElementById('myprompt');" +
                    "var message = document.getElementById('pm_ntc');" +
                    "if (notify != null && message != null) {" +
                        "var result = notify.text + ' ' + message.text;" +
                        "result" +
                    "}"
            };
            String t = await this.webView.InvokeScriptAsync("eval", arguments);
            String[] split = t.Split(' ');
            int notify = 0, message = 0, option = 0;
            if (split.Length == 2)
            {
                if (split[0].StartsWith("提醒") && split[0].Length > 2)
                {
                    notify = 1; option = 1;
                }
                if (split[1].StartsWith("消息") && split[1].Length > 2)
                {
                    message = 1; option = 1;
                }
            }
            if (notify == 1)
                this.notifyIcon.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0)); // Red
            else
                this.notifyIcon.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255)); // White
            if (message == 1)
                this.messageIcon.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0)); // Red
            else
                this.messageIcon.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255)); // White
            if (splitView.DisplayMode == SplitViewDisplayMode.Overlay && option == 1)
                this.splitViewOption.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0)); // Red
            else
                this.splitViewOption.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255)); // White
        }

        private void webView_DOMContentLoaded(WebView sender, WebViewDOMContentLoadedEventArgs args)
        {
            if (args.Uri != null)
            {
                inject();
            }
            if (this.webView.CanGoBack)
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
        }

        private void webView_FrameDOMContentLoaded(WebView sender, WebViewDOMContentLoadedEventArgs args)
        {
            if (args.Uri != null)
            {
                inject();
            }
        }

        private void webView_NewWindowRequested(WebView sender, WebViewNewWindowRequestedEventArgs args)
        {
            if (args.Referrer.Host != null)
            {
                webView.Navigate(args.Uri);
                args.Handled = true;
            }
        }

        private void webView_UnviewableContentIdentified(WebView sender, WebViewUnviewableContentIdentifiedEventArgs args)
        {
            Windows.Foundation.IAsyncOperation<bool> b = Windows.System.Launcher.LaunchUriAsync(args.Uri);
        }

        private void MainPage_BackRequested(object sender, BackRequestedEventArgs e)
        {
            if (this.webView.CanGoBack)
                this.webView.GoBack();
        }

        private void listView_ItemClick(object sender, ItemClickEventArgs e)
        {
            Object o = e.ClickedItem;
            if (o == null)
                return;
            StackPanel item = o as StackPanel;
            if (item == this.newsItem)
            {
                News_Click();
            }
            else if (item == this.forumItem)
            {
                Forum_Click();
            }
            else if (item == this.notifyItem)
            {
                Notify_Click();
            }
            else if (item == this.messageItem)
            {
                Message_Click();
            }
        }

        private void Option_Click(object sender, RoutedEventArgs e)
        {
            this.splitView.IsPaneOpen = !this.splitView.IsPaneOpen;
        }
    }
}
