using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuScript : MonoBehaviour
{
    public static int inputSeed;
    public InputField inputField;

    public void Play()
    {
        
            int.TryParse(inputField.text, out inputSeed);
            Debug.Log(inputSeed);

        SceneManager.LoadScene("WorldScene");
        
    }

    public void Quit()
    {
        Debug.Log("Exiting the game..");
        Application.Quit();
    }
}
