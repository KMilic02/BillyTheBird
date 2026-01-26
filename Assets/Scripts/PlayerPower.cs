using UnityEngine;
using UnityEngine.UI;

public partial class Player : MonoBehaviour
{
    int seeds;
    int feathers;
    float maxGlideDuration => 1.0f + (glidingUpgraded ? 2.0f : 0.0f);
    float glideDurationLeft;

    bool glidingUpgraded => feathers >= 1;
    bool canDash => feathers >= 2 && !isGrounded() && landedAfterDash;
    
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
