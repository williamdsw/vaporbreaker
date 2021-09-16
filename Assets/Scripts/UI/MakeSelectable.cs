using UnityEngine;
using UnityEngine.EventSystems;


namespace UI
{
    public class MakeSelectable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public void OnPointerEnter(PointerEventData eventData) => EventSystem.current.SetSelectedGameObject(this.gameObject);

        public void OnPointerExit(PointerEventData eventData) => EventSystem.current.SetSelectedGameObject(null);
    }
}