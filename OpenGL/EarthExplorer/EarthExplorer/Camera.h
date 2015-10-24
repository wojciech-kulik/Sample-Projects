#pragma once

#include "Culling.h"

using namespace glm;

const int MIN_FPS = 30; //for auto LoD

class Camera
{
private:
	void limitOrientation();
	void showFPS(double fps);

protected:
	GLFWwindow * window;
	mat4 viewMatrix;
	mat4 projectionMatrix;
	Culling culling;

	vec3 position;
	float horizontalAngle;
	float verticalAngle;

	float speed; // units / second
	float mouseSpeed;
	float fov;
	float viewDistance;

	int LoD;
	bool autoLOD;

	void configureAutoLoD(double fps);
	void computeMVP();
	void UpdateOrientation();
	void UpdateMVP(vec3 & direction, vec3 & up);
	virtual void handleKeys(vec3 direction, vec3 left, float deltaTime);
	virtual void showPosition();

	virtual float getMaxHorizontalAngel();
	virtual float getMinHorizontalAngel();
	virtual float getMaxVerticalAngel();
	virtual float getMinVerticalAngel();

	bool fixedSystem;

	//in not fixed system
	vec3 up, forward, left;
	void rotateX(float angle);
	void rotateY(float angle);
	void rotateZ(float angle);

public:
	Camera(GLFWwindow * w);	
	mat4 getViewMatrix();
	mat4 getProjectionMatrix();
	mat4 getMVP();
	Culling & getCulling();

	vec2 getOrientation();
	vec3 getPosition();
	void setPosition(vec3 v);

	int getLOD();
	void setLOD(int value);
	bool getAutoLOD();
	void setAutoLOD(bool value);
	void setLODandShowInfo(double startTime);

	void resetCursor();
	void ToggleFixedSystem();
};