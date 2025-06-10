using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class GeminiAPIHandler : MonoBehaviour
{
    //IMPORTANTE: La API Key no debe ser expuesta en el código fuente.
    [SerializeField]
    private string apiKey;

    private string geminiProModel = "gemini-2.0-flash-lite";

    //Clases internas para mapear la estructura de la respuesta JSON de la API.
    [System.Serializable]
    private class GeminiResponse
    {
        public Candidate[] candidates; //Lista de candidatos generados por la IA.
    }

    [System.Serializable]
    private class Candidate
    {
        public Content content; //Contenido generado por la IA.
    }

    [System.Serializable]
    private class Content
    {
        public Part[] parts; //Partes del contenido generado.
    }

    [System.Serializable]
    private class Part
    {
        public string text; //Texto generado por la IA.
    }

    //Método para enviar una consulta a la API. Recibe una consulta del usuario y un callback para manejar la respuesta.
    public void SendQueryToGemini(string userQuery, System.Action<string> callback)
    {
        StartCoroutine(PostRequest(userQuery, callback)); //Llama a la corrutina para enviar la solicitud.
    }

    //Corrutina que realiza una solicitud POST a la API.
    IEnumerator PostRequest(string query, System.Action<string> callback)
    {
        //URL de la API
        string url = $"https://generativelanguage.googleapis.com/v1beta/models/{geminiProModel}:generateContent?key={apiKey}";

        //Cuerpo de la solicitud JSON.
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

        //Configuración de la solicitud HTTP POST.
        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            //Convierte el JSON a un arreglo de bytes y lo asigna al cuerpo de la solicitud.
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonPayload);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json"); //Establece el encabezado de tipo de contenido.

            //Espera a que se complete la solicitud.
            yield return request.SendWebRequest();

            //Manejo de errores en la solicitud.
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Error: {request.error}"); 
                callback?.Invoke("Error al contactar a la IA."); //Llama al callback con un mensaje de error.
            }
            else
            {
                //Procesa la respuesta JSON de la API.
                string jsonResponse = request.downloadHandler.text;
                try
                {
                    //Convierte el JSON de respuesta en un objeto GeminiResponse.
                    GeminiResponse responseData = JsonUtility.FromJson<GeminiResponse>(jsonResponse);

                    //Extrae el texto generado por la IA.
                    string responseText = responseData?.candidates?[0]?.content?.parts?[0]?.text;

                    //Llama al callback con el texto generado o un mensaje predeterminado si no hay respuesta.
                    callback?.Invoke(responseText ?? "No se recibi� respuesta clara.");
                }
                catch (System.Exception e)
                {
                    //Manejo de errores al parsear el JSON.
                    Debug.LogError($"Error al parsear JSON (JsonUtili ty): {e.Message}");
                    callback?.Invoke("Error al procesar la respuesta de la IA.");
                }
            }
        }
    }
}
