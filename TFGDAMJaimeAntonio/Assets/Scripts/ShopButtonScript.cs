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

    void Start()
    {
        ThisShopButton.onClick.RemoveAllListeners();
        ThisShopButton.onClick.AddListener(OnClick);
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
        Manager.OnItemButtonClicked(Item);
    }

    public void SetItem(SupabaseDao.InventoryItem item)
    {
        Item = item;
    }
    public void OnPointerEnterButton()
    {
        LeanTween.scale(gameObject, new Vector3(1.1f, 1.1f, 1f), 0.2f).setEase(LeanTweenType.easeInOutSine);
    }

    public void OnPointerExitButton()
    {
        LeanTween.scale(gameObject, new Vector3(1f, 1f, 1f), 0.2f).setEase(LeanTweenType.easeInOutSine);
    }

}
