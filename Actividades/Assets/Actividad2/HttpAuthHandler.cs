using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class HttpAuthHandler : MonoBehaviour
{
    [SerializeField] string ServerApiUrl;
    [SerializeField] TextMeshProUGUI letrero;
    InputField usernameI,passwordI;

    public User user;

    public string username,token;
    // Start is called before the first frame update
    void Start()
    {
        letrero.text = "";
        ServerApiUrl = "https://sid-restapi.herokuapp.com";
        user = new User();
        usernameI= GameObject.Find("InputUsername").GetComponent<InputField>();
        passwordI= GameObject.Find("InputPassword").GetComponent<InputField>();
    }
    public void Registrar()
    {
        //la informacion que uno manda
        
        
        user.username = usernameI.text;
        user.password = passwordI.text;

        //aca se man
        string postData = JsonUtility.ToJson(user);
        StartCoroutine(Registro(postData));
    }
    public void Ingresar()
    {
        //la informacion que uno manda


        user.username = usernameI.text;
        user.password = passwordI.text;

        //aca se man
        string postData = JsonUtility.ToJson(user);
        StartCoroutine(Login(postData));
    }

    IEnumerator Registro(string postData)
    {
        //a que direccion y que informacion se manda
        UnityWebRequest www = UnityWebRequest.Put(ServerApiUrl + "/api/usuarios",postData);
        www.method = "POST";
        www.SetRequestHeader("Content-Type", "application/json");
        yield return www.SendWebRequest();  


        //verifica si hay un error
        if (www.isNetworkError)
        {
            Debug.Log("NETWORK ERROR: " + www.error);
        }
        else
        {
            Debug.Log(www.downloadHandler.text);
            //string json = www.downloadHandler.text;
            //verificar el codigo de respuesta
            if (www.responseCode == 200)
            {
                AuthJsonData jsonData = JsonUtility.FromJson<AuthJsonData>(www.downloadHandler.text);
                //Debug.Log(jsonData.usuario.username + " se registro con id: " + jsonData.usuario._id);
                letrero.text = "se ha registrado el usuario: " + jsonData.usuario.username;
                yield return new WaitForSeconds(2f);
                letrero.text = "";
                //proceso de autenticacion para obtener un token
            }
            else if (www.responseCode == 400)
            {
                letrero.text = "ese usuario ya existe";
                yield return new WaitForSeconds(2f);
                letrero.text = "intentelo de nuevo";
                yield return new WaitForSeconds(1.5f);
                letrero.text = "";
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
    IEnumerator Login(string postData)
    {
        //a que direccion y que informacion se manda
        UnityWebRequest www = UnityWebRequest.Put(ServerApiUrl + "/api/auth/login", postData);
        www.method = "POST";
        www.SetRequestHeader("Content-Type", "application/json");
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
                Debug.Log(jsonData.usuario.username + " inicio sesion");
                //proceso de autenticacion para obtener un token
                token = jsonData.token;
                Debug.Log(token);
                username = jsonData.usuario.username;
                PlayerPrefs.SetString("Username", username);
                PlayerPrefs.SetString("Token", token);
                letrero.text = "Bienvenido " + username;
                yield return new WaitForSeconds(2f);
                SceneManager.LoadScene("Menu Principal");
            }
            else
            {
                string mensaje = "Status : " + www.responseCode;  //codigo de respuesta
                mensaje += "\ncontent-type :" + www.GetResponseHeader("content-type"); //tipo de respuesta (JSON,XML, entre otros)
                mensaje += "\nError :" + www.error; //error
                Debug.Log(mensaje);
                letrero.text = "intentelo de nuevo";
                yield return new WaitForSeconds(2f);
                letrero.text = "";
            }
        }

    }
    //IEnumerator GetPerfil()
    //{
    //    //a que direccion y que informacion se manda
    //    UnityWebRequest www = UnityWebRequest.Get(ServerApiUrl + "/api/usuarios/"+username);
    //    //para enviar el token
    //    www.SetRequestHeader("x-token", token);
    //    yield return www.SendWebRequest();  //con estas dos lineas y el debug despues del error ya se tendria todo.


    //    //verifica si hay un error
    //    if (www.isNetworkError)
    //    {
    //        Debug.Log("NETWORK ERROR: " + www.error);
    //    }
    //    else
    //    {
    //        Debug.Log(www.downloadHandler.text);
    //        string json = www.downloadHandler.text;
    //        //verificar el codigo de respuesta
    //        if (www.responseCode == 200)
    //        {
    //            AuthJsonData jsonData = JsonUtility.FromJson<AuthJsonData>(www.downloadHandler.text);
    //            Debug.Log(jsonData.usuario.username + " sigue con la sesion iniciada");
    //            //cambiar de escena
                
    //        }
    //        else
    //        {
    //            string mensaje = "Status : " + www.responseCode;  //codigo de respuesta
    //            mensaje += "\ncontent-type :" + www.GetResponseHeader("content-type"); //tipo de respuesta (JSON,XML, entre otros)
    //            mensaje += "\nError :" + www.error; //error
    //            Debug.Log(mensaje);
    //        }

    //        /*

    //        else if(www.responseCode == 404)
    //        {
    //            // no se encuentra nada
    //        }

    //*/
    //    }

    //}
}
[System.Serializable]
public class User
{
    public string _id;
    public string username;
    public string password;
    public UserData data;

    public User()
    {
    }
    public User(string username,string password)
    {
        this.username = username;
        this.password = password;
    }
}
[System.Serializable]
public class AuthJsonData
{
    public User usuario;
    public string token;
    
}


[System.Serializable]
public class UserData
{
    public int score;
}



