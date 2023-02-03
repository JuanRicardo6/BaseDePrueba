using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

public class RestApiManager : MonoBehaviour
{
    [SerializeField]
    RawImage[]YourRawImage;

    [SerializeField]
    TextMeshProUGUI userName;

    int i,ii;

    [SerializeField]
    int user;

    string card;
    string serverApiPath = "https://my-json-server.typicode.com/Bleysiker/JSON-Server";
    
    string rickYMortyApi = "https://rickandmortyapi.com/api";
    // Start is called before the first frame update
    void Start()
    {
        i = 0;
        ii = 0;
        //cards = "";
    }
    public void GetCharactersClick()
    {
        StartCoroutine(GetPlayerInfo());
    }
    IEnumerator GetCharacters(string cardsID)
    {
        UnityWebRequest www = UnityWebRequest.Get(rickYMortyApi + "/character/"+card);
        yield return www.Send();  //con estas dos lineas y el debug despues del error ya se tendria todo.


        //verifica si hay un error
        if (www.isNetworkError)
        {
            Debug.Log("NETWORK ERROR: "+www.error);
        }
        else
        {
            Debug.Log(www.downloadHandler.text);
            string json = www.downloadHandler.text;
            //verificar el codigo de respuesta
            if (www.responseCode == 200)
            {
                // todo sale bien, depende de la api puede ser 200 o 201 
                Character character = JsonUtility.FromJson<Character>(json);

                //la informacion pasa de un texto a un objeto
                //Debug.Log(characters.info.count);
                
                StartCoroutine(DownloadImage(character.image));
                
            }
            else
            {
                string mensaje = "Status : " + www.responseCode;  //codigo de respuesta
                mensaje += "\ncontent-type :" + www.GetResponseHeader("content-type"); //tipo de respuesta (JSON,XML, entre otros)
                mensaje += "\nError :" + www.error; //error
                Debug.Log(mensaje);
            }

            /*

            else if(www.responseCode == 404)
            {
                // no se encuentra nada
            }

    */
        }

    }
    IEnumerator GetPlayerInfo()
    {
        card = "";
        UnityWebRequest www = UnityWebRequest.Get(serverApiPath+"/users/"+user);
        yield return www.Send();  //con estas dos lineas y el debug despues del error ya se tendria todo.


        //verifica si hay un error
        if (www.isNetworkError)
        {
            Debug.Log("NETWORK ERROR: " + www.error);
        }
        else
        {
            //Debug.Log(www.downloadHandler.text);
            
            if (www.responseCode == 200)
            {
                // todo sale bien, depende de la api puede ser 200 o 201 
                UserJsonData u= JsonUtility.FromJson<UserJsonData>(www.downloadHandler.text);
                //la informacion pasa de un texto a un objeto
                userName.text="Username: "+u.name+"\nCards";


                foreach (int cID in u.deck)
                {
                    Debug.Log(cID);
                    card = cID.ToString();
                    if (i < 5)
                    {

                        StartCoroutine(GetCharacters(card));
                        i++;
                    }
                    else
                    {
                        i = 0;
                    }
                    
                }
                
            }
            else
            {
                string mensaje = "Status : " + www.responseCode;  //codigo de respuesta
                mensaje += "\ncontent-type :" + www.GetResponseHeader("content-type"); //tipo de respuesta (JSON,XML, entre otros)
                mensaje += "\nError :" + www.error; //error
                Debug.Log(mensaje);
            }

            /*

            else if(www.responseCode == 404)
            {
                // no se encuentra nada
            }

    */
        }

    }
    IEnumerator DownloadImage(string MediaUrl)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl);
        yield return request.SendWebRequest();

        //aca se mira si sale error
        if (request.isNetworkError || request.isHttpError)
            Debug.Log(request.error);

        else YourRawImage[ii].texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
        Debug.Log(MediaUrl);
        ii++;
        if (ii >4)
        {
            ii = 0;
            RestartVariables();
        }
    }
    void RestartVariables()
    {
        i = 0;
        ii = 0;
    }
}


// aca se construyen los objetos que voy a manipular
//este es un objeto modelo que es todo lo que va a contener o lo que vamos a interpretar del texto
//a todas las clases hay que darles una etiqueta serializable
[System.Serializable] //el system.serializable se usa para objetos que tengan elementos mas complejos
public class UserJsonData
{
    public int id;
    public string name;
    public int[] deck;
}

[System.Serializable]
public class CharactersList //tiene que llamarse igual a como viene en el response
{
    public CharactersListInfo info;
    public List<Character> results;
}
//aca solo metemos lo que queremos extraer de la api
[System.Serializable]
public class CharactersListInfo
{
    public int count;
    public int pages;
    public string next;
    public string prev;
}
[System.Serializable]
public class Character
{
    public int id;
    public string name;
    public string species;
    public string image;
}
