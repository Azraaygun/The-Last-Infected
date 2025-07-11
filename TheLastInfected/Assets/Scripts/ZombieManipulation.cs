using UnityEngine;
using System.Collections;

public class ZombieManipulation : MonoBehaviour
{
    public bool isManipulating = false;
    public float manipulationDuration = 10f;

    private Coroutine manipulationCoroutine;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M) && !isManipulating)
        {
            StartManipulation();
        }
    }

    void StartManipulation()
    {
        isManipulating = true;
        Debug.Log("Manipülasyon başladı");

        if (manipulationCoroutine != null)
            StopCoroutine(manipulationCoroutine);

        manipulationCoroutine = StartCoroutine(ManipulationTimer());
    }

    IEnumerator ManipulationTimer()
    {
        yield return new WaitForSeconds(manipulationDuration);
        isManipulating = false;
        Debug.Log("Manipülasyon sona erdi");
    }
}
