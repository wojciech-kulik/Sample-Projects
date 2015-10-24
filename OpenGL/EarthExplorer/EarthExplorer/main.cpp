#include "main.h"
#include "MapCamera.h"
#include "GlobeCamera.h"
#include "window.h"
#include "Terrain.h"
#include "InfoPrinter.h"
#include "GeoUtils.h"
#include <algorithm>

using namespace glm;
using namespace std;

GLFWwindow* window;
MapCamera * mapCamera;
GlobeCamera * globeCamera;
Camera * camera;
Terrain * terrain;

static void key_callback(GLFWwindow* window, int key, int scancode, int action, int mods)
{
	if (action == GLFW_PRESS)
	{
		switch(key)
		{
		case GLFW_KEY_ESCAPE:
			glfwSetWindowShouldClose(window, GL_TRUE);
			break;
		case GLFW_KEY_V:
			camera = (camera == mapCamera) ? (Camera*)globeCamera : (Camera*)mapCamera;
			terrain->SetCamera(camera);
			break;
		case GLFW_KEY_F:
			if (camera == globeCamera)
				camera->ToggleFixedSystem();
			break;
		case GLFW_KEY_1:
		case GLFW_KEY_2:
		case GLFW_KEY_3:
		case GLFW_KEY_4:
		case GLFW_KEY_5:
		case GLFW_KEY_6:
		case GLFW_KEY_7:
		case GLFW_KEY_8:
		case GLFW_KEY_9:
			camera->setAutoLOD(false);
			camera->setLOD(key - GLFW_KEY_1);
			break;
		case GLFW_KEY_0:
			camera->setAutoLOD(true);
			break;
		}
	}
}

void parseArgs(int argcount, char* args[])
{
	if (argcount == 1)
	{
		cout << "Please set -mdir parameter (directory with hgt files)" << endl;
		exit(EXIT_FAILURE);
	}

	for(int i = 1; i < argcount; i++)
	{
		if (strcmp(args[i], "-mdir") == 0)
			terrain->LoadDirectory(args[++i]);
		else
			terrain->LoadPart(args[i]);
	}
}

void centerMap()
{
	Bounds boundsPlus1 = terrain->GetMapBounds(1, 1);
	Bounds bounds = terrain->GetMapBounds();
	float alt = std::max(boundsPlus1.top - boundsPlus1.bottom, (boundsPlus1.right - boundsPlus1.left));
	mapCamera->setPosition(vec3((bounds.right + bounds.left) / 2.0f, (bounds.top + bounds.bottom) / 2.0f, alt));
}

int main(int argcount, char* args[])
{
	srand((unsigned)time(NULL));
	int width, height;
	double nowTime, startTime, waitTime, frameTime = 1000.0 / MAX_FPS;	

	//setup window
	setupWindow(&window);	
	glfwSetKeyCallback(window, key_callback);

	//set camera
	mapCamera = new MapCamera(window);
	globeCamera = new GlobeCamera(window);
	camera = globeCamera;

	//load terrain
	terrain = new Terrain();
	terrain->SetCamera(camera);
	parseArgs(argcount, args);

	//set camera position
	TerrainPart& tp = terrain->GetTerrainPart(0);
	globeCamera->setPosition(GeoUtils::GetPositionOnGlobe((float)tp.latitude, (float)tp.longitude, (float)tp.maxAltitude * 15.0f));
	centerMap();

	while (!glfwWindowShouldClose(window))
	{		
		startTime = glfwGetTime();			

		//prepare window
		glClear( GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT );				
		glfwGetWindowSize(window, &width, &height);	
		glViewport(0, 0, width, height);
	
		//render terrain
		terrain->Render(1.0f);
		glfwSwapBuffers(window);

		//receive events		
		glfwPollEvents();

		//FPS CONTROL
		nowTime = glfwGetTime();
		waitTime = frameTime - (nowTime - startTime) * 1000.0;
		if (waitTime > 0)
			Sleep((DWORD)waitTime);	

		camera->setLODandShowInfo(startTime);
		InfoPrinter::GetInstance().WaitAndPrint();
	}

	delete terrain;
	delete mapCamera;
	delete globeCamera;
	glfwDestroyWindow(window);
	glfwTerminate();
	exit(EXIT_SUCCESS);
}