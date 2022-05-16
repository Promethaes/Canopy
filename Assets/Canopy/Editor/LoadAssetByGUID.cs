using UnityEditor;
using UnityEngine;
//https://forum.unity.com/threads/solved-loading-a-visualtreeasset-or-stylesheet-in-c-from-anywhere.799923/
namespace Canopy
{
    public class LoadAssetByGUID
    {

        public static T Load<T>(string guid) where T : UnityEngine.Object
        {
            var assetPath = AssetDatabase.GUIDToAssetPath(guid);
            return AssetDatabase.LoadAssetAtPath<T>(assetPath);
        }
    }
}