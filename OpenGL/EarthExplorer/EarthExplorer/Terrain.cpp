#include <Windows.h>
#include "Terrain.h"
#include "GlobeCamera.h"
#include "InfoPrinter.h"
#include "InfoIds.h"
#include "GeoUtils.h"
#include <algorithm>

const int segments = 180;


Terrain::Terrain()
	:  camera(NULL), isGlobeOn(true)
{
	sphereShader = LoadShaders("Shaders/MVP.vertexshader", "Shaders/FragmentShader.fragmentshader");
	mapShaders = LoadShaders( "Shaders/VertexShaderMap.vertexshader", "Shaders/FragmentShader.fragmentshader" );
	globeShaders = LoadShaders( "Shaders/VertexShaderGlobe.vertexshader", "Shaders/FragmentShader.fragmentshader" );
	sphere_mvp_loc = glGetUniformLocation(sphereShader, "MVP");

	TerrainPart::InitLevelOfDetails();
	PrepareBuffer();
}
	
Terrain::~Terrain()
{
	glDeleteProgram(sphereShader);
	glDeleteProgram(mapShaders);
	glDeleteProgram(globeShaders);
	glDeleteBuffers(1, &buffer);
	glDeleteBuffers(1, &color_buffer);

	for(unsigned int i = 0; i < terrainParts.size(); i++)
		delete terrainParts[i];
	TerrainPart::DestroyLevelOfDetails();
}
	
void Terrain::PrepareBuffer()
{
	GLfloat v[(segments+1)*3];
	GLfloat c[(segments+1)*3];

	float t;
	for( int n = 0; n <= segments; ++n ) 
	{
        t = 2*PI*(float)n/(float)segments;

        v[3*n + 0] = sin(t);
		v[3*n + 1] = cos(t);
		v[3*n + 2] = 0.0f;

		c[3*n + 0] = 0.35f;
		c[3*n + 1] = 0.35f;
		c[3*n + 2] = 0.35f;
    }

	buffer = createBuffer(v, sizeof(v));
	color_buffer = createBuffer(c, sizeof(c));
}

void Terrain::SetCamera(Camera * c)
{
	camera = c;
	isGlobeOn = dynamic_cast<GlobeCamera*>(camera) != NULL;
}

void Terrain::RenderSphere(mat4 mvp, float radius)
{
	glUseProgram(sphereShader);

	mvp = mvp * glm::scale(vec3(radius));
	mat4 tmp = mvp;
	for (int i = 0; i < 180; i++)
	{
		mvp = mvp * glm::rotate(1.0f, vec3(0.0f, 1.0f, 0.0f));
		drawArrWithColor3D(GL_LINE_LOOP, segments+1, buffer, color_buffer, sphere_mvp_loc, mvp, 3);
	}

	float h, c, yOffset;
	mat4 scale;
	mvp = tmp * glm::rotate(90.0f, vec3(1.0f, 0.0f, 0.0f));
	for (int i = 0; i < 90; i++)
	{		
		yOffset = sin(degtorad(i));

		h = radius - yOffset * radius;
		c = std::sqrt(2*h*radius - h*h) / radius;
		scale = glm::scale(vec3(c));

		drawArrWithColor3D(GL_LINE_LOOP, segments+1, buffer, color_buffer, sphere_mvp_loc, mvp * glm::translate(vec3(0.0f, 0.0f, -yOffset)) * scale, 3);
		drawArrWithColor3D(GL_LINE_LOOP, segments+1, buffer, color_buffer, sphere_mvp_loc, mvp * glm::translate(vec3(0.0f, 0.0f, yOffset)) * scale, 3);
	}
}

