using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ShopButtonScript : MonoBehaviour
{
    private ShopManager Manager;
    private SupabaseDao.InventoryItem Item;
    private Button ThisShopButton;
    private bool Initialized = false;
    private bool PlayerHasThisItem = false;

    void Start()
    {
        ThisShopButton.onClick.RemoveAllListeners();
        ThisShopButton.onClick.AddListener(OnClick);
        if (PlayerHasThisItem)
        {
            Transform itemImageTransform = ThisShopButton.transform.Find("ItemImage");
            if (itemImageTransform != null)
            {
                Image itemImage = itemImageTransform.GetComponent<Image>();
                if (itemImage != null)
                {
                    Color semiTransparentColor = itemImage.color;
                    semiTransparentColor.a = 0.5f; // Cambiar a un valor semitransparente
                    itemImage.color = semiTransparentColor;
                }
            }
        }
        else
        {
            Transform itemImageTransform = ThisShopButton.transform.Find("ItemImage");
            if (itemImageTransform != null)
            {
                Image itemImage = itemImageTransform.GetComponent<Image>();
                if (itemImage != null)
                {
                    Color opaqueColor = itemImage.color;
                    opaqueColor.a = 1f;
                    itemImage.color = opaqueColor;
                }
            }
        }
    }

    void Update()
    {
        print("Soy el objeto: " + Item.name);
    }

    public void Initialize(ShopManager manager, SupabaseDao.InventoryItem item)
    {
        Manager = manager;
        Item = item;
        Initialized = true;

        Transform shopButtonTransform = transform.Find("ShopButton");
        Button shopButton = shopButtonTransform.GetComponent<Button>();
        ThisShopButton = shopButton;
        print("Se añadió el listener para el OnClick del objeto" + Item.name);
        if (Manager == null)
            print("El manager es nulo para el objeto " + Item.name);
        else
            print("El manager existe para el objeto " + Item.name);
    }

    public void OnClick()
    {
        if (Manager == null || Item == null)
        {
            Debug.LogError("ShopButtonScript: Manager o Item es null. ¿Se llamó a Initialize()?");
            return;
        }
        while (!Initialized){ }
        StartCoroutine(Manager.OnItemButtonClicked(Item));
    }

    public void SetItem(SupabaseDao.InventoryItem item)
    {
        Item = item;
    }

    public void OnPointerEnterButton()
    {
        if (!PlayerHasThisItem)
            LeanTween.scale(gameObject, new Vector3(1.1f, 1.1f, 1f), 0.2f).setEase(LeanTweenType.easeInOutSine);
    }

    public void OnPointerExitButton()
    {
        if (!PlayerHasThisItem)
            LeanTween.scale(gameObject, new Vector3(1f, 1f, 1f), 0.2f).setEase(LeanTweenType.easeInOutSine);
    }

    public void SetPlayerHasThisItem(bool hasItem)
    {
        PlayerHasThisItem = hasItem;
    }

}
