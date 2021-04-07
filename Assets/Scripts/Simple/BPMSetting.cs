using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class BPMSetting : MonoBehaviour
{
    public TMP_InputField BPM_IF;
    public Button BPM_Button;

    static public float currentBPM = 120;

    private void Start()
    {
        BPM_Button.onClick.AddListener(() => Tap());
        BPM_IF.onEndEdit.AddListener((x) => SetBPM(float.Parse(x)));
    }

    private void Update()
    {

    }

    private float[] bpm_deltaTime = new float[5];
    private float prevTime;
    public void Tap()
    {
        float currentTime = Time.time;
        float dTime = currentTime - prevTime; //前のタップとの差分
        prevTime = currentTime; //保持する
        if (dTime > 1) //前にタップした時間と離れていたら
        {
            bpm_deltaTime = new float[5];
            return;
        }

        //配列をシフトして新しいデータを書き込む
        int dataCount = 0; //データの個数を記録
        for (int i = 4; i >= 0; i--)
        {
            if (i > 0) bpm_deltaTime[i] = bpm_deltaTime[i - 1];
            else bpm_deltaTime[i] = dTime;

            if (bpm_deltaTime[i] > 0) dataCount++; //データが0でなかったらカウント
        }

        //1ビートの間隔の平均値を計算
        float dTimeAvg = bpm_deltaTime[0];
        for (int i = 1; i < 5; i++)
        {
            dTimeAvg += bpm_deltaTime[i];
        }
        dTimeAvg = dTimeAvg / dataCount;

        currentBPM = 60f / dTimeAvg; //現在のBPMを算出
        currentBPM = Mathf.Round(currentBPM * 100f) / 100f; //小数点第2位までに丸める

        BPM_IF.text = currentBPM.ToString();
    }

    public void SetBPM(float value)
    {
        currentBPM = value;
    }

}
