using UnityEngine;

public class ProjectileTag : MonoBehaviour
{
    public float speed;
    public float lifetime;
    
    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
        
        lifetime -= Time.deltaTime;
        if (lifetime <= 0)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<Enemy>(out _) || other.gameObject.TryGetComponent<ICollectible>(out _))
            return;
        
        Destroy(gameObject);
    }
}
