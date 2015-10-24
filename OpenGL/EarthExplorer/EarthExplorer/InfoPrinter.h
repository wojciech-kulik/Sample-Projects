#pragma once

#include <string>
#include <hash_map>

using namespace std;

class InfoPrinter
{
private:
	hash_map<int, string> queue; 

protected:
	int intervalMS;
	InfoPrinter();
	void ClearQueue();
	void Print();

public:
	void WaitAndPrint();
	void PrintOnce(string msg, int id);
	void SetInterval(int ms);
	
	static InfoPrinter& GetInstance();
};