#include "human.h"
#define _USE_MATH_DEFINES
#include <math.h>

GLuint Human::ellipsoid_buffer(0);
GLuint Human::sphere_buffer(0);
GLuint Human::yellow_color_buffer(0);
GLuint Human::red_color_buffer(0);
GLuint Human::blue_color_buffer(0);
GLuint Human::green_color_buffer(0);
int Human::lines_count(0);
const float Human::SPHERE_R(1.0f);
const float Human::SELECTION_MARK_SCALE(0.5f);
const float Human::HEAD_SCALE(0.5f);
const float Human::ELLIPSOID_DEPTH(10.0f);
const float Human::ELLIPSOID_HEIGHT(24.0f);
const float Human::BODY_X_SCALE(0.18f);
const float Human::BODY_Y_SCALE(0.13f);
const float Human::BODY_Z_SCALE(0.22f);

Human::Human(GLint mvp_loc, float direction_angel, float speed, HumanColor color) : 
	MVP_LOC(mvp_loc), _direction_angel(direction_angel), _human_speed(speed), _position(0.0f), _stack(), _isActive(false),
	BODY_PART_SPEED(max(0.05f, min(0.07f, speed / 1.5f))),
	_left_calf_x(0.0f), _right_calf_x(0.0f), _leg_x(0.0f), _arm_x(0.0f), _left_forearm_x(0.0f), _right_forearm_x(0.0f), _y_offset_x(0.0f),
	_ARM_MIN_ANGEL(120.0f),    _ARM_MAX_ANGEL(240.0f), 
	_FOREARM_MIN_ANGEL(60.0f), _FOREARM_MAX_ANGEL(105.0f),
	_LEG_MIN_ANGEL(120.0f),    _LEG_MAX_ANGEL(240.0f), 
	_CALF_MIN_ANGEL(-90.0f),   _CALF_MAX_ANGEL(0.0f) 
{ 
	switch(color)
	{
	case red:
		_human_color_buffer = red_color_buffer;
		break;
	case green:
		_human_color_buffer = green_color_buffer;
		break;
	case blue:
		_human_color_buffer = blue_color_buffer;
		break;
	case yellow:
		_human_color_buffer = yellow_color_buffer;
		break;
	}
}

void Human::InitBuffers()
{
	if (sphere_buffer == 0)
		lines_count = createEllipsoid(&sphere_buffer, &yellow_color_buffer, vec3(SPHERE_R), vec4(1.0f, 1.0f, 0.0f, 0.5f)) / 12;

	if (ellipsoid_buffer == 0)
		createEllipsoid(&ellipsoid_buffer, &red_color_buffer, vec3(ELLIPSOID_DEPTH * 0.5f, ELLIPSOID_HEIGHT * 0.5f, ELLIPSOID_DEPTH * 0.5f), vec4(1.0f, 0.0f, 0.0f, 0.5f));

	if (blue_color_buffer == 0)
		createColorBuffer(&blue_color_buffer, vec4(0.3f, 0.0f, 1.0f, 0.7f), lines_count);

	if (green_color_buffer == 0)
		createColorBuffer(&green_color_buffer, vec4(0.3f, 1.0f, 0.0f, 0.7f), lines_count);
}

void Human::DestroyBuffers()
{
	glDeleteBuffers(1, &ellipsoid_buffer);	
	glDeleteBuffers(1, &sphere_buffer);

	glDeleteBuffers(1, &red_color_buffer);
	glDeleteBuffers(1, &green_color_buffer);
	glDeleteBuffers(1, &blue_color_buffer);
	glDeleteBuffers(1, &yellow_color_buffer);
}

void Human::PushMatrix(mat4 & m)
{
	_stack.push(m);
}
void Human::PopMatrix(mat4 & m)
{
	m = _stack.top();
	_stack.pop();
}

mat4 Human::PeekMatrix()
{
	return _stack.top();
}

void Human::DrawHead(mat4 & projection)
{
	mat4 model_view = PeekMatrix() * glm::translate(0.0f, 3.0f, 0.0f);
	mat4 MVP = projection * model_view * glm::scale(vec3(HEAD_SCALE));
	drawArrWithColor3D(GL_LINE_STRIP, lines_count, sphere_buffer, _human_color_buffer, MVP_LOC, MVP);

	PushMatrix(model_view);
}

void Human::DrawSelectionMark(mat4 & projection)
{
	if (IsActive())
	{		
		mat4 model_view = PeekMatrix() * glm::translate(SELECTION_MARK_SCALE * SPHERE_R, 1.8f, 0.0f);
		mat4 MVP = projection * model_view * glm::scale(vec3(SELECTION_MARK_SCALE));
		drawArrWithColor3D(GL_LINE_STRIP, lines_count, sphere_buffer, red_color_buffer, MVP_LOC, MVP);
	}
}

