#pragma once

#include <Windows.h>
#include "..\..\common\openglHelper.h"
#include "..\..\common\objectPool.h"
#include "human.h"

enum CameraType
{
	first_person,
	mouse_control,
	above_behind,
	perspective_view,
	preview_model,
};

bool overrideCondition(Human * human);

static void error_callback(int error, const char* description);

static void key_callback(GLFWwindow* window, int key, int scancode, int action, int mods);

void setupWindow();

mat4 setCamera(Human * my_human);
