package solver

import "core:os"
import "core:fmt"
import "core:slice"
import "core:strconv"
import "core:strings"
import "core:simd/x86"

/*
Total time: *ms (CPU freq *)
  Startup: * (*%)
  Read: * (*%)
  MiscSetup: * (*%)
  Parse and Sum: * (*%)
  MiscOutput: * (*%)
*/

main :: proc()
{
	startTime := ReadCPUTimer()
	cpuFreq := GetCPUEstimatedFreq()
	
	beforeReadTime, endReadTime, endParseAndSum, miscSetup : u64
	totalTime : f64
	argSize := len(os.args)
	if argSize < 2 || argSize > 3
	{
		fmt.println("haversine_release [haversine_input.json]\nhaversine_release [haversine_input.json] [answers.f64]")
		return
	}
	else
	{
		sum : f32 = 0
		openJsonFile, _ := os.open(os.args[1], os.O_RDONLY)
		beforeReadTime = ReadCPUTimer()
		content, _ := os.read_entire_file_from_handle(openJsonFile)
		endReadTime = ReadCPUTimer()
		json := transmute(string)content
		characters : [dynamic]u8
		append(&characters, 222)
		quoteCount := 0
		values : [4]f32 = { 0.0, 0.0, 0.0, 0.0 }
		foundValueCount := 0
		miscSetup = ReadCPUTimer()
		for index := 0; index < len(json); index += 1
		{
			character := json[index]
			//fmt.println(character)
			append(&characters, character)
			switch character {
				case '"': // "
					//fmt.println("Was \" found?")
					for
					{
						index += 1
						character = json[index]
						append(&characters, character)
						if character == '"'
						{
							break
						}
					}
				case ':':
					index += 1
					character = json[index]
					if character == '['
					{
						append(&characters, character)
					}
					else
					{
						length := 0
						startingPosition := index
						for
						{
							index += 1
							character = json[index]
							append(&characters, character)
							if character == ',' || character == '}'
							{
								break
							}
							length += 1
						}
						value := string(json[startingPosition:startingPosition + length + 1])
						status : bool
						values[foundValueCount], status = strconv.parse_f32(value)
						foundValueCount += 1
						//fmt.println(value)
						if foundValueCount >= 4
						{
							foundValueCount = 0
							result := referenceHaversine(values[0], values[1], values[2], values[3], 6372.8)
							//fmt.println(result)
							sum += result
						}

					}
					//fmt.println("Was : found?")
			}
		}
		//fmt.println(json)
		endParseAndSum = ReadCPUTimer()
		size, _ := os.file_size(openJsonFile)
		pos := strings.index(os.args[1], "_") + 1
		lastPos := strings.last_index(os.args[1], "_")
		fmt.println("Pair count: ", os.args[1][pos:lastPos])
		fmt.println("Input size: ", size)
		fmt.println("Haversine sum: ", sum)
		if len(os.args) == 3
		{
			openBinaryFile, _ := os.open(os.args[2], os.O_RDONLY)
			//result, success := os.read_entire_file_from_filename(os.args[1])
			result, success := os.read_entire_file_from_handle(openBinaryFile)
			//fmt.println(transmute(string)result)
			//fmt.println("End")
			arrayNumbers := slice.reinterpret([]f32, result)
			//fmt.println(arrayNumbers)
			otherSum : f32 = 0
			for index := 0; index < len(arrayNumbers); index += 1
			{
				otherSum += arrayNumbers[index]
			}
			fmt.println("\nValidation")
			fmt.println("Reference sum: ", otherSum)
			difference := sum - otherSum
			fmt.println("Difference: ", difference)
		}
	}	
	startup := (cast(f64)beforeReadTime - cast(f64)startTime) / cast(f64)cpuFreq * 1000
	read := (cast(f64)endReadTime - cast(f64)beforeReadTime) / cast(f64)cpuFreq * 1000
	miscSetupValue := (cast(f64)miscSetup - cast(f64)endReadTime) / cast(f64)cpuFreq * 1000
	parseAndSum := (cast(f64)endParseAndSum - cast(f64)miscSetup) / cast(f64)cpuFreq * 1000
	endOfAll := ReadCPUTimer()
	totalTime = (cast(f64)endOfAll - cast(f64)startTime) / cast(f64)cpuFreq * 1000
	miscOutput := (cast(f64)endOfAll - cast(f64)endParseAndSum) / cast(f64)cpuFreq * 1000
	fmt.printf("\nTotal time: %fms (CPU freq %d)\n  Startup: %f (%f%%)\n  Read: %f (%f%%)\n  MiscSetup: %f (%f%%)\n  Parse and Sum: %f (%f%%)\n  MiscOutput: %f (%f%%)", totalTime, cpuFreq, startup, startup / totalTime * 100, read, read / totalTime * 100, miscSetupValue, miscSetupValue / totalTime * 100, parseAndSum, parseAndSum / totalTime * 100, miscOutput, miscOutput / totalTime * 100)
}
