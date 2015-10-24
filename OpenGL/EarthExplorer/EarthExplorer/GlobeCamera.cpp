#include "GlobeCamera.h"

GlobeCamera::GlobeCamera(GLFWwindow * w) : Camera(w)
{
	verticalAngle = 0.0f;
	horizontalAngle = 0.75f * 2.0f * PI;
}