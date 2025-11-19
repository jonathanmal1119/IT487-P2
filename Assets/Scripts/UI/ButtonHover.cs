using Assets.Scripts;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Color hoverColor = new(1, 1, 1, 1);

    TextMeshProUGUI text;
    Color originalColor;

    public void OnPointerEnter(PointerEventData eventData)
    {
        text.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        text.color = originalColor;
    }
    void Start()
    {
        text = GetComponentsInChildren<TextMeshProUGUI>().First();
        originalColor = text.color;
    }
}
