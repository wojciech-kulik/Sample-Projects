#include "MapCamera.h"
#include "GeoUtils.h"
#include "InfoPrinter.h"
#include "InfoIds.h"
#include <algorithm>

MapCamera::MapCamera(GLFWwindow * w) : Camera(w)
{
	speed = 2.0f;
	viewDistance = 100.0f;
	fixedSystem = true;
}

float MapCamera::getMaxHorizontalAngel()
{
	return PI;
}

float MapCamera::getMinHorizontalAngel()
{
	return PI;
}

float MapCamera::getMaxVerticalAngel()
{
	return 0.0f;
}

float MapCamera::getMinVerticalAngel()
{
	return 0.0f;
}

void MapCamera::showPosition()
{
	vec3 currentPos = getCoordinates();

	char buff[100];
	sprintf_s(buff, "\n\nlongitude: %.2f  latitude: %.2f  altitude: %.2f", currentPos.x, currentPos.y, currentPos.z);
	InfoPrinter::GetInstance().PrintOnce(buff, POSITION_ID);
}

vec3 MapCamera::getCoordinates()
{
	return GeoUtils::GetCoordinatesOnMap(position);
}

void MapCamera::setCoordinates(vec3 v)
{
	position = GeoUtils::GetPositionOnMap(v.y, v.x);
	position.z = v.z;
}

void MapCamera::handleKeys(vec3 direction, vec3 left, float deltaTime)
{
	// Move down
	if (glfwGetKey(window, GLFW_KEY_LEFT_SHIFT ) == GLFW_PRESS){
		position += direction * deltaTime * speed;
	}
	// Move up
	if (glfwGetKey(window, GLFW_KEY_LEFT_CONTROL ) == GLFW_PRESS){
		position -= direction * deltaTime * speed;
	}
	// Strafe right
	if (glfwGetKey(window, GLFW_KEY_D ) == GLFW_PRESS){
		position -= left * deltaTime * speed;
	}
	// Strafe left
	if (glfwGetKey(window, GLFW_KEY_A ) == GLFW_PRESS){
		position += left * deltaTime * speed;
	}
	// Move forward
	if (glfwGetKey(window, GLFW_KEY_W ) == GLFW_PRESS){
		position += up * deltaTime * speed;
	}
	// Move backward
	if (glfwGetKey(window, GLFW_KEY_S ) == GLFW_PRESS){
		position -= up * deltaTime * speed;
	}
	// Zoom in
	if (glfwGetKey(window, GLFW_KEY_ENTER ) == GLFW_PRESS){
		fov -= 5.5f * deltaTime;
		fov = std::max(0.5f, std::min(145.0f, fov));
	}
	// Zoom out
	if (glfwGetKey(window, GLFW_KEY_BACKSPACE ) == GLFW_PRESS){
		fov += 5.5f * deltaTime;
		fov = std::max(0.5f, std::min(145.0f, fov));		
	}	
	// Reset fov
	if (glfwGetKey(window, GLFW_KEY_R ) == GLFW_PRESS){
		fov = 45.0f;		
	}	
	// View distance
	if (glfwGetKey(window, GLFW_KEY_L ) == GLFW_PRESS){
		viewDistance = 100.0f;
	}
	// View distance
	if (glfwGetKey(window, GLFW_KEY_K ) == GLFW_PRESS){
		viewDistance = 2.0f;
	}
}