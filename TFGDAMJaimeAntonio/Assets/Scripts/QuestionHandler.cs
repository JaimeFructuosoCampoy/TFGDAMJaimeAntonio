using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class QuestionHandler : MonoBehaviour
{
    public GeminiAPIHandler geminiHandler;
    public TMP_Text questionText;
    public TMP_InputField answerInput;
    public Button sendAnswerButton;
    public Button nextQuestionButton;

    private string correctAnswer;
    private bool awaitingAnswer = false;

    public GameObject PopUpFeedback;
    public TMP_Text popUpFeedbackText; // Texto dentro del popup para mostrar el feedback
    private bool isPopUpActive = false;

    // Temas específicos para las preguntas
    private string[] topics = { "lluvia", "meteoritos", "tsunamis" };

    void Start()
    {
        if (sendAnswerButton != null)
        {
            sendAnswerButton.onClick.AddListener(OnSendAnswerClicked);
        }
        if (nextQuestionButton != null)
        {
            nextQuestionButton.onClick.AddListener(AskNewQuestion);
            nextQuestionButton.gameObject.SetActive(false); // Ocultar al inicio
        }

        // Pedir la primera pregunta al iniciar la escena
        AskNewQuestion();
    }

    void AskNewQuestion()
    {
        if (geminiHandler != null && questionText != null && !awaitingAnswer)
        {
            // Seleccionar un tema aleatorio
            string randomTopic = topics[Random.Range(0, topics.Length)];

            // Generar la consulta con el tema seleccionado
            string query = $"Hazme una pregunta sencilla menos de 80 caracteres y corta sobre {randomTopic} con cuatro opciones de respuesta. Formatea la pregunta incluyendo las opciones A, B, C y D. Indica claramente cuál es la respuesta correcta al final, precedida por 'Respuesta correcta:'.";

            questionText.text = "Pensando...";
            awaitingAnswer = true;
            geminiHandler.SendQueryToGemini(query, HandleAIResponse);
            answerInput.text = "";
            ClosePopUp(); // Asegurarse de cerrar el popup al pedir una nueva pregunta
            nextQuestionButton.gameObject.SetActive(false);
            sendAnswerButton.interactable = true;
        }
    }

    void HandleAIResponse(string aiResponse)
    {
        string questionWithOptions = "";
        correctAnswer = "";
        string[] parts = aiResponse.Split(new string[] { "Respuesta correcta:" }, System.StringSplitOptions.None);

        if (parts.Length == 2)
        {
            questionWithOptions = parts[0].Trim();
            correctAnswer = parts[1].Trim().ToUpper();
            questionText.text = questionWithOptions;
            awaitingAnswer = false;
        }
        else
        {
            questionText.text = "Error al obtener la pregunta de la IA.";
            awaitingAnswer = false;
        }
    }

    void OnSendAnswerClicked()
    {
        if (answerInput != null && popUpFeedbackText != null)
        {
            string userAnswer = answerInput.text.Trim().ToUpper();
            string feedback = $"Tu respuesta: {userAnswer}\nRespuesta correcta: {correctAnswer}";

            if (userAnswer == correctAnswer && "ABCD".Contains(userAnswer))
            {
                feedback += "\n\n\n¡Correcto!";
            }
            else if ("ABCD".Contains(userAnswer))
            {
                feedback += "\n\n\n¡Incorrecto!";
            }
            else
            {
                feedback += "\nRespuesta inválida. Escribe A, B, C o D.";
            }

            popUpFeedbackText.text = feedback; // Mostrar el feedback en el popup
            OpenPopUp(); // Abrir el popup para mostrar el feedback

            sendAnswerButton.interactable = false;
            nextQuestionButton.gameObject.SetActive(true);

            // Cerrar el popup automáticamente después de 5 segundos
            Invoke(nameof(ClosePopUp), 5f);
        }
    }

    public void TogglePopUPLenguage()
    {
        if (isPopUpActive)
        {
            ClosePopUp();
        }
        else
        {
            OpenPopUp();
        }
    }

    /// <summary>
    /// Metodo para abrir el popup de seleccion de idioma
    /// </summary>
    private void OpenPopUp()
    {
        PopUpFeedback.SetActive(true);

        PopUpFeedback.transform.localScale = new Vector3(0, 0, 0);
        LeanTween.scale(PopUpFeedback, new Vector3(1, 1, 1), 0.5f).setEaseOutBack();

        isPopUpActive = true;
    }

    /// <summary>
    /// Metodo para cerrar el popup de seleccion de idioma
    /// </summary>
    private void ClosePopUp()
    {
        LeanTween.scale(PopUpFeedback, new Vector3(0, 0, 0), 0.5f).setEaseInBack().setOnComplete(() =>
        {
            PopUpFeedback.SetActive(false);
        });

        isPopUpActive = false;
    }
}
