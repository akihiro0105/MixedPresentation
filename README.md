# MixedPresentation
- Windows10 Creators Update
- Unity 5.6.1,Unity 2017.1.2, Unity 2017.2.0
- Visual Studio 2015,2017
----------
## 概要
### HoloLensでの新しいプレゼンテーションの形です
- WifiでHoloLensとWindowsパソコンを連動させてメディア(画像，音楽，動画)を操作
- WindowsパソコンはUnityEditor上かスタンドアロン動作(x86 or x64)で動作
- UnityEditor上ではパソコンからのメディアの操作も可能


youtube : [MixedPresentation](https://youtu.be/Uk7gF6PVKiw)

## サンプルシーンの使い方
- PresentationContentフォルダー内のメディアデータを取得してUnity内に表示します(HoloLensはLocalフォルダ内のPresentationContent)


- HoloLens側
1. MixedPresentation.unityを展開しHoloLens用にビルド
    - ビルド時にPlayerSetting/PublishingSetting/CapabilitiesのInternetClientとInternetClientServerにチェックを入れる
1. アプリ起動後localフォルダ内(LocalAppData/MixedPresentation/LocalState)にPresentationContentが生成
1. メディアデータをPresentationContent内にUploadする
1. アプリを再起動

- パソコン側
    + UnityEditorで利用する場合
    1. メディアデータをPresentationContent内に配置する
    1. UnityEditorのメニューバーのMixedPresentation/Compositorより別ウィンドウでカメラ画像を表示
    1. Playボタンを押して再生
    + スタンドアロンで利用する場合
    1. PlatformをPCのStandaloneに切り替えてプロジェクトをビルド
    1. ビルドされたexeファイル起動後にPresentationContentが生成される
    1. メディアデータをPresentationContent内に配置する
    1. exeファイルを再起動
### 操作方法
- HoloLens側
    + メディアの移動 : ドラッグ
    + メディアの回転 : 両手を出して片手だけドラッグ
    + メディアの拡縮 : 両手を出して両手でドラッグ
    + メディアの再生，停止 : ダブルタップ
    + カメラの切り替え : カメラを見てダブルタップ
    + カメラの移動 : ドラッグ
    + 基準点の変更 : 両手を出して片手でダブルタップ
    + メディア配置の保存 : 両手を出して両手でダブルタップ
    + メディア配置の再生 : 両手を出して片手でホールド
- パソコン側
    + メディアの移動，回転，拡縮 : EditorのInspectorから操作
    + メディアの再生，停止 : 対象メディアのInspectorの「Play」ボタンを押下
    + カメラの移動 : EditorのInspectorから操作
    + カメラの切り替え : 数字キーを入力
    + メディア配置の保存 : キーボードのE or SharingRootObjectのInspectorの「Export Transform」ボタンを押下
    + メディア配置の再生 : キーボードのI or SharingRootObjectのInspectorの「Import Transform」ボタンを押下
    
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
    + ControlObject.prefab
        * SharingRootObjectで利用するメディアの表示
- Scenes
    + MixPresentation.unity
- Scripts
    + DesktopCameraViewer.cs
        * 空間内カメラの表示
    + JsonMessageControl.cs
        * UDPによる通信をコントロール
    + JsonMessageData.cs
        * UDP通信のメッセージを定義
    + MediaControl.cs
        * メディアの読み込み : 音声(wav)動画(mp4)画像(png,jpg)
    + MixedPresentationManager.cs
        * MixedPresentationをコントロール
    + PresentationCameraControl.cs
        * プレゼンテーション用カメラのコントロール
    + TransformImportExportManager.cs
        * 配置したメディア，カメラのTransform情報の保存，再生
### PresentationContent
- sampleimage.png

## 今後の予定
- Unity2017.2の対応
- Windows Immersive HMDの対応+モーションコントローラーの対応