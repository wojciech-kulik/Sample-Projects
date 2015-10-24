#include "main.h"

#define GLFW_EXPOSE_NATIVE_WIN32
#define GLFW_EXPOSE_NATIVE_WGL
#include <GLFW\glfw3native.h>

//#define PREVIEW_MODEL

using namespace glm;
using namespace std;

GLuint programID;
GLFWwindow* window;
GLuint MVP_LOC;

int selectedHuman = 0;
const int maxHumans = 30;	
ObjectPool<Human, maxHumans> humans(&overrideCondition);

#ifdef PREVIEW_MODEL
CameraType cameraType = preview_model;
#else
CameraType cameraType = first_person;
#endif





bool overrideCondition(Human * human)
{
	return !human->IsActive();
}

static void error_callback(int error, const char* description)
{
	fputs(description, stderr);
}

static void key_callback(GLFWwindow* window, int key, int scancode, int action, int mods)
{
	if (action == GLFW_PRESS)
	{
		switch(key)
		{
		case GLFW_KEY_Q:
		case GLFW_KEY_ESCAPE:
			glfwSetWindowShouldClose(window, GL_TRUE);
			break;
		case GLFW_KEY_V:
			#ifndef PREVIEW_MODEL
				cameraType = (CameraType)((((int)cameraType) + 1) % 4);
			#endif
			break;
		case GLFW_KEY_C:
			if (humans[selectedHuman] != NULL)
				humans[selectedHuman]->SetActive(false);

			if (selectedHuman + 1 == maxHumans || humans[selectedHuman + 1] == NULL)
				selectedHuman = 0;
			else
				selectedHuman++;

			if (humans[selectedHuman] != NULL)
				humans[selectedHuman]->SetActive(true);

			break;
		}
	}
}

void setupWindow()
{
	glfwSetErrorCallback(error_callback);
	if (!glfwInit())
		exit(EXIT_FAILURE);

	glfwWindowHint(GLFW_SAMPLES, 16);
	window = glfwCreateWindow(1024, 768, "Crowd", NULL, NULL);
	if (!window)
	{
		glfwTerminate();
		exit(EXIT_FAILURE);
	}

	glfwMakeContextCurrent(window);
	glfwSetKeyCallback(window, key_callback);

	glewExperimental = true;
	if (glewInit() != GLEW_OK) 
	{
		fprintf(stderr, "Failed to initialize GLEW\n");
		glfwTerminate();
		exit(EXIT_FAILURE);
	}

	programID = LoadShaders( "Shaders/VertexShader.vertexshader", "Shaders/FragmentShader.fragmentshader" );
	MVP_LOC = glGetUniformLocation(programID, "MVP");

	glEnable(GL_BLEND);
	glBlendFunc(GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA);
	glEnable(GL_DEPTH_TEST);
	// Accept fragment if it closer to the camera than the former one
	glDepthFunc(GL_LESS);

	glClearColor(0.2f, 0.2f, 0.2f, 0.0f);
	glUseProgram(programID);
}

