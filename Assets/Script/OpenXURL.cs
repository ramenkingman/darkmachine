using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class OpenXURL : MonoBehaviour
{
    // Import the JavaScript function using the DllImport attribute
    [DllImport("__Internal")]
    private static extern void openURL(string url);

    public void OnMouseDown()
    {
        // Detect if the mouse is over the circleSpriteRenderer object
        Debug.Log("URL clicked."); 
        #if !UNITY_EDITOR && UNITY_WEBGL
        openURL("https://x.com/DarkMachineGame");
        #endif
    }
}
