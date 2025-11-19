using Riten.Native.Cursors;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData) => NativeCursor.SetCursor(NTCursors.IBeam);

    public void OnPointerExit(PointerEventData eventData) => NativeCursor.ResetCursor();
}
