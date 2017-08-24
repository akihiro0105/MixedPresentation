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

## サンプルシーンの使い方
- 基本的にStreamingAssetsフォルダー内のメディアデータを取得してUnity内に表示します(HoloLensはStreamingAssetsフォルダー+Localフォルダーを利用)


- HoloLens側
1. MixedPresentation.unityを展開しHoloLens用にビルド
    - ビルド時にPlayerSetting/PublishingSetting/CapabilitiesのInternetClientとInternetClientServerにチェックを入れる
1. プロジェクトをHoloLens内に配置後起動
    - パソコン側プロジェクトが起動済みであること

- パソコン側
    + UnityEditorで利用する場合
    1. UnityEditorのメニューバーのMixedPresentation/Compositorより別ウィンドウでカメラ画像を表示
    1. Playボタンを押して再生
    + スタンドアロンで利用する場合
    1. PlatformをPCのStandaloneに切り替えてプロジェクトをビルド
    1. ビルドされたexeファイルから起動

### メディアの追加
**HoloLens，パソコンのメディアデータは同じになるようにする**
- HoloLensのアプリ配置後にメディアを追加する場合
    + デバイスポータルよりFileExplorer/LocalAppData/MixedPresentation/LocalStateに移動
    + Upload a file to this directoryよりメディアデータをアップロードする
- パソコンのスタンドアロンビルド生成後にメディアを配置する場合
    + 生成したexeデータと同じ階層にある*_Dataフォルダ内のStreamingAssetsに配置する
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