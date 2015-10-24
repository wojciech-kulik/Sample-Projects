#pragma once

#include "../../common/openglHelper.h"

class Culling
{	
protected:
	vec4 frustum[6];
public:
	void ExtractFrustum(mat4 & mvp);
	bool PointInFrustum(vec3 & v);
	bool ShapeInFrustum(vec3 * points, int count);
};