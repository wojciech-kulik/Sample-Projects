#define _USE_MATH_DEFINES

#include "openglHelper.h"
#include <math.h>

using namespace glm;

float degtorad(int alfa)
{
	return alfa * (float)M_PI / 180.0f;
}

float degtorad(float alfa)
{
	return alfa * (float)M_PI / 180.0f;
}

float radtodeg(float alfa)
{
	return alfa * 180.0f / (float)M_PI ;
}

vec3 vec4ToVec3(vec4 v4)
{
	return vec3(v4.x, v4.y, v4.z);
}

bool isBetween(float value, float from, float to)
{
	return value >= from && value <= to;
}

bool isBetweeni(int value, int from, int to)
{
	return value >= from && value <= to;
}

void setVertexAttrib(int layout, GLuint buffer, GLint dimension)
{
	glEnableVertexAttribArray(layout);
	glBindBuffer(GL_ARRAY_BUFFER, buffer);
	glVertexAttribPointer(
		layout,           
		dimension,          // dimmension size
		GL_FLOAT,           // type
		GL_FALSE,           // normalized?
		0,                  // stride
		(void*)0            // array buffer offset
	);
}

void disableVertexAttrib(GLuint a)
{
	glDisableVertexAttribArray(a);
}

GLuint createBuffer(const GLvoid * data, GLsizeiptr size)
{
	GLuint buffer;
	glGenBuffers(1, &buffer);
	glBindBuffer(GL_ARRAY_BUFFER, buffer);
	glBufferData(GL_ARRAY_BUFFER, size, data, GL_STATIC_DRAW);

	return buffer;
}

GLuint createElementBuffer(const GLvoid * data, GLsizeiptr size)
{
	GLuint buffer;
	glGenBuffers(1, &buffer);
	glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, buffer);
	glBufferData(GL_ELEMENT_ARRAY_BUFFER, size, data, GL_STATIC_DRAW);

	return buffer;
}

GLuint createVAO()
{
	GLuint vao;
	glGenVertexArrays(1, &vao);
	return vao;
}

void replaceBuffer(GLuint buffer, const GLvoid * data, GLsizeiptr size)
{
	glBindBuffer(GL_ARRAY_BUFFER, buffer);
	glBufferData(GL_ARRAY_BUFFER, size, data, GL_STATIC_DRAW);
}

void updateBuffer(GLuint buffer, const GLvoid * data, GLsizeiptr size)
{
	glBindBuffer(GL_ARRAY_BUFFER, buffer);
	glBufferSubData(GL_ARRAY_BUFFER, 0, size, data);
}

void drawArr(GLenum mode, GLsizei count)
{
	glDrawArrays(mode, 0, count);
}

void drawArrWithColor(GLenum mode, GLsizei count, GLuint buffer, GLuint colorBuffer, GLint offset_loc, float offset_x, float offset_y)
{
	setVertexAttrib(0, buffer, 2);
	setVertexAttrib(1, colorBuffer, 4);
	glUniform2f(offset_loc, offset_x, offset_y);	
	drawArr(mode, count);
	disableVertexAttrib(0);
	disableVertexAttrib(1);
}

void drawArrWithColor3D(GLenum mode, GLsizei count, GLuint buffer, GLuint colorBuffer, GLint mvp_loc, mat4 & mvp, int colorDimension)
{
	setVertexAttrib(0, buffer, 3);
	setVertexAttrib(1, colorBuffer, colorDimension);

	glUniformMatrix4fv(mvp_loc, 1, GL_FALSE, &mvp[0][0]);
	drawArr(mode, count);

	disableVertexAttrib(0);
	disableVertexAttrib(1);
}

void createColorBuffer(GLuint * buffer, vec4 color, int count)
{
	GLfloat * col = new GLfloat[count * 4];

	int index_color = 0;
	for (int i = 0; i < count; i++)
	{
		col[index_color++] = color.x;
		col[index_color++] = color.y;
		col[index_color++] = color.z;
		col[index_color++] = color.w;
	}

	*buffer = createBuffer(col, count * 4 * sizeof(GLfloat));
	delete[] col;
}


int createEllipsoid(GLuint * buffer, GLuint * color_buffer, vec3 params, vec4 color)
{
	const int angle = 20;
	const int N = 360 / angle;

	int i,j, index = 0, index_color = 0;
	const int r = 1;

	GLfloat v[(N+1)*(N+1)*2*3];
	GLfloat col[(N+1)*(N+1)*2*4];
	
	for(i = 0; i <= N; i++)
	{
			for(j = 0; j <=N; j++)
			{				  
				v[index++] = params.x * cos(degtorad(i * angle)) * sin(degtorad(j * angle)) + params.x;
				v[index++] = params.y * sin(degtorad(i * angle)) * sin(degtorad(j * angle)) + params.y;
				v[index++] = params.z * cos(degtorad(j * angle)) + params.z;

				v[index++] = params.x * cos(degtorad((i + 1) * angle)) * sin(degtorad(j * angle)) + params.x;
				v[index++] = params.y * sin(degtorad((i + 1) * angle)) * sin(degtorad(j * angle)) + params.y;
				v[index++] = params.z * cos(degtorad(j * angle)) + params.z;


				col[index_color++] = color.x;
				col[index_color++] = color.y;
				col[index_color++] = color.z;
				col[index_color++] = color.w;

				col[index_color++] = color.x;
				col[index_color++] = color.y;
				col[index_color++] = color.z;
				col[index_color++] = color.w;
			}
	}

	*buffer = createBuffer(v, sizeof(v));
	*color_buffer = createBuffer(col, sizeof(col));
	return sizeof(v);
}

