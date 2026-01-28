using UnityEngine;

public class Feather : MonoBehaviour, ICollectible
{
    public AudioClip collectClip;

    public void IOnCollect(Player player)
    {
        AudioManager.Instance.PlaySFX(collectClip, 0.7f);
        player.addFeather();
        gameObject.SetActive(false);
    }

    void Update()
    {
        var rotation = transform.rotation.eulerAngles;
        rotation.y += Time.deltaTime * 45.0f;
        transform.rotation = Quaternion.Euler(rotation);
    }
}
