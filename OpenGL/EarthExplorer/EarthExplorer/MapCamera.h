#include "Camera.h"

using namespace glm;

class MapCamera : public Camera
{
protected:
	virtual void showPosition();
	virtual void handleKeys(vec3 direction, vec3 left, float deltaTime);

	virtual float getMaxHorizontalAngel();
	virtual float getMinHorizontalAngel();
	virtual float getMaxVerticalAngel();
	virtual float getMinVerticalAngel();

public:
	MapCamera(GLFWwindow * w);

	void setCoordinates(vec3 v);
	vec3 getCoordinates();
};