using RoboShop.Shipping.Models;

namespace RoboShop.Shipping.Services;

/// <summary>
/// Haversine distance calculator — converted from Java Calculator.java.
/// Formula reference: https://www.movable-type.co.uk/scripts/latlong.html
/// </summary>
public class Calculator
{
    private readonly double _latitude;
    private readonly double _longitude;

    public Calculator(double latitude, double longitude)
    {
        _latitude = latitude;
        _longitude = longitude;
    }

    public Calculator(City city)
    {
        _latitude = city.Latitude;
        _longitude = city.Longitude;
    }

    /// <summary>
    /// Calculate the great-circle distance (km) between this location and a target.
    /// Uses decimal lat/long degrees. Direct port of Java getDistance().
    /// </summary>
    public long GetDistance(double targetLatitude, double targetLongitude)
    {
        const double earthRadius = 6_371_000; // metres

        // Convert to radians
        double latitudeR = DegreesToRadians(_latitude);
        double targetLatitudeR = DegreesToRadians(targetLatitude);

        // Differences in radians
        double diffLatR = DegreesToRadians(targetLatitude - _latitude);
        double diffLongR = DegreesToRadians(targetLongitude - _longitude);

        double a = Math.Sin(diffLatR / 2.0) * Math.Sin(diffLatR / 2.0)
                 + Math.Cos(latitudeR) * Math.Cos(targetLatitudeR)
                 * Math.Sin(diffLongR / 2.0) * Math.Sin(diffLongR);

        double c = 2.0 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1.0 - a));

        return (long)Math.Round(earthRadius * c / 1000.0);
    }

    private static double DegreesToRadians(double degrees) =>
        degrees * Math.PI / 180.0;
}
