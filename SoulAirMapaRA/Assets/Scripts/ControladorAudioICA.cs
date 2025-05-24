using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Gestiona la reproducción y el volumen de dos AudioSources
/// Implementa transiciones suaves (fading) entre los cambios de volumen.

public class ControladorAudioICA : MonoBehaviour
{
    [Header("Audio Sources")]
    [Tooltip("AudioSource para el AudioClip 01. Este AudioSource reproducirá el audioClip1.")]
    public AudioSource audioSource1;

    [Tooltip("AudioSource para el AudioClip 02. Este AudioSource reproducirá el audioClip2.")]
    public AudioSource audioSource2;

    [Header("Audio Clips")]
    [Tooltip("AudioClip que se reproducirá en el AudioSource 01.")]
    public AudioClip audioClip1;

    [Tooltip("AudioClip que se reproducirá en el AudioSource 02.")]
    public AudioClip audioClip2;

    [Header("Configuración de Fading")]
    [Tooltip("Duración de la transición de volumen (fading) en segundos.")]
    public float duracionFading = 1.5f; // Puedes ajustar esta duración según necesites

    // Almacena las referencias a las corutinas de fading activas para poder detenerlas si es necesario.
    private Coroutine _fadingCoroutine1;
    private Coroutine _fadingCoroutine2;

    /*
    Variable condicional cuantitativa para pruebas desde el Inspector.
    En tu juego, deberás llamar a `ActualizarSonidoSegunICA(tuValorRealDelICA)`
    cuando tu valor de ICA cambie.
    
    [Header("Valor Condicional de Prueba (ICA)")]
    [Range(0, 250)] // Rango de ejemplo, ajústalo según tus necesidades de prueba
    [Tooltip("SOLO PARA PRUEBAS: Modifica este valor en el Inspector para probar la lógica. En el juego real, llama a ActualizarSonidoSegunICA() con tu variable.")]
    public int valorICAEjemplo = 0;*/

    public AirQualityCalculator _icaSystem;

    void Start()
    {
        if (audioSource1 == null || audioSource2 == null || audioClip1 == null || audioClip2 == null)
        {
            Debug.LogError("Error: Uno o más AudioSource o AudioClip no están asignados en el Inspector. Por favor, asígnalos.");
            enabled = false; // Deshabilita el script si falta configuración esencial.
            return;
        }

        // Configura los AudioSources con sus respectivos clips.
        ConfigurarAudioSource(audioSource1, audioClip1, "AudioSource1");
        ConfigurarAudioSource(audioSource2, audioClip2, "AudioSource2");

        // Establece el estado de sonido inicial .
        // Esto asegurará que los sonidos comiencen según el estado inicial del ICA.
        ActualizarSonidoSegunICA(_icaSystem.currentAverageICA);

    }

    /// <summary>
    /// Configura un AudioSource con un AudioClip específico, lo pone en loop y lo inicia.
    /// </summary>
    /// <param name="source">El AudioSource a configurar.</param>
    /// <param name="clip">El AudioClip a asignar.</param>
    /// <param name="sourceName">Nombre descriptivo del source para logs.</param>
    private void ConfigurarAudioSource(AudioSource source, AudioClip clip, string sourceName)
    {
        if (source == null)
        {
            Debug.LogError($"AudioSource '{sourceName}' no está asignado.");
            return;
        }
        if (clip == null)
        {
            Debug.LogError($"AudioClip para '{sourceName}' no está asignado.");
            return;
        }

        source.clip = clip;      // Asigna el clip de audio.
        source.loop = true;      // Configura el audio para que se repita continuamente.
        source.volume = 0f;      // Inicia con volumen 0, el fading lo ajustará.
        if (!source.isPlaying)   // Si no se está reproduciendo ya...
        {
            source.Play();       // ...inicia la reproducción.
        }
    }


