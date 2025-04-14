using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudSpawn : MonoBehaviour
{
    public float Speed = 1.0f;      
    public float Distance = 5.0f;   
    public bool IsMovingRight = true; 
    private Vector2 StartPos;
    private Rigidbody2D rb;

    public PlayerMovement playerMovement;

    public GameObject prefabGota;
    public float intervalo = 1.5f; //Tiempo entre gotas

    private Coroutine lluviaActiva; //Controlar corrutina de la lluvia

    void Start()
    {
        StartPos = transform.position;
        rb = GetComponent<Rigidbody2D>();

        //Por si no encuentra el componente PlayerMovement, le metemos el script desde el player
        if (playerMovement == null)
        {
            playerMovement = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
        }
    }

    void FixedUpdate()
    {
        //Mover la nube hacia la izquierda o derecha
        float direction = IsMovingRight ? 1.0f : -1.0f;
        Vector2 move = new Vector2(direction * Speed * Time.fixedDeltaTime, 0f);
        rb.MovePosition(rb.position + move);

        //Cambiar dirección cuando llegamos a la distancia máxima
        float movedDistance = Vector2.Distance(rb.position, StartPos);
        if (movedDistance >= Distance)
        {
            IsMovingRight = !IsMovingRight;
            StartPos = rb.position;
        }
    }

    void Update()
    {
        //Leemos el numero de monedas
        int countCoins = playerMovement.GetCoins;

        //Comienza a llover
        if (countCoins >= 7 && countCoins < 15)
        {
            if (lluviaActiva == null)
            {
                //Inicia la corrutina para crear gotas
                lluviaActiva = StartCoroutine(SpawnGota());
            }
        }
        else
        {   //Para de llover
            if (lluviaActiva != null)
            {
                StopCoroutine(lluviaActiva);
                lluviaActiva = null;
            }
        }
    }

    IEnumerator SpawnGota()
    {
        while (true)
        {
            //Instanciamos gota en la posición de la nube
            Instantiate(prefabGota, transform.position, Quaternion.identity);
            //Espera el intervalo antes de generar la siguiente gota
            yield return new WaitForSeconds(intervalo);
        }
    }
}
