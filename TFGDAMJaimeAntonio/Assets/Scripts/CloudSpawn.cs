using System.Collections;
using UnityEngine;

public class CloudSpawn : MonoBehaviour
{

    public float Speed = 1.0f;
    public float Distance = 5.0f;
    public bool IsMovingRight = true;
    private Vector2 StartPos;
    private Rigidbody2D rb;

    public GameObject prefabGota;
    public float intervalo = 1.5f;


    private Coroutine lluviaActiva;
    public bool IsRaining { get; private set; }

    void Start()
    {
        StartPos = transform.position;
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        //Movimiento horizontal de la nube
        float direction = IsMovingRight ? 1.0f : -1.0f;
        Vector2 move = new Vector2(direction * Speed * Time.fixedDeltaTime, 0f);
        rb.MovePosition(rb.position + move);

        float movedDistance = Vector2.Distance(rb.position, StartPos);
        if (movedDistance >= Distance)
        {
            IsMovingRight = !IsMovingRight;
            StartPos = rb.position;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Wall"))
        {
            IsMovingRight = !IsMovingRight;
            StartPos = rb.position;
        }
    }

    public void StartRain()
    {
        if (!IsRaining)
        {
            lluviaActiva = StartCoroutine(SpawnGota());
            IsRaining = true;
        }
    }

    public void StopRain()
    {
        if (lluviaActiva != null)
        {
            StopCoroutine(lluviaActiva);
            lluviaActiva = null;
            IsRaining = false;
        }
    }

    IEnumerator SpawnGota()
    {
        while (true)
        {
            Vector3 spawnPosition = transform.position + new Vector3(0, -0.1f, 0);
            Instantiate(prefabGota, spawnPosition, Quaternion.identity);
            yield return new WaitForSeconds(intervalo);
        }
    }
}
