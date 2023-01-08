

public interface iHealth
{
   float Health { get; set; }
   int HealthMax { get; }
   void ModifyHealth (int amount);
}
