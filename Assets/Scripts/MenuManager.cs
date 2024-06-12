using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NewBehaviourScript : MonoBehaviour
{

    private PlayerController targetPlayer;
    public void ChangeScene(string _sceneName){
        SceneManager.LoadScene(_sceneName);
    }

    public void Quit(){
        Application.Quit();
    }
}
