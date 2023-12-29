using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TNVirtualKeyboard : MonoBehaviour
{
	
	public static TNVirtualKeyboard instance;
	
	public string words = "";
	
	public GameObject vkCanvas;
	
	public TMP_InputField targetText;
	public TMP_InputField exampleTxt;
	
	
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
		HideVirtualKeyboard();
		
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	
	public void KeyPress(string k){
		targetText.text += k;	
		if (targetText.contentType == TMP_InputField.ContentType.Password)
		{
			exampleTxt.text = "";
			for (int i = 0; i < targetText.text.Length - 1; i++)
			{
				exampleTxt.text += "*";
			}
			exampleTxt.text += targetText.text[targetText.text.Length - 1];
		}
		else
		{
			exampleTxt.text = targetText.text;	
		}	
	}
	
	public void Del(){
		if (targetText.text.Length < 1)	return;
		
		targetText.text = targetText.text.Substring(0, targetText.text.Length - 1);	
		if (targetText.contentType == TMP_InputField.ContentType.Password)
		{
			exampleTxt.text = "";
			for (int i = 0; i < targetText.text.Length - 1; i++)
			{
				exampleTxt.text += "*";
			}
			exampleTxt.text += targetText.text[targetText.text.Length - 1];
		}
		else
		{
			exampleTxt.text = targetText.text;	
		}
	}
	
	public void ShowVirtualKeyboard(){
		vkCanvas.SetActive(true);
	}
	
	public void HideVirtualKeyboard(){
		vkCanvas.SetActive(false);
	}
}
