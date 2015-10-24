#include "Culling.h"

//see: http://www.crownandcutlass.com/features/technicaldetails/frustum.html

bool Culling::PointInFrustum(vec3 & v)
{	
	for(int p = 0; p < 6; p++)
		if( glm::dot(frustum[p], vec4(v, 1)) <= 0 )
			return false;

	return true;
}

bool Culling::ShapeInFrustum(vec3 * points, int count)
{
	bool in;
	for(int p = 0; p < 6; p++)
	{
		in = false;

		for(int i = 0; i < count; i++)
		{
			if( glm::dot(frustum[p], vec4(points[i], 1)) > 0 )
			{ 
				in = true;
				break;
			}
		}

		if (!in) 
			return false;
	}

	return true;
}

void Culling::ExtractFrustum(mat4 & mvp)
{
	/* Extract the RIGHT plane */
	for(int i = 0; i < 4; i++)
		frustum[0][i] = mvp[i][3] - mvp[i][0];

	/* Extract the LEFT plane */
	for(int i = 0; i < 4; i++)
		frustum[1][i] = mvp[i][3] + mvp[i][0];

	/* Extract the BOTTOM plane */
	for(int i = 0; i < 4; i++)
		frustum[2][i] = mvp[i][3] + mvp[i][1];

	/* Extract the TOP plane */
	for(int i = 0; i < 4; i++)
		frustum[3][i] = mvp[i][3] - mvp[i][1];

	/* Extract the FAR plane */
	for(int i = 0; i < 4; i++)
		frustum[4][i] = mvp[i][3] - mvp[i][2];

	/* Extract the NEAR plane */
    for(int i = 0; i < 4; i++)
		frustum[5][i] = mvp[i][3] + mvp[i][2];

	/* Normalize */
	for (int i = 0; i < 6; i++)
		frustum[i] = glm::normalize(frustum[i]);
}