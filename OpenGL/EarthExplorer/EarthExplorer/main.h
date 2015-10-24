#pragma once

#include <Windows.h>
#include "..\common\openglHelper.h"

const double MAX_FPS = 70.0;


static void error_callback(int error, const char* description);

static void key_callback(GLFWwindow* window, int key, int scancode, int action, int mods);

void setupWindow();

void showInfo(double startTime);