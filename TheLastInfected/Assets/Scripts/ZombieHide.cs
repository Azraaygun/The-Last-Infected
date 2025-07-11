using UnityEngine;

public class ZombieHide : MonoBehaviour
{
    public float hideRange = 3f;                  // Saklanma menzili
    public LayerMask hidingLayer;                // Kutuların layer'ı
    public Transform fpsCamera;                  // FPS kamera

    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Vector3 originalCamPos;
    private Quaternion originalCamRot;
    private bool isHiding = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H) && !isHiding)
        {
            TryHide();
        }
        else if (Input.GetKeyDown(KeyCode.G) && isHiding)
        {
            ExitHide();
        }
    }

    void TryHide()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, hideRange, hidingLayer);

        foreach (Collider hit in hits)
        {
            Bounds bounds = hit.bounds;
            Vector3 targetPosition = bounds.center;

            originalPosition = transform.position;
            originalRotation = transform.rotation;
            originalCamPos = fpsCamera.position;
            originalCamRot = fpsCamera.rotation;

            transform.position = targetPosition;
            fpsCamera.position = targetPosition + new Vector3(0, 0.5f, 0); // Kamera biraz yukarıda

            isHiding = true;
            Debug.Log("Saklandı.");
            return;
        }

        Debug.LogWarning("Yakında saklanacak kutu yok.");
    }

    void ExitHide()
    {
        transform.position = originalPosition;
        transform.rotation = originalRotation;
        fpsCamera.position = originalCamPos;
        fpsCamera.rotation = originalCamRot;

        isHiding = false;
        Debug.Log("Kutudan çıktı.");
    }

    public bool IsHiding()
    {
        return isHiding;
    }
}
