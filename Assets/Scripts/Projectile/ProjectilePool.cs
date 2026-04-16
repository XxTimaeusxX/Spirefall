using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilePool : MonoBehaviour
{
    public static ProjectilePool Instance { get; private set; }
    public Projectile projectilePrefab;
    public int InitialPoolSize = 30;
    private Queue<Projectile> pool = new Queue<Projectile>();
    void Awake()
    {
        if(Instance == null) Instance = this;
        else Destroy(gameObject);
        
        for (int i = 0; i < InitialPoolSize; i++)
        {
            Projectile projectile = Instantiate(projectilePrefab);
            projectile.gameObject.SetActive(false);
            pool.Enqueue(projectile);
        }
    }

    public Projectile GetProjectile(Vector3 position, Quaternion rotation)
    {
        Projectile p;

        if (pool.Count > 0)
        {
            // Reuse an existing projectile
            p = pool.Dequeue();
            p.transform.position = position;
            p.transform.rotation = rotation;
            p.gameObject.SetActive(true);
        }
        else
        {
            // Pool is empty, safely expand it by instantiating a new one
            p = Instantiate(projectilePrefab, position, rotation);
        }

        return p;
    }
    public void ReturnProjectile(Projectile p)
    {
        p.gameObject.SetActive(false);
        p.rb.linearVelocity = Vector3.zero; // Reset velocity
        pool.Enqueue(p);
    }
}
