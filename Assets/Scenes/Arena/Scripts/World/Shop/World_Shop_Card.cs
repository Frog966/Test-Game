using UnityEngine;

public class World_Shop_Card : MonoBehaviour {
    [SerializeField] private Card_Stats card;
    [SerializeField] private UnityEngine.UI.Text price;

    // Getters
    public Card_Stats GetCard() { return card; }
    public int GetPrice() { return System.Int32.Parse(price.text); }

    public void Setup(Card_Stats _card, bool isDiscounted = false) {
        card.Copy(_card);
        card.isPlayable = false; // Set the card to unplayable

        price.text = GeneratePrice(isDiscounted);
        price.color = isDiscounted ? Color.blue : Color.white; // Change text color if discounted
    }

    private string GeneratePrice(bool isDiscounted) {
        int price;

        // Get base price
        switch(card.Rarity) {
            case Game.Card.CardRarity.COMMON:
                price = 40;
            break;
            case Game.Card.CardRarity.UNCOMMON:
                price = 60;
            break;
            case Game.Card.CardRarity.RARE:
                price = 100;
            break;
            default:
                price = 200;
            break;
        }

        // Add discount to base price
        if (isDiscounted) { price /= 2; }

        // Add a slight deviation to base price
        switch(Random.Range(0, 5)) {
            case 0: return (price - 2).ToString();
            case 1: return (price - 1).ToString();
            case 2: return (price + 1).ToString();
            case 3: return (price + 2).ToString();
            default: return price.ToString();
        }
    }
}
