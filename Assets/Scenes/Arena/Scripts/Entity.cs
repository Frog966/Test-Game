// Player and Enemy classes will inherit this and Monobehaviour
public interface Entity {
    int Health { get; set; }
    int HealthMax { get; set; }
}