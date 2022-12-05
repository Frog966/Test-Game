using UnityEngine;

//! This is the base class for enemy AI
//! Do not modify this script unless you're sure you want to affect ALL enemy AIs
public abstract class Enemy_AI : MonoBehaviour {
    public abstract void Act();
}