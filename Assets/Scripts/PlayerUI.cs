using UnityEngine;
using UnityEngine.UI;

public partial class Player : MonoBehaviour
{
    public Text seedCountText;
    
    void updateUI()
    {
        seedCountText.text = seeds.ToString();
    }
}
