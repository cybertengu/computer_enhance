package solver

import "core:simd/x86"
import "core:sys/windows"

GetCPUEstimatedFreq :: proc() -> u64
{
	MillisecondsToWait : u64 = 1000
	OSFreq : u64 = GetOSTimerFreq();
	CPUStart : u64 = ReadCPUTimer();
	OSStart : u64 = ReadOSTimer();
	OSEnd : u64 = 0;
	OSElapsed : u64 = 0;
	OSWaitTime : u64 = OSFreq * MillisecondsToWait / 1000;
	for OSElapsed < OSWaitTime
	{
		OSEnd = ReadOSTimer();
		OSElapsed = OSEnd - OSStart;
	}
	
	CPUEnd : u64 = ReadCPUTimer();
	CPUElapsed : u64 = CPUEnd - CPUStart;
	cpuFreq : u64 = 0;
	if OSElapsed > 0
	{
		cpuFreq = OSFreq * CPUElapsed / OSElapsed;
	}

	return cpuFreq
}

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
