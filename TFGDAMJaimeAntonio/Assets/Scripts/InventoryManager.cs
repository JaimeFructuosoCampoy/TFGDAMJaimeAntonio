using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    static List<SupabaseDao.InventoryItem> PlayerItems;
    public GameObject InventoryItemPrefab;
    public GameObject InventoryItemPrefabParent;
    private string UrlImage;
    public Image CheckImage;
    public Button UnEquipObject;

    void Start()
    {
        PlayerItems = PlayerLoggedIn.Inventory;
        StartCoroutine(ShowInventory());
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerLoggedIn.ItemEquiped != null)
        {
            UnEquipObject.interactable = true;
        }
        else
        {
            UnEquipObject.interactable = false;
        }
    }

    /// <summary>
    /// Corrutina que muestra los objetos del inventario del jugador.
    /// Instancia los prefabs de los objetos, establece sus datos e imágenes.
    /// Muestra una marca de verificación en el objeto actualmente equipado.
    /// </summary>
    IEnumerator ShowInventory()
    {
        if (PlayerItems == null || PlayerItems.Count == 0)
        {
            Debug.Log("No hay items en el inventario del jugador");
            yield break;
        }
        foreach (var item in PlayerItems)
        {
            GameObject newItem = Instantiate(InventoryItemPrefab, InventoryItemPrefabParent.transform);
            newItem.GetComponentInChildren<TMP_Text>().text = item.name;
            newItem.GetComponent<InventoryButtonScript>().Item = item;

            Image image = newItem.transform.Find("ShopButton/ItemImage").GetComponent<Image>();
            InventoryButtonScript buttonScript = newItem.GetComponent<InventoryButtonScript>();
            yield return StartCoroutine(buttonScript.GetItemImageUrl(GetImageUrl));
            StartCoroutine(LoadSpriteFromURL(UrlImage, image, SetImageSprite));
            if (buttonScript.IsThisItemEquiped())
            {
                GameObject shopButton = newItem.transform.Find("ShopButton").gameObject;
                
                CheckImage.transform.SetParent(shopButton.transform, false);
                CheckImage.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(115f, -58f, -2.9f);
                CheckImage.GetComponent<RectTransform>().sizeDelta = new Vector2(55.05f, 51.72f);
                CheckImage.gameObject.SetActive(true);
            }
            
        }

        if (PlayerLoggedIn.ItemEquiped == null)
        {
            CheckImage.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Corrutina que carga una textura desde una URL y la convierte en un Sprite.
    /// </summary>
    /// <param name="url">La URL de la imagen a cargar.</param>
    /// <param name="image">El componente Image donde se mostrará el Sprite.</param>
    /// <param name="onComplete">Acción a ejecutar cuando se completa la carga, pasando el Sprite y el Image como parámetros.</param>
    /// <returns>IEnumerator para la corrutina.</returns>
    private IEnumerator LoadSpriteFromURL(string url, Image image, System.Action<Sprite, Image> onComplete)
    {
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(request);
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                onComplete?.Invoke(sprite, image);
            }
            else
            {
                Debug.LogError($"Error al cargar la imagen desde la URL: {request.error}");
            }
        }
    }

    private void SetImageSprite(Sprite sprite, Image imageToSet)
    {
        if (imageToSet != null && sprite != null)
            imageToSet.sprite = sprite;
    }

    public void ShowEquipedItem(GameObject item)
    {
        GameObject shopButton = item.transform.Find("ShopButton").gameObject;
        CheckImage.transform.SetParent(shopButton.transform, false);
        CheckImage.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(115f, -58f, -2.9f);
        CheckImage.GetComponent<RectTransform>().sizeDelta = new Vector2(55.05f, 51.72f);
        CheckImage.gameObject.SetActive(true);
    }

    private void GetImageUrl(string url_image)
    {
        UrlImage = url_image;
    }

    public void UnequipObject()
    {
        PlayerLoggedIn.ItemEquiped = null;
        CheckImage.gameObject.SetActive(false);
    }
}