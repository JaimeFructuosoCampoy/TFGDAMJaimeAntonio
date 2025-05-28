using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class QuestionHandler : MonoBehaviour
{
    public GeminiAPIHandler geminiHandler;

    public TMP_Text questionText; // Texto donde se mostrar� la pregunta generada.
    public Button[] answerButtons; // Arreglo de botones para las opciones de respuesta (A, B, C, D).
    public Button closeFeedbackButton; // Bot�n para cerrar el popup de feedback.

    private string correctAnswer; // Almacena la respuesta correcta de la pregunta actual.
    private bool awaitingAnswer = false; // Indica si se est� esperando una respuesta de la API.

    public GameObject popUpIA; // Popup que muestra la pregunta generada por la IA.

    public GameObject PopUpFeedback; // Popup que muestra el feedback al usuario.
    public TMP_Text popUpFeedbackText; // Texto dentro del popup para mostrar el feedback.

    public event Action onPopupClosed; // Evento que se dispara cuando el popup se cierra.
    public GameManager gameManager;

    // Lista de temas espec�ficos para las preguntas.
    private string[] topics = { "lluvia", "meteoritos", "tsunamis" };

    void Start()
    {
        // Configurar los listeners de los botones de respuesta.
        for (int i = 0; i < answerButtons.Length; i++)
        {
            int index = i; // Capturar el �ndice del bot�n actual.
            answerButtons[i].onClick.AddListener(() => OnAnswerButtonClicked(index));
        }

        // Configurar el listener del bot�n de cerrar feedback.
        closeFeedbackButton.onClick.AddListener(CloseAllPopUps);


        OpenPopUpIA();

        // Solicitar la primera pregunta al iniciar la escena.
        AskNewQuestion();
    }

    /// <summary>
    /// M�todo para solicitar una nueva pregunta a la API.
    /// </summary>
    void AskNewQuestion()
    {
        if (geminiHandler != null && questionText != null && !awaitingAnswer)
        {
            // Seleccionar un tema aleatorio de la lista de temas.
            string randomTopic = topics[UnityEngine.Random.Range(0, topics.Length)];

            // Generar la consulta con el tema seleccionado.
            string query = $"Hazme una pregunta sencilla menos de 80 caracteres y corta sobre {randomTopic} con cuatro opciones de respuesta. Formatea la pregunta incluyendo las opciones A, B, C y D. Indica claramente cu�l es la respuesta correcta al final, precedida por 'Respuesta correcta:'.";

            // Mostrar un mensaje temporal mientras se espera la respuesta.
            questionText.text = "Pensando...";
            awaitingAnswer = true; // Indicar que se est� esperando una respuesta de la API.

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
    /// M�todo para manejar la respuesta de la API.
    /// </summary>
    /// <param name="aiResponse"></param>
    void HandleAIResponse(string aiResponse)
    {
        string questionWithOptions = ""; // Almacena la pregunta con las opciones.
        correctAnswer = ""; // Reinicia la respuesta correcta.

        // Dividir la respuesta de la API en la pregunta y la respuesta correcta.
        string[] parts = aiResponse.Split(new string[] { "Respuesta correcta:" }, System.StringSplitOptions.None);

        if (parts.Length == 2)
        {
            questionWithOptions = parts[0].Trim(); // Extraer la pregunta.
            correctAnswer = parts[1].Trim().ToUpper(); // Extraer y normalizar la respuesta correcta.

            // Mostrar la pregunta en la interfaz.
            questionText.text = questionWithOptions;

            // Habilitar los botones de respuesta.
            foreach (var button in answerButtons)
            {
                button.interactable = true;
            }

            awaitingAnswer = false; // Indicar que ya no se est� esperando una respuesta.
        }
        else
        {
            // Manejar errores en la respuesta de la API.
            questionText.text = "Error al obtener la pregunta de la IA.";
            awaitingAnswer = false;
        }
    }

    /// <summary>
    /// M�todo que se ejecuta al hacer clic en un bot�n de respuesta.
    /// </summary>
    /// <param name="buttonIndex">�ndice del bot�n presionado.</param>
    void OnAnswerButtonClicked(int buttonIndex)
    {
        // Obtener la respuesta seleccionada por el usuario (A, B, C o D).
        string userAnswer = ((char)('A' + buttonIndex)).ToString();
        bool isCorrect = userAnswer == correctAnswer;

        // Generar el feedback basado en la respuesta del usuario.
        string feedback = $"Tu respuesta: {userAnswer}\nRespuesta correcta: {correctAnswer}";

        if (userAnswer == correctAnswer)
        {
            feedback += "\n\n�Correcto! La gravedad en esta partida ser� un poco menor por lo que..\n ��SALTARAS M�S!!";
        }
        else
        {
            feedback += "\n\n�Incorrecto! La gravedad en esta partida sera un poco mayor por lo que..\n Saltaras menos..";
        }

        //Mostrar el feedback en el popup.
        popUpFeedbackText.text = feedback;
        OpenPopUpFeedback();

        // Deshabilitar los botones de respuesta.
        foreach (var button in answerButtons)
        {
            button.interactable = false;
        }

        //Llamar directamente al GameManager
        if (isCorrect)
        {
            gameManager.removeGravity();
            Console.WriteLine("Masa del jugador reducida");
        }
        else
        {
            gameManager.addGravity();
            Console.WriteLine("Masa del jugador aumentada");
        }
    }

    /// <summary>
    /// M�todo para abrir el popup.
    /// </summary>
    private void OpenPopUpFeedback()
    {
        Debug.Log("Intentando abrir el popup de feedback."); // Depuraci�n
        PopUpFeedback.SetActive(true);

        PopUpFeedback.transform.localScale = new Vector3(0, 0, 0); // Reinicia la escala
        LeanTween.scale(PopUpFeedback, new Vector3(1f, 1f, 1), 0.5f)
            .setEaseOutBack()
            .setIgnoreTimeScale(true); // Ignorar Time.timeScale
        Debug.Log("Popup de feedback mostrado."); // Depuraci�n
    }



    /// <summary>
    /// M�todo para cerrar el popup.
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
                gameManager.BackgroundQuit(); // <-- Aqu�, despu�s de la animaci�n
            });
    }

    private void OpenPopUpIA()
    {
        Debug.Log("Intentando abrir el popup principal (popUpIA)..."); // Depuraci�n

        // Reinicia la escala del popup a (0, 0, 0)
        popUpIA.transform.localScale = new Vector3(0, 0, 0);
        popUpIA.SetActive(true); // Aseg�rate de que el popup est� activo

        //Escala el popup a (1, 1, 1) con animaci�n
        LeanTween.scale(popUpIA, new Vector3(1f, 1f, 1), 1f)
            .setEaseOutBack()
            .setIgnoreTimeScale(true); // Ignorar Time.timeScale
        Debug.Log("Popup principal (popUpIA) mostrado."); // Depuraci�n
    }

    private void ClosePopUpFeedback()
    {
        Debug.Log("Intentando cerrar el popup de feedback."); // Depuraci�n

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