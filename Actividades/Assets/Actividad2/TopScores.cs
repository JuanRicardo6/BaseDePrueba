using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;


public class TopScores : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI[] top;
    [SerializeField] string token,ServerApiUrl;

    [SerializeField] string[] jugador;
    [SerializeField] int[] puntajeJugador;

    void Start()
    {
        token = PlayerPrefs.GetString("Token");
        ServerApiUrl= "https://sid-restapi.herokuapp.com";
        StartCoroutine(EnlistarScores());

    }

    IEnumerator EnlistarScores()
    {
            UnityWebRequest www = UnityWebRequest.Get(ServerApiUrl + "/api/usuarios");
            www.SetRequestHeader("x-token", token);
            yield return www.SendWebRequest();
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
                Usuarios jsonData = JsonUtility.FromJson<Usuarios>(www.downloadHandler.text);
                //Debug.Log(jsonData.usuarios[0].username);
                for(int i = 0; i < 5; i++)
                {
                    DeterminarLugar(jsonData, i, top[i]);
                }
            }
                else
                {
                    string mensaje = "Status : " + www.responseCode;  //codigo de respuesta
                    mensaje += "\ncontent-type :" + www.GetResponseHeader("content-type"); //tipo de respuesta (JSON,XML, entre otros)
                    mensaje += "\nError :" + www.error; //error
                    Debug.Log(mensaje);

                }
            }
    }
    void DeterminarLugar(Usuarios jsonData,int puesto,TextMeshProUGUI top)
    {

        if (puesto == 0)
        {
            //para el primer puesto
            for (int i = 0; i < jsonData.usuarios.Length; i++)
            {
                    if (puntajeJugador[0] < jsonData.usuarios[i].data.score)
                    {

                        puntajeJugador[0] = jsonData.usuarios[i].data.score;
                        jugador[0] = jsonData.usuarios[i].username;
                    }
            }
            top.text = "1. " + jugador[0] + " : " + puntajeJugador[0];
        }
        else
        {
            for (int i = 0; i < jsonData.usuarios.Length; i++)
            {
               
                    if (puntajeJugador[puesto] < jsonData.usuarios[i].data.score)
                    {
                    if (jsonData.usuarios[i].data.score < puntajeJugador[puesto - 1])
                    {
                        puntajeJugador[puesto] = jsonData.usuarios[i].data.score;
                        jugador[puesto] = jsonData.usuarios[i].username;
                    }
                    }
                   
            }
            top.text = (puesto+1)+". " + jugador[puesto] + " : " + puntajeJugador[puesto];
        }
    }
}

[System.Serializable]
public class Usuarios
{
    public User[] usuarios;
}
