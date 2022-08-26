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
            ContentRendered += (s, e) =>
            {
                try
                {
                    LoadConfig();
                }
                catch
                {
                    MessageBox.Show(
                        "setting.cfgが見つかりません。\nDarkest Hour.exeやsetting.cfgが入っているフォルダにDHCustomLauncher.exeを入れてください。",
                        "DH Custom Launcher"
                        );
                }
                
            };
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
            //Mods内のディレクトリ名取得
            DirectoryInfo di = new DirectoryInfo(settings[13]);
            // ディレクトリ直下のすべてのディレクトリ一覧を取得する
            DirectoryInfo[] diAlls = di.GetDirectories();
            Mod.Items.Clear();
            Mod.Items.Add("(none)");
            foreach (DirectoryInfo d in diAlls)
            {
                Mod.Items.Add(d.Name);
            }
            Mod.SelectedItem = settings[14] == "" ? "(none)" : settings[14];
            //settings[15]
            //Mod画像更新
            RefreshIcon(ModsFolder.Text, Convert.ToString(Mod.SelectedItem));
            LoadInfo(ModsFolder.Text, Convert.ToString(Mod.SelectedItem));

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
            settings[14] = Convert.ToString(Mod.SelectedItem) == "(none)" ? "" : Convert.ToString(Mod.SelectedItem);
            settings[15] = "HKEY_LOCAL_MACHINE\\SOFTWARE\\Darkest Hour Team\\Darkest Hour";
            //ファイル書き込み
            StreamWriter streamWriter = new StreamWriter("settings.cfg", false, Encoding.UTF8);
            for(int i = 0; i < 16; i++) streamWriter.WriteLine(settings[i] + " " + description[i, 0]);
            streamWriter.Close();
            //Mod画像更新
            RefreshIcon(ModsFolder.Text, Convert.ToString(Mod.SelectedItem));
            SaveInfo(ModsFolder.Text, Convert.ToString(Mod.SelectedItem));
        }

        public void RefreshIcon(string mods, string selectedMod)
        {
            if (selectedMod == "(none)" || selectedMod == "") ModImage.Visibility = Visibility.Hidden;
            else
            {
                ModImage.Visibility = Visibility.Visible;
                try
                {
                    ModImage.Source = PathToImage(mods + "\\" + selectedMod + "\\icon.ico");
                }
                catch
                {
                    ModImage.Visibility = Visibility.Hidden;
                }
                
            }
        }

        public int playerBit = 769;
        public int ipBit = 833;
        public int lengthBit = 16;
        public void LoadInfo(string mods, string selectedMod)
        {
            if (selectedMod == "(none)" || selectedMod == "")
            {
                PlayerName.Text = ReadBinary("config.eu", playerBit, lengthBit);
                IP.Text = ReadBinary("config.eu", ipBit, lengthBit);
            }
            else
            {
                try
                {
                    PlayerName.Text = ReadBinary(mods + "\\" + selectedMod + "\\config.eu", playerBit, lengthBit);
                    IP.Text = ReadBinary(mods + "\\" + selectedMod + "\\config.eu", ipBit, lengthBit);
                }
                catch
                {
                    PlayerName.Text = ReadBinary("config.eu", playerBit, lengthBit);
                    IP.Text = ReadBinary("config.eu", ipBit, lengthBit);
                }
            }
        }

        public void SaveInfo(string mods, string selectedMod)
        {
            if (selectedMod == "(none)" || selectedMod == "")
            {
                WriteBinary("config.eu", PlayerName.Text, playerBit, lengthBit);
                WriteBinary("config.eu", IP.Text, ipBit, lengthBit);
            }
            else
            {
                try
                {
                    WriteBinary(mods + "\\" + selectedMod + "\\config.eu", PlayerName.Text, playerBit, lengthBit);
                    WriteBinary(mods + "\\" + selectedMod + "\\config.eu", IP.Text, ipBit, lengthBit);
                }
                catch
                {
                    WriteBinary("config.eu", PlayerName.Text, playerBit, lengthBit);
                    WriteBinary("config.eu", IP.Text, ipBit, lengthBit);
                }
            }
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

        private void SaveLaunch_Click(object sender, RoutedEventArgs e)
        {
            if (FullScreen.IsChecked != WindowMode.IsChecked)
            {
                SaveConfig();
                //ゲーム本体起動
                System.Diagnostics.Process.Start("Darkest Hour.exe");
            }
        }

        //Mod変更
        private void Mod_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshIcon(ModsFolder.Text, Convert.ToString(Mod.SelectedItem));
            LoadInfo(ModsFolder.Text, Convert.ToString(Mod.SelectedItem));
        }

        //数値限定用
        private void NumOnly_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // 0-9のみ
            e.Handled = !new Regex("[0-9]").IsMatch(e.Text);
        }
        //英数字限定用
        private void EnOnly_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // 
            e.Handled = !new Regex("[-A-Za-z_./:0-9]").IsMatch(e.Text);
        }
        //限定用
        private void Only_PreviewExecuted(object sender, ExecutedRoutedEventArgs e)
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

        //パス→BitmapImage
        public BitmapImage PathToImage(string path, int pixel = 500)
        {
            BitmapImage bmpImage = new BitmapImage();
            using (FileStream stream = File.OpenRead(path))
            {
                bmpImage.BeginInit();
                bmpImage.StreamSource = stream;
                bmpImage.DecodePixelWidth = pixel;
                bmpImage.CacheOption = BitmapCacheOption.OnLoad;
                bmpImage.CreateOptions = BitmapCreateOptions.None;
                bmpImage.EndInit();
                bmpImage.Freeze();
            }
            return bmpImage;
        }

        public string ReadBinary(string path, int start, int length)
        {
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);

            int fileSize = (int)fs.Length; // ファイルのサイズ
            byte[] buf = new byte[fileSize]; // データ格納用配列

            int readSize; // Readメソッドで読み込んだバイト数
            int remain = fileSize; // 読み込むべき残りのバイト数
            int bufPos = 0; // データ格納用配列内の追加位置

            while (remain > 0)
            {
                // 1024Bytesずつ読み込む
                readSize = fs.Read(buf, bufPos, Math.Min(1024, remain));

                bufPos += readSize;
                remain -= readSize;
            }
            fs.Dispose();

            byte[] buf_out = new byte[length];
            for (int i = 0; i < length; i++)
            {
                buf_out[i] = buf[start + i];
            }

            return ByteToString(buf_out);
        }

        public void WriteBinary(string path, string str, int start, int length = 0)
        {
            if (length == 0) length = str.Length;
            FileStream fsr = new FileStream(path, FileMode.Open, FileAccess.Read);
            FileStream fsw = new FileStream(path, FileMode.Open, FileAccess.Write);
            int fileSize = (int)fsr.Length; // ファイルのサイズ
            byte[] buf = new byte[fileSize]; // データ格納用配列

            int readSize; // Readメソッドで読み込んだバイト数
            int remain = fileSize; // 読み込むべき残りのバイト数
            int bufPos = 0; // データ格納用配列内の追加位置

            //読み込み
            while (remain > 0)
            {
                // 1024Bytesずつ読み込む
                readSize = fsr.Read(buf, bufPos, Math.Min(1024, remain));

                bufPos += readSize;
                remain -= readSize;
            }
            fsr.Dispose();

            //書き換え
            byte[] write_buf = StringToByte(str);
            for (int i = 0; i < length; i++)
            {
                if (i < str.Length) buf[start + i] = write_buf[i];
                else buf[start + i] = 0x00; //0埋め
            }

            //書き込み
            fsw.Write(buf, 0, fileSize);
            fsw.Dispose();

            
        }

        public byte[] StringToByte(string str)
        {
            return Encoding.GetEncoding("Shift_JIS").GetBytes(str);
        }
        public string ByteToString(byte[] bytes)
        {
            return Encoding.GetEncoding("Shift_JIS").GetString(bytes);
        }
    }
}
