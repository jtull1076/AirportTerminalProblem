using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Diagnostics;

namespace DFS
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
                seed = importSeed(problem);
                if (seed.board == null)
                {
                    Console.Write("Invalid problem");
                    Console.WriteLine();
                    continue;
                }
            }
            
            // Print inital space
            PrintCandidate(seed);

            /*
             * Get initial info (gates required, iteration print
             * frequency) and report
             */
            Console.Write("What is the maximum depth?");
            Console.WriteLine();
            int maxDepth = Convert.ToInt32(Console.ReadLine());      // Set maximum depth
            Console.Write("How many gates are required?");
            Console.WriteLine();
            int gatesRequired = Convert.ToInt32(Console.ReadLine());  // Set desired number of gates
            Console.Write("How often would you like an iteration to be printed?");
            Console.WriteLine();
            int iterPrint = Convert.ToInt32(Console.ReadLine());
            Console.Write("Printing every " + iterPrint + " iterations.");
            Console.WriteLine();

            // Start timer
            Stopwatch stopwatch = new();
            stopwatch.Start();
            // Run DFS
            Candidates sol = DepthFirstSearch(seed, maxDepth, gatesRequired, iterPrint);
            stopwatch.Stop();
            TimeSpan ts = stopwatch.Elapsed;
            // Stop timer


            // Print solution (if there is one)
            if (!(sol.board == null))
            {
                Console.WriteLine();
                Console.Write(new string('*', 30));
                Console.WriteLine();
                Console.Write("Solution found at depth " + sol.depth + " on iteration " + sol.iter + " after " + stopwatch.Elapsed + "!");
                Console.WriteLine();
                PrintCandidate(sol);
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
        private static Candidates DepthFirstSearch(Candidates seed, int maxDepth, int gatesRequired, int iterPrint)
        {
            Stack<Candidates> stack = new Stack<Candidates>();
            stack.Push(seed); // Push seed onto stack

            int depth = 0;
            int averageBranchingFactor = 0;
            int newStackCount = 0;
            int oldStackCount = 0;
            int branchingFactor = 0;
            int totalChildrenCreated = 0;
            int iter = 0;
            int branchingCount = 0;
            int gates = 0;

            Candidates N = new Candidates();

            while (!(stack.Count() == 0))
            {
                gates = 0;  // reset gate count
                iter++;     // add to iter count
                N = stack.Pop();    // pop candidate off stack
                N.iter = iter;          // set N iter #
                depth = N.depth;        // set N depth


                //--------------------------
                //--------------------------
                // Goal Detection
                //--------------------------
                if (N.gates >= gatesRequired)
                {
                    Console.WriteLine();
                    Console.Write("Average Branching Factor: " + totalChildrenCreated/branchingCount);
                    Console.WriteLine();
                    return N;
                }
                //--------------------------
                //--------------------------

                if (N.depth < maxDepth)
                { 
                    oldStackCount = stack.Count();
                    // Add children to stack, assigning depth = depth + 1
                    AddChildrenToStack(ref stack, N.board, depth + 1);
                    newStackCount = stack.Count();
                    branchingCount++;
                    totalChildrenCreated = totalChildrenCreated + (newStackCount - oldStackCount);

                }

                // Print iteration details if necessary
                if ((iter % iterPrint) == 0)
                {
                    Console.Write("# iter: " + iter + " , # on stack: " + stack.Count() + " , depth: " + depth + " , # gates: " + gates); ;
                    Console.WriteLine();
                    //PrintSpace(N);
                    //Console.WriteLine();
                }
            }

            // if stack is empty, then print no solution found
            if (stack.Count() == 0 && gates < gatesRequired)
            {
                Console.Write("No solution found within the given depth.");
                Console.WriteLine();
            }

            // return empty Candidate object if no solution is found
            return new Candidates();
        }

        /*
         * ====================================================================================
         * PRINT CANDIDATE: 
         * Inputs: candidate to print
         * Output: none, prints to console in-function
         * ====================================================================================
         */
        public static void PrintCandidate(Candidates candidateToPrint)
        {
            int[,] matrix = candidateToPrint.board;

            Console.Write(new string('=', matrix.GetLength(1) * 3));
            Console.WriteLine();

            // Printing space with formatting
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    if (matrix[i, j] == -1)
                    {
                        Console.BackgroundColor = ConsoleColor.Red;
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write("  " + matrix[i, j] + " ");
                    }
                    else if (matrix[i, j] == 0)
                    {
                        Console.BackgroundColor = ConsoleColor.DarkGreen;
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write("  " + matrix[i, j] + "  ");

                    }
                    else if (matrix[i, j] == 1)
                    {
                        Console.BackgroundColor = ConsoleColor.White;
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.Write("  " + matrix[i, j] + "  ");

                    }
                    else if (matrix[i, j] == 2)
                    {
                        Console.BackgroundColor = ConsoleColor.DarkBlue;
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write("  " + matrix[i, j] + "  ");

                    }
                }
                Console.BackgroundColor = ConsoleColor.Black;
                Console.WriteLine();
            }
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;

            Console.Write(new string('=', matrix.GetLength(1) * 3));
            Console.WriteLine();
        }




        /*
         * ====================================================================================
         * ADD CHILDREN TO STACK: 
         * Inputs: REF stack, parent space, depth
         * Output: none, adds to stack via reference
         * ====================================================================================
         */
        public static void AddChildrenToStack(ref Stack<Candidates> stack, int[,] parent, int depth)
        {
            /* 
             * Rule 1 and Rule 2 violation flags
             * true = rule violation | false = no rule violation
            */
            bool r1vio = true;
            bool r2vio = true;
            int gates = 0;

            /*
             * For each space on the candidate "board", check if the space creates 
             * a viable candidate (i.e. it doesn't violate r1 or r2). If it doesn't
             * violate r2, convert the space to a 1, add to queue (w/ obj val), 
             * then revert back to check r1, repeat with 2 if r1 = false
             */
            for (int i = 0; i < parent.GetLength(0); i++)
            {
                for (int j = 0; j < parent.GetLength(1); j++)
                {
                    gates = 0;

                    Candidates child = new Candidates();
                    child.board = (int[,])parent.Clone();
                    if (parent[i, j] == 0)
                    {
                        // Check rule 2
                        r2vio = CheckRule2(parent, new int[] { i, j });
                        
                        // r2vio = false: 0->1
                        if (!r2vio)
                        {
                            child.board[i, j] = 1;
                            child.depth = depth;

                            for (int u = 0; u < child.board.GetLength(0); u++)
                            {
                                for (int v = 0; v < child.board.GetLength(1); v++)
                                {
                                    if (child.board[u, v] == 2)
                                    {
                                        gates++;
                                    }
                                }
                            }

                            child.gates = gates;

                            stack.Push(child);

                            // on exit: 1->0
                            child.board = (int[,])parent.Clone();
                        }
                    }

                }
            }


            for (int i = 0; i < parent.GetLength(0); i++)
            {
                for (int j = 0; j < parent.GetLength(1); j++)
                {
                    gates = 0;
                    Candidates child = new Candidates();
                    child.board = (int[,])parent.Clone();
                    if (parent[i, j] == 0)
                    {
                        // Check rule 1
                        r1vio = CheckRule1(parent, new int[] { i, j });

                        // r1vio = true: 0->2
                        if (!r1vio)
                        {
                            child.board[i, j] = 2;
                            child.depth = depth;

                            for (int u = 0; u < child.board.GetLength(0); u++)
                            {
                                for (int v = 0; v < child.board.GetLength(1); v++)
                                {
                                    if (child.board[u, v] == 2)
                                    {
                                        gates++;
                                    }
                                }
                            }

                            child.gates = gates;

                            stack.Push(child);
                        }
                    }

                }
            }
        }

        /*
         * ====================================================================================
         * CHECK RULE 1: 
         * Inputs: Candidate "board", space n on board
         * Output: bool violation flag: true = violation, false = no violation
         * ====================================================================================
         */
        public static bool CheckRule1(int[,] matrix, int[] N)
        {
            int zeros = 0;  // counter for 0's on sides -> less than 3 is a violation
            int ones = 0;   // counter for 1's on sides -> less than 1 is a violation
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    if ((matrix[N[0] + i, N[1] + j] == -1) || ((j == 0) && (i == 0)) || !(i == 0 || j == 0))
                    {
                        continue;
                    }
                    if (matrix[N[0] + i, N[1] + j] == 1)
                    {
                        ones++;
                    }
                    else if (matrix[N[0] + i, N[1] + j] == 0)
                    {
                        zeros++;
                    }
                    else if (matrix[N[0] + i, N[1] + j] == 2)
                    {
                        return true;
                    }
                }
            }
            return (zeros != 3) || (ones < 1);
        }
        /*
         * ====================================================================================
         * CHECK RULE 2: 
         * Inputs: Candidate "board", space n on board
         * Output: bool violation flag: true = violation, false = no violation
         * ====================================================================================
         */
        public static bool CheckRule2(int[,] matrix, int[] N)
        {
            // ======================
            // Neighboring 1's Check: 
            // ======================
            int ones = 0; // Neighboring 1's counter -> must be at least 1
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    if (i == 0 && j == 0)
                    {
                        continue;
                    }
                    // Console.Write("Checking 1 at " + (N[0]+i) + ", " + (N[1]+j));
                    // Console.WriteLine();
                    if (matrix[N[0] + i, N[1] + j] == 1)
                    {
                        ones++;
                    }
                }
            }

            if (ones < 1)
            {
                return true;
            }

            // ==============================
            // Causes Rule 1 Violation Check: 
            // ==============================
            bool r1vio = false; // Rule 1 violation flag
            matrix[N[0], N[1]] = 1; // Let N be 1 to check for violations
            if ((!r1vio) && (matrix[N[0] - 1, N[1]] == 2))
            {
                r1vio = CheckRule1(matrix, new int[] { N[0] - 1, N[1] });
            }
            if ((!r1vio) && (matrix[N[0] + 1, N[1]] == 2))
            {
                r1vio = CheckRule1(matrix, new int[] { N[0] + 1, N[1] });
            }
            if ((!r1vio) && (matrix[N[0], N[1] - 1] == 2))
            {
                r1vio = CheckRule1(matrix, new int[] { N[0], N[1] - 1 });
            }
            if ((!r1vio) && (matrix[N[0], N[1] + 1] == 2))
            {
                r1vio = CheckRule1(matrix, new int[] { N[0], N[1] + 1 });
            }
            matrix[N[0], N[1]] = 0;

            return ((ones < 1) || r1vio);
        }

        /*
         * ====================================================================================
         * PRINT STACK: 
         * Inputs: stack to print
         * Output: none, prints via console in-function
         * ====================================================================================
         */
        public static void PrintStack(Stack<Candidates> stack)
        {
            if (stack.Count == 0)
            {
                return;
            }

            Candidates matrix = new Candidates();
            
            matrix = stack.Peek();
            PrintCandidate(matrix);

            stack.Pop();
            PrintStack(stack);
            stack.Push(matrix);

            Console.WriteLine();
        }

        /*
         * ====================================================================================
         * IMPORT SEED: 
         * Inputs: console input (problem letter)
         * Output: seed for that problem
         * ====================================================================================
         */
        public static Candidates importSeed(string problem)
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
            seed.depth = 0;

            return seed;
        }

        /*
         * ====================================================================================
         * (struct) CANDIDATES: 
         * Represents the candidate states (or nodes) in the tree
         * Attributes:
         *  - board: matrix representing the state's "board" (layout of the state)
         *  - depth: depth at which the state was found
         *  - iter: iteration at which the state dequeued
         *  - gates: number of gates in the state
         * ====================================================================================
         */
        public struct Candidates
        {
            public int[,] board;
            public int depth;
            public int iter;
            public int gates;
        }
    }
}