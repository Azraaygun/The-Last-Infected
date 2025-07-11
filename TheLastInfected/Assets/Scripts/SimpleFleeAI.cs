using UnityEngine;

public class SimpleFleeAI : MonoBehaviour
{
    public float fleeDistance = 5f;
    public float speed = 3f;
    public Transform zombie;

    void Update()
    {
        float distance = Vector3.Distance(transform.position, zombie.position);

        if (distance < fleeDistance)
        {
            // Bileşenleri al
            ZombieManipulation zm = zombie.GetComponent<ZombieManipulation>();
            ZombieHide zh = zombie.GetComponent<ZombieHide>();

            bool isManipulating = zm != null && zm.isManipulating;
            bool isHiding = zh != null && zh.IsHiding();

            if (!isManipulating && !isHiding)
            {
                // Kaç
                Vector3 dir = (transform.position - zombie.position).normalized;
                transform.position += dir * speed * Time.deltaTime;
            }
            else
            {
                // Zombi saklanıyorsa ya da manipüle ediyorsa kaçma
            }
        }
    }
}
