using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{

    public GameObject PopUpShop;
    private bool isPopUpActive = false;
    List<SupabaseDAO.InventoryItem> AvaibleShopItems;
    List<GameObject> ShopItemsGameObjects;
    public GameObject ShopItemPrefab;
    public GameObject ShopItemPrefabParent;
    private Image imageToSet;


    // Start is called before the first frame update
    void Start()
    {
        ShopItemsGameObjects = new List<GameObject>();
        StartCoroutine(SupabaseDAO.Instance.GetAllItems(GetItems));
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Metodo que hace de interruptor para la confirmación de compra
    /// </summary>
    public void TogglePopUPLenguage()
    {
        if (isPopUpActive)
        {
            ClosePopUp();
        }
        else
        {
            OpenPopUp();
        }
    }

    private void OpenPopUp()
    {
        PopUpShop.SetActive(true);

        PopUpShop.transform.localScale = new Vector3(0, 0, 0);
        LeanTween.scale(PopUpShop, new Vector3(2, 2, 1), 0.5f).setEaseOutBack();

        isPopUpActive = true;
    }

    /// <summary>
    /// Metodo para cerrar el popup de seleccion de idioma
    /// </summary>

    private void ClosePopUp()
    {
        LeanTween.scale(PopUpShop, new Vector3(0, 0, 0), 0.5f).setEaseInBack().setOnComplete(() =>
        {
            PopUpShop.SetActive(false);
        });

        isPopUpActive = false;
    }

    public void CancelBuy()
    {
        //Cerrar el popup de compra
        ClosePopUp();
    }

    public void ConfirmBuy()
    {
        //Cerrar el popup de compra
        ClosePopUp();

        //Logica para realizar compra
    }

    private void GetItems(List<SupabaseDAO.InventoryItem> items)
    {
        AvaibleShopItems = items;
        if (items != null)
        {
            foreach (var item in AvaibleShopItems)
            {
                GameObject ItemButton = Instantiate(ShopItemPrefab, ShopItemPrefabParent.transform);
                Transform childTransform = ItemButton.transform.Find("TextName");
                if (childTransform != null)
                {
                    TMP_Text childObject = childTransform.GetComponent<TMP_Text>();
                    childObject.text = item.name;
                }

                childTransform = ItemButton.transform.Find("TextPrice");
                if (childTransform != null)
                {
                    TMP_Text childObject = childTransform.GetComponent<TMP_Text>();
                    childObject.text = item.main_price.ToString();
                }

                childTransform = ItemButton.transform.Find("SettingsButton");
                childTransform = childTransform.transform.Find("ItemImage");
                if (childTransform != null)
                {
                    Image childObject = childTransform.GetComponent<Image>();
                    StartCoroutine(LoadSpriteFromURL(item.url_image, SetImageSprite));
                }
                ShopItemsGameObjects.Add(ItemButton);
            }
        }
    }

    private IEnumerator LoadSpriteFromURL(string url, System.Action<Sprite> onComplete)
    {
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(request);
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                onComplete?.Invoke(sprite);
            }
            else
            {
                Debug.LogError($"Error al cargar la imagen desde la URL: {request.error}");
                onComplete?.Invoke(null);
            }
        }
    }

    private void SetImageSprite(Sprite sprite)
    {
        if (imageToSet != null && sprite != null)
            imageToSet.sprite = sprite;
    }
}
