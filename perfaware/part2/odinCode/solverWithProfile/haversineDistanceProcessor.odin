package solver

import "core:io"
import "core:os"
import "core:fmt"
import "core:slice"
import "core:strconv"
import "core:strings"
import "core:bytes"

ReadEntireFile :: proc(name : string) -> ([]u8, i64)
{
	block := BlockStart(#procedure)
	defer BlockEnd(block)

	openFile, _ := os.open(name, os.O_RDONLY)
	content, _ := os.read_entire_file_from_handle(openFile)
	size, _ := os.file_size(openFile)
	
	return content, size
}

HaversinePair :: struct
{
	x0 : f64,
	y0 : f64,
	x1 : f64,
	y1 : f64,
}

SumHaversineDistances :: proc(pairs : [dynamic]HaversinePair, size : i64)
{
	block := BlockStart(#procedure)
	defer BlockEnd(block)
	
	sum : f64
	sumCoef := 1 / cast(f64)len(pairs)
	for aPair in pairs
	{
		result : f64 = sumCoef * referenceHaversine(aPair.x0, aPair.y0, aPair.x1, aPair.y1, 6372.8)
		//fmt.println(aPair)
		//fmt.println(result)
		sum += result
	}
	
	pos := strings.index(os.args[1], "_") + 1
	lastPos := strings.last_index(os.args[1], "_")
	fmt.println("Pair count: ", os.args[1][pos:lastPos])
	fmt.println("Input size: ", size)
	fmt.println("Haversine sum: ", sum)

	if len(os.args) == 3
	{
		otherSum : f64
		name := os.args[2]
		openFile, _ := os.open(name, os.O_RDONLY)
		stream : io.Stream = os.stream_from_handle(openFile)
		//fmt.println("Start of binary:")
		counter := 0
		result : [8]byte
		for i := 0; i < len(pairs) * 8; i += 1
		{
			value, error := io.read_byte(stream)
			//fmt.println(value)
			result[counter] = value
			counter = (counter + 1)
			if counter == 8
			{
				values := transmute(f64)(result)
				//fmt.println(values)
				otherSum += values
				counter = 0
			}
			if error != .None
			{
				break
			}
		}
		/*
		test : f64
		fmt.println(size_of(test))
		r : bytes.Reader
		result, _ := ReadEntireFile(os.args[2])
		bytes.reader_init(&r, result)
		outcome, _ := bytes.reader_read_byte(&r)
		//fmt.println(outcome)
		arrayNumbers := slice.reinterpret([]f64, result[:8])
		testing := slice.reinterpret([]u8, result)
		otherSum : f64
		for index := 0; index < len(arrayNumbers); index += 1
		{
			otherSum += arrayNumbers[index]
			//fmt.println("Hello")
		}
		*/
		fmt.println("\nValidation")
		fmt.println("Reference sum: ", otherSum)
		difference := sum - otherSum
		fmt.println("Difference: ", difference)
	}
}

ParseHaversinePairs :: proc(json : string, size : i64) -> [dynamic]HaversinePair
{
	block := BlockStart(#procedure)
	defer BlockEnd(block)
	
	pairs :	[dynamic]HaversinePair
	//characters : [dynamic]u8
	quoteCount := 0
	values : [4]f64 = { 0.0, 0.0, 0.0, 0.0 }
	foundValueCount := 0
	
	{
		innerBlock := BlockStart("Lookup and Convert")
		defer BlockEnd(innerBlock)

		for index := 0; index < len(json); index += 1
		{
			character := json[index]
			//append(&characters, character)
			switch character 
			{
				case '"': // "
					for
					{
						index += 1
						character = json[index]
						//append(&characters, character)
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
						//append(&characters, character)
					}
					else
					{
						length := 0
						startingPosition := index
						for
						{
							index += 1
							character = json[index]
							//append(&characters, character)
							if character == ',' || character == '}'
							{
								break
							}
							length += 1
						}
						value := string(json[startingPosition:][:length])
						status : bool
						values[foundValueCount], status = strconv.parse_f64(value)
						foundValueCount += 1
						if foundValueCount >= 4
						{
							foundValueCount = 0
							//fmt.println(values)
							haversine : HaversinePair = { values[0], values[1], values[2], values[3] }
							append(&pairs, haversine)
							//result := referenceHaversine(values[0], values[1], values[2], values[3], 6372.8)
							//sum += result
						}

					}
			}
		}
	}

	return pairs
}

main :: proc()
{
	BeginProfile()
	defer EndAndPrintProfile()

	argSize := len(os.args)
	if argSize < 2 || argSize > 3
	{
		fmt.println("haversine_release [haversine_input.json]\nhaversine_release [haversine_input.json] [answers.f64]")
		return
	}
	else
	{
		content, size := ReadEntireFile(os.args[1])
		json := transmute(string)content
		pairs := ParseHaversinePairs(json, size)
		SumHaversineDistances(pairs, size)
	}	
}

