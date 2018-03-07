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


float* Mite::Cpp::Logic::BackSlashArray(int span, float minval, float maxval)
{
	float* av = new float[span*span];
	float step = (maxval - minval) / (float)span;
	float fi, fj;
	for (int i = 0; i < span; i++)
	{
		fi = minval + i * step;
		for (int j = 0; j < span; j++)
		{
			fj = minval + (j * 1.5) * step;
			av[i + j * span] = (fi < fj) ? fi : fj;
		}
	}
	return av;
}