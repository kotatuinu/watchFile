/* 
 * 参考：DOBON.NET > プログラミング道 > .NET Tips > ファイル、フォルダ「フォルダ、ファイルの変更を監視する」
 * http://dobon.net/vb/dotnet/file/filesystemwatcher.html
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace watchFile
{
    public partial class Form1 : Form
    {
        private System.IO.FileSystemWatcher watcher_ = null;

        public Form1()
        {
            InitializeComponent();
        }

        private void btnSetDir_Click(object sender, EventArgs e)
        {
            if(folderBrowserDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                listView1.Items.Clear();
                label1.Text = folderBrowserDialog1.SelectedPath;
                start(folderBrowserDialog1.SelectedPath);
            }
        }

        //Button1のClickイベントハンドラ
        private void start(string path)
        {
            if (watcher_ != null)
            {
                stop();
            }

            watcher_ = new System.IO.FileSystemWatcher();
            //監視するディレクトリを指定
            watcher_.Path = path;
            //最終アクセス日時、最終更新日時、ファイル、フォルダ名の変更を監視する
            watcher_.NotifyFilter =
                (System.IO.NotifyFilters.LastAccess
                | System.IO.NotifyFilters.LastWrite
                | System.IO.NotifyFilters.FileName
                | System.IO.NotifyFilters.DirectoryName);
            //すべてのファイルを監視
            watcher_.Filter = "";
            watcher_.IncludeSubdirectories = true;
            //UIのスレッドにマーシャリングする
            //コンソールアプリケーションでの使用では必要ない
            watcher_.SynchronizingObject = this;

            //イベントハンドラの追加
            watcher_.Changed += new System.IO.FileSystemEventHandler(watcher_Changed);
            watcher_.Created += new System.IO.FileSystemEventHandler(watcher_Changed);
            watcher_.Deleted += new System.IO.FileSystemEventHandler(watcher_Changed);
            watcher_.Renamed += new System.IO.RenamedEventHandler(watcher_Renamed);

            //監視を開始する
            watcher_.EnableRaisingEvents = true;
            Console.WriteLine("監視を開始しました。");
        }

        //Button2のClickイベントハンドラ
        private void stop()
        {
            //監視を終了
            watcher_.EnableRaisingEvents = false;
            watcher_.Dispose();
            watcher_ = null;
            Console.WriteLine("監視を終了しました。");
        }

        //イベントハンドラ
        private void watcher_Changed(System.Object source, System.IO.FileSystemEventArgs e)
        {
            ListViewItem item1;
            switch (e.ChangeType)
            {
                case System.IO.WatcherChangeTypes.Changed:
                    item1 = new ListViewItem("Changed", 0);
                    item1.SubItems.Add(e.FullPath);
                    listView1.Items.Add(item1);

                    Console.WriteLine(
                        "ファイル 「" + e.FullPath + "」が変更されました。");
                    break;
                case System.IO.WatcherChangeTypes.Created:
                    item1 = new ListViewItem("Created", 0);
                    item1.SubItems.Add(e.FullPath);
                    listView1.Items.Add(item1);

                    Console.WriteLine(
                        "ファイル 「" + e.FullPath + "」が作成されました。");
                    break;
                case System.IO.WatcherChangeTypes.Deleted:
                    item1 = new ListViewItem("Deleted", 0);
                    item1.SubItems.Add(e.FullPath);
                    listView1.Items.Add(item1);

                    Console.WriteLine(
                        "ファイル 「" + e.FullPath + "」が削除されました。");
                    break;
            }
        }

        private void watcher_Renamed(System.Object source, System.IO.RenamedEventArgs e)
        {
            ListViewItem item1 = new ListViewItem("Renamed", 0);
            item1.SubItems.Add(String.Format("{0} → {1}", e.OldFullPath, e.FullPath));
            listView1.Items.Add(item1);

            Console.WriteLine(
                "ファイル 「" + e.FullPath + "」の名前が変更されました。");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            listView1.View = View.Details;
            ListViewItem item1 = new ListViewItem("Event", 0);
            listView1.Columns.Add("Evant", 100, HorizontalAlignment.Left);
            listView1.Columns.Add("FileName", -2, HorizontalAlignment.Left);

        }

        private void listView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == (Keys.Control |  Keys.C)){
                string line = "";
                if (listView1.Items.Count > 0)
                {
                    foreach (ListViewItem item in listView1.Items)
                    {
                        line += string.Format("{0}\t{1}\n", item.SubItems[0].Text, item.SubItems[1].Text);
                    }
                }
                if (line == "")
                {
                    Clipboard.Clear();
                } else {
                    Clipboard.SetText(line);
                }
            }

        }
    }
}
