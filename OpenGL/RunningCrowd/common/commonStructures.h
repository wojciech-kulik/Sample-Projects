#pragma once

struct Color
{
	float R;
	float G;
	float B;
	float A;

	Color() { }

	Color(float r, float g, float b, float a = 1.0f)
	{
		R = r;
		G = g;
		B = b;
		A = a;
	}
};

struct Point
{
	float x;
	float y;
};

struct Point3D
{
	float x;
	float y;
	float z;
};

struct RectStruct
{
	Point p1, p2, p3, p4;
};

enum CollisionResult
{
	noCollision,
	sideCollision,
	topCollision
};