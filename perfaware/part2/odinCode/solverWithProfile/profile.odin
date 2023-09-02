package solver

import "core:fmt"
import "core:runtime"

profileAnchor :: struct
{
	TSCElapsed : u64,
	TSCElapsedChildren,
	hitCount : u64,
	isChild : bool,
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
	startTSC : u64,
	parentIndex : u32,
	anchorIndex : u32,
}

BlockStart :: proc(label_ := #caller_location) -> profileBlock
{
	globalAnchorIndex += 1
	assert(globalAnchorIndex < 4096, "Number of profile points exceeds size of profiler.anchors array")
	parentIndex := globalProfilerParent
	globalProfilerParent = globalAnchorIndex
	return profileBlock{ label_, ReadCPUTimer(), parentIndex, globalAnchorIndex }
}

BlockEnd :: proc(block : profileBlock)
{
	elapsed := ReadCPUTimer() - block.startTSC
	globalProfilerParent = block.parentIndex

	parent : ^profileAnchor = &globalProfiler.anchors[block.parentIndex]
	anchor : ^profileAnchor = &globalProfiler.anchors[block.anchorIndex]

	parent.TSCElapsedChildren += elapsed
	anchor.TSCElapsed += elapsed
	anchor.hitCount += 1

	anchor.label = block.label
}

globalProfiler : profiler
globalProfilerParent : u32
globalAnchorIndex : u32

PrintTimeElapsed :: proc(totalTSCElapsed : u64, anchor : ^profileAnchor)
{
	elapsed := anchor.TSCElapsed - anchor.TSCElapsedChildren
	percent := 100 * (cast(f64)elapsed / cast(f64)totalTSCElapsed)
	fmt.printf("  %s[%d]: %d (%.2f%%", anchor.label.procedure, anchor.hitCount, elapsed, percent)

	if anchor.TSCElapsedChildren != 0
	{
		percentWithChildren := 100.0 * (cast(f64)anchor.TSCElapsed / cast(f64)totalTSCElapsed)
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

	upMostAnchor : ^profileAnchor = &globalProfiler.anchors[0]
	for anchorIndex : u32 = 1; anchorIndex < len(globalProfiler.anchors); anchorIndex += 1
	{
		anchor : ^profileAnchor = &globalProfiler.anchors[anchorIndex]
		if anchor.label.procedure == upMostAnchor.label.procedure
		{
			upMostAnchor.hitCount += 1
			anchor.isChild = true
		}
		else
		{
			upMostAnchor = anchor
		}
	}

	for anchorIndex : u32 = 0; anchorIndex < len(globalProfiler.anchors); anchorIndex += 1
	{
		anchor : ^profileAnchor = &globalProfiler.anchors[anchorIndex]
		if anchor.TSCElapsed != 0 && anchor.isChild == false
		{
			PrintTimeElapsed(totalCpuElasped, anchor)
		}
	}
}

