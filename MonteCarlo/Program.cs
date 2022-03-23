using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Diagnostics;

namespace MonteCarlo
{
    public class Program
    {
        public Program()
        {
        }

        public static void Main(string[] args)
        {
            /*
             * Create seed as Candidate object
             * and take problem choice from user
             */
            Candidates seed = new Candidates();
            string? problem = null;
            while (problem == null || seed.board == null)
            {
                Console.Write("Solve which problem?");
                Console.WriteLine();
                problem = Console.ReadLine();
                if (problem == null)
                {
                    Console.Write("No problem provided");
                    Console.WriteLine();
                }
                seed = ImportSeed(problem);
                if (seed.board == null)
                {
                    Console.Write("Invalid problem");
                    Console.WriteLine();
                    continue;
                }
            }

            // Print inital space
            seed.Print();

            /*
             * Get initial info (gates required, iteration print
             * frequency) and report
             */
            Console.Write("What is the time limit (in seconds)?");
            Console.WriteLine();
            int timeLimitSec = Convert.ToInt32(Console.ReadLine());      // Set time limit
            int timeLimitMil = timeLimitSec * 1000;
            /*
            Console.Write("How often would you like an iteration to be printed?");
            Console.WriteLine();
            int iterPrint = Convert.ToInt32(Console.ReadLine());
            Console.Write("Printing every " + iterPrint + " iterations.");
            Console.WriteLine();
            */

            Candidates sol = new();

            sol = MonteCarlo(seed, timeLimitMil);


            // Print solution (if there is one)
            if (!(sol.board == null))
            {
                Console.WriteLine();
                Console.Write(new string('*', 30));
                Console.WriteLine();
                sol.Print();
                Console.Write(new string('*', 30));
            }

        }

        /*
         * ====================================================================================
         * DEPTH FIRST SEARCH:
         * Inputs: seed, maximum depth allowed, required gates, iteration print frequency
         * Output: goal state or null (if not goal is found)
         * ====================================================================================
         */
        private static Candidates MonteCarlo(Candidates seed, int timeLimitMil)
        {
            Stopwatch sw = Stopwatch.StartNew();
            while (true)
            {
                if (sw.ElapsedMilliseconds > timeLimitMil)
                {
                    break;
                }


            }

            return new Candidates();
        }

        private static Candidates Selection(Candidates node)
        {
            
            while (node.expanded)
            {
                node = best_ucb(node);
            }

            return new Candidates();
        }

        private static Candidates best_ucb(Candidates node)
        {
            return new Candidates();
        }

        



        /*
         * ====================================================================================
         * IMPORT SEED: 
         * Inputs: console input (problem letter)
         * Output: seed for that problem
         * ====================================================================================
         */
        public static Candidates ImportSeed(string problem)
        {
            Candidates seed = new Candidates();
            string fileName = "";

            switch (problem)
            {
                case "A":
                case "a":
                    fileName = "A.csv";
                    break;
                case "B":
                case "b":
                    fileName = "B.csv";
                    break;
                case "C":
                case "c":
                    fileName = "C.csv";
                    break;
                case "D":
                case "d":
                    fileName = "D.csv";
                    break;
                case "E":
                case "e":
                    fileName = "E.csv";
                    break;
                case "F":
                case "f":
                    fileName = "F.csv";
                    break;
                default:
                    return new Candidates();
            }

            // write file to string array
            string[][] csvLines = File.ReadAllLines(fileName).Select(l => l.Split(',').ToArray()).ToArray();

            // create matrix from array length
            int num_rows = csvLines.Length;
            int num_cols = csvLines[0].Length;
            string[,] strMatrix = new string[num_rows, num_cols];
            int[,] numMatrix = new int[num_rows, num_cols];

            // convert string array to matrix
            for (int r = 0; r < num_rows; r++)
            {
                for (int c = 0; c < num_cols; c++)
                {
                    strMatrix[r, c] = csvLines[r][c];
                    numMatrix[r, c] = Convert.ToInt32(strMatrix[r, c]);
                }
            }

            // set seed to integer matrix and give depth = 0;
            seed.board = numMatrix;

            return seed;
        }

    }
}