mat4 setCamera()
{
	mat4 view;
	float z, rotation;
	glm::detail::tvec3<float> * look_at;
	Human * my_human = humans[selectedHuman];

	POINT point;
	GetCursorPos(&point);
	ScreenToClient(glfwGetWin32Window(window), &point);

	if (my_human != NULL)
	{
		z = -0.5f - my_human->GetMovementOffset();
		rotation = my_human->GetDirectionAngle() - 90.0f;
	}
	else
	{
		return glm::lookAt(
					glm::rotate(vec3(1.4f, 2.5f, 8.0f), (point.x - 1024.0f / 2.0f), vec3(0.0f, 1.0f, 0.0f)),
					vec3(0.0f, 1.0f, 0.0f),	
					vec3(0,1,0)
				);
	}



	switch(cameraType)
	{
	case mouse_control:
	case first_person:					
		if (cameraType == mouse_control)
			look_at = &glm::rotate(vec3((point.x - 1024.0f / 2.0f) / 20.0f, -((point.y - 768.0f / 2.0f) / 20.0f) + my_human->GetYOffset(),z - 10.0f), rotation, vec3(0.0f, 1.0f, 0.0f));
		else 
			look_at = &glm::rotate(vec3(0.5f, -0.3f + my_human->GetYOffset(), z - 10.0f), rotation, vec3(0.0f, 1.0f, 0.0f));

		view = glm::lookAt(
			glm::rotate(vec3(0.5f, 1.5f + my_human->GetYOffset(), z + 0.1f), rotation, vec3(0.0f, 1.0f, 0.0f)),
			*look_at,	
			vec3(0,1,0)
		);

		break;
	case above_behind:
		view = glm::lookAt(
			glm::rotate(vec3(0.5f,  5.5f, z + 7.0f),  rotation, vec3(0.0f, 1.0f, 0.0f)),
			glm::rotate(vec3(0.5f, -5.3f, z - 10.0f), rotation, vec3(0.0f, 1.0f, 0.0f)),	
			vec3(0,1,0)
		);
		break;
	case perspective_view:
		view = glm::lookAt(
			vec3(1.4f, 30.5f, 30.0f),
			vec3(0.0f, 20.0f, 0.0f),	
			vec3(0,1,0)
		);
		break;
	case preview_model:
		view = glm::lookAt(
			glm::rotate(vec3(1.4f, 2.5f, 8.0f), (point.x - 1024.0f / 2.0f), vec3(0.0f, 1.0f, 0.0f)),
			vec3(0.0f, 1.0f, 0.0f),	
			vec3(0,1,0)
		);
		break;
	}

	return view;
}

int main(void)
{
	srand(time(NULL));
	int width, height;
	double startTime, waitTime, frameTime = 1000.0 / 80.0;	

	setupWindow();	
	Human::InitBuffers();
	bool done = false;
	#ifdef PREVIEW_MODEL
	Human * my_human = new Human(MVP_LOC, rand() % 90 + 45, 0.0f, (HumanColor)(rand() % 4));
	#else
	Human * my_human = new Human(MVP_LOC, rand() % 90 + 45, (rand() % 100 + 20.0f) / 1000.0f, (HumanColor)(rand() % 4));
	#endif
	my_human->SetActive(true);
	humans.Add(my_human); 

	// Projection matrix : 80° Field of View, 4:3 ratio, display range : 0.1 unit <-> 100 units
	mat4 projection = glm::perspective(80.0f, 4.0f / 3.0f, 0.1f, 1000.0f);
	mat4 view;

	int counter = 0;
	int next_human = 100;
	while (!glfwWindowShouldClose(window))
	{		
		startTime = glfwGetTime();			

		glClear( GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT );				
		glfwGetWindowSize(window, &width, &height);	
		glViewport(0, 0, width, height);
			
		view = setCamera();

		for(int i = 0; i < maxHumans; i++)
			if (humans[i] != NULL)
				humans[i]->Draw(projection, view, cameraType != first_person && cameraType != mouse_control, cameraType == perspective_view);

		#ifndef PREVIEW_MODEL
		if (++counter == next_human)
		{		
			next_human = rand() % 100 + 60;
			counter = 0;
			humans.Add(new Human(MVP_LOC, rand() % 90 + 45,
								(rand() % 100 + 20.0f) / 1000.0f, 
								(HumanColor)(rand() % 4))); 
		}
		#endif

		glfwSwapBuffers(window);
		glfwPollEvents();

		waitTime = frameTime - (glfwGetTime() - startTime) * 1000.0;		
		if (waitTime > 0)
			Sleep(waitTime);		
	}

	Human::DestroyBuffers();
	glfwDestroyWindow(window);
	glfwTerminate();
	exit(EXIT_SUCCESS);
}