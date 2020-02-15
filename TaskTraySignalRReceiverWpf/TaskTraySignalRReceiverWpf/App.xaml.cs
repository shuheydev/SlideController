using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Windows;
using System.Windows.Forms;
using wpf = System.Windows;

namespace TaskTraySignalRReceiverWpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : wpf.Application
    {
        private readonly string _appName = "SlideControllReceiver";
        private readonly int _showDuration = 2000;
        private readonly string _signalRServerUrl = @"SignalRHubのURLをここに";

        private HubConnection _hubConnection;
        private NotifyIcon _icon;

        private bool _isSlideshowOn = false;

        /// <summary>
        /// アプリケーション起動時の処理
        /// </summary>
        /// <param name="e"></param>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            #region タスクトレイにアイコンを追加
            _icon = new NotifyIcon();

            //アイコン設定
            _icon.Icon = new System.Drawing.Icon("./communication.ico");
            _icon.Text = _appName;
            //表示する
            _icon.Visible = true;

            #region Context Menuの作成
            var menu = new ContextMenuStrip();

            //終了アイテム
            ToolStripMenuItem menuItem_Exit = new ToolStripMenuItem();
            menuItem_Exit.Text = "Exit";
            menuItem_Exit.Click += MenuItem_Exit_Click;

            //接続アイテム
            ToolStripMenuItem menuItem_Connect = new ToolStripMenuItem();
            menuItem_Connect.Text = "Connect";
            menuItem_Connect.Click += MenuItem_Connect_Click;

            //切断アイテム
            ToolStripMenuItem menuItem_Disconnect = new ToolStripMenuItem();
            menuItem_Disconnect.Text = "Disconnect";
            menuItem_Disconnect.Click += MenuItem_Disconnect_Click;

            //Context MenuにMenuItemを追加
            menu.Items.Add(menuItem_Exit);
            menu.Items.Add(menuItem_Connect);
            menu.Items.Add(menuItem_Disconnect);
            //Menuをタスクトレイのアイコンに追加
            _icon.ContextMenuStrip = menu;

            #endregion

            #endregion

            #region SignalRへの接続設定など

            _hubConnection = new HubConnectionBuilder()
                .WithUrl(_signalRServerUrl)//サーバー側でMapHubで指定したURLを指定する.
                .WithAutomaticReconnect()
                .Build();

            //サーバーからメッセージを受け取ったときに行う処理を登録する
            //サーバーは,この第1引数で指定した文字列を使ってクライアント側の処理を呼び出す
            _hubConnection.On<string>("Receive", Receive);

            //この時点ではまだ接続していない.

            #endregion
        }

        #region イベントハンドラー

        /// <summary>
        /// 接続処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void MenuItem_Connect_Click(object sender, EventArgs e)
        {
            //すでに接続済みの場合はメッセージを表示してreturn
            if (!string.IsNullOrEmpty(_hubConnection.ConnectionId))
            {
                ShowBalloonTip(_appName, "Already Connected", _showDuration);
                return;
            }

            //ここで接続
            await _hubConnection.StartAsync();

            if (!string.IsNullOrEmpty(_hubConnection.ConnectionId))
            {
                ShowBalloonTip(_appName, "Connected", _showDuration);
            }
            else
            {
                ShowBalloonTip(_appName, "Connection Failed", _showDuration);
            }
        }

        /// <summary>
        /// 切断処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void MenuItem_Disconnect_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_hubConnection.ConnectionId))
            {
                ShowBalloonTip(_appName, "Already Disconnected", _showDuration);
                return;
            }

            //切断
            await _hubConnection.StopAsync();

            if (string.IsNullOrEmpty(_hubConnection.ConnectionId))
            {
                ShowBalloonTip(_appName, "Disconnected", _showDuration);
            }
        }

        /// <summary>
        /// アプリ終了処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Exit_Click(object sender, EventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        #endregion

        /// <summary>
        /// サーバーからメッセージを受け取ったときの処理
        /// </summary>
        /// <param name="message"></param>
        private void Receive(string message)
        {
            switch (message)
            {
                case "next":
                    SendKeys.SendWait("{RIGHT}");
                    break;
                case "prev":
                    SendKeys.SendWait("{LEFT}");
                    break;
                case "blackout":
                    SendKeys.SendWait("B");
                    break;
                case "toggleSlideShow":
                    if (_isSlideshowOn)
                    {
                        SendKeys.SendWait("{ESC}");
                    }
                    else
                    {
                        SendKeys.SendWait("+{F5}");
                    }

                    //状態を反転する
                    _isSlideshowOn = !_isSlideshowOn;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 接続,切断などの状態の通知を表示する
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="durationByMilliseconds"></param>
        private void ShowBalloonTip(string title, string message, int durationByMilliseconds)
        {
            _icon.BalloonTipTitle = title;
            _icon.BalloonTipText = message;
            _icon.ShowBalloonTip(durationByMilliseconds);
        }
    }
}
