using UnityEngine;
using UnityEngine.UI;

public partial class Player : MonoBehaviour
{
    public Text seedCountText;
    
    void updateUI()
    {
        if (seedCountText == null)
            return;
        
        seedCountText.text = seeds.ToString();
    }
}
