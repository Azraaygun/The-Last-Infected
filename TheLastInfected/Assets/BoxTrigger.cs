using UnityEngine;

public class BoxTrigger : MonoBehaviour
{
    public Ch2 zombieScript;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Zombie"))
        {
            zombieScript.nearBox = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Zombie"))
        {
            zombieScript.nearBox = false;
        }
    }
}