    // --- EJEMPLO DE USO EN UPDATE (SOLO PARA PRUEBAS EN EDITOR) ---
    // // Descomenta este bloque si quieres probar cambiando `valorICAEjemplo` en el Inspector en tiempo real.
    // // Recuerda que en un juego real, deberías llamar a `ActualizarSonidoSegunICA`
    // // solo cuando el valor del ICA realmente cambie, no en cada frame.
    private float _valorICAPrevioParaPrueba = -1; // Para detectar cambios en el inspector
    void Update()
    {
         if (_icaSystem.currentAverageICA != _valorICAPrevioParaPrueba)
         {
             ActualizarSonidoSegunICA(_icaSystem.currentAverageICA);
             _valorICAPrevioParaPrueba = _icaSystem.currentAverageICA;
         }
     }
    // --- FIN DE EJEMPLO DE USO EN UPDATE ---

    /// <summary>
    /// Función principal para ajustar el volumen de los AudioSources según el valor del ICA.
    /// Esta es la función que debes llamar desde tu sistema de juego cuando el valor del ICA cambie.
    /// </summary>
    /// <param name="valorICA">
    /// **LA VARIABLE CONDICIONAL CUANTITATIVA**.
    /// Este es el valor actual del ICA (Índice de Calidad del Aire) que determinará los volúmenes.
    /// </param>
    public void ActualizarSonidoSegunICA(float valorICA)
    {
        float volumenObjetivoClip1 = 0f;
        float volumenObjetivoClip2 = 0f;

        // Determinar los volúmenes objetivo para cada AudioClip basados en el valorICA
        if (valorICA >= 0 && valorICA <= 50)
        {
            // Rango 0-50: AudioClip 01 - 100% Volumen, AudioClip 02 - 0% Volumen
            volumenObjetivoClip1 = 1.00f;
            volumenObjetivoClip2 = 0.0f;
             Debug.Log($"ICA ({valorICA}): Rango 0-50. Clip1: 100%, Clip2: 0%");
        }
        else if (valorICA >= 51 && valorICA <= 100)
        {
            // Rango 51-100: AudioClip 01 - 50% Volumen, AudioClip 02 - 25% Volumen
            volumenObjetivoClip1 = 0.5f;
            volumenObjetivoClip2 = 0.25f;
             Debug.Log($"ICA ({valorICA}): Rango 51-100. Clip1: 50%, Clip2: 25%");
        }
        else if (valorICA >= 101 && valorICA <= 150)
        {
            // Rango 101-150: AudioClip 01 - 25% Volumen, AudioClip 02 - 50% Volumen
            volumenObjetivoClip1 = 0.25f;
            volumenObjetivoClip2 = 0.5f;
            Debug.Log($"ICA ({valorICA}): Rango 101-150. Clip1: 25%, Clip2: 50%");
        }
        else if (valorICA >= 151 && valorICA <= 200)
        {
            // Rango 151-200: AudioClip 01 - 0% Volumen, AudioClip 02 - 100% Volumen
            volumenObjetivoClip1 = 0.0f;
            volumenObjetivoClip2 = 1.0f;
             Debug.Log($"ICA ({valorICA}): Rango 151-200. Clip1: 0%, Clip2: 100%");
        }
        else if (valorICA > 200)
        {
            // Para valores ICA mayores a 200, se asume el comportamiento del rango máximo (151-200).
            // Puedes ajustar esto si necesitas un comportamiento diferente (ej. silenciar todo).
            volumenObjetivoClip1 = 0.0f;
            volumenObjetivoClip2 = 1.0f;
            Debug.LogWarning($"Valor ICA ({valorICA}) es mayor que 200. Se aplica configuración del rango 151-200.");
        }
        else // valorICA < 0
        {
            // Para valores ICA menores a 0 (si es posible), se asume el comportamiento del rango mínimo (0-50).
            volumenObjetivoClip1 = 1.0f;
            volumenObjetivoClip2 = 0.0f;
            Debug.LogWarning($"Valor ICA ({valorICA}) es menor que 0. Se aplica configuración del rango 0-50.");
        }

        // Iniciar o actualizar las corutinas de fading para cada AudioSource.
        // Se detiene la corutina anterior si ya existe una para ese AudioSource,
        // para evitar múltiples fadings simultáneos y asegurar una transición al nuevo objetivo.

        if (audioSource1 != null)
        {
            if (_fadingCoroutine1 != null)
            {
                StopCoroutine(_fadingCoroutine1);
            }
            // Solo iniciar una nueva corutina si el volumen actual es diferente al objetivo.
            if (audioSource1.volume != volumenObjetivoClip1)
            {
                _fadingCoroutine1 = StartCoroutine(FadeAudioSource(audioSource1, volumenObjetivoClip1, duracionFading, "AudioSource1"));
            }
        }

        if (audioSource2 != null)
        {
            if (_fadingCoroutine2 != null)
            {
                StopCoroutine(_fadingCoroutine2);
            }
            if (audioSource2.volume != volumenObjetivoClip2)
            {
                _fadingCoroutine2 = StartCoroutine(FadeAudioSource(audioSource2, volumenObjetivoClip2, duracionFading, "AudioSource2"));
            }
        }
    }