void Human::DrawBody(mat4 & projection)
{
	mat4 model_view = PeekMatrix() * glm::translate(BODY_X_SCALE * 0.25f * -ELLIPSOID_DEPTH, BODY_Y_SCALE * 0.98f * -ELLIPSOID_HEIGHT, BODY_Z_SCALE * 0.25f * -ELLIPSOID_DEPTH);
	mat4 MVP = projection * model_view * glm::scale(BODY_X_SCALE, BODY_Y_SCALE, BODY_Z_SCALE);
	
	drawArrWithColor3D(GL_LINE_STRIP, lines_count, ellipsoid_buffer, _human_color_buffer, MVP_LOC, MVP);
}

void Human::DrawArm(mat4 & projection, bool left, float arm_angel, float forearm_angel)
{
	mat4 model_view, mvp, translate_model;
	float reflection = left ? 1.0f : -1.0f;
	const float ARM_X_SCALE = 0.05f, ARM_Y_SCALE = 0.07f, ARM_Z_SCALE = 0.05f;
	const float FOREARM_X_SCALE = 0.05f, FOREARM_Y_SCALE = 0.055f, FOREARM_Z_SCALE = 0.05f;	

	//shared model_view matrix
	model_view = PeekMatrix() * glm::translate(0.2f * BODY_X_SCALE * ELLIPSOID_DEPTH, 
											   (left ? 0.15f : 0.2f) * BODY_Y_SCALE * -ELLIPSOID_HEIGHT, 
											   left ? ARM_Z_SCALE * -ELLIPSOID_DEPTH : 0.5f * BODY_Z_SCALE * ELLIPSOID_DEPTH);
	model_view = model_view * glm::rotate(reflection * -arm_angel, vec3(0.0f, 0.0f, 1.0f)) * glm::translate(0.5f * ARM_X_SCALE * -ELLIPSOID_DEPTH, 0.00f, 0.0f);
																										//move rotation axis
	model_view = model_view * glm::rotate(-30.0f * reflection, vec3(1.0f, 0.0f, 0.0f));


	//arm
	mvp = projection * model_view * glm::scale(ARM_X_SCALE, ARM_Y_SCALE, ARM_Z_SCALE); 	
	drawArrWithColor3D(GL_LINE_STRIP, lines_count, ellipsoid_buffer, _human_color_buffer, MVP_LOC, mvp);

	//forearm
	model_view = model_view * glm::translate(0.5f * FOREARM_X_SCALE * ELLIPSOID_DEPTH, 1.1f * FOREARM_Y_SCALE * ELLIPSOID_HEIGHT, 0.0f) * glm::rotate(forearm_angel, vec3(0.0f, 0.0f, 1.0f)); 
	mvp = projection * model_view * glm::scale(FOREARM_X_SCALE, FOREARM_Y_SCALE, FOREARM_Z_SCALE); 	
	drawArrWithColor3D(GL_LINE_STRIP, lines_count, ellipsoid_buffer, _human_color_buffer, MVP_LOC, mvp);
	
	//hand
	model_view = model_view * glm::translate(0.0f, 0.9f * FOREARM_Y_SCALE * ELLIPSOID_HEIGHT, 0.0f); 
	mvp = projection * model_view * glm::scale(0.22f, 0.22f, 0.22f); 	
	drawArrWithColor3D(GL_LINE_STRIP, lines_count, sphere_buffer, _human_color_buffer, MVP_LOC, mvp);
}

void Human::DrawLeg(mat4 & projection, bool left, float leg_angel, float calf_angel)
{
	mat4 model_view, mvp, translate_model;
	float reflection = left ? 1.0f : -1.0f;
	const float TIGH_X_SCALE = 0.05f, TIGH_Y_SCALE = 0.08f, TIGH_Z_SCALE = 0.07f;
	const float CALF_X_SCALE = 0.05f, CALF_Y_SCALE = 0.10f, CALF_Z_SCALE = 0.07f;
	const float FOOT_X_SCALE = 0.05f, FOOT_Y_SCALE = 0.05f, FOOT_Z_SCALE = 0.07f;

	//shared model_view matrix
	model_view = PeekMatrix() * glm::translate(0.25f * BODY_X_SCALE * ELLIPSOID_DEPTH, 
											   0.88f * BODY_Y_SCALE * -ELLIPSOID_HEIGHT, 
											   left ? 0.12f * BODY_Z_SCALE * -ELLIPSOID_DEPTH : 0.32f * BODY_Z_SCALE * ELLIPSOID_DEPTH) * 
								glm::rotate(reflection * leg_angel, vec3(0.0f, 0.0f, 1.0f)) * glm::translate(-0.2f, 0.0f, 0.0f);

	//tigh
	mvp = projection * model_view * glm::scale(TIGH_X_SCALE, TIGH_Y_SCALE, TIGH_Z_SCALE); 	
	drawArrWithColor3D(GL_LINE_STRIP, lines_count, ellipsoid_buffer, _human_color_buffer, MVP_LOC, mvp);

	//calf
	model_view = model_view * glm::translate(0.0f, 0.98f * TIGH_Y_SCALE * ELLIPSOID_HEIGHT, 0.0f) * glm::rotate(calf_angel, vec3(0.0f, 0.0f, 1.0f)); 
	mvp = projection * model_view * glm::scale(CALF_X_SCALE, CALF_Y_SCALE, CALF_Z_SCALE); 	
	drawArrWithColor3D(GL_LINE_STRIP, lines_count, ellipsoid_buffer, _human_color_buffer, MVP_LOC, mvp);

	//foot
	model_view = model_view * glm::translate(0.2f * FOOT_X_SCALE * ELLIPSOID_HEIGHT, 0.91f * CALF_Y_SCALE * ELLIPSOID_HEIGHT, 0.0f) * glm::rotate(90.0f, vec3(0.0f, 0.0f, 1.0f)); 
	mvp = projection * model_view * glm::scale(FOOT_X_SCALE, FOOT_Y_SCALE, FOOT_Z_SCALE); 	
	drawArrWithColor3D(GL_LINE_STRIP, lines_count, ellipsoid_buffer, _human_color_buffer, MVP_LOC, mvp);
}

