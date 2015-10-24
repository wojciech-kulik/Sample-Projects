#include "InfoPrinter.h"
#include "GLFW/glfw3.h"

void InfoPrinter::PrintOnce(string msg, int id)
{
	queue[id] = msg;
}

void InfoPrinter::ClearQueue()
{
	queue.clear();
}

void InfoPrinter::Print()
{
	printf("\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n");
	for (unsigned int i = 0; i < queue.size(); i++)
		printf("%s", queue[i].c_str());

	ClearQueue();
}

void InfoPrinter::WaitAndPrint()
{
	static double lastTime = glfwGetTime();
	double currentTime = glfwGetTime();

	if (currentTime - lastTime >= intervalMS / 1000.0)
	{		
		Print();
		lastTime = currentTime;
	}
}

void InfoPrinter::SetInterval(int ms)
{
	intervalMS = ms;
}

InfoPrinter::InfoPrinter()
{
	intervalMS = 100;
}

InfoPrinter& InfoPrinter::GetInstance()
{
	static InfoPrinter instance;
	return instance;
}