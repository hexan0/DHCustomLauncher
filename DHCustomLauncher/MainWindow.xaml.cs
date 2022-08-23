using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

        public string[,] description = { 
            { "# LANGUAGE (0 = English, 1 = French, etc.)", "使用言語" },
            { "# MUSIC (1 for on)", "音楽(BGM)" },
            { "# Remote path of the latest game version information, used by the launcher for updates", "最新バージョン" },
            { "# Display resolution: 0 = do not change (default: 1024x768), 1 = 1152x864, 2 = 1280x768, 3 = 1280x1024, 4 = 1440x900, 5 = 1600x1024, 6 = user specified - see below", "ゲーム画面解像度" },
            { "# Screen Width (used if 6 is selected as screen resolution - see above)", "カスタム解像度の横幅" },
            { "# Screen Height (used if 6 is selected as screen resolution - see above)", "カスタム解像度の縦幅" },
            { "# Display mode: 0 = full screen, 1 = windowed", "ディスプレイモード" },
            { "# Start-up movie: 0 = skip, 1 = play", "オープニングムービーの再生" },
            { "# Extra debug logs (savedebug.txt): 0 = disabled, 1 = enabled, 2 = enabled + extra map statistics", "デバッグログ(savedebug.txt)" },
            { "# Load game sounds: 0 = no sounds, 1 = load sounds", "効果音(Sounds)のロード" },
            { "# Load unit sprites: 0 = no sprites, 1 = load unit sprites", "ユニットスプライトのロード" },
            { "# Load country specific unit and brigade pictures/models: 0 = load only generic pictures/models, 1 = load all available", "各国専用グラフィックのロード" },
            { "# Refresh map on resolutions higher then 1024x768 when not on Map mode. Enabling this will decrease game speed a bit (on higher resolutions only!), but resolves some cosmetic problems. 1 = Enabled, 0 = Disabled", "1024x768より高解像度で、マップモードでないときマップを更新するか。ゲームスピードが少し落ちる代わりに見た目の問題が解決します。" },
            { "# MODDIR folder. Default is Mods", "Modのフォルダを入れるフォルダ" },
            { "# Selected mod (must be a folder into MODDIR)", "Mod選択" },
            { "# Location of the game information in the Registry, lets the launcher determine the current game version", "現在バージョン" }
        };

        public void LoadConfig()
        {
            //FullScreen.IsChecked = true;
            //Resolution.SelectedIndex = 6;
            
            Char sharp = '#';
            String[] settings = {"","","","","","","","","","","","","","","","" };
            
            //ファイル読み込み
            StreamReader streamReader = new StreamReader("settings.cfg", Encoding.UTF8);
            int i = 0;
            while (streamReader.EndOfStream == false)
            {
                string text = streamReader.ReadLine();
                settings[i] = text.Substring(0, text.IndexOf(sharp) - 1);// '#'のふたつ前まで
                i++;
            }
            streamReader.Close();

            //設定反映
            Language.SelectedIndex = int.Parse(settings[0]);
            Music.IsChecked = StringToBool(settings[1]);
            //settings[2]
            Resolution.SelectedIndex = int.Parse(settings[3]);
            Width.Text = settings[4];
            Height.Text = settings[5];
            if (settings[6] == "0")
            {
                FullScreen.IsChecked = true;
                WindowMode.IsChecked = false;
            }
            else
            {
                FullScreen.IsChecked = false;
                WindowMode.IsChecked = true; 
            }
            OpeningMovie.IsChecked = StringToBool(settings[7]);
            if(settings[8] == "0")
            {
                DebugLog.IsChecked = false;
                DebugExtra.IsChecked = false;
            }
            else if(settings[8] == "1")
            {
                DebugLog.IsChecked = true;
                DebugExtra.IsChecked = false;
            }
            else
            {
                DebugLog.IsChecked = true;
                DebugExtra.IsChecked = true;
            }
            Sounds.IsChecked = StringToBool(settings[9]);
            UnitSprites.IsChecked = StringToBool(settings[10]);
            Specific.IsChecked = StringToBool(settings[11]);
            RefreshMap.IsChecked = StringToBool(settings[12]);
            ModsFolder.Text = settings[13];
            Mod.SelectedItem = settings[14];
            //settings[15]


        }

        public void SaveConfig()
        {
            String[] settings = { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" };
            //設定代入
            settings[0] = Convert.ToString(Language.SelectedIndex);
            settings[1] = (bool)Music.IsChecked ? "1" : "0";
            settings[2] = "http://darkest-hour-game.com/patches/DH_Version.txt";
            settings[3] = Convert.ToString(Resolution.SelectedIndex);
            settings[4] = Width.Text;
            settings[5] = Height.Text;
            settings[6] = (bool)WindowMode.IsChecked ? "1" : "0";
            settings[7] = (bool)OpeningMovie.IsChecked ? "1" : "0";
            settings[8] = (bool)DebugLog.IsChecked ? ((bool)DebugExtra.IsChecked ? "2" : "1") : "0";
            settings[9] = (bool)Sounds.IsChecked ? "1" : "0";
            settings[10] = (bool)UnitSprites.IsChecked ? "1" : "0";
            settings[11] = (bool)Specific.IsChecked ? "1" : "0";
            settings[12] = (bool)RefreshMap.IsChecked ? "1" : "0";
            settings[13] = ModsFolder.Text;
            settings[14] = Convert.ToString(Mod.SelectedItem);
            settings[15] = "HKEY_LOCAL_MACHINE\\SOFTWARE\\Darkest Hour Team\\Darkest Hour";
            //ファイル書き込み
            StreamWriter streamWriter = new StreamWriter("settings.cfg", false, Encoding.UTF8);
            for(int i = 0; i < 16; i++) streamWriter.WriteLine(settings[i] + " " + description[i, 0]);
            streamWriter.Close();
        }

        private void Load_Click(object sender, RoutedEventArgs e)
        {
            LoadConfig();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if(FullScreen.IsChecked != WindowMode.IsChecked) SaveConfig();
        }

        private void Launch_Click(object sender, RoutedEventArgs e)
        {
            //ゲーム本体起動
            System.Diagnostics.Process.Start("Darkest Hour.exe");
        }

        //数値限定用
        private void numOnly_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // 0-9のみ
            e.Handled = !new Regex("[0-9]").IsMatch(e.Text);
        }
        private void numOnly_PreviewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            // 貼り付けを許可しない
            if (e.Command == ApplicationCommands.Paste)
            {
                e.Handled = true;
            }
        }

        //文字列0,1→真偽値
        private bool StringToBool(String str)
        {
            return str == "1";
        }
    }
}
