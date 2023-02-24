using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonScene : MonoBehaviour
{
    [SerializeField] string scene;
    

    public void CargarEscena()
    {
        SceneManager.LoadScene(scene);
    }
}
