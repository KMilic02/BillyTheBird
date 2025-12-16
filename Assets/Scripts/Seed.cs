using UnityEngine;

public class Seed : MonoBehaviour, ICollectible
{
    public int amount;
    
    public void IOnCollect(Player player)
    {
        player.addSeeds(amount);
        gameObject.SetActive(false);
    }

    void Update()
    {
        var rotation = transform.rotation.eulerAngles;
        rotation.y += Time.deltaTime * 45.0f;
        transform.rotation = Quaternion.Euler(rotation);
    }
}
