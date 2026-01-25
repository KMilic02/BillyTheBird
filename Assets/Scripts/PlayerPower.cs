using UnityEngine;
using UnityEngine.UI;

public partial class Player : MonoBehaviour
{
    int seeds;
    int feathers;

    bool glidingUpgraded => feathers >= 1;
    bool canDash => feathers >= 2 && !isGrounded();
    
    public void addSeeds(int amount)
    {
        seeds += amount;
        updateUI();
    }

    public void addFeather()
    {
        feathers++;
        updateUI();
    }
}