/*
int createSphere(GLuint * buffer, GLuint * color_buffer, vec3 params, vec4 color)
{
	const int sectors = 20;
	const int rings = 20;

	float const R = 1./(float)(rings-1);
    float const S = 1./(float)(sectors-1);
    int r, s;

	int index = 0, index_color = 0;

	GLfloat v[rings*sectors*3];
	GLfloat col[rings*sectors*4];
	
	for(r = 0; r < rings; r++) 
		for(s = 0; s < sectors; s++) 
		{
            v[index++] = sin( -M_PI_2 + M_PI * r * R );
            v[index++] = cos(2*M_PI * s * S) * sin( M_PI * r * R );
            v[index++] = sin(2*M_PI * s * S) * sin( M_PI * r * R );			  

			col[index_color++] = color.x;
			col[index_color++] = color.y;
			col[index_color++] = color.z;
			col[index_color++] = color.w;
		}

	*buffer = createBuffer(v, sizeof(v));
	*color_buffer = createBuffer(col, sizeof(col));
	return sizeof(v);
}*/

/*int createEllipsoid2(GLuint * buffer, GLuint * color_buffer, vec3 params, vec4 color)
{
	int i, j;
	const int lats = 20, longs = 20;
	int index = 0, index_color = 0;

	GLfloat v[(lats+1)*(longs+1)*4*3];
	GLfloat col[(lats+1)*(longs+1)*4*4];

    for(i = 0; i <= lats; i++)
    {
        float lat0 = M_PI * (-0.5f + (float) (i - 1) / lats);
        float z0  = sin(lat0);
        float zr0 = cos(lat0);
        
        float lat1 = M_PI * (-0.5f + (float) i / lats);
        float z1 = sin(lat1);
        float zr1 = cos(lat1);
        
        for(j = 0; j <= longs; j++)
        {
            float lng = 2 * M_PI * (float) (j - 1) / longs;
            float x = cos(lng);
            float y = sin(lng);

			v[index++] = x * zr0;
			v[index++] = y * zr0;
			v[index++] = z0;

			v[index++] = x * zr0 * params.x;
			v[index++] = y * zr0 * params.y;
			v[index++] = z0 * params.z; 

			v[index++] = x * zr1;
			v[index++] = y * zr1;
			v[index++] = z1; 

			v[index++] = x * zr1 * params.x;
			v[index++] = y * zr1 * params.y;
			v[index++] = z1 * params.z; 
            

			col[index_color++] = color.x;
			col[index_color++] = color.y;
			col[index_color++] = color.z;
			col[index_color++] = color.w;

			col[index_color++] = color.x;
			col[index_color++] = color.y;
			col[index_color++] = color.z;
			col[index_color++] = color.w;

			col[index_color++] = color.x;
			col[index_color++] = color.y;
			col[index_color++] = color.z;
			col[index_color++] = color.w;

			col[index_color++] = color.x;
			col[index_color++] = color.y;
			col[index_color++] = color.z;
			col[index_color++] = color.w;
        }
    }

	*buffer = createBuffer(v, sizeof(v));
	*color_buffer = createBuffer(col, sizeof(col));
	return sizeof(v);
}*/

int createCube(GLuint * buffer, GLuint * color_buffer, vec4 color)
{
	const GLfloat v[] = {
		-1.0f,-1.0f,-1.0f, // triangle 1 : begin
		-1.0f,-1.0f, 1.0f,
		-1.0f, 1.0f, 1.0f, // triangle 1 : end
		1.0f, 1.0f,-1.0f, // triangle 2 : begin
		-1.0f,-1.0f,-1.0f,
		-1.0f, 1.0f,-1.0f, // triangle 2 : end
		1.0f,-1.0f, 1.0f,
		-1.0f,-1.0f,-1.0f,
		1.0f,-1.0f,-1.0f,
		1.0f, 1.0f,-1.0f,
		1.0f,-1.0f,-1.0f,
		-1.0f,-1.0f,-1.0f,
		-1.0f,-1.0f,-1.0f,
		-1.0f, 1.0f, 1.0f,
		-1.0f, 1.0f,-1.0f,
		1.0f,-1.0f, 1.0f,
		-1.0f,-1.0f, 1.0f,
		-1.0f,-1.0f,-1.0f,
		-1.0f, 1.0f, 1.0f,
		-1.0f,-1.0f, 1.0f,
		1.0f,-1.0f, 1.0f,
		1.0f, 1.0f, 1.0f,
		1.0f,-1.0f,-1.0f,
		1.0f, 1.0f,-1.0f,
		1.0f,-1.0f,-1.0f,
		1.0f, 1.0f, 1.0f,
		1.0f,-1.0f, 1.0f,
		1.0f, 1.0f, 1.0f,
		1.0f, 1.0f,-1.0f,
		-1.0f, 1.0f,-1.0f,
		1.0f, 1.0f, 1.0f,
		-1.0f, 1.0f,-1.0f,
		-1.0f, 1.0f, 1.0f,
		1.0f, 1.0f, 1.0f,
		-1.0f, 1.0f, 1.0f,
		1.0f,-1.0f, 1.0f
	};

	GLfloat col[6 * 2 * 3 * 4];

	int index = 0;
	for (int i = 0; i < 6 * 2 * 3; i++)
	{
		col[index++] = color.x;
		col[index++] = color.y;
		col[index++] = color.z;
		col[index++] = color.w;
	}

	*buffer = createBuffer(v, sizeof(v));
	*color_buffer = createBuffer(col, sizeof(col));
	return sizeof(v);
}