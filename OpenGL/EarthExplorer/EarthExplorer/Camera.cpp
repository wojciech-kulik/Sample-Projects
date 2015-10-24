#include "Camera.h"
#include "TerrainPart.h"
#include "InfoPrinter.h"
#include "InfoIds.h"
#include <algorithm>

const double LOD_REFRESH_TIME = 0.1; //seconds

Camera::Camera(GLFWwindow * w)
	: window(w), 
	  position(0.0f), horizontalAngle(PI), verticalAngle(-PI / 2.0f), 
	  fov(45.0f), viewDistance(1.2f), speed(0.1f), mouseSpeed(0.005f), 
	  LoD(0), autoLOD(true)
{
	up = vec3(0.0f, 1.0f, 0.0f);
	forward  = vec3(0.0f, 0.0f, -1.0f);
	left = vec3(1.0f, 0.0f, 0.0f);
	fixedSystem = true;
}

float Camera::getMaxHorizontalAngel()
{
	return 2.0f * PI;
}

float Camera::getMinHorizontalAngel()
{
	return 0.0f;
}

float Camera::getMaxVerticalAngel()
{
	return PI / 2.0f;
}

float Camera::getMinVerticalAngel()
{
	return -PI / 2.0f;
}

int Camera::getLOD()
{
	return LoD;
}

void Camera::setLOD(int value)
{
	LoD = value;
}

bool Camera::getAutoLOD()
{
	return autoLOD;
}

void Camera::setAutoLOD(bool value)
{
	if (value) LoD = 0;
	autoLOD = value;
}

mat4 Camera::getProjectionMatrix()
{
	return projectionMatrix;
}

mat4 Camera::getViewMatrix()
{
	return viewMatrix;
}

Culling & Camera::getCulling()
{
	return culling;
}

vec2 Camera::getOrientation()
{
	return vec2(horizontalAngle, verticalAngle);
}

vec3 Camera::getPosition()
{
	return position; 
}

void Camera::setPosition(vec3 v)
{
	position = v;
}

mat4 Camera::getMVP()
{
	computeMVP();	
	return projectionMatrix * viewMatrix;
}

void Camera::resetCursor()
{
	// Get window size
	int width, height;
	glfwGetWindowSize(window, &width, &height);

	// Reset mouse position for next frame
	glfwSetCursorPos(window, width/2, height/2);
}

void Camera::limitOrientation()
{
	// Keep horizontal angel between [0; 360]
	if (horizontalAngle < 0.0f)
		horizontalAngle = 2.0f * PI + horizontalAngle;
	if (horizontalAngle > 2.0f * PI)
		horizontalAngle = horizontalAngle - 2.0f * PI;

	// Limit vertical angel
	if (verticalAngle < getMinVerticalAngel())
		verticalAngle = getMinVerticalAngel();
	if (verticalAngle > getMaxVerticalAngel())
		verticalAngle = getMaxVerticalAngel();

	// Limit horizontal angel
	if (horizontalAngle < getMinHorizontalAngel())
		horizontalAngle = getMinHorizontalAngel();
	if (horizontalAngle > getMaxHorizontalAngel())
		horizontalAngle = getMaxHorizontalAngel();
}

