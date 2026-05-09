using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HoverButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    Image image;
    [SerializeField] Sprite buttonUp;
    [SerializeField] Sprite buttonDown;
    
    void Awake()
    {
        image = transform.GetComponent<Image>();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Cursor Entering " + name);
        image.sprite = buttonDown;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("Cursor Exiting " + name);
        image.sprite = buttonUp; 
    }
}
