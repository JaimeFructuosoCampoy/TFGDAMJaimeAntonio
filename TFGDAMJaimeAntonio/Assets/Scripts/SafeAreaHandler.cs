using UnityEngine;

// Este script se asegura de que el panel al que est� a�adido se ajuste al �rea segura de la pantalla.
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

    // Se usa Update para detectar cambios en la orientaci�n del dispositivo (vertical/horizontal)
    void Update()
    {
        if (Screen.safeArea != lastSafeArea)
        {
            ApplySafeArea();
        }
    }

    private void ApplySafeArea()
    {
        Rect safeArea = Screen.safeArea;
        lastSafeArea = safeArea;

        // Convertir el rect�ngulo del �rea segura de p�xeles a porcentajes (0 a 1)
        Vector2 anchorMin = safeArea.position;
        Vector2 anchorMax = safeArea.position + safeArea.size;

        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;

        // Aplicar los porcentajes a los anclajes del panel
        panel.anchorMin = anchorMin;
        panel.anchorMax = anchorMax;
    }
}