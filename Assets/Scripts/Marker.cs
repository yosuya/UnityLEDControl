using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class Marker : MonoBehaviour, IPointerEnterHandler
{
    public int ID { get; private set; }
    protected MarkerManager Manager;

    public void Setup(int id, MarkerManager manager)
    {
        Manager = manager;
        ID = id;
    }

    ///<summary>マウスカーソルが乗ったときの処理</summary>
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (Input.GetMouseButton(1) && MarkerManager.CurrentMode == MarkerManager.MarkerEditorMode.Edit) //右クリックしているとき＆編集モード
        {
            Manager.RemoveMarker(this); //マーカー削除を通知
            Destroy(gameObject); //マーカーを削除する
        }
    }
}
