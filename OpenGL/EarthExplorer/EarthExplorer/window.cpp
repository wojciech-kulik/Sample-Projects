#include "window.h"

const int WINDOW_WIDTH = 1024;
const int WINDOW_HEIGHT = 768;

static void error_callback(int error, const char* description)
{
	printf(description);
	printf("\n");
}

void setupWindow(GLFWwindow ** window)
{
	//initialize glfw
	glfwSetErrorCallback(error_callback);
	if (!glfwInit())
		exit(EXIT_FAILURE);

	//create window
	//glfwWindowHint(GLFW_SAMPLES, 4);
	*window = glfwCreateWindow(WINDOW_WIDTH, WINDOW_HEIGHT, "EarthExplorer", NULL, NULL);
	if (!*window)
	{
		glfwTerminate();
		exit(EXIT_FAILURE);
	}
	glfwSetWindowPos(*window, 1500 - WINDOW_WIDTH, 30);

	//configure glfw
	glfwMakeContextCurrent(*window);
	glfwSetCursorPos(*window, WINDOW_WIDTH/2, WINDOW_HEIGHT/2);

	//initialize glew
	glewExperimental = true;
	if (glewInit() != GLEW_OK) 
	{
		fprintf(stderr, "Failed to initialize GLEW\n");
		glfwTerminate();
		exit(EXIT_FAILURE);
	}

	//blending
	glEnable(GL_BLEND);
	glBlendFunc(GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA);

	//depth test
	glEnable(GL_DEPTH_TEST);	
	glDepthFunc(GL_LESS); // Accept fragment if it closer to the camera than the former one

	//hide cursor
	glfwSetInputMode(*window, GLFW_CURSOR, GLFW_CURSOR_HIDDEN);

	glEnable(GL_CULL_FACE);
	glCullFace(GL_FRONT);
	//glDepthMask(GL_TRUE);

	glClearColor(0.1f, 0.1f, 0.1f, 0.0f);
}