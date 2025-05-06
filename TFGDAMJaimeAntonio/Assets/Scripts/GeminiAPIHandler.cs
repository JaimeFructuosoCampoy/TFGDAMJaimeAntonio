using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class GeminiAPIHandler : MonoBehaviour
{
    [SerializeField]
    private string apiKey = "AIzaSyDpUT5kFtd7h4LnksO9JRexG1izcwAzko0";

    private string geminiProModel = "gemini-2.0-flash-lite";

    [System.Serializable]
    private class GeminiResponse
    {
        public Candidate[] candidates;
    }

    [System.Serializable]
    private class Candidate
    {
        public Content content;
    }

    [System.Serializable]
    private class Content
    {
        public Part[] parts;
    }

    [System.Serializable]
    private class Part
    {
        public string text;
    }

    public void SendQueryToGemini(string userQuery, System.Action<string> callback)
    {
        StartCoroutine(PostRequest(userQuery, callback));
    }

    IEnumerator PostRequest(string query, System.Action<string> callback)
    {
        string url = $"https://generativelanguage.googleapis.com/v1beta/models/{geminiProModel}:generateContent?key={apiKey}";

        string jsonPayload = $@"  
       {{  
           ""contents"": [  
               {{  
                   ""parts"": [  
                       {{  
                           ""text"": ""{query}""  
                       }}  
                   ]  
               }}  
           ]  
       }}";

        Debug.Log("JSON Payload: " + jsonPayload);

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonPayload);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json"); // Reemplazado el método inexistente SetContentType  

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Error: {request.error}");
                callback?.Invoke("Error al contactar a la IA.");
            }
            else
            {
                string jsonResponse = request.downloadHandler.text;
                try
                {
                    GeminiResponse responseData = JsonUtility.FromJson<GeminiResponse>(jsonResponse);
                    string responseText = responseData?.candidates?[0]?.content?.parts?[0]?.text;
                    callback?.Invoke(responseText ?? "No se recibió respuesta clara.");
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Error al parsear JSON (JsonUtility): {e.Message}");
                    callback?.Invoke("Error al procesar la respuesta de la IA.");
                }
            }
        }
    }
}
