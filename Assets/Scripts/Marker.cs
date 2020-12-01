using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class Marker : MonoBehaviour, IPointerEnterHandler, IDragHandler, IPointerClickHandler
{
    public int ID { get; private set; } = -1;
    static protected MarkerManager Manager;
    static protected RectTransform MarkerField;

    public GameObject MarkerListContent { get; private set; } = null;

    protected TMP_InputField ID_IF;

    ///<summary>Markerのクラスのセットアップ</summary>
    public static void Setup(MarkerManager manager, RectTransform markerField)
    {
        Manager = manager;
        MarkerField = markerField;
    }

    ///<summary>このマーカーにIDを割り当てる</summary>
    public void SetID(int id) => ID = id;
    ///<summary>このマーカーの情報を表示するマーカーリストの要素を登録</summary>
    public void SetMarkerContent(GameObject contentObj)
    {
        MarkerListContent = contentObj;
        ID_IF = contentObj.transform.Find("ID_IF").GetComponent<TMP_InputField>();
        ID_IF.onEndEdit.AddListener((x) => OnEndEdit_ID(x));
    }


    ///<summary>マウスカーソルが乗ったときの処理</summary>
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (Input.GetMouseButton(1) && MarkerManager.CurrentMode == MarkerManager.MarkerEditorMode.Edit) //右クリックしているとき＆編集モード
        {
            RemoveThisMarker();
        }
    }

    ///<summary>マウスカーソルでクリックしたときの処理</summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right && MarkerManager.CurrentMode == MarkerManager.MarkerEditorMode.Edit) //右クリックしているとき＆編集モード
        {
            RemoveThisMarker();
        }
    }

    ///<summary>ドラッグ時の処理</summary>
    public void OnDrag(PointerEventData eventData)
    {
        if (MarkerManager.CurrentMode == MarkerManager.MarkerEditorMode.Move) //移動モードのとき
        {
            Vector2 localMousePos = Vector2.zero;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(MarkerField, eventData.position, null, out localMousePos);

            //ドラッグしている範囲が特定の領域内であるときのみ有効
            if (localMousePos.x >= 0 && localMousePos.y <= 0 && localMousePos.x <= MarkerField.sizeDelta.x && localMousePos.y >= -MarkerField.sizeDelta.y)
            {
                transform.localPosition = localMousePos; //位置を更新
            }
        }
    }


    ///<summary>マーカーリストのID_IFの値が変更された時</summary>
    public void OnEndEdit_ID(string value)
    {
        int requestedID = int.Parse(value);
        var conflictIndex = Manager.FindConflictIndex(requestedID);
        if (conflictIndex.Count == 0) //競合するIDでは無い
        {
            SetID(requestedID);
        }
        else //他のマーカーのIDと競合している
        {
            ID_IF.text = ID.ToString();
        }
    }

    ///<summary>このマーカーを削除する</summary>
    protected void RemoveThisMarker()
    {
        Manager.RemoveMarker(this); //マーカー削除を通知
        Destroy(gameObject); //マーカーを削除する
        Destroy(MarkerListContent); //マーカーリストの表示から消す
    }

}