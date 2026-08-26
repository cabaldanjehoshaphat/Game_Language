using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class play_to_language_scene : MonoBehaviour
{
    public void LoadByName(string sceneName)
    {
        SceneManager.LoadScene("Language_Selection_Menu");
        
    }
}
