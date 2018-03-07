#include "Logic.h"
#include "..\Mite.Cpp\Logic.h"
#include <string>
#include <Windows.h>

using namespace std;

Mite::Cpp::CLI::Logic::Logic()
	: _impl(new Cpp::Logic()) 
	// Allocate some memory for the native implementation
{
}

int Mite::Cpp::CLI::Logic::Get()
{
	return _impl->Get(); // Call native Get
}

int Mite::Cpp::CLI::Logic::GetAdd(int val)
{
	return _impl->GetAdd(val); // Call native Get
}


cli::array<float>^ Mite::Cpp::CLI::Logic::BackSlashArray(int span, float minval, float maxval)
{
	cli::array<float>^ retArray = gcnew cli::array<float>(span * span);
	float *bb = _impl->BackSlashArray(span, minval, maxval);
	for (int i = 0; i < span*span; i++)
	{
		retArray[i] = bb[i];
	}

	return retArray;
}


cli::array<int>^ Mite::Cpp::CLI::Logic::Yobba()
{
	cli::array<int>^ intArray = gcnew cli::array<int>(5);
	int *bb = _impl->Yob();
	for (int i = 0; i < 5; i++)
	{
		intArray[i] = bb[i];
	}
	return intArray;
}

void Mite::Cpp::CLI::Logic::Destroy()
{
	if (_impl != nullptr)
	{
		delete _impl;
		_impl = nullptr;
	}
}

Mite::Cpp::CLI::Logic::~Logic()
{
	// C++ CLI compiler will automaticly make all ref classes implement IDisposable.
	// The default implementation will invoke this method + call GC.SuspendFinalize.
	Destroy(); // Clean-up any native resources 
}

Mite::Cpp::CLI::Logic::!Logic()
{
	// This is the finalizer
	// It's essentially a fail-safe, and will get called
	// in case Logic was not used inside a using block.
	Destroy(); // Clean-up any native resources 
}

string ManagedStringToStdString(System::String^ str)
{
	cli::array<unsigned char>^ bytes = System::Text::Encoding::ASCII->GetBytes(str);
	pin_ptr<unsigned char> pinned = &bytes[0];
	std::string nativeString((char*)pinned, bytes->Length);
	return nativeString;
}

//
//cli::array<int>^ Yob()
//{
//	cli::array<int>^ intArray = gcnew cli::array<int>(5);
//	return intArray;
//}


//string ManagedArrrayToStdArray(System:: str)
//{
//	cli::array<unsigned char>^ bytes = System::Text::Encoding::ASCII->GetBytes(str);
//	pin_ptr<unsigned char> pinned = &bytes[0];
//	std::string nativeString((char*)pinned, bytes->Length);
//	return nativeString;
//}

void Mite::Cpp::CLI::Logic::InitializeLibrary(System::String^ path)
{
	string nativePath = ManagedStringToStdString(path);
	LoadLibrary(nativePath.c_str()); // Actually load the delayed library from specific location
}
