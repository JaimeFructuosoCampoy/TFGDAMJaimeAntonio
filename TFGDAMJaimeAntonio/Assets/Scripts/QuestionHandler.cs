using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class QuestionHandler : MonoBehaviour
{
    public GeminiAPIHandler geminiHandler;

    public TMP_Text questionText; // Texto donde se mostrará la pregunta generada.
    public Button[] answerButtons; // Arreglo de botones para las opciones de respuesta (A, B, C, D).
    public Button closeFeedbackButton; // Botón para cerrar el popup de feedback.

    private string correctAnswer; // Almacena la respuesta correcta de la pregunta actual.
    private bool awaitingAnswer = false; // Indica si se está esperando una respuesta de la API.

    public GameObject popUpIA; // Popup que muestra la pregunta generada por la IA.
    public GameObject backBlack;

    public GameObject PopUpFeedback; // Popup que muestra el feedback al usuario.
    public TMP_Text popUpFeedbackText; // Texto dentro del popup para mostrar el feedback.

    public event Action onPopupClosed; // Evento que se dispara cuando el popup se cierra.
    public GameManager gameManager;
    private string[] currentAnswerOptions = new string[4];

    // Lista de temas específicos para las preguntas.
    private string[] topics = { "lluvia", "meteoritos", "tsunamis, animales, planeta tierra" };

    void Start()
    {
        // Configurar los listeners de los botones de respuesta.
        for (int i = 0; i < answerButtons.Length; i++)
        {
            int index = i; // Capturar el índice del botón actual.
            answerButtons[i].onClick.AddListener(() => OnAnswerButtonClicked(index));
        }

        // Configurar el listener del botón de cerrar feedback.
        closeFeedbackButton.onClick.AddListener(CloseAllPopUps);


        OpenPopUpIA();

        // Solicitar la primera pregunta al iniciar la escena.
        AskNewQuestion();
    }

    /// <summary>
    /// Método para solicitar una nueva pregunta a la API.
    /// </summary>
    void AskNewQuestion()
    {
        if (geminiHandler != null && questionText != null && !awaitingAnswer)
        {
            // Seleccionar un tema aleatorio de la lista de temas.
            string randomTopic = topics[UnityEngine.Random.Range(0, topics.Length)];

            // Generar la consulta con el tema seleccionado.
            string query = $"Hazme una pregunta sencilla menos de 80 caracteres y corta sobre {randomTopic} con cuatro opciones de respuesta. Formatea la pregunta incluyendo las opciones A, B, C y D. Indica claramente cuál es la respuesta correcta al final, precedida por 'Respuesta correcta:'.";

            // Mostrar un mensaje temporal mientras se espera la respuesta.
            questionText.text = "Pensando...";
            awaitingAnswer = true; // Indicar que se está esperando una respuesta de la API.

            // Enviar la consulta a la API.
            geminiHandler.SendQueryToGemini(query, HandleAIResponse);

            // Deshabilitar los botones de respuesta mientras se espera la pregunta.
            foreach (var button in answerButtons)
            {
                button.interactable = false;
            }
        }
    }

    /// <summary>
    /// Método para manejar la respuesta de la API.
    /// </summary>
    /// <param name="aiResponse"></param>
    /// <summary>
    /// Método para manejar la respuesta de la API.
    /// </summary>
    /// <param name="aiResponse"></param>
    /// <summary>
    /// Método para manejar la respuesta de la API.
    /// </summary>
    void HandleAIResponse(string aiResponse)
    {
        Debug.Log("RESPUESTA COMPLETA DE LA API: " + aiResponse);

        string questionWithOptions = "";
        correctAnswer = "";

        string[] parts = aiResponse.Split(new string[] { "Respuesta correcta:" }, StringSplitOptions.None);

        if (parts.Length == 2)
        {
            questionWithOptions = parts[0].Trim();
            correctAnswer = parts[1].Trim().ToUpper();

            Debug.Log("VALOR GUARDADO EN 'correctAnswer': " + correctAnswer);

            // --- NUEVA LÓGICA DE PROCESAMIENTO ---
            // Ahora procesamos las opciones y las guardamos en nuestro array
            string[] lines = questionWithOptions.Split('\n');
            foreach (string line in lines)
            {
                if (line.Trim().StartsWith("A)")) currentAnswerOptions[0] = line.Trim();
                else if (line.Trim().StartsWith("B)")) currentAnswerOptions[1] = line.Trim();
                else if (line.Trim().StartsWith("C)")) currentAnswerOptions[2] = line.Trim();
                else if (line.Trim().StartsWith("D)")) currentAnswerOptions[3] = line.Trim();
            }
            // --- FIN DE LA NUEVA LÓGICA ---

            questionText.text = questionWithOptions;

            foreach (var button in answerButtons)
            {
                button.interactable = true;
            }

            awaitingAnswer = false;
        }
        else
        {
            questionText.text = "Error al obtener la pregunta de la IA.";
            awaitingAnswer = false;
        }
    }

    /// <summary>
    /// Método que se ejecuta al hacer clic en un botón de respuesta.
    /// </summary>
    void OnAnswerButtonClicked(int buttonIndex)
    {
        string userAnswerLetter = ((char)('A' + buttonIndex)).ToString();
        bool isCorrect = userAnswerLetter == correctAnswer;

        // --- LÓGICA SIMPLIFICADA ---
        // Ahora leemos los textos directamente desde nuestro array, no desde la UI

        // 1. Obtenemos el texto completo de la respuesta del usuario desde nuestro array.
        string userAnswerText = currentAnswerOptions[buttonIndex];

        // 2. Obtenemos el texto completo de la respuesta correcta desde nuestro array.
        int correctIndex = correctAnswer[0] - 'A';
        string correctAnswerText = currentAnswerOptions[correctIndex];

        // --- FIN DE LA LÓGICA SIMPLIFICADA ---

        // 3. Construimos el feedback
        string feedback = $"Respuesta correcta: {correctAnswerText}";

        if (isCorrect)
        {
            feedback += "\n\n¡Correcto! La gravedad será un poco menor por lo que..\n ¡¡SALTARAS MáS!!";
        }
        else
        {
            feedback += "\n\n¡Incorrecto! La gravedad será un poco mayor por lo que..\n Saltaras menos..";
        }

        popUpFeedbackText.text = feedback;
        OpenPopUpFeedback();

        foreach (var button in answerButtons)
        {
            button.interactable = false;
        }

        if (isCorrect)
        {
            gameManager.removeGravity();
            Debug.Log("Gravedad del jugador reducida");
        }
        else
        {
            gameManager.addGravity();
            Debug.Log("Gravedad del jugador aumentada");
        }
    }

    /// <summary>
    /// Método para abrir el popup.
    /// </summary>
    private void OpenPopUpFeedback()
    {
        Debug.Log("Intentando abrir el popup de feedback."); // Depuración
        PopUpFeedback.SetActive(true);

        PopUpFeedback.transform.localScale = new Vector3(0, 0, 0); // Reinicia la escala
        LeanTween.scale(PopUpFeedback, new Vector3(1f, 1f, 1), 0.5f)
            .setEaseOutBack()
            .setIgnoreTimeScale(true); // Ignorar Time.timeScale
        Debug.Log("Popup de feedback mostrado."); // Depuración
    }



    /// <summary>
    /// Método para cerrar el popup.
    /// </summary>
    private void ClosePopUpIA()
    {
        LeanTween.scale(popUpIA, new Vector3(0, 0, 0), 0.5f)
            .setEaseInBack()
            .setIgnoreTimeScale(true)
            .setOnComplete(() =>
            {
                popUpIA.SetActive(false);
                onPopupClosed?.Invoke();
                backBlack.SetActive(false); // <-- Aquí, después de la animación
            });
    }

    private void OpenPopUpIA()
    {
        Debug.Log("Intentando abrir el popup principal (popUpIA)..."); // Depuración

        backBlack.SetActive(true);

        // Reinicia la escala del popup a (0, 0, 0)
        popUpIA.transform.localScale = new Vector3(0, 0, 0);
        popUpIA.SetActive(true); // Asegúrate de que el popup esté activo

        //Escala el popup a (1, 1, 1) con animación
        LeanTween.scale(popUpIA, new Vector3(1f, 1f, 1), 1f)
            .setEaseOutBack()
            .setIgnoreTimeScale(true); // Ignorar Time.timeScale
        Debug.Log("Popup principal (popUpIA) mostrado."); // Depuración
    }

    private void ClosePopUpFeedback()
    {
        Debug.Log("Intentando cerrar el popup de feedback."); // Depuración

        LeanTween.scale(PopUpFeedback, new Vector3(0, 0, 0), 0.5f)
            .setEaseInBack()
            .setIgnoreTimeScale(true)
            .setOnComplete(() =>
            {
                PopUpFeedback.SetActive(false);
                Debug.Log("Popup de feedback desactivado.");
            });
    }

    private void CloseAllPopUps()
    {
        ClosePopUpFeedback();
        ClosePopUpIA();
    }
}