package solver

import "core:math"

square :: proc(n: f32) -> f32
{
	result : f32 = n * n
	return result
}

radiansFromDegrees :: proc(degrees: f32) -> f32
{
    result : f32 = 0.01745329251994329577 * degrees;
    return result;
}

// NOTE(casey): EarthRadius is generally expected to be 6372.8
referenceHaversine :: proc(X0: f32, Y0: f32, X1: f32, Y1: f32, EarthRadius: f32 = 6372.8) -> f32
{
    /* NOTE(casey): This is not meant to be a "good" way to calculate the Haversine distance.
       Instead, it attempts to follow, as closely as possible, the formula used in the real-world
       question on which these homework exercises are loosely based.
    */
    
    lat1 := Y0
    lat2 := Y1
    lon1 := X0
    lon2 := X1
    
    dLat := radiansFromDegrees(lat2 - lat1);
    dLon := radiansFromDegrees(lon2 - lon1);
    lat1 = radiansFromDegrees(lat1);
    lat2 = radiansFromDegrees(lat2);
    
    a := square(math.sin_f32(dLat/2.0)) + math.cos_f32(lat1)*math.cos_f32(lat2)*square(math.sin_f32(dLon/2));
    c := 2.0*math.asin_f32(math.sqrt_f32(a));
    
    result := EarthRadius * c;
    
    return result;
}
