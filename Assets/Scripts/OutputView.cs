using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UniRx;

public class OutputView : MonoBehaviour
{
    static OutputView Instance; //どこからでもアクセスできるようにするため

    protected Transform Content_Transform; //ここにTemplateオブジェクトを生成する
    protected GameObject TemplateObject;   //このテンプレを複製する
    protected ScrollRect _scrollRect;
    protected ContentSizeFitter _contentSizeFitter;

    protected TMP_Text Text; //テンプレートのテキスト

    protected List<GameObject> Contents = new List<GameObject>();

    protected void Awake()
    {
        Content_Transform = transform.Find("Viewport/Content");
        TemplateObject = transform.Find("Viewport/Content/Template").gameObject;
        _scrollRect = GetComponent<ScrollRect>();
        _contentSizeFitter = transform.Find("Viewport/Content").GetComponent<ContentSizeFitter>();

        Text = TemplateObject.transform.Find("Text").GetComponent<TMP_Text>();

        Instance = this;
    }

    //静的バージョンの追加メソッド
    protected void Add_NS(string text)
    {
        GameObject newContent = Instantiate(TemplateObject, Vector3.zero, Quaternion.identity, Content_Transform);

        newContent.transform.Find("Text").GetComponent<TMP_Text>().text = $"[{DateTime.Now.ToString("hh:mm:ss")}]   {text}";
        newContent.name = $" Log_{DateTime.Now.ToString("hh:mm:ss")}";

        newContent.SetActive(true);

        Contents.Add(newContent); //リストに追加

        Observable.NextFrame().Subscribe(_ =>
        {
            _contentSizeFitter.SetLayoutVertical();
            _scrollRect.verticalNormalizedPosition = 0; //最新の出力を表示
        });

    }

    /// <summary>OutputViewにコンテンツを追加する</summary>
    static public void Add(string text)
    {
        if (Instance == null)
        {
            Debug.LogWarning("OutputViewが初期化されていません.");
            return;
        }

        Instance.Add_NS(text);
    }
}

