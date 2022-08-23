using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DHCustomLauncher
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //DataContextプロパティにViewModelのインスタンスを入れる
            //DataContext(ViewModel)のプロパティがXAMLから参照可能になる
            this.DataContext = new MainViewModel();
        }

        public void LoadConfig()
        {

        }

        public void SaveConfig()
        {

        }

        private void Load_Click(object sender, RoutedEventArgs e)
        {
            LoadConfig();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            SaveConfig();
        }

        private void Launch_Click(object sender, RoutedEventArgs e)
        {
            //ゲーム本体起動
            System.Diagnostics.Process.Start("Darkest Hour.exe");
        }
    }
}
