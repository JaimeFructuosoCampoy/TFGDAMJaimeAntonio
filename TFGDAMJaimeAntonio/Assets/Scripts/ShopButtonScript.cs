using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopButtonScript : MonoBehaviour
{
    private ShopManager Manager;
    private SupabaseDao.InventoryItem Item;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Initialize(ShopManager manager, SupabaseDao.InventoryItem item)
    {
        Manager = manager;
        Item = item;

        Transform shopButtonTransform = transform.Find("ShopButton");
        Button shopButton = shopButtonTransform.GetComponent<Button>();
        shopButton.onClick.AddListener(OnClick);
    }

    public void OnClick()
    {
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
