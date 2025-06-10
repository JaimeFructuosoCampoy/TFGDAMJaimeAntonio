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
    }

    void Update()
    {
        print("Soy el objeto: " + Item.name);
    }

    /// <summary>
    /// Se ejecuta cuando el puntero del ratón entra en el área del botón.
    /// Escala el botón para dar un efecto de resaltado.
    /// </summary>
    public void OnPointerEnterButton()
    {
        LeanTween.scale(gameObject, new Vector3(1.1f, 1.1f, 1f), 0.2f).setEase(LeanTweenType.easeInOutSine);
    }

    /// <summary>
    /// Se ejecuta cuando el puntero del ratón sale del área del botón.
    /// Restaura la escala original del botón.
    /// </summary>
    public void OnPointerExitButton()
    {
        LeanTween.scale(gameObject, new Vector3(1f, 1f, 1f), 0.2f).setEase(LeanTweenType.easeInOutSine);
    }

    /// <summary>
    /// Establece si el jugador tiene este objeto equipado actualmente.
    /// </summary>
    public void SetPlayerHasThisItemEquiped()
    {
        IsEquiped = PlayerLoggedIn.ItemEquiped == Item;
    }

    /// <summary>
    /// Devuelve si este objeto está equipado por el jugador.
    /// </summary>
    /// <returns>True si el objeto está equipado, false en caso contrario.</returns>
    public bool IsThisItemEquiped()
    {
        return IsEquiped;
    }

    /// <summary>
    /// Se ejecuta cuando se hace clic en el botón.
    /// Establece este objeto como el objeto equipado por el jugador y actualiza la interfaz de usuario.
    /// </summary>
    public void OnClickButton()
    {
        PlayerLoggedIn.ItemEquiped = Item;
        Manager.ShowEquipedItem(gameObject);
    }

    /// <summary>
    /// Corrutina para obtener la URL de la imagen del objeto desde la base de datos.
    /// </summary>
    /// <param name="onComplete">Acción a ejecutar cuando se completa la obtención de la URL, pasando la URL como parámetro.</param>
    /// <returns>IEnumerator para la corrutina.</returns>
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
                    Debug.LogWarning("No se encontr� url_image para el item.");
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
