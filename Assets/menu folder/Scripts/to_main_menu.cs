using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class to_main_menu : MonoBehaviour
{
    public void LoadByName(string sceneName)
    {
        SceneManager.LoadScene("Main_menu");
        
    }
}
