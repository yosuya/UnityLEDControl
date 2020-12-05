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

    public Camera UICamera;

    /// UI
    public Toggle Select_Toggle;
    public Toggle Move_Toggle;
    public Toggle Edit_Toggle;


    ///<summary>マーカーを操作するときのモード</summary>
    public static MarkerEditorMode CurrentMode = MarkerEditorMode.Select;

    ///<summary>マーカー管理リスト</summary>
    protected List<Marker> markerList = new List<Marker>();

    protected int assignedID = 0;

    public static bool AllowToEditMarker { get; private set; } = true; //マーカーの編集が可能かどうか


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

        //直線ツールダイアログの設定
        SLTD_Count_IF = StraightLineTool_Dialog.transform.Find("Count_IF").GetComponent<TMP_InputField>();
        StraightLineTool_Dialog.transform.Find("Apply_Button").GetComponent<Button>().onClick.AddListener(() => StraightLineTool(3)); //値確定
        StraightLineTool_Dialog.transform.Find("Close_Button").GetComponent<Button>().onClick.AddListener(() => StraightLineTool(5)); //設定の中止

        //マーカーの初期設定
        Marker.Setup(this, MarkerField);
    }

    protected void Update()
    {
        switch (CurrentMode)
        {
            case MarkerEditorMode.Edit:
                StraightLineTool();
                break;
        }
    }

    ///<summary>マウスボタンがマーカー配置領域内で押されたときのコールバック</summary>
    public void OnPointerClick(BaseEventData eventData)
    {
        Vector2 localMousePos = Vector2.zero;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(MarkerField, Input.mousePosition, UICamera, out localMousePos);

        PointerEventData pointerEvent = (PointerEventData)eventData; //PointerEventDataに変換

        //モードによって処理を変える
        switch (CurrentMode)
        {
            case MarkerEditorMode.Edit: //Editモードのときの処理
                //押されたボタンによって処理を変える
                switch (pointerEvent.button)
                {
                    case PointerEventData.InputButton.Left://左クリックした
                        if (AllowToEditMarker) AddMarker(localMousePos);
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

        //マーカーリスト（画面上の表示）に追加する
        GameObject newMarkerListContent = Instantiate(MarkerListContentTemplate, Vector3.zero, Quaternion.identity, MarkerListContentTransform);
        newMarkerListContent.transform.localPosition = Vector3.zero;
        newMarkerListContent.name = "MarkerContent";
        newMarkerListContent.SetActive(true);

        //マーカーコンテンツの登録
        markerComponent.SetMarkerContent(newMarkerListContent);

        // IDの割当
        while (markerList.Select(x => x.ID).Contains(assignedID)) assignedID++; //競合していたら別の値にする
        markerComponent.SetID(assignedID); //ID割り当てる

        // 位置の設定
        markerComponent.SetPosition(pos);
    }


    ///<summary>マーカーを削除する</summary>
    public void RemoveMarker(Marker marker)
    {
        markerList.Remove(marker);
    }


    ///<summary>引数idと競合しているIDのインデックスを探す（競合していない場合はListの要素数が0になる）</summary>
    public List<int> FindConflictIndex(int id)
    {
        List<int> conflictIndexList = new List<int>(); //競合しているインデックスを格納

        for (int idx = 0; idx < markerList.Count; idx++)
        {
            if (markerList[idx].ID == id)
            {
                conflictIndexList.Add(idx);
            }
        }
        return conflictIndexList;
    }


    protected int SLTD_SettingStep = 0; //設定状態
    public LineRenderer lineRenderer;
    protected Vector2 LineEndPos = Vector2.zero;
    public GameObject StraightLineTool_Dialog; //直線上にマーカーをいくつ置くかを決めるダイアログ
    public RectTransform MarkerCanvas_RectTransform;
    protected TMP_InputField SLTD_Count_IF; //マーカー数の入力フィールド

    /// <summary>LEDを直線状に等間隔で配置する</summary>
    protected void StraightLineTool(int step = -1)
    {
        if (step == -1) step = SLTD_SettingStep; //指定がなければ現在のStepを代入

        switch (step)
        {
            case 0: //線が未表示
            case 1: //線が表示されていて終点が確定していない
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    if (markerList.Count == 0) return; //マーカーが1つも追加されていない

                    Vector2 localMousePos = Vector2.zero; //マーカーフィールド内での位置
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(MarkerField, Input.mousePosition, UICamera, out localMousePos); //UI空間上でのマウス位置

                    //範囲外では処理しない
                    if (localMousePos.x < 0 || localMousePos.y > 0 || localMousePos.x > MarkerField.sizeDelta.x || localMousePos.y < -MarkerField.sizeDelta.y) return;

                    //マーカーフィールド内での位置を記録しておく
                    LineEndPos = localMousePos;

                    /// ----- ここから線を引く処理 -----
                    Vector3 endPosition = Vector3.zero;
                    RectTransformUtility.ScreenPointToWorldPointInRectangle(MarkerField, Input.mousePosition, UICamera, out endPosition);

                    //先の太さを設定
                    lineRenderer.startWidth = 0.5f; lineRenderer.endWidth = 0.5f;

                    //先の頂点を決定
                    lineRenderer.positionCount = 2;
                    Vector3[] linePos = new Vector3[2] { markerList.Last().transform.position, endPosition };
                    lineRenderer.SetPositions(linePos);

                    SLTD_SettingStep = 1; //線を引いている


                    /// ----- 終点位置を確定させて設置するマーカーの個数を決めるダイアログを出す -----
                    if (Input.GetMouseButtonDown(0)) //左クリックされたら
                    {
                        AllowToEditMarker = false; //マーカーの追加を無効化

                        SLTD_SettingStep = 2; //マーカー個数設定中

                        StraightLineTool_Dialog.SetActive(true); //表示する
                        ModeSelectToggle_SetInteractable(false); //モード選択を無効化

                        Vector2 canvasMousePos = Vector2.zero; //Canvas内での位置
                        RectTransformUtility.ScreenPointToLocalPointInRectangle(MarkerCanvas_RectTransform, Input.mousePosition, UICamera, out canvasMousePos); //UI空間上でのマウス位置
                        StraightLineTool_Dialog.GetComponent<RectTransform>().anchoredPosition = canvasMousePos;
                    }
                }
                else
                {
                    lineRenderer.positionCount = 0; //線を消す
                    SLTD_SettingStep = 0; //0: 設定中ではない
                }
                break;

            case 2: break; //マーカーの個数が帰ってくるまで待機

            case 3: //マーカーの個数を確定させる（Applyのコールバック）
                int count = int.Parse(SLTD_Count_IF.text); //入力されている値の読み取り

                if (count < 3) // 3未満の場合は無効な入力値として決定しない
                {
                    SLTD_Count_IF.text = "3"; //最小値を上書きする
                    return;
                }

                // ----- 決定処理 ------
                Vector2 lineStartPos = markerList.Last().Position; //線の始点
                Vector2 lineVector = LineEndPos - lineStartPos; //線のベクトル
                assignedID = markerList.Last().ID; //割り当て済みIDを線の始点のものにする（始点のIDから連番）

                float step_width = 1.0f / (count - 1);
                for (int i = 1; i < count; i++)
                {
                    Vector2 pos = lineStartPos + lineVector * (step_width * i);
                    AddMarker(pos);
                }

                //設定終了
                goto case 5;

            case 5: // 設定ダイアログを閉じる
                StraightLineTool_Dialog.SetActive(false); //非表示する
                SLTD_SettingStep = 0; //未設定に戻す
                AllowToEditMarker = true;
                ModeSelectToggle_SetInteractable(true);
                break;

        }

    }


    /// <summary>Toggleの選択によるモード選択（UIのコールバックに設定）</summary>
    public void SelectMode(bool isOn, MarkerEditorMode mode_num)
    {
        if (!isOn) return; //選択されたトグルでなければ終了

        if (CurrentMode == mode_num) return; //選択されたものが現在のモードなら終了

        CurrentMode = mode_num;
    }

    /// <summary>ModeSelectToggleをユーザが選択できるかを変更</summary>
    public void ModeSelectToggle_SetInteractable(bool interactable)
    {
        Select_Toggle.interactable = interactable;
        Move_Toggle.interactable = interactable;
        Edit_Toggle.interactable = interactable;
    }

    public enum MarkerEditorMode
    {
        Select,
        Move,
        Edit
    }
}
