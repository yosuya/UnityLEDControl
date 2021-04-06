using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LEDTapeViewGroup : MonoBehaviour
{
    public static LEDTapeViewGroup Instance { get; private set; }

    public GameObject LEDTapeViewTemplate;
    private List<LEDTapeView> LEDTapeViewList = new List<LEDTapeView>();

    private int NumOfTapes;

    private void Awake()
    {
        Instance = this;
    }

    public void Setup(int num_of_tapes, int num_of_leds)
    {
        NumOfTapes = num_of_tapes;
        for (int i = 0; i < NumOfTapes; i++)
        {
            GameObject instance = Instantiate(LEDTapeViewTemplate, gameObject.transform);
            instance.name = $"LEDTapeView_{i}";

            LEDTapeView ledTapeView = instance.GetComponent<LEDTapeView>();
            LEDTapeViewList.Add(ledTapeView);
            ledTapeView.Setup(num_of_leds);

            instance.SetActive(true);
        }
    }

    public void SetColor(Color color)
    {
        for (int i = 0; i < NumOfTapes; i++)
        {
            LEDTapeViewList[i].SetColor(color);
        }
    }
}
