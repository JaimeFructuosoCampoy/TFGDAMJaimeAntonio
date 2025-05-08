using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using UnityEngine.LowLevel;
using UnityEngine.SceneManagement;
public class SupabaseDAO : MonoBehaviour
{
    private static SupabaseDAO instance;
    public static SupabaseDAO Instance => instance;

    private string EdgeFunctionUrl = "https://your-edge-function-url.supabase.co/function/v1/your-function-name";

    private string AccessToken;
    private string RefreshToken;
    private long TokenExpirationTime;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {

    }

    //Hacer la petición a la API de Supabase para iniciar sesión y después
    //establecer en un script modelo de jugador los datos del jugador y en GlobalData
    //establecer el inicio de sesión
    public void Login(string email, string password)
    {
        if (!string.IsNullOrEmpty(AccessToken) && !IsTokenExpired())
        {
            Debug.Log("El token aún es válido. No es necesario iniciar sesión nuevamente.");
            return;
        }
        StartCoroutine(LoginCoroutine(email, password));
    }

    // Después de que el usuario se registre como usuario:
    public async Task CreatePlayer(string playerName)
    {

    }

    IEnumerator LoginCoroutine(string email, string password)
    {
        string url = "https://bxjubueuyzobmpvdwefk.supabase.co/auth/v1/token?grant_type=password";

        string jsonData = JsonConvert.SerializeObject
        (new
        {
            email = email,
            password = password
        }
        );

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("apikey", GlobalData.SUPABASE_DB_KEY);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                var response = JsonConvert.DeserializeObject<AuthResponse>(request.downloadHandler.text);
                StoreTokens(response.access_token, response.refresh_token, response.expires_in);
                Debug.Log(request.downloadHandler.text);
                StartCoroutine(LoginPlayerCoroutine());
            }
            else
            {
                Debug.Log("Error: " + request.error + request.downloadHandler.text);
            }
        }
    }

    public void StoreTokens(string accessToken, string refreshToken, int expiresIn)
    {
        AccessToken = accessToken;
        RefreshToken = refreshToken;
        TokenExpirationTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds() + expiresIn;

        PlayerPrefs.SetString("supabase_refresh_token", RefreshToken);
    }

    public bool IsTokenExpired()
    {
        return DateTimeOffset.UtcNow.ToUnixTimeSeconds() >= TokenExpirationTime - 60;
    }

    IEnumerator LoginPlayerCoroutine()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(GlobalData.SUPABASE_DB_URL + $"Player?select=*"))
        {
            webRequest.SetRequestHeader("apikey", GlobalData.SUPABASE_DB_KEY);
            webRequest.SetRequestHeader("Authorization", $"Bearer {AccessToken}");
            webRequest.SetRequestHeader("Content-Type", "application/json");

            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Datos recibidos: " + webRequest.downloadHandler.text);

                try
                {
                    PlayerData playerData = null;
                    var playerList = JsonConvert.DeserializeObject<List<PlayerData>>(webRequest.downloadHandler.text);
                    if (playerList != null && playerList.Count > 0)
                    {
                        playerData = playerList[0];
                    }
                    else
                    {
                        Debug.LogError("No se encontraron datos del jugador.");
                    }

                    if (playerData != null)
                    {
                        Debug.Log($"Player ID: {playerData.id}, Coins: {playerData.coins}, Points: {playerData.points}");
                        PlayerLoggedIn.InitializeOrUpdatePlayerData(playerData);
                        GlobalData.PlayerLoggedIn = true;
                        SceneManager.LoadScene("MenuScene");
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError("Error al deserializar los datos del jugador: " + ex.Message);
                }
            }
            else
            {
                Debug.LogError("Error: " + webRequest.error);
            }
        }
    }
    //public IEnumerator SavePlayerProgress(PlayerData data)
    //{
    //    // Verificar si el token ha expirado
    //    if (IsTokenExpired())
    //    {
    //        Debug.Log("El token ha expirado. Intentando refrescar...");
    //        yield return RefreshTokenFunction((success) =>
    //        {
    //            if (!success)
    //            {
    //                Debug.LogError("No se pudo refrescar el token. Mostrar UI de reconexión.");
    //                yieldbreak;
    //            }
    //        });
    //    }

    //    // Continuar con la operación si el token es válido
    //    string jsonData = JsonUtility.ToJson(data);
    //    using (UnityWebRequest request = new UnityWebRequest("https://api.example.com/save-progress", "POST"))
    //    {
    //        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
    //        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
    //        request.downloadHandler = new DownloadHandlerBuffer();
    //        request.SetRequestHeader("Content-Type", "application/json");
    //        request.SetRequestHeader("Authorization", $"Bearer {AccessToken}");

    //        yield return request.SendWebRequest();

    //        if (request.result == UnityWebRequest.Result.Success)
    //        {
    //            Debug.Log("Progreso guardado exitosamente.");
    //        }
    //        else
    //        {
    //            Debug.LogError($"Error al guardar el progreso: {request.error}");
    //        }
    //    }
    //}

    public IEnumerator RefreshTokenFunction(Action<bool> onComplete)
    {
        if (string.IsNullOrEmpty(RefreshToken))
        {
            RefreshToken = PlayerPrefs.GetString("supabase_refresh_token");
        }

        WWWForm form = new WWWForm();
        form.AddField("grant_type", "refresh_token");
        form.AddField("refresh_token", RefreshToken);

        using (UnityWebRequest request = UnityWebRequest.Post(
            GlobalData.SUPABASE_DB_URL, form))
        {
            request.SetRequestHeader("apikey", GlobalData.SUPABASE_DB_KEY);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                var response = JsonUtility.FromJson<AuthResponse>(request.downloadHandler.text);
                StoreTokens(response.access_token, response.refresh_token, response.expires_in);
                onComplete?.Invoke(true);
            }
            else
            {
                Debug.LogError($"Refresh failed: {request.error}");
                onComplete?.Invoke(false);
            }
        }
    }

    //public IEnumerator SavePlayerProgress(PlayerData data)
    //{
    //    // Verificar token antes de cada operación sensible
    //    if (IsTokenExpired())
    //    {
    //        yield return RefreshTokenFunction((success) => {
    //            if (!success)
    //            {
    //                // Mostrar UI de reconexión
    //            }
    //        });
    //    }

    //    // Continuar con la operación...
    //    string jsonData = JsonUtility.ToJson(data);
    //    using (UnityWebRequest request = /* Crear request API */)
    //    {
    //        request.SetRequestHeader("Authorization", $"Bearer {AccessToken}");
    //        yield return request.SendWebRequest();
    //        // Manejar respuesta...
    //    }
    ////}
    ///
    [System.Serializable]
    private class AuthResponse
    {
        public string access_token;
        public string refresh_token;
        public int expires_in;
        public string token_type;
    }

    public class PlayerData
    {
        public string id { get; set; }
        public string player_friendly_name { get; set; }
        public DateTime created_at { get; set; }
        public int coins { get; set; }
        public int points { get; set; }
        public int diamonds { get; set; }
        public List<InventoryItem> Inventory { get; set; }

        public PlayerData()
        {
            Inventory = new List<InventoryItem>();
        }

        public PlayerData(string playerId, string playerFriendlyName, DateTime createdAt, int coins, int points, int diamonds)
        {
            id = playerId;
            player_friendly_name = playerFriendlyName;
            created_at = createdAt;
            this.coins = coins;
            this.points = points;
            this.diamonds = diamonds;
            Inventory = new List<InventoryItem>();
        }

        public void AddToInventory(string itemId, float cost, DateTime? timestamp = null)
        {
            Inventory.Add(new InventoryItem
            {
                ItemId = itemId,
                CreatedAt = timestamp ?? DateTime.Now,
                Cost = cost
            });
        }

        public float GetInventoryValue()
        {
            float total = 0f;
            foreach (var item in Inventory)
            {
                total += item.Cost;
            }
            return total;
        }

        public Dictionary<string, object> ToDictionary()
        {
            return new Dictionary<string, object>
        {
            { "player_id", id },
            { "created_at", created_at.ToString("o") },
            { "coins", coins },
            { "points", points },
            { "diamonds", diamonds },
            { "inventory", Inventory },
            { "inventory_value", GetInventoryValue() }
        };
        }
    }

    public class InventoryItem
    {
        public string ItemId { get; set; }
        public DateTime CreatedAt { get; set; }
        public float Cost { get; set; }
    }
}
