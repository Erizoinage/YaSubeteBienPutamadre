using UnityEngine;

[System.Serializable]
public class SlotInventario
{
    [SerializeField] private DatosObjeto_Base objeto;
    [SerializeField] private int cantidad;

    public DatosObjeto_Base Objeto => objeto;
    public int Cantidad => cantidad;

    public bool EstaVacio()
    {
        return objeto == null;
    }

    public bool TieneElMismoObjeto(DatosObjeto_Base nuevoObjeto)
    {
        return objeto == nuevoObjeto;
    }

    public virtual bool PuedeGuardar(DatosObjeto_Base nuevoObjeto)
    {
        if (nuevoObjeto == null)
        {
            return false;
        }

        // Un slot vacío puede aceptar cualquier objeto.
        if (EstaVacio())
        {
            return true;
        }

        // No permite mezclar objetos diferentes.
        if (!TieneElMismoObjeto(nuevoObjeto))
        {
            return false;
        }

        // Un objeto no stackeable no puede acumularse.
        if (!objeto.esStackeable)
        {
            return false;
        }

        return cantidad < objeto.cantidadMaxima;
    }

    public virtual int AgregarObjeto(DatosObjeto_Base nuevoObjeto,int cantidadRecibida)
    {
        if (nuevoObjeto == null || cantidadRecibida <= 0)
        {
            return cantidadRecibida;
        }

        // Si es un SlotEspecial, llamará automáticamente
        // a la versión especial de PuedeGuardar().
        if (!PuedeGuardar(nuevoObjeto))
        {
            return cantidadRecibida;
        }

        if (EstaVacio())
        {
            objeto = nuevoObjeto;
            cantidad = 0;
        }

        if (!objeto.esStackeable)
        {
            cantidad = 1;
            return cantidadRecibida - 1;
        }

        int espacioDisponible =objeto.cantidadMaxima - cantidad;

        int cantidadAgregada = Mathf.Min(cantidadRecibida,espacioDisponible);

        cantidad += cantidadAgregada;

        return cantidadRecibida - cantidadAgregada;
    }

    public bool MoverA(SlotInventario slotDestino)
    {
        if (slotDestino == null || slotDestino == this)
        {
            return false;
        }

        if (EstaVacio())
        {
            return false;
        }

        if (!slotDestino.EstaVacio())
        {
            return false;
        }

        // Comprueba las reglas del destino.
        // Si es especial, comprobará su etiqueta.
        if (!slotDestino.PuedeGuardar(objeto))
        {
            return false;
        }

        int cantidadSobrante = slotDestino.AgregarObjeto(objeto,cantidad);

        // El movimiento solo se completa si cupo todo.
        if (cantidadSobrante > 0)
        {
            slotDestino.VaciarSlot();
            return false;
        }

        VaciarSlot();
        return true;
    }

    public void VaciarSlot()
    {
        objeto = null;
        cantidad = 0;
    }
}