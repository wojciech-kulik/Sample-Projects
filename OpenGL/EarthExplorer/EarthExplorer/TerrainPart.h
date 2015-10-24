#pragma once

#include "../../common/openglHelper.h"
#include <string>

using namespace std;

const int DETAIL_LEVELS_COUNT = 9;



class TerrainPart
{	
private:
	void SetupBuffer(GLfloat * data);
	void SetBounds();

protected:	
	GLuint _buffer, _vao;
	vec3 _boundsOnMap[4];
	vec3 _boundsOnGlobe[8];

	static const int MATRIX_SIZE = 1201;
	static const int LOD_STEP = 5;
	static GLuint * DETAILS_LEVEL;	
	static GLuint DETAILS_COUNT[DETAIL_LEVELS_COUNT];
	static int GetDetailsCount(int level);

public:
	TerrainPart(string path);
	TerrainPart(short latitude, short longitude);
	~TerrainPart(void);

	void LoadData(string hgtFilePath);
	void Render(mat4 mvp, int levelOfDetails, GLuint mvp_loc, GLuint part_pos_loc);

	static void InitLevelOfDetails();
	static void DestroyLevelOfDetails();
	static int NumberOfTriangles(int levelOfDetails);

	vec3 * GetBoundsOnMap();
	vec3 * GetBoundsOnGlobe();

	int maxAltitude;
	short longitude;
	short latitude;
};

