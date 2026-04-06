using System.Diagnostics;
using System.IO;
using System.Media;
using System.Net.Http;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Security.Policy;
using System.Text;
using System.Text.Json;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Path = System.IO.Path;
using System;
namespace Diamond_Clicker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly HttpClient client = new HttpClient();
        //音
        SoundPlayer Main_Sound = new SoundPlayer(@"Content\Sound\Tsuruhashi.wav");
        SoundPlayer Ok_Sound = new SoundPlayer(@"Content\Sound\Ok.wav");
        SoundPlayer No_Sound = new SoundPlayer(@"Content\Sound\No.wav");
        //Timer
        DispatcherTimer One_S;
        //基本設定
        ulong Diamond = 0;
        ulong Ck = 1;
        ulong S = 0;
        ulong Ck_Plus = 1;
        ulong S_Plus = 1;
        bool Black_Bool = false;
        //商品
        uint Tsuruhashi_int = 1;
        uint Doriru_int = 1;
        uint Kojou_int = 1;
        uint Inbou_int = 1;
        uint Kodai_Int = 1;
        //商品設定[]
        uint Kodai_Option_int = 500000;
        uint Kodai_Option_Ck_int = 250;
        uint Kodai_Option_S_int = 50;

        uint Kojou_Option_int = 5000;
        uint Kojou_Option_Ck_int = 50;
        uint Kojou_Option_S_int = 20;

        uint Doriru_Option_int = 500;
        uint Doriru_Option_Ck_int = 10;
        uint Doriru_Option_S_int = 5;

        uint Tsuruhashi_Option_int = 10;
        uint Tsuruhashi_Option_Ck_int = 1;
        uint Tsuruhashi_Option_S_int = 1;

        uint Inbou_Option_int = 50000;
        uint Inbou_Option_Ck_int = 100;
        uint Inbou_Option_S_int = 40;

        uint Sale_Option_int = 100;
        uint CkPlus_Option_int = 50;
        uint SPlus_Option_int = 50;
        uint Gatya_Option_int = 100;
        //tmp
        ulong Tmp_Shop_All;
        ulong Tmp_Shop_Old;
        string Tmp_Shop_Old_String;
        uint Tmp_Shop_Money;
        uint Tmp_Shop_Ck_Plus;
        string version = "1.30";
        uint Tmp_Shop_S_Plus;
        //random
        Random R = new Random();
        public MainWindow()
        {
            Sound_Loading();
            Data_Loading();
            InitializeComponent();
            Color_Option();
            Content_Loading();
            Timer_Loading();
            this.Loaded += async (s, e) => await NewVersion();
        }

        private async Task NewVersion()
        {
            int Random = R.Next(100);
            string url = $"https://raw.githubusercontent.com/CShapDaisuki-x64/Diamond_Clicker/refs/heads/master/Version.txt?t={Random}";
            // 実際に渡されるURLをカッコで囲んで出力し、余計な空白がないか見る
            Debug.WriteLine($"[DEBUG] Requesting URL: [{url}]");

            try
            {
                string content = await client.GetStringAsync(url);
                if(version != content)
                {
                    MessageBoxResult Version_Info = MessageBox.Show($"新しいバージョンがあります\r\n" +
                        $"更新しますか？\r\n" +
                        $"新バージョン名:{content}",
                        "Diamond_Clicker",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Information);
                    if(Version_Info == MessageBoxResult.Yes)
                    {
                        var startInfo = new System.Diagnostics.ProcessStartInfo("https://github.com/CShapDaisuki-x64/Diamond_Clicker/releases");
                        startInfo.UseShellExecute = true;
                        System.Diagnostics.Process.Start(startInfo);
                    }
                }
            }
            catch (UriFormatException ex)
            {
                Debug.WriteLine($"URLの形式エラーです: {ex.Message}");
                MessageBox.Show($"エラー\r\n新バージョン情報が取得出来ません\r\nURL取得に関するエラー\r\n{ex.Message}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"エラー: {ex.Message}");
                MessageBox.Show($"エラー\r\n新バージョン情報が取得出来ません\r\n{ex.Message}");
            }
        }
        private void Timer_Loading()
        {
            One_S = new DispatcherTimer();
            One_S.Tick += One_S_Tick;
            One_S.Interval = TimeSpan.FromSeconds(1);
            One_S.Start();
        }

        private void One_S_Tick(object sender, EventArgs e)
        {
            try
            {
                checked
                {
                    Diamond += S;
                    Content_Loading();
                }
            }
            catch(OverflowException)
            {
                MessageBox.Show("ゲームクリア！！\r\n" +
                    "これを気にセーブデータを初期化して\r\n" +
                    "一からやり直しましょう！",
                    "Diamond_Clicker",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                All_Del();
            }
        }

        private void Tmp_Shop_All_Setting()
        {
            Tmp_Shop_All = (Tsuruhashi_int + Doriru_int + Kojou_int);
        }
        private void Sound_Loading()
        {
            Main_Sound.Load();
            No_Sound.Load();
            Ok_Sound.Load();
        }
        private void Shop_del()
        {
            Tsuruhashi_int = 1;
            Doriru_int = 1;
            Kojou_int = 1;
            Inbou_int = 1;
        }
        public class SaveData 
        {
            public ulong Diamond { get; set; }
            public ulong Ck { get; set; } 
            public ulong S { get; set; } 
            public ulong Ck_Plus { get; set; }
            public ulong S_Plus { get; set; }
            public bool Black_Bool { get; set; }
            public uint Tsuruhashi_int { get; set; }
            public uint Doriru_int { get; set; }
            public uint Kojou_int { get; set; }
            public uint Inbou_int { get; set; }
            public uint Kodai_int { get; set; }
        }
        private void Save()
        {
            try
            {
                string folderPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "Diamond_Clicker"
                );

                Directory.CreateDirectory(folderPath);

                string savePath = Path.Combine(folderPath, "save.json");

                SaveData data = new SaveData
                {
                    Diamond = Diamond,
                    Ck = Ck,
                    S = S,
                    Ck_Plus = Ck_Plus,
                    S_Plus = S_Plus,
                    Black_Bool = Black_Bool,
                    Tsuruhashi_int = Tsuruhashi_int,
                    Doriru_int = Doriru_int,
                    Kojou_int = Kojou_int,
                    Inbou_int = Inbou_int,
                    Kodai_int = Kodai_Int
                };

                string json = JsonSerializer.Serialize(data, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                File.WriteAllText(savePath, json);
            }
            catch (Exception error)
            {
                MessageBox.Show($"セーブに失敗しました。\nエラー: {error.Message}");
            }
        }
        private void Int_Fixes()
        {
            Ck = Math.Max(Ck, 1);
            Ck_Plus = Math.Max(Ck_Plus, 1);
            S_Plus = Math.Max(S_Plus, 1);
            Tsuruhashi_int = Math.Max(Tsuruhashi_int, 1);
            Doriru_int = Math.Max(Doriru_int, 1);
            Kojou_int = Math.Max(Kojou_int, 1);
            Inbou_int = Math.Max(Inbou_int, 1);
            Kodai_Int = Math.Max(Kodai_Int, 1);
        }
        private void Content_Loading()
        {
            Text_Loading();
            Shop_Color_Loading();
        }
        private void Shop_Color_Loading()
        {
            Tmp_Shop_All_Setting();
            var Shop_Color_If = new List<(Button Shop_Button,
                ulong Shop_Old,
                ulong Shop_New,
                uint Shop_int)>
            {   (GatyaGatya_Button, 2, 1, Gatya_Option_int),
                (Shop_Tsuruhashi_Button, 2, Tsuruhashi_int, Tsuruhashi_Option_int),
                (Shop_Sale_Button, Tsuruhashi_int,Tmp_Shop_All, Sale_Option_int),
                (Shop_CkPlus_Button, Tsuruhashi_int, Tmp_Shop_All, CkPlus_Option_int),
                (Shop_SPlus_Button, Tsuruhashi_int, Tmp_Shop_All, SPlus_Option_int),
                (Shop_Doriru_Button, Tsuruhashi_int, Doriru_int, Doriru_Option_int),
                (Shop_Kojou_Button, Doriru_int, Kojou_int, Kojou_Option_int),
                (Shop_Inbou_Button, Kojou_int, Inbou_int, Inbou_Option_int),
                (Shop_Kodai_Button, Inbou_int, Kodai_Int, Kodai_Option_int)
            };
            foreach (var (Shop_Button, Shop_Old, Shop_New, Shop_int) 
                in Shop_Color_If)
            {
                if (Shop_Old > 1 && Diamond >= (Shop_New * Shop_int))
                {
                    Shop_Button.Background = Brushes.Gainsboro;
                }
                else
                {
                    Shop_Button.Background = Brushes.Gray;
                }
            }
        }
        private void Shop(ref uint Tmp_Shop_Ref)
        {
            if(Tmp_Shop_Old > 1)
            {
                if(Diamond >= Tmp_Shop_Money * Tmp_Shop_Ref)
                {
                    Ck += (Tmp_Shop_Ck_Plus * Ck_Plus);
                    S += (Tmp_Shop_S_Plus * S_Plus);
                    Diamond -= Tmp_Shop_Ref * Tmp_Shop_Money;
                    Tmp_Shop_Ref += 2;
                    Ok_Sound.Play();
                }
                else
                {
                    No_Sound.Play();
                }
            }
            else
            {
                No_Sound.Play();
                MessageBox.Show($"{Tmp_Shop_Old_String}を買ってから\r\n買ってください",
                    "Diamond_Clicker",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            Content_Loading();
        }
        private void Data_Loading()
        {
            try
            {
                string folderPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Diamond_Clicker"
                );

                string savePath = Path.Combine(folderPath, "save.json");

                if (File.Exists(savePath))
                {
                    string json = File.ReadAllText(savePath);

                    SaveData? data = JsonSerializer.Deserialize<SaveData>(json);

                    if (data != null)
                    {
                        Diamond = data.Diamond;
                        Ck = data.Ck;
                        S = data.S;
                        Ck_Plus = data.Ck_Plus;
                        S_Plus = data.S_Plus;
                        Black_Bool = data.Black_Bool;
                        Tsuruhashi_int = data.Tsuruhashi_int;
                        Doriru_int = data.Doriru_int;
                        Kojou_int = data.Kojou_int;
                    }
                }
            }catch(Exception)
            {
                MessageBox.Show("セーブデータが存在しないか不正です\r\n" +
                    "新しいセーブデータを作成します", "Diamond_Clicker", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            Int_Fixes();
        }
        private void Text_Loading()
        {
            Setting_Vertion.Text = (version);
            Tmp_Shop_All_Setting();
            Main_Text.Text = $"{Diamond:N0}ダイヤ\r\n" +
                $"{Ck}ダイヤ/クリック\r\n" +
                $"{S}ダイヤ/秒";
            if(Black_Bool)
            {
                Setting_Color_Button.Content = "ライトモードにする●";
            }
            else
            {
                Setting_Color_Button.Content = "ダークモードにする●";
            }
            var Shop_Text = new List<(TextBlock Shop_TextBlock, string Shop_Item,
                uint Shop_Money,
                uint Shop_Money_Option,
                uint Shop_Ck,
                uint Shop_S)>
            {
                (Shop_Tsuruhashi_Text, 
                "ツルハシ", 
                Tsuruhashi_int, 
                Tsuruhashi_Option_int, 
                Tsuruhashi_Option_Ck_int, 
                Tsuruhashi_Option_S_int),
                (Shop_Doriru_Text,
                "ドリル",
                Doriru_int,
                Doriru_Option_int,
                Doriru_Option_Ck_int,
                Doriru_Option_S_int),
                (Shop_Kojou_Text,
                "工場",
                Kojou_int,
                Kojou_Option_int,
                Kojou_Option_Ck_int,
                Kojou_Option_S_int),
                (Shop_Inbou_Text,
                "秘密結社",
                Inbou_int,
                Inbou_Option_int,
                Inbou_Option_Ck_int,
                Inbou_Option_S_int),
                (Shop_Kodai_Text, 
                "古代の機械", 
                Kodai_Int, 
                Kodai_Option_int, 
                Kodai_Option_Ck_int, 
                Kodai_Option_S_int)
            };
            foreach (var (Shop_TextBlock, Shop_Item, Shop_Money, Shop_Money_Option, Shop_Ck, Shop_S)
                in Shop_Text)
            {
                Shop_TextBlock.Text = ($"{Shop_Item} :{Shop_Money * Shop_Money_Option}ダイヤ\r\n" +
                    $"クリックごとのダイヤを{Shop_Ck * Ck_Plus}ダイヤ追加する\r\n" +
                    $"1秒ごとのダイヤを{Shop_S * S_Plus}ダイヤ追加する");
            }
            string Tmp_Item;
            string Tmp_Item_Money;
            string Tmp_Item_Ck;
            Shop_Sale_Text.Text = $"バーゲンセール:{Tmp_Shop_All * Sale_Option_int}ダイヤ\r\n" +
                $"値段を最初の値段にする";
            Shop_CkPlus_Text.Text = $"CKドリンク:{Tmp_Shop_All * CkPlus_Option_int}ダイヤ\r\n" +
                $"クリックの増加量を増やす";
            Shop_SPlus_Text.Text = $"時の巻物:{(Tmp_Shop_All * SPlus_Option_int):N0}ダイヤ\r\n" +
                $"秒の増加量を増やす";
        }
        private void Color_Option()
        {
            if(Black_Bool)
            {
                Black();
            }
            else
            {
                White();
            }
        }
        private void White()
        {
            this.Resources["Back_Color"] = new SolidColorBrush(Colors.Gainsboro);
            this.Resources["Accent_Color"] = new SolidColorBrush(Colors.DarkGray);
            this.Resources["Text_Color"] = new SolidColorBrush(Colors.Black);
        }
        private void Black()
        {
            this.Resources["Back_Color"] = new SolidColorBrush(Colors.DimGray);
            this.Resources["Accent_Color"] = new SolidColorBrush(Colors.Gray);
            this.Resources["Text_Color"] = new SolidColorBrush(Colors.White);
        }
        private void Main_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                checked
                {
                    Diamond += Ck;
                    Main_Sound.Play();
                    Content_Loading();
                }
            }
            catch (OverflowException)
            {
                MessageBox.Show("ゲームクリア！！\r\n" +
    "これを気にセーブデータを初期化して\r\n" +
    "一からやり直しましょう！",
    "Diamond_Clicker",
    MessageBoxButton.OK,
    MessageBoxImage.Information);
                All_Del();
            }
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Save();
        }

        private void All_Del()
        {
            MessageBoxResult result = MessageBox.Show("本当にデータを削除しますか？",
                "Diamond_Clicker",
                MessageBoxButton.YesNo,
                MessageBoxImage.Exclamation);

            if (result == MessageBoxResult.Yes)
            {
                Diamond = 0;
                Ck = 1;
                S = 0;
                Black_Bool = false;
                Ck_Plus = 1;
                S_Plus = 1;
                Shop_del();
                Save();
                Content_Loading();
            }
            else
            {

            }
        }

        private void Setting_Savedata_del_Click(object sender, RoutedEventArgs e)
        {
            All_Del();
        }
        private void Setting_Color_Button_Click(object sender, RoutedEventArgs e)
        {
            if(!Black_Bool)
            {
                Black_Bool = true;
            }
            else
            {
                Black_Bool = false;
            }
            Content_Loading();
            Color_Option();
        }
        private void Shop_Tsuruhashi_Button_Click(object sender, RoutedEventArgs e)
        {
            Tmp_Shop_Money = Tsuruhashi_Option_int;
            Tmp_Shop_Ck_Plus = Tsuruhashi_Option_Ck_int;
            Tmp_Shop_S_Plus = Tsuruhashi_Option_S_int;
            Tmp_Shop_Old = 2;
            Tmp_Shop_Old_String = "Null";
            Shop(ref Tsuruhashi_int);
        }
        private void Shop_Sale_Button_Click(object sender, RoutedEventArgs e)
        {
            Tmp_Shop_All_Setting();
            if (Tsuruhashi_int > 1)
            {
                if (Diamond >= (Sale_Option_int * Tmp_Shop_All))
                {
                    Diamond -= (Sale_Option_int * Tmp_Shop_All);
                    Shop_del();
                    Ok_Sound.Play();
                }
                else
                {
                    No_Sound.Play();
                }
            }
            else
            {
                No_Sound.Play();
                MessageBox.Show($"ツルハシを買ってから\r\n買ってください",
                    "Diamond_Clicker",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            Content_Loading();
        }
        private void Shop_CkPlus_Button_Click(object sender, RoutedEventArgs e)
        {
            Tmp_Shop_All_Setting();
            if (Tsuruhashi_int > 1)
            {
                if (Diamond >= (CkPlus_Option_int * Tmp_Shop_All))
                {
                    Diamond -= (CkPlus_Option_int * Tmp_Shop_All);
                    Ck_Plus += 1;
                    Tsuruhashi_int++;
                    Ok_Sound.Play();
                }
                else
                {
                    No_Sound.Play();
                }
            }
            else
            {
                No_Sound.Play();
                MessageBox.Show($"ツルハシを買ってから\r\n買ってください",
                    "Diamond_Clicker",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            Content_Loading();
        }
        private void Shop_SPlus_Button_Click(object sender, RoutedEventArgs e)
        {
            Tmp_Shop_All_Setting();
            if (Tsuruhashi_int > 1)
            {
                if (Diamond >= (SPlus_Option_int * Tmp_Shop_All))
                {
                    Diamond -= (SPlus_Option_int * Tmp_Shop_All);
                    S_Plus += 1;
                    Tsuruhashi_int++;
                    Ok_Sound.Play();
                }
                else
                {
                    No_Sound.Play();
                }
            }
            else
            {
                No_Sound.Play();
                MessageBox.Show($"ツルハシを買ってから\r\n買ってください",
                    "Diamond_Clicker",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            Content_Loading();
        }
        private void Shop_Doriru_Button_Click(object sender, RoutedEventArgs e)
        {
            Tmp_Shop_Money = Doriru_Option_int;
            Tmp_Shop_Ck_Plus = Doriru_Option_Ck_int;
            Tmp_Shop_S_Plus = Doriru_Option_S_int;
            Tmp_Shop_Old = Tsuruhashi_int;
            Tmp_Shop_Old_String = "ツルハシ";
            Shop(ref Doriru_int);
        }
        private void Shop_Kojou_Button_Click(object sender, RoutedEventArgs e)
        {
            Tmp_Shop_Money = Kojou_Option_int;
            Tmp_Shop_Ck_Plus = Kojou_Option_Ck_int;
            Tmp_Shop_S_Plus = Kojou_Option_S_int;
            Tmp_Shop_Old = Doriru_int;
            Tmp_Shop_Old_String = "ドリル";
            Shop(ref Kojou_int);
        }

        private void Shop_Inbou_Button_Click(object sender, RoutedEventArgs e)
        {
            Tmp_Shop_Money = Inbou_Option_int;
            Tmp_Shop_Ck_Plus = Inbou_Option_Ck_int;
            Tmp_Shop_S_Plus = Inbou_Option_S_int;
            Tmp_Shop_Old = Kojou_int;
            Tmp_Shop_Old_String = "工場";
            Shop(ref Inbou_int);
        }
        private void GatyaGatya_Button_Click(object sender, RoutedEventArgs e)
        {
            int RandomGatya = R.Next(11);
            if (RandomGatya == 1)
            {
                MessageBox.Show("あたり！100ダイヤを贈呈！！", "Diamond_Clicker");
                Ok_Sound.Play();
                Diamond += 100;
            }
            else
            {
                No_Sound.Play();
                MessageBox.Show("ハズレ！！", "Diamond_Clicker");
                if(Diamond >= 100)
                {
                    MessageBox.Show("100ポイント没収だよ", "Diamond_Clicker");
                    Diamond -= 100;
                }
                else
                {
                    No_Sound.Play();
                    Thread.Sleep(50);
                    No_Sound.Play();
                    Thread.Sleep(50);
                    No_Sound.Play();
                    Thread.Sleep(50); 
                    No_Sound.Play();
                    Thread.Sleep(50);
                    No_Sound.Play();
                    Thread.Sleep(50);
                    No_Sound.Play();
                    Thread.Sleep(50);
                    No_Sound.Play();
                    Thread.Sleep(50);
                    No_Sound.Play();
                    Thread.Sleep(50);
                    No_Sound.Play();
                    Thread.Sleep(50);
                    No_Sound.Play();
                    MessageBox.Show("金が足らぬみたいだね…\r\n" +
                        "時間払いで払ってもらうよ\r\n" +
                        "時間は10秒だよ", "Diamond_Clicker");
                    Thread.Sleep(10000);
                    MessageBox.Show("もう終わったよ", "Diamond_Clicker");
                }
            }
        }
        private void Setting_Save_Button_Click(object sender, RoutedEventArgs e)
        {
            Save();
        }

        private void Setting_Help_Button_Click(object sender, RoutedEventArgs e)
        {
            var startInfo = new System.Diagnostics.ProcessStartInfo("https://github.com/CShapDaisuki-x64/Diamond_Clicker/issues/new");
            startInfo.UseShellExecute = true;
            System.Diagnostics.Process.Start(startInfo);
        }

        private void Shop_Kodai_Button_Click(object sender, RoutedEventArgs e)
        {
            Tmp_Shop_Money = Kodai_Option_int;
            Tmp_Shop_Ck_Plus = Kodai_Option_Ck_int;
            Tmp_Shop_S_Plus = Kodai_Option_S_int;
            Tmp_Shop_Old = Inbou_int;
            Tmp_Shop_Old_String = "秘密結社";
            Shop(ref Kodai_Int);
        }
    }
}