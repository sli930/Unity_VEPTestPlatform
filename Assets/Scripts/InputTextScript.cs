using System;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class InputTextScript : MonoBehaviour
{
    public GameObject text_Prefab;
    public Transform middle_Panel;

    private TMP_InputField input_Field;
    private bool isCalculated = false;
    private int number_length = 10;
    private string randomNumber_String = string.Empty;
    private List<GameObject> gameObjects = new List<GameObject>();

    void Start()
    {
        input_Field = GetComponent<TMP_InputField>();
        input_Field.ActivateInputField();
        GeneratRandomNumber(number_length);
    }

    public void CheckCorrect()
    {
        int inputField_length = input_Field.text.Length;
        if (inputField_length > 0)
        {
            int index = Math.Min(inputField_length - 1, 9);

            TextMeshProUGUI text_Text = gameObjects[index].GetComponent<TextMeshProUGUI>();
            if (input_Field.text[index] == char.Parse(text_Text.text))
            {
                text_Text.color = Color.green;
            }
            else
            {
                text_Text.color = Color.red;
            }
        }


        if (!isCalculated && inputField_length == number_length)
        {
            Debug.Log($"Result: {CalculateCorrectRate()}");
            isCalculated = true;
        }
    }

    public double CalculateCorrectRate()
    {
        double correct = 0;
        for (int i = 0; i < number_length; i++)
        {
            if (randomNumber_String[i] == input_Field.text[i]) correct++;
        }
        return correct/number_length;
    }

    private void GeneratRandomNumber(int length)
    {
        for (int i = 0; i < length; i++)
        {
            string randomInteger_String = UnityEngine.Random.Range(0, 10).ToString();

            randomNumber_String += randomInteger_String;

            GameObject inputText = Instantiate(text_Prefab);
            inputText.transform.SetParent(middle_Panel);
            inputText.GetComponent<TextMeshProUGUI>().text = randomInteger_String;
            gameObjects.Add(inputText);
        }
    }

    public void PrintContent(string content)
    {
        Debug.Log(content);
    }

    public void FocusInputField()
    {
        input_Field.ActivateInputField();
    }
}
