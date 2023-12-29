using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SwicherInput : MonoBehaviour
{
    [SerializeField] TMP_InputField[] inputField;
    public TNVirtualKeyboard inputTarget;
    public void Swich(int index)
    {
        inputTarget.targetText = inputField[index];
        if (inputTarget.targetText.contentType == TMP_InputField.ContentType.Password)
		{
			inputTarget.exampleTxt.text = "";
			for (int i = 0; i < inputTarget.targetText.text.Length - 1; i++)
			{
				inputTarget.exampleTxt.text += "*";
			}
            if (inputTarget.exampleTxt.text.Length > 0)
			    inputTarget.exampleTxt.text += inputTarget.targetText.text[inputTarget.targetText.text.Length - 1];
		}
		else
		{
			inputTarget.exampleTxt.text = inputTarget.targetText.text;	
		}
    }
}
