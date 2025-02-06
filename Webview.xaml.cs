using System.Windows;

namespace testControl.TestApp
{
    /// <summary>
    /// Interaction logic for Webview.xaml
    /// </summary>
    public partial class Webview : Window
    {
        public Webview()
        {
            InitializeComponent();
            InitializeWebView();
        }

        private async void InitializeWebView()
        {
            await webview.EnsureCoreWebView2Async(null);
            webview.CoreWebView2.Navigate("https://github.com/hieuvm688");
            webview.CoreWebView2.NavigationCompleted += (sender, args) => { };
        }
        private async void ExcuteScript()
        {
            string next2 = @"
            document.querySelector('button#global-create-menu-anchor').click();
            document.querySelector('a[href=""/new""]').click();
            setTimeout(function() {
                var inputField = document.querySelector('input[data-component=""input""].UnstyledTextInput__ToggledUnstyledTextInput-sc-14ypya-0.jkNcAv');
                if (inputField) {
                    inputField.value = 'Check automation create in wpf';
                    inputField.dispatchEvent(new Event('input', { bubbles: true })); // Kích hoạt sự kiện input để cập nhật giá trị
                    console.log('Input field is fill');
                    document.querySelector('button.Box-sc-g0xbh4-0.jLvIcQ.prc-Button-ButtonBase-c50BI').click();
                    console.log('Đã click button Create repository');
                } else {
                    console.log('Input field not found');
                }
            }, 2000);";
           
            await webview.CoreWebView2.ExecuteScriptAsync(next2);
        }
        private void btnReloadAuto_Click(object sender, RoutedEventArgs e)
        {
            ExcuteScript();
        }
    }
}
