#pragma once

#include "../../common/openglHelper.h"

class TestGlobe
{
protected:
	GLuint buffer;
	GLuint shader;
	GLuint mvp_loc;
public:
	TestGlobe(char * vertexShaderPath, char * fragmentShaderPath);
	~TestGlobe();
	void LoadData();
	void Render(mat4 mvp);
};

