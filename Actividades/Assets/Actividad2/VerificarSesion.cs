using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.SceneManagement;

public class VerificarSesion : MonoBehaviour
{
    [SerializeField] GameObject boton1, boton2;
    [SerializeField] string ServerApiUrl, username, token;
    [SerializeField] TextMeshProUGUI letrero,usuario;


    void Start()
    {
        boton1.SetActive(false);
        boton2.SetActive(false);
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
            usuario.text = username;
            //verificar token
            StartCoroutine(GetPerfil());
        }

    }

    IEnumerator GetPerfil()
    {
        //a que direccion y que informacion se manda
        yield return new WaitForSeconds(2f);
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
                letrero.text = "Sesion activa";
                yield return new WaitForSeconds(1f);
                boton1.SetActive(true);
                boton2.SetActive(true);
                //Debug.Log(jsonData.usuario.username + " sigue con la sesion iniciada");
                //cambiar de escena
            }
            else
            {
                string mensaje = "Status : " + www.responseCode;  //codigo de respuesta
                mensaje += "\ncontent-type :" + www.GetResponseHeader("content-type"); //tipo de respuesta (JSON,XML, entre otros)
                mensaje += "\nError :" + www.error; //error
                Debug.Log(mensaje);
                letrero.text = "Sesion vencida.....redireccionando a pagina de inicio";
                yield return new WaitForSeconds(2f);
                SceneManager.LoadScene("Login Inicial");
            }
        }

    }
}
