# MixedPresentation
- Windows10 Creators Update
- Unity 5.6.1
- Visual Studio 2015
----------
## 概要
### HoloLensでの新しいプレゼンテーションの形です
- WifiでHoloLensとWindowsパソコンを連動させてメディア(画像，音楽，動画)を操作
- WindowsパソコンはUnityEditor上かスタンドアロン動作(x86 or x64)で動作
- UnityEditor上ではパソコンからのメディアの操作も可能


youtube : [MixedPresentation](https://youtu.be/Uk7gF6PVKiw)
## 内容
### HoloLensModule
- [HoloLensModule](https://github.com/akihiro0105/HoloLensModule)を利用
### MixedPresentation
- Editor
    + CompositorWindow.cs
        * UnityEditor内で別ウィンドウとして表示
    + CreateAssetBundles.cs
        * AssetBundle作成時に利用
    + PreviewWindow.cs
        * UnityEditor内の別ウィンドウからフルスクリーンに切り替え
- Prefabs
    + MediaObject.prefab
        * SharingRootObjectで利用するStreamingAssetsからのメディアの表示
    + SharingRootObject.prefab
        * ネットワーク処理とメディアの読み込み表示を行う
- Scenes
    + MixPresentation.unity
- Scripts
    + DesktopCameraViewer.cs
        * 空間内カメラの表示
    + MediaObjectControl.cs
        * メディアの読み込み : 音声(wav)動画(mp4)画像(png,jpg)
    + MediaObjectManager.cs
        * StreamingAssetsフォルダからメディアの読み込み処理
    + TcpNetworkObjectControl.cs
        * UDPによるサーバーIPのブロードキャスト通知とTCPによるオブジェクトの同期処理
    + TransformImportExportManager.cs
        * 配置したメディア，カメラのTransform情報の保存，再生
### StreamingAssets
- sampleimage.png
- dummy.dummy
    + StreamingAssetsフォルダが空だとHololensのパッケージ内でStreamingAssetsフォルダが生成されないため必要
## 今後の予定
- Unity，VisualStudioのバージョンアップに伴う改修