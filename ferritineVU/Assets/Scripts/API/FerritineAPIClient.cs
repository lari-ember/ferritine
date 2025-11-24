using System;
}
    }
        }
            }
                onSuccess?.Invoke(www.downloadHandler.text);
            {
            else
            }
                onError?.Invoke(www.error);
            {
            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)

            yield return www.SendWebRequest();
        {
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        string url = new Uri(new Uri(baseUrl), endpoint).ToString();
    {
    public IEnumerator GetRawJson(string endpoint, Action<string> onSuccess, Action<string> onError = null)

    }
        }
            }
                }
                    onError?.Invoke(ex.Message);
                {
                catch (Exception ex)
                }
                    onSuccess?.Invoke(state);
                    var state = JsonUtility.FromJson<WorldState>(json);
                    var json = www.downloadHandler.text;
                {
                try
            {
            else
            }
                onError?.Invoke(www.error);
            {
            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)

            yield return www.SendWebRequest();
        {
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        string url = new Uri(new Uri(baseUrl), "world/state").ToString();
    {
    public IEnumerator GetWorldState(Action<WorldState> onSuccess, Action<string> onError = null)

    public string baseUrl = "http://localhost:8000/";
    [Tooltip("Base URL da API, ex: https://api.exemplo.com/")]
{
public class FerritineAPIClient : MonoBehaviour
// Cliente HTTP simples compatível com UnityWebRequest

using UnityEngine.Networking;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

