using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;

public class SupabaseDao : MonoBehaviour
{
    private static SupabaseDao instance;
    public static SupabaseDao Instance => instance;

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

    public void SignUp(string email, string password, string name)
    {
        Debug.Log("1. " + name);
        StartCoroutine(SignUpCoroutine(email, password, name));
    }

    IEnumerator SignUpCoroutine(string email, string password, string userName)
    {
        string url = "https://bxjubueuyzobmpvdwefk.supabase.co/auth/v1/signup";

        string jsonData = JsonConvert.SerializeObject
        (new
        {
            email = email,
            password = password,
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
                var response = JsonConvert.DeserializeObject<SignUpResponse>(request.downloadHandler.text);

                Debug.Log(request.downloadHandler.text);
                Debug.Log("2. " + userName);
                StartCoroutine(SignUpLoginCoroutine(email, password, userName));
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
                        GetInventory();
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

    IEnumerator SignUpPlayerCoroutine(SignUpResponse response, string name)
    {
        using (UnityWebRequest webRequest = new UnityWebRequest(GlobalData.SUPABASE_DB_URL + "Player", "POST"))
        {
            var playerInsert = new
            {
                id = response.id,
                friendly_name = name,
                created_at = DateTime.Parse(response.created_at)
                     .ToUniversalTime()
                     .ToString("yyyy-MM-ddTHH:mm:ss.ffffffZ"),
                coins = 0,
                points = 0,
                diamonds = 0
            };
            string jsonData = JsonConvert.SerializeObject(playerInsert);
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("apikey", GlobalData.SUPABASE_DB_KEY);
            webRequest.SetRequestHeader("Authorization", $"Bearer {AccessToken}");
            webRequest.SetRequestHeader("Content-Type", "application/json");
            webRequest.SetRequestHeader("Prefer", "return=minimal");

            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                PlayerLoggedIn.InitializeOrUpdatePlayerData(
                    new PlayerData(
                        response.id, name, playerInsert.created_at, 0, 0, 0
                        )
                    );
                GlobalData.PlayerLoggedIn = true;
                Debug.Log(name);
                SceneManager.LoadScene("MenuScene");
            }
            else
            {
                Debug.LogError("Error: " + webRequest.error);
            }
        }
    }

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
    IEnumerator SignUpLoginCoroutine(string email, string password, string userName)
    {
        string url = "https://bxjubueuyzobmpvdwefk.supabase.co/auth/v1/token?grant_type=password";

        string jsonData = JsonConvert.SerializeObject(new
        {
            email = email,
            password = password
        });

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

                // Extrae el id y created_at del usuario
                var user = response.user;
                response.user.friendly_name = userName;
                if (user == null)
                {
                    Debug.LogError("No se pudo obtener el usuario tras el login.");
                    yield break;
                }
                SignUpResponse signUpResponse = new SignUpResponse(user.id, user.created_at);
                // Ahora sí, inserta en la tabla Player
                Debug.Log("3. " + userName);
                StartCoroutine(SignUpPlayerCoroutine(signUpResponse, userName));
            }
            else
            {
                Debug.Log("Error al hacer login tras signup: " + request.error + request.downloadHandler.text);
            }
        }
    }
    public void GetInventory()
    {
        StartCoroutine(GetInventoryIdCoroutine());
    }

