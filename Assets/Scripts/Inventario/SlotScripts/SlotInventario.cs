using UnityEngine;

[System.Serializable]
public class SlotInventario
{
    [SerializeField] private DatosObjeto objeto;
    [SerializeField] private int cantidad;

    public DatosObjeto Objeto => objeto;
    public int Cantidad => cantidad;

    public bool EstaVacio()
    {
        return objeto == null;
    }

    public bool TieneElMismoObjeto(DatosObjeto nuevoObjeto)
    {
        return objeto == nuevoObjeto;
    }

    public bool PuedeGuardar(DatosObjeto nuevoObjeto)
    {
        if (nuevoObjeto == null)
        {
            return false;
        }

        if (EstaVacio())
        {
            return true;
        }

        if (!TieneElMismoObjeto(nuevoObjeto))
        {
            return false;
        }

        if (!objeto.esStackeable)
        {
            return false;
        }

        return cantidad < objeto.cantidadMaxima;
    }

    public int AgregarObjeto(
        DatosObjeto nuevoObjeto,
        int cantidadRecibida
    )
    {
        if (nuevoObjeto == null || cantidadRecibida <= 0)
        {
            return cantidadRecibida;
        }

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

        int cantidadAgregada = Mathf.Min( cantidadRecibida,espacioDisponible);

        cantidad += cantidadAgregada;

        return cantidadRecibida - cantidadAgregada;
    }

    public bool MoverA(SlotInventario slotDestino)
    {
        if (slotDestino == null)
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

        slotDestino.objeto = objeto;
        slotDestino.cantidad = cantidad;

        VaciarSlot();

        return true;
    }

    public void VaciarSlot()
    {
        objeto = null;
        cantidad = 0;
    }
}