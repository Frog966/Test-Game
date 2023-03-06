using UnityEngine;
using UnityEngine.UI;

public class Player_Energy : MonoBehaviour {
    [SerializeField] private int energyMaxTrue = 3, energy, energyMax;  // Energy amounts

    [Header("UI Stuff")]
    [SerializeField] private Text energyText, energyMaxText;
    [SerializeField] private Image fill;
    [SerializeField] private Animator animator;

    public void ResetEnergy() {
        energy = energyMax = energyMaxTrue;

        UpdateUI();
    }

    // Increase energy amount
    public void IncreaseEnergy(int i) {
        // Debug.Log("IncreaseEnergy: " + energy + ", " + i);

        energy += i;

        UpdateUI();
    }

    // Decrease energy amount
    // Cannot go below 0
    public void DecreaseEnergy(int i) {
        // Debug.Log("DecreaseEnergy: " + energy + ", " + i);

        energy = (energy - i < 0) ? 0 : energy - i;
        
        UpdateUI();
    }

    // A simple bool to check if player can play the card
    public bool CanPayEnergyCost(int cost) { return energy >= cost; }
    
    // Contains the animation that plays to tell the player they don't have enough energy
    public void NotEnoughEnergy() { 
        Debug.Log("Player does not have enough energy to play the card!");

        animator.Play("Energy UI Red");
    }

    private void UpdateUI() {
        // Debug.Log("UpdateUI: " + energy + ", " + energyMax);

        // Update UI text
        energyText.text = energy.ToString();
        energyMaxText.text = energyMax.ToString();
        
        // Calculates UI's fill amount with simple division
        // Type casting to float because we need it as float
        fill.fillAmount = (float)energy / (float)energyMax;
    }

    // Start is called before the first frame update
    void Start() {
        ResetEnergy();
    }
}
