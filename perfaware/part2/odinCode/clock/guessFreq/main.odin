package timer

import "core:os"
import "core:strconv"
import "core:fmt"

main :: proc()
{
	MillisecondsToWait : u64 = 1000
	ArgCount := len(os.args)
	if(ArgCount == 2)
	{
		MillisecondsToWait, _ = strconv.parse_u64_of_base(os.args[1], 10);
	}

	OSFreq : u64 = GetOSTimerFreq();
	fmt.printf("    OS Freq: %d (reported)\n", OSFreq);

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
	CPUFreq : u64 = 0;
	if OSElapsed > 0
	{
		CPUFreq = OSFreq * CPUElapsed / OSElapsed;
	}
	
	fmt.printf("   OS Timer: %d -> %d = %d elapsed\n", OSStart, OSEnd, OSElapsed)
	fmt.printf(" OS Seconds: %.4f\n", cast(f64)OSElapsed/cast(f64)OSFreq)
	
	fmt.printf("  CPU Timer: %d -> %d = %d elapsed\n", CPUStart, CPUEnd, CPUElapsed);
	fmt.printf("   CPU Freq: %d (guessed)\n", CPUFreq)
}
