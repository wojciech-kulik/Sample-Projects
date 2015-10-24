#include "TestGlobe.h"
#include <iostream>
#include <fstream>
#include <string>

using namespace std;


TestGlobe::TestGlobe(char * vertexShaderPath, char * fragmentShaderPath)
{
	LoadData();
	shader = LoadShaders( vertexShaderPath, fragmentShaderPath);
	mvp_loc = glGetUniformLocation(shader, "MVP");
}

TestGlobe::~TestGlobe()
{
	glDeleteProgram(shader);
	glDeleteBuffers(1, &buffer);
}

void TestGlobe::LoadData()
{
	fstream f;
	f.open("airports.txt", ios::in);
	
	if (f.good())
	{	
		GLfloat data[15326];

		int index = 0;
		while (index < 15326)
		{
			f >> data[index++];
		}

		buffer = createBuffer(data, sizeof(data));

		f.close();	
	}
}

void TestGlobe::Render(mat4 mvp)
{
	glUseProgram(shader);
	setVertexAttrib(0, buffer, 2);
	glUniformMatrix4fv(mvp_loc, 1, GL_FALSE, &mvp[0][0]);	
	drawArr(GL_POINTS, 15326);
	disableVertexAttrib(0);
}