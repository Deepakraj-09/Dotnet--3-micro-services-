using ShippingService.Models;

namespace ShippingService.Services
{
    public class Calculator
    {
        private readonly double _latitude;
        private readonly double _longitude;

        public Calculator(City city)
        {
            _latitude = city.Latitude;
            _longitude = city.Longitude;
        }

        public long GetDistance(double targetLatitude, double targetLongitude)
        {
            const double earthRadius = 6371e3;

            var latitudeR = DegreesToRadians(_latitude);
            var targetLatitudeR = DegreesToRadians(targetLatitude);
            var diffLatR = DegreesToRadians(targetLatitude - _latitude);
            var diffLongR = DegreesToRadians(targetLongitude - _longitude);

n            var a = Math.Sin(diffLatR / 2.0) * Math.Sin(diffLatR / 2.0) +
                    Math.Cos(latitudeR) * Math.Cos(targetLatitudeR) *
                    Math.Sin(diffLongR / 2.0) * Math.Sin(diffLongR / 2.0);

            var c = 2.0 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1.0 - a));
            return (long)Math.Round(earthRadius * c / 1000.0);
        }

        private static double DegreesToRadians(double degrees) => degrees * Math.PI / 180.0;
    }
}