    IEnumerator GetInventoryIdCoroutine()
    {
        string url = GlobalData.SUPABASE_DB_URL + "Inventory?select=item_id";

        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            webRequest.SetRequestHeader("apikey", GlobalData.SUPABASE_DB_KEY);
            webRequest.SetRequestHeader("Authorization", $"Bearer {AccessToken}");
            webRequest.SetRequestHeader("Content-Type", "application/json");

            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Inventario recibido: " + webRequest.downloadHandler.text);
                var inventoryList = JsonConvert.DeserializeObject<List<PlayerInventory>>(webRequest.downloadHandler.text);
                if (inventoryList != null && inventoryList.Count > 0)
                {
                    StartCoroutine(GetInventoryItems(inventoryList));
                }
            }
            else
            {
                Debug.LogError("Error al obtener el inventario: " + webRequest.error + " - " + webRequest.downloadHandler.text);
            }
        }
    }

    IEnumerator GetInventoryItems(List<PlayerInventory> inventoryList)
    {
        string url = "https://bxjubueuyzobmpvdwefk.supabase.co/rest/v1/Items?select=id,name,description,main_price&id=in.(";
        foreach (var inventoryItem in inventoryList)
        {
            url += inventoryItem.item_id + ",";
        }
        url = url.TrimEnd(',') + ")";
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            webRequest.SetRequestHeader("apikey", GlobalData.SUPABASE_DB_KEY);
            webRequest.SetRequestHeader("Authorization", $"Bearer {AccessToken}");
            webRequest.SetRequestHeader("Content-Type", "application/json");
            yield return webRequest.SendWebRequest();
            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Items recibidos: " + webRequest.downloadHandler.text);
                var itemsList = JsonConvert.DeserializeObject<List<InventoryItem>>(webRequest.downloadHandler.text);
                if (itemsList != null && itemsList.Count > 0)
                {
                    PlayerLoggedIn.InitializeOrUpdateInventory(itemsList);
                }
            }
            else
            {
                Debug.LogError("Error al obtener los items: " + webRequest.error + " - " + webRequest.downloadHandler.text);
            }
        }

    }
    public IEnumerator GetAllItems(Action<List<InventoryItem>> onResult)
    {
        yield return StartCoroutine(GetAllItemsCoroutine(onResult));
    }

    private IEnumerator GetAllItemsCoroutine(Action<List<InventoryItem>> onResult)
    {
        string url = GlobalData.SUPABASE_DB_URL + "Items?select=*";
        List<InventoryItem> itemsList = null;

        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            webRequest.SetRequestHeader("apikey", GlobalData.SUPABASE_DB_KEY);
            webRequest.SetRequestHeader("Authorization", $"Bearer {AccessToken}");
            webRequest.SetRequestHeader("Content-Type", "application/json");

            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Items recibidos: " + webRequest.downloadHandler.text);
                itemsList = JsonConvert.DeserializeObject<List<InventoryItem>>(webRequest.downloadHandler.text);
            }
            else
            {
                Debug.LogError("Error al obtener los items: " + webRequest.error + " - " + webRequest.downloadHandler.text);
            }
        }

        onResult?.Invoke(itemsList);
    }

    //public IEnumerator ModifyUserByCoins(int coinsValue)
    //{
    //    yield return StartCoroutine(GetPlayerCoins(GetCoins));
    //    string url = $"{GlobalData.SUPABASE_DB_URL}Player?coins=eq.{coinsValue}";
    //    var body = new
    //    {
    //        coins = coinsValue
    //    };
    //    string jsonData = JsonConvert.SerializeObject(body);

    //    using (UnityWebRequest request = new UnityWebRequest(url, "PATCH"))
    //    {
    //        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
    //        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
    //        request.downloadHandler = new DownloadHandlerBuffer();
    //        request.SetRequestHeader("apikey", GlobalData.SUPABASE_DB_KEY);
    //        request.SetRequestHeader("Authorization", $"Bearer {AccessToken}");
    //        request.SetRequestHeader("Content-Type", "application/json");
    //        request.SetRequestHeader("Prefer", "return=minimal");

    //        yield return request.SendWebRequest();

    //        if (request.result == UnityWebRequest.Result.Success)
    //        {
    //            Debug.Log("PATCH realizado correctamente.");
    //        }
    //        else
    //        {
    //            Debug.LogError("Error en PATCH: " + request.error + " - " + request.downloadHandler.text);
    //        }
    //    }
    //}

    public IEnumerator GetPlayerCoins(Action<int> onComplete)
    {
        string url = $"{GlobalData.SUPABASE_DB_URL}Player?select=coins";

        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            webRequest.SetRequestHeader("apikey", GlobalData.SUPABASE_DB_KEY);
            webRequest.SetRequestHeader("Authorization", $"Bearer {AccessToken}");

            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    var response = JsonConvert.DeserializeObject<List<Dictionary<string, int>>>(webRequest.downloadHandler.text);
                    if (response != null && response.Count > 0 && response[0].ContainsKey("coins"))
                    {
                        onComplete?.Invoke(response[0]["coins"]);
                    }
                    else
                    {
                        Debug.LogError("No se encontraron datos de monedas en la respuesta.");
                        onComplete?.Invoke(-1);
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Error al procesar la respuesta de monedas: {ex.Message}");
                    onComplete?.Invoke(-1);
                }
            }
            else
            {
                Debug.LogError($"Error al obtener las monedas: {webRequest.error} - {webRequest.downloadHandler.text}");
                onComplete?.Invoke(-1);
            }
        }
    }
    public IEnumerator GetItemPriceByName(string itemName, Action<int> onComplete)
    {
        string url = $"{GlobalData.SUPABASE_DB_URL}Items?select=main_price&name=eq.{itemName}";

        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            webRequest.SetRequestHeader("apikey", GlobalData.SUPABASE_DB_KEY);
            webRequest.SetRequestHeader("Authorization", $"Bearer {AccessToken}");

            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    var response = JsonConvert.DeserializeObject<List<Dictionary<string, int>>>(webRequest.downloadHandler.text);
                    if (response != null && response.Count > 0 && response[0].ContainsKey("main_price"))
                    {
                        onComplete?.Invoke(response[0]["main_price"]);
                    }
                    else
                    {
                        Debug.LogError("No se encontró el precio del item en la respuesta.");
                        onComplete?.Invoke(-1);
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Error al procesar la respuesta de precio: {ex.Message}");
                    onComplete?.Invoke(-1);
                }
            }
            else
            {
                Debug.LogError($"Error al obtener el precio del item: {webRequest.error} - {webRequest.downloadHandler.text}");
                onComplete?.Invoke(-1);
            }
        }
    }

    [System.Serializable]
    private class AuthResponse
    {
        public string access_token;
        public string refresh_token;
        public int expires_in;
        public string token_type;
        public PlayerData user;
    }

    public class PlayerData
    {
        public string id { get; set; }
        public string friendly_name { get; set; }
        public string created_at { get; set; }
        public int coins { get; set; }
        public int points { get; set; }
        public int diamonds { get; set; }
        public List<InventoryItem> Inventory { get; set; }

        public PlayerData()
        {
            Inventory = new List<InventoryItem>();
        }

        public PlayerData(string playerId, string playerFriendlyName, string createdAt, int coins, int points, int diamonds)
        {
            id = playerId;
            friendly_name = playerFriendlyName;
            created_at = createdAt;
            this.coins = coins;
            this.points = points;
            this.diamonds = diamonds;
        }

    }

    public class InventoryItem
    {
        public string id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string created_at { get; set; }
        public int main_price { get; set; }
        public int number { get; set; }
        public bool active { get; set; }
        public string url_image { get; set; }
    }

    public class SignUpResponse
    {
        public string id;
        public string created_at;

        public SignUpResponse(string id, string created_at)
        {
            this.id = id;
            this.created_at = created_at;
        }
    }

    public class PlayerInventory
    {
        public string player_id { get; set; }
        public string item_id { get; set; }
        public string created_at { get; set; }
    }
}