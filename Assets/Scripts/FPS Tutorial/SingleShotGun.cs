using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleShotGun : Gun
{
    [SerializeField] Camera cam;

    PhotonView PV;

    [SerializeField] Collider bodyCollider;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    public override void Use()
    {
        Shoot();
    }

    void Shoot()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        ray.origin = cam.transform.position;

        /*
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            hit.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(((GunInfo)itemInfo).damage);
            PV.RPC("RPC_Shoot", RpcTarget.All, hit.point, hit.normal);
        }*/

        /*
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // Check if the hit object has a PhotonView
            PhotonView hitPV = hit.collider.gameObject.GetComponent<PhotonView>();

            // Only allow shooting if the hit object is not the shooter (comparing PhotonView IDs)
            if (hitPV != null && hitPV.ViewID != PV.ViewID)
            {
                // Apply damage if the object implements IDamageable
                hit.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(((GunInfo)itemInfo).damage);

                // RPC call to handle the bullet impact on all clients
                PV.RPC("RPC_Shoot", RpcTarget.All, hit.point, hit.normal);
            }
            else
            {
                Debug.Log("Shot blocked: cannot shoot yourself.");
            }
        }*/

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            
            // Only allow shooting if the hit object is not the shooter (comparing PhotonView IDs)
            if (hit.collider != bodyCollider)
            {
                // Apply damage if the object implements IDamageable
                hit.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(((GunInfo)itemInfo).damage);

                // RPC call to handle the bullet impact on all clients
                PV.RPC("RPC_Shoot", RpcTarget.All, hit.point, hit.normal);
            }
            else
            {
                Debug.Log("Shot blocked: cannot shoot yourself.");
            }
        }
    }

    [PunRPC]
    void RPC_Shoot(Vector3 hitPosition, Vector3 hitNormal)
    {
        Collider[] colliders = Physics.OverlapSphere(hitPosition, 0.3f);
        if (colliders.Length != 0)
        {
            GameObject bulletImpactObj = Instantiate(bulletImpactPrefab, hitPosition + hitNormal * 0.001f, Quaternion.LookRotation(hitNormal, Vector3.up) * bulletImpactPrefab.transform.rotation);
            Destroy(bulletImpactObj, 10f);
            bulletImpactObj.transform.SetParent(colliders[0].transform);
        }

        muzzleFlashPS.Play();
    }
}
