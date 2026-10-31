using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SlotInventarioVisual :
    MonoBehaviour,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler,
    IDropHandler
{
    [Header("Elementos visuales")]
    [SerializeField] private Image imagenIcono;
    [SerializeField] private TMP_Text textoCantidad;

    private SlotInventario slot;
    private InventoryManager inventoryManager;
    private Canvas canvas;

    private GameObject iconoArrastrado;
    private RectTransform rectIconoArrastrado;

    public SlotInventario Slot => slot;

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();

        if (imagenIcono != null)
        {
            imagenIcono.preserveAspect = true;

            // El icono no debe bloquear los eventos del slot.
            imagenIcono.raycastTarget = false;
        }

        if (textoCantidad != null)
        {
            textoCantidad.raycastTarget = false;
        }
    }

    public void AsignarSlot(SlotInventario nuevoSlot)
    {
        slot = nuevoSlot;
        ActualizarVisual();
    }

    public void AsignarInventoryManager(InventoryManager nuevoInventoryManager)
    {
        inventoryManager = nuevoInventoryManager;
    }

    public void ActualizarVisual()
    {
        if (slot == null || slot.EstaVacio())
        {
            MostrarVacio();
            return;
        }

        if (imagenIcono != null)
        {
            imagenIcono.sprite = slot.Objeto.icono;
            imagenIcono.preserveAspect = true;
            imagenIcono.enabled = true;
        }

        if (textoCantidad != null)
        {
            bool mostrarCantidad = slot.Cantidad > 1;

            textoCantidad.text = mostrarCantidad? slot.Cantidad.ToString(): "";

            textoCantidad.gameObject.SetActive(mostrarCantidad);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (slot == null || slot.EstaVacio())
        {
            return;
        }

        if (canvas == null || imagenIcono == null)
        {
            return;
        }

        CrearIconoArrastrado();

        imagenIcono.color = new Color(1f, 1f,1f,0.4f);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (rectIconoArrastrado == null)
        {
            return;
        }

        rectIconoArrastrado.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (imagenIcono != null)
        {
            imagenIcono.color = Color.white;
        }

        DestruirIconoArrastrado();

    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null)
        {
            return;
        }

        SlotInventarioVisual slotOrigen =
            eventData.pointerDrag.GetComponent<SlotInventarioVisual>();

        if (slotOrigen == null || slotOrigen == this)
        {
            return;
        }

        if (slot == null || !slot.EstaVacio())
        {
            return;
        }

        if (inventoryManager == null)
        {
            Debug.LogWarning("El slot no tiene asignado el InventoryManager.");

            return;
        }

        inventoryManager.MoverObjeto(slotOrigen.Slot, slot);
    }

    private void CrearIconoArrastrado()
    {
        iconoArrastrado = new GameObject("IconoArrastrado",typeof(RectTransform),typeof(CanvasRenderer),typeof(Image),typeof(CanvasGroup));

        iconoArrastrado.transform.SetParent(canvas.transform,false);

        rectIconoArrastrado =iconoArrastrado.GetComponent<RectTransform>();

        rectIconoArrastrado.sizeDelta =imagenIcono.rectTransform.rect.size;

        rectIconoArrastrado.position =imagenIcono.rectTransform.position;

        Image imagenArrastrada =iconoArrastrado.GetComponent<Image>();

        imagenArrastrada.sprite = imagenIcono.sprite;
        imagenArrastrada.preserveAspect = true;
        imagenArrastrada.raycastTarget = false;

        CanvasGroup canvasGroup =iconoArrastrado.GetComponent<CanvasGroup>();

        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;

        iconoArrastrado.transform.SetAsLastSibling();
    }

    private void DestruirIconoArrastrado()
    {
        if (iconoArrastrado != null)
        {
            Destroy(iconoArrastrado);
        }

        iconoArrastrado = null;
        rectIconoArrastrado = null;
    }

    private void MostrarVacio()
    {
        if (imagenIcono != null)
        {
            imagenIcono.sprite = null;
            imagenIcono.enabled = false;
            imagenIcono.color = Color.white;
        }

        if (textoCantidad != null)
        {
            textoCantidad.text = "";
            textoCantidad.gameObject.SetActive(false);
        }
    }
}