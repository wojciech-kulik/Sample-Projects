#include <gl\glew.h>
#include <GLFW\glfw3.h>
#include <glm\glm.hpp>
#include <glm\ext.hpp>

#include "../../common/shader.hpp"


#ifndef OPENGLHELPER_H
#define OPENGLHELPER_H

	using namespace glm;

	const float PI	= 3.14159f;

	struct RectStruct
	{
		RectStruct(float _leftTopX,		float _leftTopY, 
			 float _rightTopX,		float _rightTopY, 
			 float _leftBottomX,	float _leftBottomY,
			 float _rightBottomX,	float _rightBottomY)
		{
			leftTopX = _leftTopX;
			leftTopY = _leftTopY;

			rightTopX = _rightTopX;
			rightTopY = _rightTopY;

			leftBottomX = _leftBottomX;
			leftBottomY = _leftBottomY;

			rightBottomX = _rightBottomX;
			rightBottomY = _rightBottomY;
		}

		float leftTopX,		leftTopY; 
		float rightTopX,	rightTopY;
		float leftBottomX,	leftBottomY;
		float rightBottomX, rightBottomY;
	};

	float degtorad(int alfa);

	float degtorad(float alfa);

	float radtodeg(float a);

	vec3 vec4ToVec3(vec4 v4);

	bool isBetween(float value, float from, float to);

	bool isBetweeni(int value, int from, int to);

	void setVertexAttrib(int layout, GLuint buffer, GLint dimension);

	void disableVertexAttrib(GLuint a);

	GLuint createBuffer(const GLvoid * data, GLsizeiptr size);

	GLuint createElementBuffer(const GLvoid * data, GLsizeiptr size);

	GLuint createVAO();

	void updateBuffer(GLuint buffer, const GLvoid * data, GLsizeiptr size);

	void replaceBuffer(GLuint buffer, const GLvoid * data, GLsizeiptr size);

	void drawArr(GLenum mode, GLsizei count);

	void drawArrWithColor(GLenum mode, GLsizei count, GLuint buffer, GLuint colorBuffer, GLint offset_loc, float offset_x, float offset_y);

	void drawArrWithColor3D(GLenum mode, GLsizei count, GLuint buffer, GLuint colorBuffer, GLint mvp_loc, mat4 & mvp, int colorDimension = 4);

	int createEllipsoid(GLuint * buffer, GLuint * color_buffer, vec3 params, vec4 color);

	int createCube(GLuint * buffer, GLuint * color_buffer, vec4 color);

	void createColorBuffer(GLuint * buffer, vec4 color, int count);

#endif