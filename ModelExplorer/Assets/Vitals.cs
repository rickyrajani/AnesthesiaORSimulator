using UnityEngine;
using System;

public class Vitals : MonoBehaviour {
    string text1 = "70";
    string text2 = "97F";
    string text3 = "140/90";
    float number1 = 0;
    float number2 = 0;
    float number3 = 0;
    // Use this for initialization
    void Start () {
        GetComponent<TextMesh>().text = "HB: " + text1 + "\nBP: " + text2 + "\nTEMP: " + text3.Replace("\n", Environment.NewLine);
    }
	
	// Update is called once per frame
	void Update () {
        // Change this so data is received from elsewhere to update number for vitals
        // number = PlayerPrefs.GetInt("points");
        // text = number.ToString();
        GetComponent<TextMesh>().text = "HB: " + text1 + "\nBP: " + text2 + "\nTEMP: " + text3.Replace("\n", Environment.NewLine);
    }
}
