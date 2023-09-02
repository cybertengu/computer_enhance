package cputimer

import "core:simd/x86"
import "core:sys/windows"

GetOSTimerFreq :: proc() -> u64
{
	Freq : windows.LARGE_INTEGER
	windows.QueryPerformanceFrequency(&Freq)
	return u64(Freq)
}

ReadOSTimer :: proc() -> u64 
{
	Value : windows.LARGE_INTEGER
	windows.QueryPerformanceCounter(&Value)
	return u64(Value)
}

/* NOTE(casey): This does not need to be "inline", it could just be "static"
   because compilers will inline it anyway. But compilers will warn about 
   static functions that aren't used. So "inline" is just the simplest way 
   to tell them to stop complaining about that. */
ReadCPUTimer :: proc() -> u64
{
	// NOTE(casey): If you were on ARM, you would need to replace __rdtsc
	// with one of their performance counter read instructions, depending
	// on which ones are available on your platform.
	
	return x86._rdtsc();
}
