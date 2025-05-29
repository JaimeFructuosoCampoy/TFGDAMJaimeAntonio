using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance { get; private set; }

    public GameObject PopUpShop;
    private bool isPopUpActive = false;
    List<SupabaseDao.InventoryItem> AvaibleShopItems;
    List<GameObject> ShopItemsGameObjects;
    public GameObject ShopItemPrefab;
    public GameObject ShopItemPrefabParent;
    private bool BuyingTransactionFinalized = false;
    private int SelectedItemPrice;
    private int SelectedUserCoins;
    private bool CanBuyPopUp = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        ShopItemsGameObjects = new List<GameObject>();
        StartCoroutine(SupabaseDao.Instance.GetAllItems(GetItems));
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

    private void GetItems(List<SupabaseDao.InventoryItem> items)
    {
        AvaibleShopItems = items;
        if (items != null)
        {
            foreach (var item in AvaibleShopItems)
            {
                GameObject ItemButton = Instantiate(ShopItemPrefab, ShopItemPrefabParent.transform);
                ItemButton.GetComponent<ShopButtonScript>().Initialize(this, item);
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
                    if (PlayerHasItem(item))
                    {
                        childObject.text = "Owned";
                    }
                    else
                    {
                        childObject.text = item.main_price.ToString();
                    }
                }

                childTransform = ItemButton.transform.Find("ShopButton");
                childTransform = childTransform.transform.Find("ItemImage");
                if (childTransform != null)
                {
                    Image childObject = childTransform.GetComponent<Image>();
                    StartCoroutine(LoadSpriteFromURL(item.url_image, childObject, SetImageSprite));
                }
                ShopItemsGameObjects.Add(ItemButton);
            }
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

    public void ShowPopUpInfo(string itemSelectedName)
    {
        foreach (var shopGameObject in ShopItemsGameObjects)
        {
            if (shopGameObject.transform.Find("TextName").GetComponent<TMP_Text>().text == itemSelectedName)
            {
                Transform attribute = PopUpShop.transform.Find("ObjectSelected/ObjectSelectedImage");
                Image image = attribute.GetComponent<Image>();
                image.sprite = shopGameObject.transform.Find("ShopButton/ItemImage").GetComponent<Image>().sprite;

                attribute = PopUpShop.transform.Find("ObjectSelected/TextName");
                TMP_Text textName = attribute.GetComponent<TMP_Text>();
                textName.text = shopGameObject.transform.Find("TextName").GetComponent<TMP_Text>().text;

                attribute = PopUpShop.transform.Find("ObjectSelected/TextPrice");
                TMP_Text textPrice = attribute.GetComponent<TMP_Text>();
                textPrice.text = shopGameObject.transform.Find("TextPrice").GetComponent<TMP_Text>().text;
                break;
            }
        }
        StartCoroutine(CheckIfCanBuy());
        OpenPopUp();
    }
    private void SetImageSprite(Sprite sprite, Image imageToSet)
    {
        if (imageToSet != null && sprite != null)
            imageToSet.sprite = sprite;
    }

    private bool PlayerHasItem(SupabaseDao.InventoryItem item)
    {
        foreach (var inventoryItem in PlayerLoggedIn.Inventory)
        {
            if (inventoryItem.id == item.id)
            {
                return true;
            }
        }
        return false;
    }

    public void OnItemButtonClicked(SupabaseDao.InventoryItem item)
    {
        foreach (var shopGameObject in ShopItemsGameObjects)
        {
            if (shopGameObject.transform.Find("TextName").GetComponent<TMP_Text>().text == item.name)
            {
                ShowPopUpInfo(item.name);
                break;
            }
        }
    }
    
    IEnumerator CheckIfCanBuy()
    {
        Transform objectSelectedTransform = PopUpShop.transform.Find("ObjectSelected/TextName");
        string itemName = objectSelectedTransform.GetComponent<TMP_Text>().text;

        yield return StartCoroutine(SupabaseDao.Instance.GetPlayerCoins(SetPlayerCoins));
        yield return StartCoroutine(SupabaseDao.Instance.GetItemPriceByName(itemName, SetItemPrice));
        SetIfCanBuy(SelectedUserCoins, SelectedItemPrice);
    }

    private void SetItemPrice(int itemPrice)
    {
        SelectedItemPrice = itemPrice;
    }

    private void SetPlayerCoins(int userCoins)
    {
        SelectedUserCoins = userCoins;
    }

    private void SetIfCanBuy(int playerCoins, int itemPrice)
    {
        CanBuyPopUp = playerCoins >= itemPrice;
    }


}
