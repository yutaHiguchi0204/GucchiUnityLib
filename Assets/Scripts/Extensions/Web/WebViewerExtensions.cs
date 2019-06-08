using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebViewerExtensions : MonoBehaviour
{
    private const int WEB_PAGE_MARGIN = 3;

    [SerializeField]
    private bool _isJumpPage = false;

    [SerializeField]
    private WebPage.WebPageType _webPageType = WebPage.WebPageType.None;

    [SerializeField]
    private WebPage.LocalPageType _localPageType = WebPage.LocalPageType.None;

    [SerializeField]
    private WebViewObject _webViewObj;

    private WebViewObject _view;

    public void OnClick()
    {
        // すでに開かれている場合は閉じる
        if (_view != null && _view.GetVisibility())
        {
            _view.SetVisibility(false);
        }

        // URL取得
        string url = GetURL();

        // Webページに飛ぶ
        if (_isJumpPage)
        {
            Application.OpenURL(url);
            return;
        }

        // WebViewで表示
        if (_webViewObj == null)
        {
            Debug.LogError("WebViewObjectがアタッチされていません。");
            return;
        }
        _view = Instantiate(_webViewObj);
        _view.Init(null);
        _view.LoadURL(url);
        _view.SetMargins(Screen.width / WEB_PAGE_MARGIN, Screen.height / WEB_PAGE_MARGIN, Screen.width / WEB_PAGE_MARGIN, Screen.height / WEB_PAGE_MARGIN);
        _view.SetVisibility(true);
    }

    private string GetURL()
    {
        // Webページが設定されていたらローカルページが設定されていてもWebページを優先する
        if (_webPageType != WebPage.WebPageType.None)
        {
            return WebPage.Instance.PageName[(int)_webPageType];
        }

        // ローカルページ
        if (_localPageType == WebPage.LocalPageType.None)
        {
            Debug.LogError("Webページもローカルページも設定されていません。");
            return "";
        }
        string url = System.IO.Path.Combine(Application.streamingAssetsPath + "/" + WebPage.Instance.PageName[(int)_localPageType]);
        Debug.Log(url);
        return "file://" + url.Replace(" ", "%20");
        //return url;
    }
}
