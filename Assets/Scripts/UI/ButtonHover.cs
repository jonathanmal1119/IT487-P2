using Assets.Scripts;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using Riten.Native.Cursors;

public class ButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Color hoverColor = new(1, 1, 1, 1);
    public float transitionSpeed = 15;

    TextMeshProUGUI text;
    Color originalColor;

    bool hovering = false;

    void Start()
    {
        text = GetComponentsInChildren<TextMeshProUGUI>().First();
        originalColor = text.color;
    }

    public void Update()
    {
        if (hovering)
        {
            text.color = Color.Lerp(text.color, hoverColor, Time.deltaTime * transitionSpeed);
        }
        else
        {
            text.color = Color.Lerp(text.color, originalColor, Time.deltaTime * transitionSpeed);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hovering = true;
        NativeCursor.SetCursor(NTCursors.Link);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        hovering = false;
        NativeCursor.ResetCursor();
    }
}
