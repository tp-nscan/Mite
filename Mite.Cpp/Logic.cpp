#include <stdlib.h>
//#include <stdio.h>
#include "Logic.h"

int Mite::Cpp::Logic::Get() const
{
	return 42; // Really, what else did you expect?
}

int Mite::Cpp::Logic::GetAdd(int val)
{
	return 42 + val; // Really, what else did you expect?
}

int* Mite::Cpp::Logic::Yob()
{
	const int count = 5;
	int *randData = (int *)malloc(sizeof(int) * 5);
	for (int i = 0; i < count; i++)
	{
		randData[i] = i * 3;
	}
	return randData;
}