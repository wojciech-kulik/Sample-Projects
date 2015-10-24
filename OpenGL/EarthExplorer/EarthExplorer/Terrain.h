#pragma once

#include "../../common/openglHelper.h"
#include "TerrainPart.h"
#include "Camera.h"
#include <string>
#include <vector>

using namespace std;

struct Bounds
{
	float left, right, top, bottom;
	float centerX, centerY;
};

class Terrain
{
private:
	bool isGlobeOn;

protected:
	Camera * camera;
	GLuint buffer, color_buffer;
	GLuint sphereShader, mapShaders, globeShaders;
	GLuint sphere_mvp_loc;
	vector<TerrainPart*> terrainParts;

	void PrepareBuffer();
	void RenderSphere(mat4 mvp, float radius);
	void RenderParts(mat4 mvp);

public:
	Terrain();
	~Terrain();
	void Render(float radius);
	void LoadPart(string path);
	void LoadDirectory(string path);
	void SetCamera(Camera * c);
	TerrainPart& GetTerrainPart(int index);
	Bounds GetMapBounds(int addX = 0, int addY = 0);
};