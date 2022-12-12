using UnityEngine;
using UnityEngine.UI;

public class Player_Energy : MonoBehaviour {
    private Player player;

    [SerializeField] private Text energyText, energyMaxText;
    [SerializeField] private UnityEngine.UI.Image fill;

    // Start is called before the first frame update
    void Start() {
        //! Sanity Checks
        player = this.gameObject.GetComponent<Player>();

        ResetEnergy();
    }

    private void UpdateUI() {
        // Debug.Log("UpdateUI: " + player.energy + ", " + player.energyMax);

        // Update UI text
        energyText.text = player.energy.ToString();
        energyMaxText.text = player.energyMax.ToString();
        
        // Calculates UI's fill amount with simple division
        // Includes sanity check to avoid dividing 0 with number
        // Type casting to float because we need it as float
        fill.fillAmount = (float)player.energy > 0.0f ? (float)player.energy / (float)player.energyMax : 0.0f;
    }

    public void ResetEnergy() {
        player.energy = player.energyMax = player.energyMaxTrue;

        UpdateUI();
    }

    // Increase energy amount
    public void IncreaseEnergy(int i) {
        // Debug.Log("IncreaseEnergy: " + player.energy + ", " + i);

        player.energy += i;

        UpdateUI();
    }

    // Decrease energy amount
    // Cannot go below 0
    public void DecreaseEnergy(int i) {
        // Debug.Log("DecreaseEnergy: " + player.energy + ", " + i);

        player.energy = (player.energy - i < 0) ? 0 : player.energy - i;
        
        UpdateUI();
    }
}
