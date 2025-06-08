using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class SafeAreaHandler : MonoBehaviour
{
    private RectTransform panel;
    private Rect lastSafeArea = new Rect(0, 0, 0, 0);

    void Awake()
    {
        panel = GetComponent<RectTransform>();
        ApplySafeArea();
    }

    void Update()
    {
        // Vuelve a aplicar si el área segura cambia (ej. al girar el dispositivo)
        if (Screen.safeArea != lastSafeArea)
        {
            ApplySafeArea();
        }
    }

    private void ApplySafeArea()
    {
        Rect safeArea = Screen.safeArea;
        lastSafeArea = safeArea;

        // Convertir el área segura de píxeles a porcentajes (0 a 1)
        Vector2 anchorMin = safeArea.position;
        Vector2 anchorMax = safeArea.position + safeArea.size;

        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;

        
        // Forzamos el anclaje superior (anchorMax.y) a llegar siempre al borde de la pantalla (valor de 1),
        // ignorando el recorte del notch. Los otros tres lados se mantienen.
        anchorMax.y = 1f;
        

        // Aplicar los porcentajes a los anclajes del panel
        panel.anchorMin = anchorMin;
        panel.anchorMax = anchorMax;

    }
}