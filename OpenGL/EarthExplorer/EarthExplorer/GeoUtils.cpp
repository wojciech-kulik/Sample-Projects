#include "GeoUtils.h"
#include <math.h>

const float PI	= 3.14159f;

float GeoUtils::DegToRad(int alfa)
{
	return alfa * (float)PI / 180.0f;
}

float GeoUtils::DegToRad(float alfa)
{
	return alfa * (float)PI / 180.0f;
}

float GeoUtils::RadToDeg(float alfa)
{
	return alfa * 180.0f / (float)PI ;
}

vec3 GeoUtils::GetCoordinatesOnMap(vec3 position, float scale)
{
	//see: http://en.wikipedia.org/wiki/Mercator_projection
	float longitude = RadToDeg(position.x / scale);
	float latitude = RadToDeg(2.0f * atan(exp(position.y / scale)) - PI / 2.0f);

	return vec3(longitude, latitude, position.z);
}

vec3 GeoUtils::GetPositionOnMap(float lat, float lon, float scale)
{
	//see: http://en.wikipedia.org/wiki/Mercator_projection
	float x = scale * DegToRad(lon);
	float y = scale * log(tan(PI / 4.0f + DegToRad(lat) / 2.0f));

	return vec3(x, y, 0.0f);
}

vec3 GeoUtils::GetPositionOnMapi(int lat, int lon, float scale)
{
	return GetPositionOnMap((float)lat, (float)lon, scale);
}

vec3 GeoUtils::GetPositionOnGlobe(float lat, float lon, float alt, float earth_radius)
{	
	//see: http://stackoverflow.com/questions/10473852/convert-latitude-and-longitude-to-point-in-3d-space
	float longitude = DegToRad(lon);
	float latitude  = DegToRad(lat);
	float scaledAlt = 0.02f * alt / 5500.0f;

	float X = (earth_radius + scaledAlt) * cos(latitude) * sin(longitude);
    float Y = (earth_radius + scaledAlt) * sin(latitude);
    float Z = (earth_radius + scaledAlt) * cos(latitude) * cos(longitude);

	return vec3(X, Y, Z);
}

vec3 GeoUtils::GetPositionOnGlobei(int lat, int lon, int alt, float earth_radius)
{	
	return GetPositionOnGlobe((float)lat, (float)lon, (float)alt, earth_radius);
}