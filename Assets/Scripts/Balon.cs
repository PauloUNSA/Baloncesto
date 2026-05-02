using UnityEngine;
using UnityEngine.SceneManagement;

public class Balon : MonoBehaviour
{
    [Header("Configuración de Lanzamiento")]
    public float fuerzaDeDisparo = 1f; // Multiplicador de la fuerza
    public float fuerzaActual = 8f;
    public float fuerzaMaxima = 80f;
    public float fuerzaMinima = 1f;
    public Vector2 direccionLanzamiento = new Vector2(-1, 1);
    public float velocidadRotacionAngulo = 100f;
    public float velocidadCambioFuerza = 12f;
    
    [Header("Predicción")]
    public GameObject prediccionBalon;
    public int maximaTrayectoriaPredecida = 50;
    
    [Header("Audio")]
    public AudioSource reproductorAudio;
    public AudioClip audioLanzamiento;

    [Header("Posiciones Aleatorias")]
    // cuadrado invisible donde el balón puede aparecer
    public float minX = -4f; // Límite izquierdo
    public float maxX = 6f; // derecho
    public float minY = -3f; // inferior
    public float maxY = 1f;  // superior

    private Vector2 posicionPorDefectoDelBalon;
    private Rigidbody2D rb;
    private Scene escenaPrincipal;
    private PhysicsScene2D fisicaEscenaPrincipal;
    private Scene escenaPredecida;
    private PhysicsScene2D fisicaEscenaPredecida;
    private bool yaLanzado = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (reproductorAudio == null) reproductorAudio = GetComponent<AudioSource>();
    }
    // toda la logica de la prediccion es tomada de la serie de videos: 
    // https://www.youtube.com/playlist?list=PLQq25VykDHnsbYUvE_0OniZKV_yhmVKIg
    void Start()
    {
        // Configuración inicial para simulación manual de física
        Physics2D.simulationMode = SimulationMode2D.Script; 
        rb.bodyType = RigidbodyType2D.Kinematic; 
        posicionPorDefectoDelBalon = transform.position; 
        direccionLanzamiento = direccionLanzamiento.normalized;
        crearEscenaPrincipal();
        crearEscenaPredecida();
    }

    void Update()
    {
        if (yaLanzado) return;

        // Controles de Ángulo (Arriba / Abajo)
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.S))
            direccionLanzamiento = Quaternion.Euler(0, 0, velocidadRotacionAngulo * Time.deltaTime) * direccionLanzamiento;
        
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.W))
            direccionLanzamiento = Quaternion.Euler(0, 0, -velocidadRotacionAngulo * Time.deltaTime) * direccionLanzamiento;

        // Controles de Fuerza (Derecha / Izquierda)
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.A))
            fuerzaActual = Mathf.Min(fuerzaActual + velocidadCambioFuerza * Time.deltaTime, fuerzaMaxima);
            
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.D))
            fuerzaActual = Mathf.Max(fuerzaActual - velocidadCambioFuerza * Time.deltaTime, fuerzaMinima);

        // linea de trayerectoria predecida
        MostrarTrayectoria();

        // Lanzar con la espacio
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Lanzar();
        }
    }

    void MostrarTrayectoria()
    {
        GameObject nuevaPrediccionBalon = GameObject.Instantiate(prediccionBalon);
        SceneManager.MoveGameObjectToScene(nuevaPrediccionBalon, escenaPredecida);
        nuevaPrediccionBalon.transform.position = transform.position;
        
        nuevaPrediccionBalon.GetComponent<Rigidbody2D>().AddForce(direccionLanzamiento * (fuerzaActual * fuerzaDeDisparo), ForceMode2D.Impulse);

        LineRenderer lineaBalon = GetComponent<LineRenderer>();
        lineaBalon.positionCount = maximaTrayectoriaPredecida; 

        for (int i = 0; i < maximaTrayectoriaPredecida; i++)
        {
            fisicaEscenaPredecida.Simulate(Time.fixedDeltaTime); 
            lineaBalon.SetPosition(i, new Vector3(nuevaPrediccionBalon.transform.position.x, nuevaPrediccionBalon.transform.position.y, 0));
        }
        Destroy(nuevaPrediccionBalon);
    }

    void Lanzar()
    {
        yaLanzado = true;
        GetComponent<LineRenderer>().positionCount = 0; // Limpiar línea
        rb.bodyType = RigidbodyType2D.Dynamic; 
        rb.AddForce(direccionLanzamiento * (fuerzaActual * fuerzaDeDisparo), ForceMode2D.Impulse);

        if(audioLanzamiento != null)
            reproductorAudio.PlayOneShot(audioLanzamiento);
    }

    void FixedUpdate()
    {
        if(!fisicaEscenaPrincipal.IsValid()) return;
        fisicaEscenaPrincipal.Simulate(Time.fixedDeltaTime);
    }

    // Se llama desde el ControladorJuego para preparar el siguiente tiro
    public void ReiniciarBalon()
    {
        rb.bodyType = RigidbodyType2D.Kinematic;
        MoverAPosicionAleatoria(); // se mueve aleatoriamente en vez de volver al origen
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        
        // Reseteamos el lanzamiento, reinicializando hacia a donde apunta el balon
        direccionLanzamiento = new Vector2(-1, 1).normalized;
        fuerzaActual = 8f; 
        
        yaLanzado = false;
    }


    private void MoverAPosicionAleatoria()
    {//lógica propia
        float posX = Random.Range(minX, maxX);
        float posY = Random.Range(minY, maxY);
        transform.position = new Vector2(posX, posY);
    }
    private void crearEscenaPrincipal()
    {
        escenaPrincipal = SceneManager.CreateScene("EscenaPrincipal");
        fisicaEscenaPrincipal = escenaPrincipal.GetPhysicsScene2D();    
    }

    private void crearEscenaPredecida()
    {
        CreateSceneParameters parametrosEscena = new CreateSceneParameters(LocalPhysicsMode.Physics2D);
        escenaPredecida = SceneManager.CreateScene("EscenaPredecida", parametrosEscena);
        fisicaEscenaPredecida = escenaPredecida.GetPhysicsScene2D();
    }
}