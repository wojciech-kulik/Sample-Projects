#include "TerrainPart.h"
#include "GeoUtils.h"
#include <iostream>
#include <sstream>
#include <fstream>
#include <algorithm>

using namespace std;

GLuint * TerrainPart::DETAILS_LEVEL;
GLuint TerrainPart::DETAILS_COUNT[DETAIL_LEVELS_COUNT];


int TerrainPart::NumberOfTriangles(int levelOfDetails)
{
	return DETAILS_COUNT[levelOfDetails] / 2 - 2;
}

int TerrainPart::GetDetailsCount(int level)
{
	if (level == 0)
		return (MATRIX_SIZE - 1) * (MATRIX_SIZE + 1) * 2;

	int add = (MATRIX_SIZE - 1) % (level * LOD_STEP) == 0 ? 0 : 1; //to avoid part resize
	int result = (MATRIX_SIZE - 1) * (MATRIX_SIZE / (level*LOD_STEP) + 2 + add) * 2;
	return result;
}

void TerrainPart::InitLevelOfDetails()
{
	DETAILS_LEVEL = new GLuint[DETAIL_LEVELS_COUNT];

	for (int i = 0; i < DETAIL_LEVELS_COUNT; i++)
	{		
		int last, index = 0;
		DETAILS_COUNT[i] = GetDetailsCount(i);
		GLuint * indices = new GLuint[DETAILS_COUNT[i]];

		//prepare indices
		for (int r = 0; r < MATRIX_SIZE - 1; ++r) 
		{
			indices[index++] = r * MATRIX_SIZE;
			for (int c = 0; c < MATRIX_SIZE; ++c) 
				if ((i == 0) || (c % (i * LOD_STEP) == 0) || (c == MATRIX_SIZE - 1)) //last condition is to avoid part resize
				{
					indices[index++] = r * MATRIX_SIZE + c;
					indices[index++] = last = (r + 1) * MATRIX_SIZE + c;
				}
			indices[index++] = last;
		}

		//create buffer
		DETAILS_LEVEL[i] = createElementBuffer(indices, DETAILS_COUNT[i] * sizeof(GLuint));

		//delete array of indices
		delete[] indices;
	}
}

void TerrainPart::DestroyLevelOfDetails()
{
	glDeleteBuffers(DETAIL_LEVELS_COUNT, DETAILS_LEVEL);
	delete[] DETAILS_LEVEL;
}

TerrainPart::TerrainPart(string path) 
	: maxAltitude(0)
{
	int i = -1, dotIndex = -1;

	for (i = path.length(); i >= 0; i--)
		if (path[i] == '\\')
			break;
		else if (dotIndex == -1 && path[i] == '.')
			dotIndex = i;

	string fileName = path.substr(i + 1, dotIndex - i - 1);

	if (fileName == "")
		return;

	//check if on south
	int north = 1;
	if(fileName[0] == 's' || fileName[0] == 'S')
		north = -1;

	//check in on west
	int east = 1;
	if (fileName[3] == 'w' || fileName[3] == 'W')
		east = -1;

	//convert to coordinates
	latitude = north * atoi(fileName.substr(1, 2).c_str());
	longitude = east * atoi(fileName.substr(4, 3).c_str());

	LoadData(path);
}

TerrainPart::TerrainPart(short latitude, short longitude) 
	: longitude(longitude), latitude(latitude), maxAltitude(0)
{
	char lat = latitude >= 0 ? 'n' : 's';
	char lon = longitude >= 0 ? 'e' : 'w';

	stringstream ss;
	ss << "..\\hgt\\" << lat << (abs(latitude) < 10 ? "0" : "") << abs(latitude) 
	   << lon << (abs(longitude) < 100 ? (abs(longitude) < 10 ? "00" : "0") : "") << abs(longitude) << ".hgt";

	LoadData(ss.str());
}

TerrainPart::~TerrainPart(void)
{
	if (_buffer != 0)
		glDeleteBuffers(1, &_buffer);
	if (_vao != 0)
		glDeleteBuffers(1, &_vao);
}

void TerrainPart::SetupBuffer(GLfloat * data)
{
	//create buffer
	_buffer = createBuffer(data, MATRIX_SIZE * MATRIX_SIZE * 3 * sizeof(GLfloat));

	//create VAO
	_vao = createVAO();

	//bind VAO
	glBindVertexArray(_vao);

	//setup VAO
	glBindBuffer(GL_ARRAY_BUFFER, _buffer);
	glEnableVertexAttribArray(0);
	glVertexAttribPointer(
		0,           
		3,					// dimmension size
		GL_FLOAT,           // type
		GL_FALSE,           // normalized?
		0,                  // stride
		(void*)0            // array buffer offset
	);

	glBindVertexArray(0);
}

