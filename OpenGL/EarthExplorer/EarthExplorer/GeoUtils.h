#pragma once

#include <glm/glm.hpp>

using namespace glm;



class GeoUtils
{
public:
	static float DegToRad(int alfa);
	static float DegToRad(float alfa);
	static float RadToDeg(float alfa);

	static vec3 GetCoordinatesOnMap(vec3 position, float scale = 20.0f);

	static vec3 GetPositionOnMap(float lat, float lon, float scale = 20.0f);	
	static vec3 GetPositionOnMapi(int lat, int lon, float scale = 20.0f);

	static vec3 GetPositionOnGlobe(float lat, float lon, float alt = 0, float earth_radius = 1.0f);		
	static vec3 GetPositionOnGlobei(int lat, int lon, int alt = 0, float earth_radius = 1.0f);
};
