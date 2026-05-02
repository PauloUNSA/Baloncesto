using UnityEngine;

public class AroBaloncesto : MonoBehaviour
{
    private bool yaPuntuo = false;

    void OnTriggerEnter2D(Collider2D colision)
    {
        // Verifica si el objeto que atravesó el aro es el balón
        if (colision.CompareTag("Balon"))
        {
            Rigidbody2D rb = colision.GetComponent<Rigidbody2D>();
            
            // Si linearVelocity.y es menor a 0, el balón va hacia abajo (entró por arriba)
            if (rb.linearVelocity.y < 0 && !yaPuntuo)
            {
                ControladorJuego.instancia.AnotarCanasta();
                yaPuntuo = true; // Evitar que puntúe varias veces en un solo tiro
            }
        }
    }

    public void ResetearAro()
    {
        yaPuntuo = false;
    }
}