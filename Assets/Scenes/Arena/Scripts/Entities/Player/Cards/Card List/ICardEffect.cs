using System.Collections;

//! This is the base interface for card scripts
//! Do not modify this script unless you're sure you want to affect ALL cards
public interface ICardEffect {
    // Card effect
    // Always pass effect in CardHandler.ResolveCard() instead of calling Effect()
    IEnumerator Effect();
}