    /// <summary>
    /// Corutina para cambiar suavemente (fading) el volumen de un AudioSource
    /// desde su volumen actual hasta un volumen objetivo a lo largo de una duración específica.
    /// </summary>
    /// <param name="source">El AudioSource cuyo volumen se ajustará.</param>
    /// <param name="volumenObjetivo">El volumen final deseado (entre 0.0 y 1.0).</param>
    /// <param name="duracion">El tiempo en segundos que tomará la transición de volumen.</param>
    /// <param name="sourceName">Nombre descriptivo para logs.</param>
    /// <returns>IEnumerator para la corutina.</returns>
    private IEnumerator FadeAudioSource(AudioSource source, float volumenObjetivo, float duracion, string sourceName)
    {
        // Si la duración es 0 o negativa, simplemente establece el volumen y termina.
        if (duracion <= 0)
        {
            source.volume = volumenObjetivo;
            // Debug.Log($"Fading instantáneo para {sourceName} a volumen {volumenObjetivo}");
            yield break; // Termina la corutina aquí.
        }

        float tiempoPasado = 0f;
        float volumenInicial = source.volume; // Volumen actual al iniciar el fading.

        // Debug.Log($"Iniciando fading para {sourceName}: de {volumenInicial} a {volumenObjetivo} en {duracion}s");

        // Asegurarse de que el AudioSource esté reproduciendo para escuchar el fade,
        // especialmente si estamos subiendo el volumen desde cero.
        if (!source.isPlaying && volumenObjetivo > 0.001f && source.clip != null)
        {
            // Debug.Log($"AudioSource {sourceName} no estaba sonando y el volumen objetivo es > 0. Iniciando Play().");
            source.Play();
        }

        while (tiempoPasado < duracion)
        {
            // Incrementa el tiempo pasado usando el tiempo real entre frames.
            tiempoPasado += Time.deltaTime;

            // Calcula el nuevo volumen usando una interpolación lineal (Lerp).
            // Esto crea una transición suave.
            float nuevoVolumen = Mathf.Lerp(volumenInicial, volumenObjetivo, tiempoPasado / duracion);
            source.volume = nuevoVolumen;

            // Espera hasta el siguiente frame antes de continuar el bucle.
            yield return null;
        }

        // Al finalizar el bucle, asegura que el volumen sea exactamente el volumen objetivo
        // para evitar pequeñas imprecisiones debidas al Time.deltaTime.
        source.volume = volumenObjetivo;
        // Debug.Log($"Fading completado para {sourceName}. Volumen final: {source.volume}");

        // Opcional: Si el volumen objetivo es 0 (o muy cercano), podrías detener el AudioSource
        // para ahorrar algo de recursos. Sin embargo, si está en loop y el volumen
        // vuelve a subir, `Play()` se llamaría de nuevo. Generalmente es seguro dejarlo "reproduciendo" con volumen 0.
        // if (volumenObjetivo <= 0.001f && source.isPlaying)
        // {
        //    Debug.Log($"AudioSource {sourceName} alcanzando volumen 0. Deteniendo.");
        //    source.Stop();
        // }
    }
}
