using UnityEngine;

public class Suelo : MonoBehaviour
{
    public AroBaloncesto aro; // Se debe colocar el objeto que simula el aro

    void OnCollisionEnter2D(Collision2D colision)
    {
        if (colision.gameObject.CompareTag("Balon"))
        {
            Balon scriptBalon = colision.gameObject.GetComponent<Balon>();
            
            // GameManager procesa el final del turno
            ControladorJuego.instancia.TerminarTurno(scriptBalon, aro);
        }
    }
}
