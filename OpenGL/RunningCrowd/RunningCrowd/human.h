#pragma once

#include "../../common/openglHelper.h"
#include <stack>

enum HumanColor
{
	yellow,
	green,
	red,
	blue
};

class Human
{
private:
	const float BODY_PART_SPEED;
	float _ARM_MIN_ANGEL, _ARM_MAX_ANGEL;
	float _FOREARM_MIN_ANGEL, _FOREARM_MAX_ANGEL;
	float _LEG_MIN_ANGEL, _LEG_MAX_ANGEL;
	float _CALF_MIN_ANGEL, _CALF_MAX_ANGEL;	

	GLuint _human_color_buffer;
	float _human_speed, _position;
	float _direction_angel;
	float _y_offset;
	float _left_calf_x, _right_calf_x, _leg_x, _arm_x, _left_forearm_x, _right_forearm_x, _y_offset_x;

	void CalculateYOffset(float & y_offset, float leg_angel);
	void CalculateArmAngel(float & arm_angel, float & left_forearm_angel, float & right_forearm_angel);
	void CalculateLegAngel(float & leg_angel, float & left_calf_angel, float & right_calf_angel);
	float CalculateAngel(float min_angel, float max_angel, float & x, int a = 1, float b = 0.0f);

protected:
	//static
	static const float SPHERE_R, ELLIPSOID_DEPTH, ELLIPSOID_HEIGHT;
	static const float SELECTION_MARK_SCALE, HEAD_SCALE, BODY_X_SCALE, BODY_Y_SCALE, BODY_Z_SCALE;
	static int lines_count;
	static GLuint ellipsoid_buffer, yellow_color_buffer;
	static GLuint sphere_buffer, blue_color_buffer, red_color_buffer, green_color_buffer;
	//----------------------------------------------------

	GLint MVP_LOC;
	bool _isActive;	
	std::stack<mat4> _stack;
	
	void DrawSelectionMark(mat4 & projection);
	void DrawHead(mat4 & projection);
	void DrawBody(mat4 & projection);
	void DrawArm(mat4 & projection, bool left, float arm_angel, float forearm_angel);
	void DrawLeg(mat4 & projection, bool left, float leg_angel, float calf_angel);

	void PushMatrix(mat4 & m);
	void PopMatrix(mat4 & m);
	mat4 PeekMatrix();

public:
	static void InitBuffers();
	static void DestroyBuffers();

	Human(GLint mvp_loc, float direction_angel, float speed, HumanColor color);
	void Draw(mat4 projection, mat4 view, bool drawHead = true, bool drawSelectionMark = true);
	float GetDirectionAngle() { return _direction_angel; }
	float GetSpeed() { return _human_speed; }
	float GetYOffset() { return _y_offset; }
	float GetMovementOffset() { return _position; }

	bool IsActive();
	void SetActive(bool value);
};