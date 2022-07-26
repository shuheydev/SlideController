using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace XamrinSlideController.ViewModels
{
    class MainPageViewModel : INotifyPropertyChanged
    {
        private readonly string _signalRServerUrl = @"SignalRHubのURLをここに";

        private HubConnection _hubConnection;

        #region プロパティ,コマンド
        private string _statusMessage;
        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                if (_statusMessage == value)
                    return;

                _statusMessage = value;
                OnPropertyChanged();
            }
        }

        public ICommand ConnectCommand { get; }
        public ICommand SendMessageCommand { get; }
        #endregion

        public MainPageViewModel()
        {
            //SignalR接続の準備
            this.StatusMessage = "未接続";
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(_signalRServerUrl)//接続先のSignalRHub
                .WithAutomaticReconnect()
                .Build();

            #region 接続関連のイベントハンドラの登録
            _hubConnection.Closed += _hubConnection_Closed;
            _hubConnection.Reconnecting += _hubConnection_Reconnecting;
            _hubConnection.Reconnected += _hubConnection_Reconnected;
            #endregion

            #region ボタンが押されたときの処理を登録
            //接続ボタンが押されたとき
            ConnectCommand = new Command(async () => await Connect());

            //その他のパワポ操作ボタンが押されたとき
            SendMessageCommand = new Command<string>(
                    execute: async (string message) =>
                     {
                         await SendMessage(message);
                     }
                );
            #endregion
        }

        #region イベントハンドラ
        private async Task _hubConnection_Reconnected(string arg)
        {
            await Task.Run(() =>
            {
                this.StatusMessage = "再接続済";
            });
        }

        private async Task _hubConnection_Reconnecting(Exception arg)
        {
            await Task.Run(() =>
            {
                this.StatusMessage = "再接続中";
            });
        }

        private async Task _hubConnection_Closed(Exception arg)
        {
            await Task.Run(() =>
            {
                this.StatusMessage = "切断済";
            });
        }
        #endregion

        #region コマンド

        /// <summary>
        /// サーバーにメッセージを送信
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private async Task SendMessage(string message)
        {
            //押したことがわかるように振動させる
            await Task.Run(() => { Viberate(50); });

            try
            {
                //サーバー側の｢Send｣メソッドを呼び出す
                await _hubConnection.InvokeAsync("SendToOthers", message);
            }
            catch
            {
                this.StatusMessage = "接続してください";
            }

            this.StatusMessage = message;
        }

        /// <summary>
        /// 接続処理
        /// </summary>
        /// <returns></returns>
        private async Task Connect()
        {
            //押したことがわかるように振動させる
            await Task.Run(() => { Viberate(50); });

            if (string.IsNullOrEmpty(_hubConnection.ConnectionId))
            {
                this.StatusMessage = "接続中";
                try
                {
                    await _hubConnection.StartAsync();
                }
                catch
                {
                    this.StatusMessage = "接続失敗";
                }
            }

            this.StatusMessage = "接続済";
        }

        #endregion


        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Viberate(int milliSeconds)
        {
            try
            {
                var duration = TimeSpan.FromMilliseconds(milliSeconds);
                Vibration.Vibrate(duration);
            }
            catch
            {
                //
            }
        }
    }
}
