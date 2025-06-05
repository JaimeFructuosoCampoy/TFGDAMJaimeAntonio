using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InventoryButtonScript : MonoBehaviour
{
    private InventoryManager Manager;
    public SupabaseDao.InventoryItem Item;
    private bool IsEquiped = false;

    void Start()
    {
        Manager = FindObjectOfType<InventoryManager>();
        SetPlayerHasThisItemEquiped();
        if (Manager != null)
        {
            
        }
    }

    void Update()
    {
        print("Soy el objeto: " + Item.name);
    }

    public void OnPointerEnterButton()
    {
        LeanTween.scale(gameObject, new Vector3(1.1f, 1.1f, 1f), 0.2f).setEase(LeanTweenType.easeInOutSine);
    }

    public void OnPointerExitButton()
    {
        LeanTween.scale(gameObject, new Vector3(1f, 1f, 1f), 0.2f).setEase(LeanTweenType.easeInOutSine);
    }

    public void SetPlayerHasThisItemEquiped()
    {
        IsEquiped = PlayerLoggedIn.ItemEquiped == Item;
    }

    public bool IsThisItemEquiped()
    {
        return IsEquiped;
    }

    public void OnClickButton()
    {
        PlayerLoggedIn.ItemEquiped = Item;
        Manager.ShowEquipedItem(gameObject);
    }

    public IEnumerator GetItemImageUrl(System.Action<string> onComplete)
    {
        if (Item == null || string.IsNullOrEmpty(Item.id))
        {
            Debug.LogError("El Item o su id es nulo.");
            onComplete?.Invoke(null);
            yield break;
        }

        string url = $"{GlobalData.SUPABASE_DB_URL}Items?select=url_image&id=eq.{Item.id}";

        using (UnityEngine.Networking.UnityWebRequest request = UnityEngine.Networking.UnityWebRequest.Get(url))
        {
            request.SetRequestHeader("apikey", GlobalData.SUPABASE_DB_KEY);
            request.SetRequestHeader("Authorization", $"Bearer {SupabaseDao.Instance.AccessToken}");

            yield return request.SendWebRequest();

            if (request.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
            {
                var list = JsonConvert.DeserializeObject<List<UrlImageResponse>>(request.downloadHandler.text);
                if (list != null && list.Count > 0)
                {
                    string urlImage = list[0].url_image;
                    onComplete?.Invoke(urlImage);
                }
                else
                {
                    Debug.LogWarning("No se encontró url_image para el item.");
                    onComplete?.Invoke(null);
                }
            }
            else
            {
                Debug.LogError("Error al obtener url_image: " + request.error + " - " + request.downloadHandler.text);
                onComplete?.Invoke(null);
            }
        }
    }

}

[System.Serializable]
public class UrlImageResponse
{
    public string url_image;
}
