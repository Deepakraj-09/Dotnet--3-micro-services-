namespace ShippingService.Models
{
    public class Ship
    {
        public long Distance { get; set; }
        public double Cost { get; set; }

        public Ship()
        {
            Distance = 0;
            Cost = 0.0;
        }

        public Ship(long distance, double cost)
        {
            Distance = distance;
            Cost = cost;
        }
    }
}
