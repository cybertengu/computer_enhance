package solver

import "core:fmt"
import "core:runtime"

profileAnchor :: struct
{
	TSCElapsedExclusive : u64,
	TSCElapsedInclusive : u64,
	hitCount : u64,
	label : runtime.Source_Code_Location,
}

profiler :: struct
{
	anchors : [4096]profileAnchor,

	startTSC : u64,
	endTSC : u64,
}

profileBlock :: struct
{
	label : runtime.Source_Code_Location,
	oldTSCElapsedInclusive : u64,
	startTSC : u64,
	parentIndex : u32,
	anchorIndex : u32,
}

BlockStart :: proc(label_ := #caller_location) -> profileBlock
{
	parentIndex := globalProfilerParent

	if globalAnchorIndex > 0
	{
		if globalPriorCaller.procedure != label_.procedure
		{
			globalPriorCaller = label_
			globalAnchorIndex += 1
		}
	}
	else
	{
		globalPriorCaller = label_
		globalAnchorIndex += 1
	}
	assert(globalAnchorIndex < 4096, "Number of profile points exceeds size of profiler.anchors array")

	anchorIndex := globalAnchorIndex
	anchor := globalProfiler.anchors[anchorIndex]
	oldTSCElapsedInclusive := anchor.TSCElapsedInclusive

	globalProfilerParent = globalAnchorIndex
	startTSC := ReadCPUTimer()

	return profileBlock{ label_, oldTSCElapsedInclusive, startTSC, parentIndex, anchorIndex }
}

BlockEnd :: proc(block : profileBlock)
{
	elapsed := ReadCPUTimer() - block.startTSC
	globalProfilerParent = block.parentIndex

	parent : ^profileAnchor = &globalProfiler.anchors[block.parentIndex]
	anchor : ^profileAnchor = &globalProfiler.anchors[block.anchorIndex]

	parent.TSCElapsedExclusive -= elapsed
	anchor.TSCElapsedExclusive += elapsed
	anchor.TSCElapsedInclusive = block.oldTSCElapsedInclusive + elapsed
	anchor.hitCount += 1

	anchor.label = block.label
}

globalProfiler : profiler
globalProfilerParent : u32
globalAnchorIndex : u32
globalPriorCaller : runtime.Source_Code_Location

PrintTimeElapsed :: proc(totalTSCElapsed : u64, anchor : ^profileAnchor)
{
	percent := 100 * (cast(f64)anchor.TSCElapsedExclusive / cast(f64)totalTSCElapsed)
	fmt.printf("  %s[%d]: %d (%.2f%%", anchor.label.procedure, anchor.hitCount, anchor.TSCElapsedExclusive, percent)

	if anchor.TSCElapsedInclusive != anchor.TSCElapsedExclusive
	{
		percentWithChildren := 100.0 * (cast(f64)anchor.TSCElapsedInclusive / cast(f64)totalTSCElapsed)
		fmt.printf(", %.2f%% w/children", percentWithChildren)
	}
	fmt.printf(")\n")
}

BeginProfile :: proc()
{
	globalProfiler.startTSC = ReadCPUTimer()
}

EndAndPrintProfile :: proc()
{
	globalProfiler.endTSC = ReadCPUTimer()
	cpuFreq := EstimatedCpuFreq()
	
	totalCpuElasped := globalProfiler.endTSC - globalProfiler.startTSC

	fmt.printf("\nTotal time: %0.4fms (CPU freq %d)\n", 1000 * cast(f64)totalCpuElasped / cast(f64)cpuFreq, cpuFreq)

	for anchorIndex : u32 = 0; anchorIndex < len(globalProfiler.anchors); anchorIndex += 1
	{
		anchor : ^profileAnchor = &globalProfiler.anchors[anchorIndex]
		if anchor.TSCElapsedInclusive != 0
		{
			PrintTimeElapsed(totalCpuElasped, anchor)
		}
	}
}

