#pragma once

#include "openglHelper.h"

class DebugAxis
{
protected:
	GLuint buffer, color_buffer;
	GLuint shader;
	GLuint mvp_loc;

	void PrepareBuffer();
public:
	DebugAxis(char * vertexShaderPath, char * fragmentShaderPath);
	~DebugAxis();
	void Render(mat4 mvp, vec3 pos, float scale = 1.0f);
};