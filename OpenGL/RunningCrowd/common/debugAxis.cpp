#include "debugAxis.h"

DebugAxis::DebugAxis(char * vertexShaderPath, char * fragmentShaderPath)
{
	shader = LoadShaders( vertexShaderPath, fragmentShaderPath);
	mvp_loc = glGetUniformLocation(shader, "MVP");
	PrepareBuffer();
}
	
DebugAxis::~DebugAxis()
{
	glDeleteProgram(shader);
	glDeleteBuffers(1, &buffer);
	glDeleteBuffers(1, &color_buffer);
}
	
void DebugAxis::PrepareBuffer()
{
	GLfloat v[3*2*3] = {	0.0f, 0.0f, 0.0f, //x-axis
							1.0f, 0.0f, 0.0f,

							0.0f, 0.0f, 0.0f, //y-axis
							0.0f, 1.0f, 0.0f,

							0.0f, 0.0f, 0.0f,
							0.0f, 0.0f, 1.0f, //z-axis
						};

	GLfloat c[3*2*3] = {	1.0f, 0.0f, 0.0f, //x-axis
							1.0f, 0.0f, 0.0f,

							0.0f, 0.0f, 1.0f, //y-axis blue
							0.0f, 0.0f, 1.0f,

							0.0f, 1.0f, 0.0f,
							0.0f, 1.0f, 0.0f, //z-axis green
						};

	buffer = createBuffer(v, sizeof(v));
	color_buffer = createBuffer(c, sizeof(c));
}

void DebugAxis::Render(mat4 mvp, vec3 pos, float scale)
{
	glUseProgram(shader);
	glLineWidth(5.0f);
	mvp = mvp * glm::translate(pos) * glm::scale(vec3(scale));
	drawArrWithColor3D(GL_LINES, 3*2, buffer, color_buffer, mvp_loc, mvp, 3);
	glLineWidth(1.0f);
}