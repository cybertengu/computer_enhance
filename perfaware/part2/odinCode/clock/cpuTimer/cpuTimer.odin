package cputimer

import "core:fmt"

main :: proc()
{
	OSFreq : u64 = GetOSTimerFreq()
	fmt.printf("    OS Freq: %d\n", OSFreq)

	CPUStart : u64 = ReadCPUTimer()
	OSStart : u64 = ReadOSTimer()
	OSEnd : u64 = 0
	OSElapsed : u64 = 0
	for OSElapsed < OSFreq
	{
		OSEnd = ReadOSTimer()
		OSElapsed = OSEnd - OSStart
	}
	
	CPUEnd : u64 = ReadCPUTimer()
	CPUElapsed : u64 = CPUEnd - CPUStart
	
	fmt.printf("   OS Timer: %d -> %d = %d elapsed\n", OSStart, OSEnd, OSElapsed)
	fmt.printf(" OS Seconds: %.4f\n", cast(f64)OSElapsed/cast(f64)OSFreq)
	
	fmt.printf("  CPU Timer: %d -> %d = %d elapsed\n", CPUStart, CPUEnd, CPUElapsed)
}