float Human::CalculateAngel(float min_angel, float max_angel, float & x, int a, float b)
{
	const float angel_neutral    = (max_angel + min_angel) / 2.0f;
	const float angel_max_offset = (max_angel - min_angel) / 2.0f;

	x += BODY_PART_SPEED;
	if (x > 2.0f * M_PI)
		x = BODY_PART_SPEED; 

	return angel_neutral + a*sin(x + b) * angel_max_offset;
}


void Human::CalculateLegAngel(float & leg_angel, float & left_calf_angel, float & right_calf_angel)
{	
	leg_angel = CalculateAngel(_LEG_MIN_ANGEL, _LEG_MAX_ANGEL, _leg_x);
	left_calf_angel  = CalculateAngel(_CALF_MIN_ANGEL, _CALF_MAX_ANGEL, _left_calf_x, 1, -M_PI_2);
	right_calf_angel = CalculateAngel(_CALF_MIN_ANGEL, _CALF_MAX_ANGEL, _right_calf_x, -1, -M_PI_2);
}

void Human::CalculateArmAngel(float & arm_angel, float & left_forearm_angel, float & right_forearm_angel)
{
	arm_angel = CalculateAngel(_ARM_MIN_ANGEL, _ARM_MAX_ANGEL, _arm_x);
	left_forearm_angel  = CalculateAngel(_FOREARM_MIN_ANGEL, _FOREARM_MAX_ANGEL, _left_forearm_x, 1, -M_PI_2);
	right_forearm_angel = CalculateAngel(_FOREARM_MIN_ANGEL, _FOREARM_MAX_ANGEL, _right_forearm_x, -1, -M_PI_2);
}

void Human::CalculateYOffset(float & y_offset, float leg_angel)
{
	_y_offset_x += BODY_PART_SPEED;
	if (_y_offset_x > 2.0f * M_PI)
		_y_offset_x = BODY_PART_SPEED; 

	y_offset = abs(sin(_y_offset_x)) / 2.0f;
}

void Human::Draw(mat4 projection, mat4 view, bool drawHead, bool drawSelectionMark)
{
	mat4 model_view;
	float leg_angel, arm_angel, left_calf_angel, right_calf_angel, right_forearm_angel, left_forearm_angel;

	CalculateArmAngel(arm_angel, left_forearm_angel, right_forearm_angel);
	CalculateLegAngel(leg_angel, left_calf_angel, right_calf_angel);		
	CalculateYOffset(_y_offset, leg_angel);	

	//human movement
	_position += _human_speed;
	vec4 position = glm::rotate(_direction_angel, vec3(0.0f, 1.0f, 0.0f)) * vec4(_position, _y_offset, 0.0f, 1.0f);
	mat4 rotateView = glm::rotate(_direction_angel, vec3(0.0f, 1.0f, 0.0f));
	model_view = view * glm::translate(position.x, position.y, position.z) * rotateView; //set in proper place
	PushMatrix(model_view);

	//head
	if (drawHead || !IsActive())
		DrawHead(projection);

	//selection mark
	if (drawSelectionMark)
		DrawSelectionMark(projection);

	//body
	DrawBody(projection);

	//left arm
	DrawArm(projection, true, arm_angel, left_forearm_angel);

	//right arm
	DrawArm(projection, false, arm_angel, right_forearm_angel);

	//left leg
	DrawLeg(projection, true, leg_angel, left_calf_angel);

	//right leg
	DrawLeg(projection, false, leg_angel, right_calf_angel);

	PopMatrix(model_view);
}

bool Human::IsActive()
{
	return _isActive;
}

void Human::SetActive(bool value)
{
	_isActive = value;
}