using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class WebPage : ScriptableObject
{
    private static readonly string RESOURCE_PATH = "Data/WebPage";

    private static WebPage _instance = null;
    public static WebPage Instance
    {
        get
        {
            if (_instance == null)
            {
                var asset = Resources.Load(RESOURCE_PATH) as WebPage;
                if (asset == null)
                {
                    Debug.AssertFormat(false, "指定したパスにアセットがありません。 path={0}", RESOURCE_PATH);
                    asset = CreateInstance<WebPage>();
                }

                _instance = asset;
            }

            return _instance;
        }
    }

    public enum WebPageType
    {
        None,
        Sample,

        TypeNum
    }

    public enum LocalPageType
    {
        None = WebPageType.TypeNum,
        Sample,

        TypeNum
    }

    [SerializeField]
    private List<string> _pageName = new List<string>();
    public List<string> PageName
    {
        get
        {
            if ((int)LocalPageType.TypeNum != _pageName.Count)
            {
                Debug.LogWarning("enumの設定数とページ名の設定数が一致していないため不具合が起こる可能性があります。");
            }
            return _pageName;
        }
    }

    [MenuItem("Assets/Create/WebPageAsset")]
    private static void CreateWebPageAsset()
    {
        WebPage webPageAsset = CreateInstance<WebPage>();
        string path = AssetDatabase.GenerateUniqueAssetPath("Assets/Resources/Data/WebPage.asset");
        AssetDatabase.CreateAsset(webPageAsset, path);
        AssetDatabase.Refresh();
    }
}