void TerrainPart::LoadData(string hgtFilePath)
{
	fstream f;
	f.open(hgtFilePath.c_str(), ios::in | ios::binary);
	
	if (f.good())
	{
		printf("Loading %s\n", hgtFilePath.c_str());

		const int data_bytes_count = MATRIX_SIZE * MATRIX_SIZE * 2;
		char * buff = new char[data_bytes_count];
		f.read(buff, data_bytes_count);
		f.close();
	
		GLfloat * data = new GLfloat[MATRIX_SIZE * MATRIX_SIZE * 3];

		//prepare array
		int c;
		maxAltitude = 0;
		for(int r = 0; r < MATRIX_SIZE; r++)
		{		
			for(c = 0; c < MATRIX_SIZE; c++)						
			{
				int index = r*MATRIX_SIZE + c;
				data[3*index + 0] = (float)c;
				data[3*index + 1] = (float)r;
				data[3*index + 2] = (float)((buff[2*index] << 8) | (0x00FF & buff[2*index + 1]));

				//no data - set nearby height
				if (!isBetween(data[3*index + 2], -500.0f, 9000.0f)) 
				{
					if (index > 0)
					{
						if (c == 0)
							data[3*index + 2] = data[3*((r-1)*MATRIX_SIZE + c) + 2];
						else
							data[3*index + 2] = data[3*index - 1];
					}
					else 
					{
						data[3*index + 2] = 0;
					}
				}

				if (data[3*index + 2] > maxAltitude)
					maxAltitude = (int)data[3*index + 2];
			}
		}

		//delete buffer
		delete[] buff;

		SetupBuffer(data);

		//delete array
		delete[] data;

		SetBounds();
	}
	else
	{
		printf("Could not open %s\n", hgtFilePath.c_str());
	}
}

void TerrainPart::Render(mat4 mvp, int levelOfDetails, GLuint mvp_loc, GLuint part_pos_loc)
{
	levelOfDetails = std::min(DETAIL_LEVELS_COUNT - 1, std::max(0, levelOfDetails));

	glBindVertexArray(_vao);

	//pass data
	glUniformMatrix4fv(mvp_loc, 1, GL_FALSE, &mvp[0][0]);	
	glUniform2f(part_pos_loc, longitude, latitude);

	//draw
	glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, DETAILS_LEVEL[levelOfDetails]);
	glDrawElements(GL_TRIANGLE_STRIP, DETAILS_COUNT[levelOfDetails], GL_UNSIGNED_INT, (void*)0);
}

void TerrainPart::SetBounds()
{
	_boundsOnGlobe[0] = GeoUtils::GetPositionOnGlobei(latitude, longitude);
	_boundsOnGlobe[1] = GeoUtils::GetPositionOnGlobei(latitude, longitude + 1);
	_boundsOnGlobe[2] = GeoUtils::GetPositionOnGlobei(latitude - 1, longitude + 1);
	_boundsOnGlobe[3] = GeoUtils::GetPositionOnGlobei(latitude - 1, longitude);
	_boundsOnGlobe[4] = GeoUtils::GetPositionOnGlobei(latitude, longitude, maxAltitude);
	_boundsOnGlobe[5] = GeoUtils::GetPositionOnGlobei(latitude, longitude + 1, maxAltitude);
	_boundsOnGlobe[6] = GeoUtils::GetPositionOnGlobei(latitude - 1, longitude + 1, maxAltitude);
	_boundsOnGlobe[7] = GeoUtils::GetPositionOnGlobei(latitude - 1, longitude, maxAltitude);

	_boundsOnMap[0] = GeoUtils::GetPositionOnMapi(latitude, longitude);
	_boundsOnMap[1] = GeoUtils::GetPositionOnMapi(latitude, longitude + 1);
	_boundsOnMap[2] = GeoUtils::GetPositionOnMapi(latitude - 1, longitude + 1);
	_boundsOnMap[3] = GeoUtils::GetPositionOnMapi(latitude - 1, longitude);
}

vec3 * TerrainPart::GetBoundsOnMap()
{
	return &_boundsOnMap[0];
}

vec3 * TerrainPart::GetBoundsOnGlobe()
{
	return &_boundsOnGlobe[0];
}
