using UnityEditor;

namespace MixedPresentation
{
    public class CreateAssetBundles
    {
        [MenuItem("MixedPresentation/Build AssetBundles")]
        static void BuildAllAssetBundles()
        {
            BuildPipeline.BuildAssetBundles("Assets/StreamingAssets", BuildAssetBundleOptions.None, BuildTarget.WSAPlayer);
        }
    }
}
