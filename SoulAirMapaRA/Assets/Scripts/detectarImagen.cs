using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ImageTracking : MonoBehaviour
{
    public ARTrackedImageManager trackedImageManager;
    public GameObject modelPrefab;

    void Awake()
    {
        trackedImageManager = GetComponent<ARTrackedImageManager>();
    }

    void OnEnable()
    {
        trackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    void OnDisable()
    {
        trackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (ARTrackedImage trackedImage in eventArgs.added)
        {
            // Instancia el modelo 3D sobre la imagen detectada
            Instantiate(modelPrefab, trackedImage.transform.position, trackedImage.transform.rotation);
        }

        foreach (ARTrackedImage trackedImage in eventArgs.updated)
        {
            // Actualiza la posición del modelo si es necesario
        }

        foreach (ARTrackedImage trackedImage in eventArgs.removed)
        {
            // Opcionalmente, destruye el modelo si la imagen no es visible
        }
    }
}
