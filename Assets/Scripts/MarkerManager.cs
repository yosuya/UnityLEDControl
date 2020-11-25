using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MarkerManager : MonoBehaviour
{
    public static MarkerManager Instance;

    public RectTransform MarkerField; //マーカーを配置する領域
    public GameObject MarkerTemplate; //マーカーのテンプレート（これを複製する）

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
        entry.eventID = EventTriggerType.PointerDown;
        entry.callback.AddListener((x) => OnMouseButtonClick(x));
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
    public void OnMouseButtonClick(BaseEventData eventData)
    {
        Vector2 localMousePos = Vector2.zero;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(MarkerField, Input.mousePosition, null, out localMousePos);

        PointerEventData pointerEvent = (PointerEventData)eventData;

        //モードによって処理を変える
        switch (CurrentMode)
        {
            case MarkerEditorMode.Edit:
                if (Input.GetMouseButton(0))//左クリックした時のみ
                {
                    AddMarker(localMousePos);
                }
                else if (Input.GetMouseButton(1))//右クリックしたとき
                {
                    /*
                    var raycastResult = new List<RaycastResult>();
                    EventSystem.current.RaycastAll(pointerEvent, raycastResult);
                    GameObject targetObj = raycastResult.First().gameObject;

                    if (targetObj.name == "Marker") RemoveMarker(targetObj);
                    */
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

        while (markerList.Select(x => x.ID).Contains(assignedID)) assignedID++; //競合していたら別の値にする
        markerComponent.SetID(assignedID); //ID割り当て、管理クラス(this)を設定
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
