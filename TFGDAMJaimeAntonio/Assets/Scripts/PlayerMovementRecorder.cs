using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementRecorder : MonoBehaviour
{
    public float RecordInterval = 0.1f;
    private List<Vector3> RecordedPositions = new List<Vector3>();

    void Start()
    {
        StartCoroutine(RecordPlayerMovement());
    }

    IEnumerator RecordPlayerMovement()
    {
        while (true)
        {
            RecordedPositions.Add(transform.position);
            yield return new WaitForSeconds(RecordInterval);
        }
    }

    public List<Vector3> GetRecordedPositions()
    {
        return RecordedPositions;
    }
}
