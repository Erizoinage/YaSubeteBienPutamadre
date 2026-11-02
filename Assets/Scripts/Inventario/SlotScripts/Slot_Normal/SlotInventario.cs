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

        /*
         * Si este objeto es realmente un SlotEspecial,
         * C# llamará a la versión sobrescrita de
         * PuedeGuardar() y comprobará sus etiquetas.
         */
        if (!PuedeGuardar(nuevoObjeto))
        {
            return cantidadRecibida;
        }

        if (EstaVacio())
        {
            objeto = nuevoObjeto;
            cantidad = 0;
        }

        // Los objetos no stackeables solo guardan una unidad.
        if (!objeto.esStackeable)
        {
            cantidad = 1;
            return cantidadRecibida - 1;
        }

        int espacioDisponible =
            objeto.cantidadMaxima - cantidad;

        int cantidadAgregada = Mathf.Min(cantidadRecibida,espacioDisponible);

        cantidad += cantidadAgregada;

        // Devuelve la cantidad que no pudo almacenarse.
        return cantidadRecibida - cantidadAgregada;
    }

    public bool RetirarCantidad(int cantidadARetirar)
    {
        if (cantidadARetirar <= 0)
        {
            return false;
        }

        if (EstaVacio())
        {
            return false;
        }

        if (cantidad < cantidadARetirar)
        {
            return false;
        }

        cantidad -= cantidadARetirar;

        // Si ya no queda ninguna unidad, limpia el slot.
        if (cantidad <= 0)
        {
            VaciarSlot();
        }

        return true;
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

        // Por ahora solo se mueve a slots vacíos.
        if (!slotDestino.EstaVacio())
        {
            return false;
        }

        /*
         * Comprueba las reglas del destino.
         * Si el destino es SlotEspecial, también comprobará
         * la etiqueta permitida y el tipo de equipamiento.
         */
        if (!slotDestino.PuedeGuardar(objeto))
        {
            return false;
        }

        int cantidadSobrante = slotDestino.AgregarObjeto(objeto,cantidad);

        /*
         * El movimiento solamente se completa si toda
         * la cantidad pudo almacenarse.
         */
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