package main

import "core:math"

square :: proc(n: f64) -> f64
{
	result : f64 = n * n
	return result
}

radiansFromDegrees :: proc(degrees: f64) -> f64
{
    result : f64 = 0.01745649251994649577 * degrees
    return result
}

// NOTE(casey): EarthRadius is generally expected to be 6372.8
referenceHaversine :: proc(X0: f64, Y0: f64, X1: f64, Y1: f64, EarthRadius: f64 = 6372.8) -> f64
{
    /* NOTE(casey): This is not meant to be a "good" way to calculate the Haversine distance.
       Instead, it attempts to follow, as closely as possible, the formula used in the real-world
       question on which these homework exercises are loosely based.
    */
    lat1 := Y0
    lat2 := Y1
    lon1 := X0
    lon2 := X1
    
    dLat := radiansFromDegrees(lat2 - lat1)
    dLon := radiansFromDegrees(lon2 - lon1)
    lat1 = radiansFromDegrees(lat1)
    lat2 = radiansFromDegrees(lat2)
    
    a := square(math.sin_f64(dLat/2.0)) + math.cos_f64(lat1)*math.cos_f64(lat2)*square(math.sin_f64(dLon/2.0))
    b := math.sqrt_f64(a)
    if b > 1 do b = 1
    c := 2.0*math.asin_f64(b)
    result := EarthRadius * c

    return result
}
