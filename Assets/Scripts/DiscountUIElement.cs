using TMPro;
using UnityEngine;

public class DiscountUIElement : MonoBehaviour
{
    [SerializeField] private TMP_Text _shopText;
    [SerializeField] private TMP_Text _shopAvailabilityText;
    [SerializeField] private TMP_Text _discountText;

    public void Initialize(Promocion discount)
    {
        _shopText.text = discount.comercio_nombre;
        _shopAvailabilityText.text = discount.fecha_inicio;
        _discountText.text = discount.descuento;
    }

    public void Initialize(Comercio shop)
    {
        _shopText.text = shop.nombre;
        _shopAvailabilityText.text = shop.direccion;
    }
}