void Terrain::RenderParts(mat4 mvp)
{
	GLuint mvp_loc, part_pos_loc;
	bool inFrustum = false;
	int renderedParts = 0;

	//set shaders
	if (isGlobeOn)
	{
		glUseProgram(globeShaders);
		mvp_loc = glGetUniformLocation(globeShaders, "MVP");
		part_pos_loc = glGetUniformLocation(globeShaders, "partPos");
	}
	else
	{
		glUseProgram(mapShaders);
		mvp_loc = glGetUniformLocation(mapShaders, "MVP");
		part_pos_loc = glGetUniformLocation(mapShaders, "partPos");
	}		
	

	//render terrain parts
	for(unsigned int i = 0; i < terrainParts.size(); i++)
	{
		if (isGlobeOn)
			inFrustum = camera->getCulling().ShapeInFrustum(terrainParts[i]->GetBoundsOnGlobe(), 8);
		else
			inFrustum = camera->getCulling().ShapeInFrustum(terrainParts[i]->GetBoundsOnMap(), 4);

		if (inFrustum)
		{
			renderedParts++;
			terrainParts[i]->Render(mvp, camera->getLOD(), mvp_loc, part_pos_loc);
		}
	}

	//show info about rendered parts/triangles
	char buff[100];
	sprintf_s(buff, "\n\nRendered parts: %d/%d\nRendered triangles: %d", renderedParts, terrainParts.size(), TerrainPart::NumberOfTriangles(camera->getLOD()) * renderedParts);
	InfoPrinter::GetInstance().PrintOnce(buff, RENDERED_PARTS_ID);

	glBindVertexArray(0);
}

void Terrain::Render(float radius)
{
	mat4 mvp = camera->getMVP();
	
	if (isGlobeOn)
		RenderSphere(mvp, radius);
	
	RenderParts(mvp);
}

void Terrain::LoadPart(string path)
{
	terrainParts.push_back(new TerrainPart(path));
}

void Terrain::LoadDirectory(string path)
{
	system("CLS");
	if (path[path.length()-1] != '\\')
		path += "\\";
	
	string searchExp = path + "*.hgt";

	WIN32_FIND_DATAA FindFileData;
    HANDLE hFind = FindFirstFileA(searchExp.c_str(), &FindFileData);

	if (hFind != INVALID_HANDLE_VALUE)
	{		
		TerrainPart * tp = new TerrainPart(path + string(FindFileData.cFileName));
		terrainParts.push_back(tp);

		while (FindNextFileA(hFind, &FindFileData))
		{
			tp = new TerrainPart(path + string(FindFileData.cFileName));
			terrainParts.push_back(tp);
		}
	}

	FindClose(hFind);
}

TerrainPart& Terrain::GetTerrainPart(int index)
{
	return *terrainParts[index];
}

Bounds Terrain::GetMapBounds(int addX, int addY)
{
	Bounds result;
	int count = terrainParts.size();

	if (count > 0)
	{
		result.top		= GeoUtils::GetPositionOnMapi(terrainParts[0]->latitude + addY,	terrainParts[0]->longitude - addX).y;
		result.bottom	= GeoUtils::GetPositionOnMapi(terrainParts[0]->latitude - 1,	terrainParts[0]->longitude - addX).y;
		result.left		= GeoUtils::GetPositionOnMapi(terrainParts[0]->latitude + addY,	terrainParts[0]->longitude - addX).x;
		result.right	= GeoUtils::GetPositionOnMapi(terrainParts[0]->latitude + addY,	terrainParts[0]->longitude + 1).x;			
	}

	for (int i = 1; i < count; i++)
	{
		result.top		= std::max(result.top,		GeoUtils::GetPositionOnMapi(terrainParts[i]->latitude + addY,	terrainParts[i]->longitude - addX).y);
		result.bottom	= std::min(result.bottom,	GeoUtils::GetPositionOnMapi(terrainParts[i]->latitude - 1,		terrainParts[i]->longitude - addX).y);
		result.left		= std::min(result.left,		GeoUtils::GetPositionOnMapi(terrainParts[i]->latitude + addY,	terrainParts[i]->longitude - addX).x);
		result.right	= std::max(result.right,	GeoUtils::GetPositionOnMapi(terrainParts[i]->latitude + addY,	terrainParts[i]->longitude + 1).x);
	}

	return result;
}