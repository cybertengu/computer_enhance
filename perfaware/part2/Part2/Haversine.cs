namespace Part2
{
    internal class Haversine
    {
        static public double Square(double a)
        {
            double result = a * a;
            return result;
        }

        static public double RadiansFromDegrees(double degrees)
        {
            double result = 0.01745329251994329577 * degrees;
            return result;
        }


        static public double ReferenceHaversine(double x0, double y0, double x1, double y1, double earthRadius = 6372.8)
        {
            double lat1 = y0;
            double lat2 = y1;
            double lon1 = x0;
            double lon2 = x1;
            
            double dLat = RadiansFromDegrees(lat2 - lat1);
            double dLon = RadiansFromDegrees(lon2 - lon1);
            lat1 = RadiansFromDegrees(lat1);
            lat2 = RadiansFromDegrees(lat2);
            
            double a = Square(Math.Sin(dLat/2.0)) + Math.Cos(lat1)*Math.Cos(lat2)*Square(Math.Sin(dLon/2));
            double c = 2.0*Math.Asin(Math.Sqrt(a));
            
            double Result = earthRadius * c;
            
            return Result;
        }



        public class Point
        {
            public double X0;
            public double Y0;
            public double X1;
            public double Y1;

            public Point(double x0, double y0, double x1, double y1)
            {
                X0 = x0;
                Y0 = y0;
                X1 = x1;
                Y1 = y1;
            }
        }
    }
}