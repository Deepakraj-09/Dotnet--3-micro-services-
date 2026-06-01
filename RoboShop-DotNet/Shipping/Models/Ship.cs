namespace RoboShop.Shipping.Models;

/// <summary>
/// DTO to hold shipping calculation result — converted from Java Ship.java
/// </summary>
public class Ship
{
    public long Distance { get; set; }
    public double Cost { get; set; }

    public Ship() { }

    public Ship(long distance, double cost)
    {
        Distance = distance;
        Cost = cost;
    }

    public override string ToString() =>
        $"Distance: {Distance} Cost: {Cost:F6}";
}
