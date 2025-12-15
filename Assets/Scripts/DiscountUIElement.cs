using System;
using TMPro;
using UnityEngine;

public class DiscountUIElement : MonoBehaviour
{
    [SerializeField] private TMP_Text _shopText;
    [SerializeField] private TMP_Text _shopAvailabilityText;
    [SerializeField] private TMP_Text _discountText;

    private Comercio _shopData;
    private Promocion _discountData;

    public event Action<Comercio, Promocion> OnClick = delegate {};
    
    public void Initialize(Comercio shop)
    {
        _shopText.text = shop.nombre;
        _shopAvailabilityText.text = shop.direccion;
        _discountText.text = "No hay descuentos por el momento";
    }

    public void Initialize(Comercio shop, Promocion discount)
    {
        _shopText.text = shop.nombre;
        _shopAvailabilityText.text = shop.direccion;
        _discountText.text = $"{discount.descuento}% de descuento";
        _shopData = shop;
        _discountData = discount;
    }

    public void OnClicked()
    {
        if(_shopData == null)
            return;

        _discountData ??= new Promocion();
        
        OnClick.Invoke(_shopData, _discountData);
    }
}
