using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditorInternal.Profiling.Memory.Experimental;
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

    void Start()
    {
        PlayerItems = PlayerLoggedIn.Inventory;
        StartCoroutine(ShowInventory());
    }

    // Update is called once per frame
    void Update()
    {

    }

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
}