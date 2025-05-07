using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

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
    public TMP_Text popUpFeedbackText; 
    private bool isPopUpActive = false; 

    //Lista de temas para las preguntas
    private string[] topics = { "lluvia", "meteoritos", "tsunamis" };

    //Historial de preguntas generadas para evitar repeticiones.
    private HashSet<string> questionHistory = new HashSet<string>();

    void Start()
    {
        sendAnswerButton.onClick.AddListener(OnSendAnswerClicked); 

        nextQuestionButton.onClick.AddListener(AskNewQuestion); //Llama a AskNewQuestion

        //Solicitamos la primera pregunta al iniciar la escena.
        AskNewQuestion();
    }

    /// <summary>
    /// Método para solicitar una nueva pregunta a la API.
    /// </summary>
    void AskNewQuestion()
    {
        if (geminiHandler != null && questionText != null && !awaitingAnswer)
        {
            //Ramdomizar un tema de la lista
            string randomTopic = topics[Random.Range(0, topics.Length)];

            //Generar la consulta con el tema seleccionado.
            string query = $"Hazme una pregunta sencilla menos de 80 caracteres y corta sobre {randomTopic} con cuatro opciones de respuesta. Formatea la pregunta incluyendo las opciones A, B, C y D. Indica claramente cuál es la respuesta correcta al final, precedida por 'Respuesta correcta:'.";

            //Mientras se espera respuesta
            questionText.text = "Pensando...";
            awaitingAnswer = true;

            //Enviarmos a la api.
            geminiHandler.SendQueryToGemini(query, HandleAIResponse);

            //Limpiar el campo de entrada y cerrar el popup.
            answerInput.text = "";
            ClosePopUp();

            //Habilitar el botón de send
            sendAnswerButton.interactable = true;
        }
    }

    /// <summary>
    /// Método para manejar la respuesta de la API.
    /// </summary>
    /// <param name="aiResponse"></param>
   
    void HandleAIResponse(string aiResponse)
    {
        string questionWithOptions = ""; //Almacena la pregunta con las opciones.
        correctAnswer = ""; //Reinicia la respuesta correcta.

        //Dividir la respuesta de la API en la pregunta y la respuesta correcta.
        string[] parts = aiResponse.Split(new string[] { "Respuesta correcta:" }, System.StringSplitOptions.None);

        if (parts.Length == 2)
        {
            questionWithOptions = parts[0].Trim(); //Extraer la pregunta.
            correctAnswer = parts[1].Trim().ToUpper(); //Extraer y normalizar la respuesta correcta.

            //Verificar si la pregunta ya existe en el historial.
            if (questionHistory.Contains(questionWithOptions))
            {
                Debug.Log("Pregunta repetida, solicitando una nueva...");
                awaitingAnswer = false; //Permitir solicitar otra pregunta.
                AskNewQuestion(); //Solicitar una nueva pregunta.
                return;
            }

            //Agregar la pregunta al historial y mostrarla.
            questionHistory.Add(questionWithOptions);
            questionText.text = questionWithOptions;
            awaitingAnswer = false; //Indicar que ya no se está esperando una respuesta.
        }
        else
        {
            //Manejar errores en la respuesta de la API.
            questionText.text = "Error al obtener la pregunta de la IA.";
            awaitingAnswer = false;
        }
    }

    /// <summary>
    /// Método que se ejecuta al hacer clic en el botón de enviar respuesta.
    /// </summary>
    void OnSendAnswerClicked()
    {
        if (answerInput != null && popUpFeedbackText != null)
        {
            //Obtener la respuesta del usuario y normalizarla.
            string userAnswer = answerInput.text.Trim().ToUpper();

            //Generar el feedback basado en la respuesta del usuario.
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

            //Mostrar el feedback en el popup.
            popUpFeedbackText.text = feedback;
            OpenPopUp();

            // Deshabilitar el botón de enviar respuesta y habilitar el de "Siguiente pregunta".
            sendAnswerButton.interactable = false;
            nextQuestionButton.gameObject.SetActive(true);

            // Cerrar el popup automáticamente después de 2 segundos.
            Invoke(nameof(ClosePopUp), 2f);
        }
    }

    /// <summary>
    /// Método para abrir el popup.
    /// </summary>
    private void OpenPopUp()
    {
        PopUpFeedback.SetActive(true);

        PopUpFeedback.transform.localScale = new Vector3(0, 0, 0);
        LeanTween.scale(PopUpFeedback, new Vector3(1, 1, 1), 0.5f).setEaseOutBack();

        isPopUpActive = true;
    }

    /// <summary>
    /// Método para cerrar el popup.
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