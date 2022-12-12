// Player and Enemy classes will inherit this and Monobehaviour
public interface IEntity {
    int Health { get; set; }
    int HealthMax { get; set; }
}