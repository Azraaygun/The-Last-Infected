using UnityEngine;
using System.Collections;

public class SoldierShooting : MonoBehaviour
{
    [Header("Fire Settings")]
    public Transform firePoint;
    public float fireRate = 1f;
    public float fireRange = 50f;
    public int damage = 10;
    public LayerMask playerLayer;

    [Header("Effects")]
    public GameObject muzzleFlashPrefab;
    public AudioSource gunAudio;

    private float fireTimer;
    private bool isShooting = false;

    void Update()
    {
        if (!isShooting) return;

        fireTimer += Time.deltaTime;
        if (fireTimer >= fireRate)
        {
            fireTimer = 0f;
            ShootRay();
        }
    }

    public void StartShooting()
    {
        isShooting = true;
    }

    public void StopShooting()
    {
        isShooting = false;
    }

    void ShootRay()
    {
        if (muzzleFlashPrefab)
        {
            GameObject flash = Instantiate(muzzleFlashPrefab, firePoint.position, firePoint.rotation, firePoint);
            Destroy(flash, 0.3f);
        }

        if (gunAudio)
        {
            gunAudio.Play();
        }

        Ray ray = new Ray(firePoint.position, firePoint.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, fireRange, playerLayer))
        {
            Debug.Log("Hasar " + hit.collider.name);

            StartCoroutine(ShowBulletLine(firePoint.position, hit.point));
        }
        else
        {
            StartCoroutine(ShowBulletLine(firePoint.position, firePoint.position + firePoint.forward * fireRange));
        }
    }

    IEnumerator ShowBulletLine(Vector3 start, Vector3 end)
    {
        GameObject lineObj = new GameObject("BulletLine");
        LineRenderer lr = lineObj.AddComponent<LineRenderer>();

        lr.positionCount = 2;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);

        lr.startWidth = 0.05f;
        lr.endWidth = 0.05f;

        lr.material = new Material(Shader.Find("Unlit/Color"));
        lr.material.color = Color.yellow;

        yield return new WaitForSeconds(0.05f);
        Destroy(lineObj);
    }
}
