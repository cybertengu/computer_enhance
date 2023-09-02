package main

import "core:time"
import "core:os"
import "core:fmt"
import "core:runtime"
import "core:strconv"
import "core:math/rand"
import "core:intrinsics"
import "core:strings"
import "core:slice"

/*
	[-180, 180] -> x
	[-90, 90] -> y
	Need to be json format: no last comma on last element in the array. Pairs will have an array of x0, x1, y0, and y1.
	Need to read in args to get number of pairs.
	Need to print out the average.
	Need to make it as binary file.
	Need to make it as a json file.
	Need to use a random number generator. Need to avoid a natural converse to a value. I need to know the set.
*/

Vector2 :: struct {
	x: f64,
	y: f64,
}

main :: proc() 
{
	time.read_cycle_counter()
	argsSize := len(os.args)
	coordinatePairsAmount, randomSeed : int 
	isUniform : bool
	sum : f64 = 0
	if argsSize != 4
	{
		fmt.println("Usage: odinCode.exe [uniform/cluster] [random seed] [number of coordinate pairs to generate]")
	}
	else
	{
		//fmt.println(os.args[1])
		if os.args[1] == "uniform"
		{
			isUniform = true
			//fmt.println("Uniform")
		}
		else if os.args[1] == "cluster"
		{
			isUniform = false
			//fmt.println("Cluster")
		}
		else
		{
			fmt.print("Not sure what to do with value '")
			fmt.print(os.args[1])
			fmt.println("'")
			return
		}

		// NOTE(david): I noticed if I pass in letter into the atoi procedure, I will get back a 0.
		randomSeed = strconv.atoi(string(os.args[2]))
		coordinatePairsAmountString := os.args[3]
		//fmt.printf(coordinatePairsAmountString)
		coordinatePairsAmount = strconv.atoi(coordinatePairsAmountString)

		randomGenerator := rand.create(cast(u64)randomSeed)
		stringName := strings.builder_make()
		filename := fmt.sbprintf(&stringName, "%s%s%s", "data_", coordinatePairsAmountString, "_flex.json")
		//fmt.println(filename)
		binaryStringName := strings.builder_make()
		binaryFileName := fmt.sbprintf(&binaryStringName, "%s%s%s", "data_", coordinatePairsAmountString, "_haveranswer.f64")
		//fmt.println(binaryFileName)
		f, error := os.open(filename, os.O_WRONLY | os.O_RDONLY | os.O_CREATE)
		binaryFile, _ := os.open(binaryFileName, os.O_WRONLY | os.O_RDONLY | os.O_CREATE)
		defer os.close(binaryFile)
		defer os.close(f)
		if isUniform
		{
			if error != 0
			{
				fmt.println(error)
			}
			//file : os.Handle
			os.write_string(f, "{\"pairs\":[")
			currentSum : f64 = 0
			//fmt.println(coordinatePairsAmount)
			sumCoef := 1.0 / cast(f64)coordinatePairsAmount
			for i := 0; i < coordinatePairsAmount; i += 1
			{
				stringBuilder := strings.builder_make()

				// TODO(david): How should I handle the high value? It is not being included.
				x0 := rand.float64_range(-180, 180, &randomGenerator)
				y0 := rand.float64_range(-90, 90, &randomGenerator)
				x1 := rand.float64_range(-180, 180, &randomGenerator)
				y1 := rand.float64_range(-90, 90, &randomGenerator)
				strings.write_string(&stringBuilder, "{\"x0\":")
				strings.write_f64(&stringBuilder, x0, 'f')
				strings.write_string(&stringBuilder, ",\"y0\":")
				strings.write_f64(&stringBuilder, y0, 'f')
				strings.write_string(&stringBuilder, ",\"x1\":")
				strings.write_f64(&stringBuilder, x1, 'f')
				strings.write_string(&stringBuilder, ",\"y1\":")
				strings.write_f64(&stringBuilder, y1, 'f')
				strings.write_string(&stringBuilder, "}")
				if i != coordinatePairsAmount - 1
				{
					strings.write_string(&stringBuilder, ",")
				}
				os.write_string(f, strings.to_string(stringBuilder))
				currentSum = sumCoef * referenceHaversine(x0, y0, x1, y1, 6372.8)
				//fmt.println(currentSum)
				bytes := slice.bytes_from_ptr(&currentSum, size_of(currentSum))
				byteBuilder := strings.builder_make()
				strings.write_bytes(&byteBuilder, bytes)
				os.write_string(binaryFile, strings.to_string(byteBuilder))
				sum += currentSum
			}
			os.write_string(f, "]}")
			fmt.println("Method: uniform")
		}
		else
		{
			clusters := [64]Vector2{Vector2{rand.float64_range(-180, 180, &randomGenerator), rand.float64_range(-90, 90, &randomGenerator)},
						 Vector2{rand.float64_range(-180, 180, &randomGenerator), rand.float64_range(-90, 90, &randomGenerator)},
						 Vector2{rand.float64_range(-180, 180, &randomGenerator), rand.float64_range(-90, 90, &randomGenerator)},
						 Vector2{rand.float64_range(-180, 180, &randomGenerator), rand.float64_range(-90, 90, &randomGenerator)},
						 Vector2{rand.float64_range(-180, 180, &randomGenerator), rand.float64_range(-90, 90, &randomGenerator)},
						 Vector2{rand.float64_range(-180, 180, &randomGenerator), rand.float64_range(-90, 90, &randomGenerator)},
						 Vector2{rand.float64_range(-180, 180, &randomGenerator), rand.float64_range(-90, 90, &randomGenerator)},
						 Vector2{rand.float64_range(-180, 180, &randomGenerator), rand.float64_range(-90, 90, &randomGenerator)},
						 Vector2{rand.float64_range(-180, 180, &randomGenerator), rand.float64_range(-90, 90, &randomGenerator)},
						 Vector2{rand.float64_range(-180, 180, &randomGenerator), rand.float64_range(-90, 90, &randomGenerator)},
						 Vector2{rand.float64_range(-180, 180, &randomGenerator), rand.float64_range(-90, 90, &randomGenerator)},
						 Vector2{rand.float64_range(-180, 180, &randomGenerator), rand.float64_range(-90, 90, &randomGenerator)},
						 Vector2{rand.float64_range(-180, 180, &randomGenerator), rand.float64_range(-90, 90, &randomGenerator)},
						 Vector2{rand.float64_range(-180, 180, &randomGenerator), rand.float64_range(-90, 90, &randomGenerator)},
						 Vector2{rand.float64_range(-180, 180, &randomGenerator), rand.float64_range(-90, 90, &randomGenerator)},
						 Vector2{rand.float64_range(-180, 180, &randomGenerator), rand.float64_range(-90, 90, &randomGenerator)},
						 Vector2{rand.float64_range(-180, 180, &randomGenerator), rand.float64_range(-90, 90, &randomGenerator)},
						 Vector2{rand.float64_range(-180, 180, &randomGenerator), rand.float64_range(-90, 90, &randomGenerator)},
						 Vector2{rand.float64_range(-180, 180, &randomGenerator), rand.float64_range(-90, 90, &randomGenerator)},
						 Vector2{rand.float64_range(-180, 180, &randomGenerator), rand.float64_range(-90, 90, &randomGenerator)},
						 Vector2{rand.float64_range(-180, 180, &randomGenerator), rand.float64_range(-90, 90, &randomGenerator)},
						 Vector2{rand.float64_range(-180, 180, &randomGenerator), rand.float64_range(-90, 90, &randomGenerator)},
						 Vector2{rand.float64_range(-180, 180, &randomGenerator), rand.float64_range(-90, 90, &randomGenerator)},
						 Vector2{rand.float64_range(-180, 180, &randomGenerator), rand.float64_range(-90, 90, &randomGenerator)},
						 Vector2{rand.float64_range(-180, 180, &randomGenerator), rand.float64_range(-90, 90, &randomGenerator)},
						 Vector2{rand.float64_range(-180, 180, &randomGenerator), rand.float64_range(-90, 90, &randomGenerator)},
						 Vector2{rand.float64_range(-180, 180, &randomGenerator), rand.float64_range(-90, 90, &randomGenerator)},
						 Vector2{rand.float64_range(-180, 180, &randomGenerator), rand.float64_range(-90, 90, &randomGenerator)},
						 Vector2{rand.float64_range(-180, 180, &randomGenerator), rand.float64_range(-90, 90, &randomGenerator)},
						 Vector2{rand.float64_range(-180, 180, &randomGenerator), rand.float64_range(-90, 90, &randomGenerator)},
						 Vector2{rand.float64_range(-180, 180, &randomGenerator), rand.float64_range(-90, 90, &randomGenerator)},
						 Vector2{rand.float64_range(-180, 180, &randomGenerator), rand.float64_range(-90, 90, &randomGenerator)},
						 Vector2{rand.float64_range(-180, 180, &randomGenerator), rand.float64_range(-90, 90, &randomGenerator)},
						 Vector2{rand.float64_range(-180, 180, &randomGenerator), rand.float64_range(-90, 90, &randomGenerator)},
						 Vector2{rand.float64_range(-180, 180, &randomGenerator), rand.float64_range(-90, 90, &randomGenerator)},
						 Vector2{rand.float64_range(-180, 180, &randomGenerator), rand.float64_range(-90, 90, &randomGenerator)},
						 Vector2{rand.float64_range(-180, 180, &randomGenerator), rand.float64_range(-90, 90, &randomGenerator)},
						 Vector2{rand.float64_range(-180, 180, &randomGenerator), rand.float64_range(-90, 90, &randomGenerator)},
						 Vector2{rand.float64_range(-180, 180, &randomGenerator), rand.float64_range(-90, 90, &randomGenerator)},
						 Vector2{rand.float64_range(-180, 180, &randomGenerator), rand.float64_range(-90, 90, &randomGenerator)},
						 Vector2{rand.float64_range(-180, 180, &randomGenerator), rand.float64_range(-90, 90, &randomGenerator)},
						 Vector2{rand.float64_range(-180, 180, &randomGenerator), rand.float64_range(-90, 90, &randomGenerator)},
						 Vector2{rand.float64_range(-180, 180, &randomGenerator), rand.float64_range(-90, 90, &randomGenerator)},
						 Vector2{rand.float64_range(-180, 180, &randomGenerator), rand.float64_range(-90, 90, &randomGenerator)},
						 Vector2{rand.float64_range(-180, 180, &randomGenerator), rand.float64_range(-90, 90, &randomGenerator)},
						 Vector2{rand.float64_range(-180, 180, &randomGenerator), rand.float64_range(-90, 90, &randomGenerator)},
						 Vector2{rand.float64_range(-180, 180, &randomGenerator), rand.float64_range(-90, 90, &randomGenerator)},
						 Vector2{rand.float64_range(-180, 180, &randomGenerator), rand.float64_range(-90, 90, &randomGenerator)},
						 Vector2{rand.float64_range(-180, 180, &randomGenerator), rand.float64_range(-90, 90, &randomGenerator)},
						 Vector2{rand.float64_range(-180, 180, &randomGenerator), rand.float64_range(-90, 90, &randomGenerator)},
						 Vector2{rand.float64_range(-180, 180, &randomGenerator), rand.float64_range(-90, 90, &randomGenerator)},
						 Vector2{rand.float64_range(-180, 180, &randomGenerator), rand.float64_range(-90, 90, &randomGenerator)},
						 Vector2{rand.float64_range(-180, 180, &randomGenerator), rand.float64_range(-90, 90, &randomGenerator)},
						 Vector2{rand.float64_range(-180, 180, &randomGenerator), rand.float64_range(-90, 90, &randomGenerator)},
						 Vector2{rand.float64_range(-180, 180, &randomGenerator), rand.float64_range(-90, 90, &randomGenerator)},
						 Vector2{rand.float64_range(-180, 180, &randomGenerator), rand.float64_range(-90, 90, &randomGenerator)},
						 Vector2{rand.float64_range(-180, 180, &randomGenerator), rand.float64_range(-90, 90, &randomGenerator)},
						 Vector2{rand.float64_range(-180, 180, &randomGenerator), rand.float64_range(-90, 90, &randomGenerator)},
						 Vector2{rand.float64_range(-180, 180, &randomGenerator), rand.float64_range(-90, 90, &randomGenerator)},
						 Vector2{rand.float64_range(-180, 180, &randomGenerator), rand.float64_range(-90, 90, &randomGenerator)},
						 Vector2{rand.float64_range(-180, 180, &randomGenerator), rand.float64_range(-90, 90, &randomGenerator)},
						 Vector2{rand.float64_range(-180, 180, &randomGenerator), rand.float64_range(-90, 90, &randomGenerator)},
						 Vector2{rand.float64_range(-180, 180, &randomGenerator), rand.float64_range(-90, 90, &randomGenerator)},
						 Vector2{rand.float64_range(-180, 180, &randomGenerator), rand.float64_range(-90, 90, &randomGenerator)}}
			
			if error != 0
			{
				fmt.println(error)
			}
			os.write_string(f, "{\"pairs\":[")
			sumCoef := 1.0 / cast(f64)coordinatePairsAmount
			for i := 0; i < coordinatePairsAmount; 
			{
				for j := 0; j < 64 && i < coordinatePairsAmount; j += 1
				{
					stringBuilder := strings.builder_make()

					// TODO(david): How should I handle the high value? It is not being included.
					x0 := rand.float64_range(-clusters[j].x, clusters[j].x, &randomGenerator)
					y0 := rand.float64_range(-clusters[j].y, clusters[j].y, &randomGenerator)
					x1 := rand.float64_range(-clusters[j].x, clusters[j].x, &randomGenerator)
					y1 := rand.float64_range(-clusters[j].y, clusters[j].y, &randomGenerator)
					strings.write_string(&stringBuilder, "{\"x0\":")
					strings.write_f64(&stringBuilder, x0, 'f')
					strings.write_string(&stringBuilder, ",\"y0\":")
					strings.write_f64(&stringBuilder, y0, 'f')
					strings.write_string(&stringBuilder, ",\"x1\":")
					strings.write_f64(&stringBuilder, x1, 'f')
					strings.write_string(&stringBuilder, ",\"y1\":")
					strings.write_f64(&stringBuilder, y1, 'f')

					strings.write_string(&stringBuilder, "}")
					if i < coordinatePairsAmount - 1
					{
						strings.write_string(&stringBuilder, ",")
					}
					i += 1

					os.write_string(f, strings.to_string(stringBuilder))
					currentSum := sumCoef * referenceHaversine(x0, y0, x1, y1, 6372.8)
					//fmt.printf("%f %f %f %f %f\n", x0, y0, x1, y1, sumCoef)
					//fmt.println(currentSum)
					bytes := slice.bytes_from_ptr(&currentSum, size_of(currentSum))
					byteBuilder := strings.builder_make()
					strings.write_bytes(&byteBuilder, bytes)
					os.write_string(binaryFile, strings.to_string(byteBuilder))
					//fmt.println(bytes)
					//fmt.println(len(bytes))
					//test := slice.reinterpret([]f64, bytes)
					//fmt.println(test)
					sum += currentSum
				}
			}
			os.write_string(f, "]}")
			fmt.println("Method: cluster")
		}
		fmt.println("Random seed: ", randomSeed)
		fmt.println("Pair count: ", coordinatePairsAmountString)
		fmt.println("Expected sum: ", sum)
		openBinaryFile, _ := os.open(binaryFileName, os.O_RDONLY)
		/*fmt.println("Start")
		data, _ := os.read_entire_file_from_handle(openBinaryFile)
		fmt.println(data)
		fmt.println("End")*/
	}
}

GenerateRandomVector2 :: proc(x, y: f64, randomGenerator: ^rand.Rand) -> Vector2 {
	return Vector2{rand.float64_range(-x, x, randomGenerator), rand.float64_range(-y, y, randomGenerator)}
}
