using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LEDTapeViewGroup : MonoBehaviour
{
    public GameObject LEDTapeViewTemplate;
    private List<LEDTapeView> LEDTapeViewList = new List<LEDTapeView>();

    public int NumOfTapes;

    private void Start()
    {
        SetLEDTapes(NumOfTapes);
    }

    public void SetLEDTapes(int num_of_tapes)
    {
        for (int i = 0; i < num_of_tapes; i++)
        {
            GameObject instance = Instantiate(LEDTapeViewTemplate, gameObject.transform);
            instance.name = $"LEDTapeView_{i}";
            RectTransform rectTransform = instance.GetComponent<RectTransform>();
            instance.SetActive(true);
        }
    }
}
