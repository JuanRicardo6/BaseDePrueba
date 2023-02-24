using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class UpdateScore : MonoBehaviour
{
    
    [SerializeField] string ServerApiUrl,token,username;
    [SerializeField] TextMeshProUGUI scoreText;
    int score;
    public User user;
    // Start is called before the first frame update
    void Start()
    {
        scoreText.gameObject.SetActive(false);
        score = PlayerPrefs.GetInt("ActualPlayerScore");
        ServerApiUrl = "https://sid-restapi.herokuapp.com";
        user = new User();
        user.data = new UserData();
        //scoreField = GameObject.Find("ScoreField").GetComponent<InputField>();
        username = PlayerPrefs.GetString("Username");
        
        token = PlayerPrefs.GetString("Token");
        StartCoroutine(GetPlayerScore());
    }
    public void ActualizarScore()
    {
        //la informacion que uno manda


        user.username = username;
        score = Random.Range(1, 10000);
        
        PlayerPrefs.SetInt("ActualPlayerScore", score);
        user.data.score = score;

        //aca se manda
        string postData = JsonUtility.ToJson(user);
        Debug.Log(postData);
        StartCoroutine(AjustarPuntaje(postData));
        
    }
    IEnumerator AjustarPuntaje(string postData)
    {
        //a que direccion y que informacion se manda
        UnityWebRequest www = UnityWebRequest.Put(ServerApiUrl + "/api/usuarios", postData);
        www.method = "PATCH";
        www.SetRequestHeader("Content-Type", "application/json");
        www.SetRequestHeader("x-token", token);
        
        yield return www.SendWebRequest();  //con estas dos lineas y el debug despues del error ya se tendria todo.


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
                Debug.Log(jsonData.usuario.data.score);
                StartCoroutine(GetPlayerScore());
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
    IEnumerator GetPlayerScore()
    {
        UnityWebRequest www = UnityWebRequest.Get(ServerApiUrl + "/api/usuarios/"+ username);
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
                AuthJsonData jsonData = JsonUtility.FromJson<AuthJsonData>(www.downloadHandler.text);
                scoreText.text = "score: " + jsonData.usuario.data.score;
                scoreText.gameObject.SetActive(true);
                //Debug.Log(jsonData.usuario.data.score); 
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
}

