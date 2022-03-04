using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.Events;


public class ApiRest : MonoBehaviour
{
    [SerializeField]
    private string BaseURL;

    private float tempScore = 0;

    //Esta shit deberia ir en otro lado pero aja
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text SignUpDone;
    [SerializeField] GameObject loginPanel;
    [SerializeField] public static bool loginSucces = false;
    private bool sumintComplete=false;

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        GameObject.Find("Password").GetComponent<TMP_InputField>().contentType = TMP_InputField.ContentType.Password;
        //ElementSystem.death_event += SumbitPoints;
        //if (WizardLegacy.revive) ElementSystem.death_event2 += SumbitPoints;
        Player.OnDestruction += SumbitPoints;
        //Player.OnDestruction += GetScore;

    }
    private void Update()
    {
        if (sumintComplete == true)
        {
            GetScore();
            sumintComplete = false;
        }
    }
    public void Registro()
    {
       
        
        JSONAuthData data = new JSONAuthData();
        data.username = GameObject.Find("UserName").GetComponent<TMP_InputField>().text;
        data.password = GameObject.Find("Password").GetComponent<TMP_InputField>().text;
        string postData = JsonUtility.ToJson(data);
        StartCoroutine(RegistroPost(postData));
    }
    public void Login()
    {
        
        JSONAuthData data = new JSONAuthData();
        data.username = GameObject.Find("UserName").GetComponent<TMP_InputField>().text;
        data.password = GameObject.Find("Password").GetComponent<TMP_InputField>().text;
        string postData = JsonUtility.ToJson(data);
        StartCoroutine(LoginPost(postData));
    }
    public void GetScore()
    {

        string token = PlayerPrefs.GetString("token");
        StartCoroutine(GetScore(token));
        Player.OnDestruction -= GetScore;

    }

    public void SumbitPoints()
    {
        //ElementSystem.death_event -= SumbitPoints;
        //if (WizardLegacy.revive) ElementSystem.death_event2 -= SumbitPoints;
        if(tempScore< PlayerPrefs.GetInt("bestScore"))
        {
            UserData data = new UserData();
            data.username = PlayerPrefs.GetString("userName");
            data.score = PlayerPrefs.GetInt("bestScore");
            var postData = JsonUtility.ToJson(data);
            var token = PlayerPrefs.GetString("token");
            StartCoroutine(SumbitPoints(postData, token));
            Player.OnDestruction -= SumbitPoints;
        }

    }

    IEnumerator RegistroPost(string postData)
    {
        string url = BaseURL + "/usuarios";
        UnityWebRequest req = UnityWebRequest.Put(url, postData);
        req.method = "POST";
        req.SetRequestHeader("content-type", "application/json");
        //Proceso UI        Debug.Log("Sendind Request...");
        yield return req.SendWebRequest();
        if (req.isNetworkError)
        {
            Debug.Log("NETWORK ERROR ;" + req.error);
            //Proceso UI
        }
        else
        {
            Debug.Log(req.downloadHandler.text);
            if (req.responseCode == 200)
            {
                
                JSONReceiver resData = JsonUtility.FromJson<JSONReceiver>(req.downloadHandler.text);

                Debug.Log(resData.usuario.username);
                SignUpDone.gameObject.SetActive(true);
            }


        }

       
    }
    IEnumerator LoginPost(string postData)
    {

        string url = BaseURL + "/auth/login";
        UnityWebRequest req = UnityWebRequest.Put(url, postData);
        req.method = "POST";
        req.SetRequestHeader("content-type", "application/json");
        yield return req.SendWebRequest();
        if (req.isNetworkError)
        {
            Debug.Log("NETWORK ERROR ;" + req.error);

        }
        else
        {
            Debug.Log(req.downloadHandler.text);
            if (req.responseCode == 200)
            {
                SignUpDone.gameObject.SetActive(false);
                JSONReceiver logData = JsonUtility.FromJson<JSONReceiver>(req.downloadHandler.text);
                //Debug.Log(logData.usuario.username);
                PlayerPrefs.SetString("token", logData.token);
                Debug.Log(PlayerPrefs.GetString("token"));
                PlayerPrefs.SetString("userName", logData.usuario.username);
                Debug.Log(PlayerPrefs.GetString("userName"));
                tempScore = logData.usuario.score;
                loginSucces = true;
                loginPanel.SetActive(false);
                Time.timeScale = 1;
            }
        }
    }
    IEnumerator GetScore(string token)
    {
        string url = BaseURL + "/usuarios?limit=20&sort=1";
        UnityWebRequest req = UnityWebRequest.Get(url);
        req.method = "GET";
        req.SetRequestHeader("content-type", "application/json"); //NECESARIO
        req.SetRequestHeader("x-token", token);

        Debug.Log("Sendind Get Perfil Request...");
        yield return req.SendWebRequest();
        if (req.isNetworkError)
        {
            Debug.Log("NETWORK ERROR ;" + req.error);
        }
        //Proceso UI        }
        else
        {
            Debug.Log(req.downloadHandler.text);
            if (req.responseCode == 200)
            {
                //JSONAuthData resData = JsonUtility.FromJson<JSONAuthData>(req.downloadHandler.text);
                JSONscore resData = JsonUtility.FromJson<JSONscore>(req.downloadHandler.text);
                scoreText.text = "Jugador" + "   " + "Puntaje\n";
                for (int i = 0; i < resData.usuarios.Count; i++)
                {
                    scoreText.text += resData.usuarios[i].username + "   " + resData.usuarios[i].score + "\n";
                }
                Debug.Log("Autenticacion Exitosa");

            }
            else
            {
                string messsage = "Status :" + req.responseCode;
                messsage += "\ncontent-type:" + req.GetRequestHeader("content-type");
                messsage += "\nError :" + req.error;
                Debug.Log(messsage);
            }
        }
    }
    IEnumerator SumbitPoints(string postData, string token)
    {

        string url = BaseURL + "/usuarios";
        UnityWebRequest req = UnityWebRequest.Put(url, postData);
        req.method = "PATCH";
        req.SetRequestHeader("content-type", "application/json");
        req.SetRequestHeader("x-token", token);

        Debug.Log("Sendind Get Perfil Request...");
        yield return req.SendWebRequest();
        if (req.isNetworkError)
        {
            Debug.Log("NETWORK ERROR ;" + req.error);
        }
        //Proceso UI        }
        else
        {
            Debug.Log(req.downloadHandler.text);
            if (req.responseCode == 200)
            {
                UserData resData = JsonUtility.FromJson<UserData>(req.downloadHandler.text);
                Debug.LogError("El puntaje fue actualizado");
                sumintComplete = true;

            }
            else
            {
                string messsage = "Status :" + req.responseCode;
                messsage += "\ncontent-type:" + req.GetRequestHeader("content-type");
                messsage += "\nError :" + req.error;
                Debug.Log(messsage);
            }
        }

    }
}
[System.Serializable]
class JSONAuthData
{
    public string username;
    public string password;
}
[System.Serializable]
class JSONReceiver
{
    public UserData usuario;
    public string token;
}
[System.Serializable]
class JSONscore
{
    public List<UserData> usuarios;
}
[System.Serializable]
class UserData
{
    public int _id;
    public string username;
    public bool estado;
    public float score;
}
