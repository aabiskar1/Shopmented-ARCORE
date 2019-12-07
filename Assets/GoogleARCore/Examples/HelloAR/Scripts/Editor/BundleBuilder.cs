#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BundleBuilder : Editor
{
    [MenuItem("Prefabs/Build AssetBundles")]
    static void BuildAllAssetBundles() {
        BuildPipeline.BuildAssetBundles(@"D:\UnityAssetBundles\Bundles", BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.Android);
    }
}
#endif