void Camera::handleKeys(vec3 direction, vec3 left, float deltaTime)
{
	// Move forward
	if (glfwGetKey(window, GLFW_KEY_W ) == GLFW_PRESS){
		position += direction * deltaTime * speed;
	}
	// Move backward
	if (glfwGetKey(window, GLFW_KEY_S ) == GLFW_PRESS){
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
	// Move down
	if (glfwGetKey(window, GLFW_KEY_LEFT_SHIFT ) == GLFW_PRESS){
		position += up * deltaTime * speed;
	}
	// Move up
	if (glfwGetKey(window, GLFW_KEY_LEFT_CONTROL ) == GLFW_PRESS){
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
		viewDistance = 1.2f;
	}
	// View distance
	if (glfwGetKey(window, GLFW_KEY_K ) == GLFW_PRESS){
		viewDistance = 0.2f;
	}

	if (!fixedSystem)
	{
		// Roll left
		if (glfwGetKey(window, GLFW_KEY_Q ) == GLFW_PRESS){
			rotateZ(2.0f);
		}
		// Roll right
		if (glfwGetKey(window, GLFW_KEY_E ) == GLFW_PRESS){
			rotateZ(-2.0f);
		}
	}
}

void Camera::ToggleFixedSystem()
{
	fixedSystem = !fixedSystem;
}

void Camera::UpdateOrientation()
{
	// Get mouse position
	double xpos, ypos;
	glfwGetCursorPos(window, &xpos, &ypos);

	// Get window size
	int width, height;
	glfwGetWindowSize(window, &width, &height);

	// Reset mouse position for next frame
	glfwSetCursorPos(window, width/2, height/2);

	// Compute new orientation
	if (fixedSystem)
	{
		horizontalAngle += mouseSpeed * float( width/2 - xpos );
		verticalAngle   += mouseSpeed * float( height/2 - ypos );
		limitOrientation();
	}
	else
	{
		horizontalAngle = mouseSpeed * float( width/2 - xpos );
		verticalAngle   = mouseSpeed * float( height/2 - ypos );
	}
}

void Camera::UpdateMVP(vec3 & direction, vec3 & up)
{
	// Projection matrix
	projectionMatrix = glm::perspective(fov, 4.0f / 3.0f, 0.00001f, 100.0f);

	// Camera matrix
	viewMatrix = glm::lookAt(
					position,           // Camera is here
					position+direction, // and looks here : at the same position, plus "direction"
					up                  // Head is up (set to 0,-1,0 to look upside-down)
				);

	// Update culling data
	culling.ExtractFrustum(glm::perspective(fov, 4.0f / 3.0f, 0.00001f, viewDistance) * viewMatrix);
}

void Camera::rotateX(float angle)
{
	//see: http://jerome.jouvie.free.fr/opengl-tutorials/Tutorial26.php
    /*double radians = degtorad(angle);
    vec3 v1 = up * (float)-sin(radians); 
    vec3 v2 = forward * (float)cos(radians); 
    forward = glm::normalize(v1 + v2);*/

	forward = glm::normalize(glm::rotate(forward, angle, left));
    up = glm::normalize(glm::cross(forward, left));
}

void Camera::rotateY(float angle)
{
    /*double radians = degtorad(angle);
    vec3 v1 = left * (float)sin(radians);     
    vec3 v2 = forward * (float)cos(radians);
    forward = glm::normalize(v1 + v2);*/

	forward = glm::normalize(glm::rotate(forward, angle, up));
    left = glm::normalize(glm::cross(up, forward));
}

void Camera::rotateZ(float angle)
{
	/*double radians = degtorad(angle);
	vec3 v1 = left * (float)cos(radians);
	vec3 v2 = up * (float)-sin(radians); 
	left = glm::normalize(v1 + v2);*/

	left = glm::normalize(glm::rotate(left, -angle, forward));
	up = glm::normalize(glm::cross(forward, left));
}

void Camera::computeMVP()
{
	// glfwGetTime is called only once, the first time this function is called
	static double lastTime = glfwGetTime();

	// Compute time difference between current and last frame
	double currentTime = glfwGetTime();
	float deltaTime = float(currentTime - lastTime);

	UpdateOrientation();

	if (!fixedSystem)
	{
		rotateY(radtodeg(horizontalAngle));
		rotateX(radtodeg(-verticalAngle));

		handleKeys(forward, left, deltaTime);
		UpdateMVP(forward, up);
	}
	else
	{
		// Direction : Spherical coordinates to Cartesian coordinates conversion
		vec3 direction_vec(	
			cos(verticalAngle) * sin(horizontalAngle), 
			sin(verticalAngle),
			cos(verticalAngle) * cos(horizontalAngle)
		);

		// Right vector
		vec3 right_vec = vec3(
			sin(horizontalAngle - PI/2.0f), 
			0,
			cos(horizontalAngle - PI/2.0f)
		);	

		// Up vector
		vec3 up_vec = glm::cross(right_vec, direction_vec);

		handleKeys(direction_vec, -right_vec, deltaTime);
		UpdateMVP(direction_vec, up_vec);
	}

	// Save time
	lastTime = currentTime;
}

void Camera::showFPS(double fps)
{
	char buff[100];
	sprintf_s(buff, "\n\nFPS: %.2f\nLoD: %d (auto = %s)\nView distance: %.2f", fps, LoD + 1, autoLOD ? "YES" : "NO", viewDistance);
	InfoPrinter::GetInstance().PrintOnce(buff, FPS_ID);
}

void Camera::showPosition()
{
	vec3 currentPos = getPosition();
	vec2 oRad = getOrientation();
	vec2 oDeg = vec2(radtodeg(oRad.x), radtodeg(oRad.y)); 

	char buff[200];
	if (fixedSystem)
		sprintf_s(buff, "\n\nX: %.2f  Y: %.2f  Z: %.2f\nHorizontalAngel: %.1f\nVerticalAngel: %.1f", currentPos.x, currentPos.y, currentPos.z, oDeg.x, oDeg.y);
	else
		sprintf_s(buff, "\n\nX: %.2f  Y: %.2f  Z: %.2f", currentPos.x, currentPos.y, currentPos.z);
	InfoPrinter::GetInstance().PrintOnce(buff, POSITION_ID);
}

void Camera::configureAutoLoD(double fps)
{
	if (autoLOD)
	{
		if (fps <= MIN_FPS)
			LoD = std::min(DETAIL_LEVELS_COUNT - 1, LoD + 1);
		else if (fps > MIN_FPS + 5)
			LoD = std::max(0, LoD - 1);
	}
}

void Camera::setLODandShowInfo(double startTime)
{
	static int fps_counter = 0;
	static double lastTime = glfwGetTime();
	double currentTime = glfwGetTime();

	fps_counter++;

	if (currentTime - lastTime >= LOD_REFRESH_TIME)
	{		
		double fps = fps_counter / (currentTime - lastTime);
		showFPS(fps);
		showPosition();

		configureAutoLoD(fps);
		lastTime = currentTime;
		fps_counter = 0;
	}
}