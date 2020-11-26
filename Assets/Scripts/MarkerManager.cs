using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class MarkerManager : MonoBehaviour
{
    public static MarkerManager Instance;

    public RectTransform MarkerField; //マーカーを配置する領域
    public GameObject MarkerTemplate; //マーカーのテンプレート（これを複製する）

    public Transform MarkerListContentTransform; //ここにマーカーリストのコンテンツを入れる
    public GameObject MarkerListContentTemplate; //これがリストの1行のオブジェクト

    /// UI
    public Toggle Select_Toggle;
    public Toggle Move_Toggle;
    public Toggle Edit_Toggle;


    ///<summary>マーカーを操作するときのモード</summary>
    public static MarkerEditorMode CurrentMode = MarkerEditorMode.Select;

    ///<summary>マーカー管理リスト</summary>
    protected List<Marker> markerList = new List<Marker>();

    protected int assignedID = 0;


    public void Setup()
    {
        Instance = this; //外部クラスから呼び出しやすくするためにインスタンスを代表として登録

        //イベントを登録
        EventTrigger trigger = MarkerField.gameObject.AddComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener((x) => OnPointerClick(x));
        trigger.triggers.Add(entry);

        //Toggleの設定
        Select_Toggle.isOn = true;
        Select_Toggle.onValueChanged.AddListener((x) => SelectMode(x, MarkerEditorMode.Select));
        Move_Toggle.onValueChanged.AddListener((x) => SelectMode(x, MarkerEditorMode.Move));
        Edit_Toggle.onValueChanged.AddListener((x) => SelectMode(x, MarkerEditorMode.Edit));

        //マーカーの初期設定
        Marker.Setup(this, MarkerField);
    }

    ///<summary>マウスボタンがマーカー配置領域内で押されたときのコールバック</summary>
    public void OnPointerClick(BaseEventData eventData)
    {
        Vector2 localMousePos = Vector2.zero;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(MarkerField, Input.mousePosition, null, out localMousePos);

        PointerEventData pointerEvent = (PointerEventData)eventData; //PointerEventDataに変換

        //モードによって処理を変える
        switch (CurrentMode)
        {
            case MarkerEditorMode.Edit: //Editモードのときの処理
                //押されたボタンによって処理を変える
                switch (pointerEvent.button)
                {
                    case PointerEventData.InputButton.Left://左クリックした
                        AddMarker(localMousePos);
                        break;
                    case PointerEventData.InputButton.Right://右クリックした

                        break;
                }
                break;
        }

    }

    ///<summary>マーカーを追加する</summary>
    public void AddMarker(Vector2 pos)
    {
        //オブジェクトの作成と設定
        GameObject newMarker = Instantiate(MarkerTemplate, Vector3.zero, Quaternion.identity, MarkerField);
        newMarker.transform.localPosition = pos;
        newMarker.name = $"Marker";
        newMarker.SetActive(true);

        Marker markerComponent = newMarker.GetComponent<Marker>();
        markerList.Add(markerComponent); //作成したマーカーを管理リストに追加

        //IDの割当
        while (markerList.Select(x => x.ID).Contains(assignedID)) assignedID++; //競合していたら別の値にする
        markerComponent.SetID(assignedID); //ID割り当て、管理クラス(this)を設定

        //マーカーリスト（画面上の表示）に追加する
        GameObject newMarkerListContent = Instantiate(MarkerListContentTemplate, Vector3.zero, Quaternion.identity, MarkerListContentTransform);
        newMarkerListContent.name = "MarkerContent";
        newMarkerListContent.SetActive(true);
        newMarkerListContent.transform.Find("ID_Text").GetComponent<TMP_Text>().text = assignedID.ToString();
        newMarkerListContent.transform.Find("PosX_Text").GetComponent<TMP_Text>().text = pos.x.ToString("0.0");
        newMarkerListContent.transform.Find("PosY_Text").GetComponent<TMP_Text>().text = pos.y.ToString("0.0");
        markerComponent.SetMarkerContent(newMarkerListContent); //マーカーコンテンツの登録
    }


    ///<summary>マーカーを削除する</summary>
    public void RemoveMarker(Marker marker)
    {
        markerList.Remove(marker);
    }


    /// <summary>Toggleの選択によるモード選択（UIのコールバックに設定）</summary>
    public void SelectMode(bool isOn, MarkerEditorMode mode_num)
    {
        if (!isOn) return; //選択されたトグルでなければ終了

        if (CurrentMode == mode_num) return; //選択されたものが現在のモードなら終了

        CurrentMode = mode_num;
    }


    public enum MarkerEditorMode
    {
        Select,
        Move,
        Edit
    }
}
