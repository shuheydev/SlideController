# SlideController
SignalRを使ったPowerPointリモコンです.

# アプリケーションの構成

以下のような仕組みでスマートフォンのアプリからPC上のPowerPointを操作します.

![image](https://user-images.githubusercontent.com/43431002/74585642-159de180-5022-11ea-86b4-c15768a7081b.png)

## SlideControllerSignalRHub
メッセージを中継するSignalRサーバーです.
ASP.NET Core 3.1です.

## XamrinSlideController
リモコンとなるスマホアプリです.
Xamarin.Formsです.

- Xamarin.Forms 4.5 pre3

## TaskTraySignalRReceiverWpf
PowerPointを操作するPC側のレシーバーです.
タスク常駐型のアプリです.
WPF(.NET Core)です.

-.NET Core 3.1

# システムの構築手順

## リポジトリを取得
このリポジトリをzipファイルとしてダウンロード,もしくはCloneする.

## SlideControllerSignalRHub
Visual Studioでプロジェクトを開きます.

`Startup.cs`内の以下の設定によって,クライアントからは`{ホスト名}/SlideController`というURLで接続することになります.

```cs
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("SignalR is working.");
                });
                endpoints.MapHub<SlideControllerHub>("/SlideController");
            });
```

Visual StudioからAzure App Serviceにデプロイします.

デプロイするときのプランはFreeを選んでおきましょう.

デプロイ先のURLを後でクライアント側から使用します.

## TaskTraySignalRReceiverWpf
Visual Studioでプロジェクトを開きます.

`App.xaml.cs`を開き,以下の変数に`{デプロイ先のURL}/SlideController`を入れます.

```cs
        private readonly string _signalRServerUrl = @"SignalRHubのURLをここに";
```

ビルドします.

このアプリケーションはタスクトレイに常駐させるために,以下の2つのDLLを使用しています.
このDLLのパスが環境によって異なる場合があり,そのときはビルドに失敗します.

- System.Drawing.Common
- System.Windows.Forms


このような場合は,以下の場所を探して各DLLの参照を追加してから,再びビルドしてください.

`C:\Program Files\dotnet\shared\Microsoft.WindowsDesktop.App\{.NET Coreのバージョン}`


アプリケーションを起動したらタスクトレイにアイコンが表示されるので,右クリックしてメニューから`Connect`を選択します.

![image](https://user-images.githubusercontent.com/43431002/74586767-4e43b800-502e-11ea-9b3d-f66de4edc9a1.png)

以下のように`Connected`の通知が出たら接続成功です.

![image](https://user-images.githubusercontent.com/43431002/74586956-69afc280-5030-11ea-89ee-54b47cc2e4c8.png)


## XamrinSlideController
Xamarin.Forms製でプラットフォーム固有の機能などは使っていないので,Android,iOS,UWPのいずれでも動作するはずです.

実機をつないでアプリケーションを配置します.

配置が終わったらアプリを起動して,｢接続｣ボタンを押す.

![20200215_213755](https://user-images.githubusercontent.com/43431002/74587902-800f4b80-503b-11ea-8e5d-460f667fdb1c.png)

左上に｢接続済｣と表示されたら接続成功です.


# 使い方
WPF製のレシーバー,Xamarin.Formsアプリをそれぞれ起動してSignalRサーバーに接続します.

PowerPointを起動してお好きな資料を開きます.

PowerPointがアクティブ(フォーカスがあたっている状態)で,以下の操作が行えることを確認します.

- 停止/再開 : スライドショーの開始,停止
- 前 : 次のスライドへ.スライドショーが始まっている状態であること.
- 後 : 前のスライドへ.スライドショーが始まっている状態であること.
- Black out : BlackOutのOn/Off.スライドショーが始まっている状態であること.

# あとがき
ソースコードを見たり,使ってみたりするとわかると思いますが,サーバーからのメッセージに応じて｢各キーが押されたことにしている｣だけなので,PowerPointじゃないと使えない,というわけではなかったりします.

例えば,Youtubeなどの動画アプリだと動画を数秒送ったり戻したりといった操作になるので,こちらでも簡単なリモコンとして使えますね.これも以外と便利です.



