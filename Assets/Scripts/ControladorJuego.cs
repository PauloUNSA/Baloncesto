using UnityEngine;
using TMPro;

public class ControladorJuego : MonoBehaviour
{
    public static ControladorJuego instancia;

    [Header("Puntuación")]
    public int puntuacion = 0;
    //public Text textoPuntuacion; 
    public TextMeshProUGUI textoPuntuacion;
    [Header("Audios de Resultado")]
    public AudioSource reproductorAudio;
    public AudioClip audioExito;
    public AudioClip audioFracaso;

    [HideInInspector] public bool encestoEsteTurno = false;

    void Awake()
    {
        // Implementación básica de Singleton para fácil acceso
        if (instancia == null) instancia = this;
        else Destroy(gameObject);
        
        if (reproductorAudio == null) reproductorAudio = GetComponent<AudioSource>();
    }

    public void AnotarCanasta()
    {
        puntuacion += 1; // Punto por canasta
        encestoEsteTurno = true;
        ActualizarUI();
        
        if (audioExito != null) reproductorAudio.PlayOneShot(audioExito);
    }

    public void TerminarTurno(Balon scriptBalon, AroBaloncesto scriptAro)
    {
        // Si no encestó, reproducir sonido de fracaso
        if (!encestoEsteTurno && audioFracaso != null)
        {
            reproductorAudio.PlayOneShot(audioFracaso);
        }

        // Preparar para el siguiente tiro
        encestoEsteTurno = false;
        scriptBalon.ReiniciarBalon();
        scriptAro.ResetearAro();
    }

    void ActualizarUI()
    {
        if (textoPuntuacion != null)
            textoPuntuacion.text = "Canastas: " + puntuacion;
    }
}