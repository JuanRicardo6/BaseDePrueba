using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using TMPro;
public class AutenticationLogin : MonoBehaviour
{
    [SerializeField] string ServerApiUrl,username,token;
    [SerializeField] TextMeshProUGUI letrero;


    void Start()
    {
        ServerApiUrl = "https://sid-restapi.herokuapp.com";
        username = PlayerPrefs.GetString("Username");
        token = PlayerPrefs.GetString("Token");
        if (string.IsNullOrEmpty(token))
        {
            Debug.Log("No hay token");
            SceneManager.LoadScene("Login Inicial");
        }
        else
        {
            Debug.Log(token);
            Debug.Log(username);
            //verificar token
            StartCoroutine(GetPerfil());
        }
        
    }

    IEnumerator GetPerfil()
    {
        //a que direccion y que informacion se manda
        yield return new WaitForSeconds(5f);
        UnityWebRequest www = UnityWebRequest.Get(ServerApiUrl + "/api/usuarios/" + username);
        //para enviar el token
        www.SetRequestHeader("x-token", token);
        yield return www.SendWebRequest();  


        //verifica si hay un error
        if (www.isNetworkError)
        {
            Debug.Log("NETWORK ERROR: " + www.error);
        }
        else
        {
            Debug.Log(www.downloadHandler.text);
            string json = www.downloadHandler.text;
            //verificar el codigo de respuesta
            if (www.responseCode == 200)
            {
                AuthJsonData jsonData = JsonUtility.FromJson<AuthJsonData>(www.downloadHandler.text);
                letrero.text = "Bienvenido de vuelta " + jsonData.usuario.username;
                //Debug.Log(jsonData.usuario.username + " sigue con la sesion iniciada");
                //cambiar de escena
                yield return new WaitForSeconds(3f);
                SceneManager.LoadScene("Menu Principal");
            }
            else
            {
                string mensaje = "Status : " + www.responseCode;  //codigo de respuesta
                mensaje += "\ncontent-type :" + www.GetResponseHeader("content-type"); //tipo de respuesta (JSON,XML, entre otros)
                mensaje += "\nError :" + www.error; //error
                Debug.Log(mensaje);
                letrero.text = "Redireccionando a pagina de inicio";
                yield return new WaitForSeconds(3f);
                SceneManager.LoadScene("Login Inicial");
            }
        }

    }
}
