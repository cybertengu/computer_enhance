package timer

import "core:fmt"

main :: proc()
{
	OSFreq : u64 = GetOSTimerFreq()
	fmt.printf("    OS Freq: %d\n", OSFreq)

	OSStart : u64 = ReadOSTimer()
	OSEnd : u64 = 0
	OSElapsed : u64 = 0
	for OSElapsed < OSFreq
	{
		OSEnd = ReadOSTimer()
		OSElapsed = OSEnd - OSStart
	}
	
	fmt.printf("   OS Timer: %d -> %d = %d elapsed\n", OSStart, OSEnd, OSElapsed)
	fmt.printf(" OS Seconds: %.4f\n", cast(f64)OSElapsed/cast(f64)OSFreq)
}
