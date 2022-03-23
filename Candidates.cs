using System;

namespace MonteCarlo
{
	public Candidates()
	{
		/*
         * ====================================================================================
         * (class) CANDIDATES: 
         * Represents the candidate states (or nodes) in the tree
         * Attributes:
         *  - board: matrix representing the state's "board" (layout of the state)
         *  - depth: depth at which the state was found
         *  - iter: iteration at which the state dequeued
         *  - gates: number of gates in the state
         * ====================================================================================
         */
        public int[,]? board;
        public int gates;
        public double ucb;
        public bool visited = false;
        public bool expanded = false;
        public Candidates[]? successors;

        public void ExpandNode(Candidates node)
        {
            /* 
         * Rule 1 and Rule 2 violation flags
         * true = rule violation | false = no rule violation
        */
            bool r1vio = true;
            bool r2vio = true;

            /*
             * For each space on the candidate "board", check if the space creates 
             * a viable candidate (i.e. it doesn't violate r1 or r2). If it doesn't
             * violate r2, convert the space to a 1, add to queue (w/ obj val), 
             * then revert back to check r1, repeat with 2 if r1 = false
             */
            for (int i = 0; i < node.GetLength(0); i++)
            {
                for (int j = 0; j < node.GetLength(1); j++)
                {
                    Candidates child = new Candidates();
                    child.board = (int[,])node.Clone();

                    if (node[i, j] == 0)
                    {
                        // Check rule 2
                        r2vio = CheckRule2(node, new int[] { i, j });

                        // r2vio = false: 0->1
                        if (!r2vio)
                        {
                            child.board[i, j] = 1;
                            // on exit: 1->0
                            child.board = (int[,])node.Clone();
                        }

                        // Check rule 1
                        r1vio = CheckRule1(node, new int[] { i, j });

                        // r1vio = true: 0->2
                        if (!r1vio && node[i, j] == 0)
                        {
                            child.board[i, j] = 2;
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
        private static bool CheckRule1(int[,] matrix, int[] n)
        {
            int zeros = 0;  // counter for 0's on sides -> less than 3 is a violation
            int ones = 0;   // counter for 1's on sides -> less than 1 is a violation
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    if ((matrix[n[0] + i, n[1] + j] == -1) || ((j == 0) && (i == 0)) || !(i == 0 || j == 0))
                    {
                        continue;
                    }
                    if (matrix[n[0] + i, n[1] + j] == 1)
                    {
                        ones++;
                    }
                    else if (matrix[n[0] + i, n[1] + j] == 0)
                    {
                        zeros++;
                    }
                    else if (matrix[n[0] + i, n[1] + j] == 2)
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
        private static bool CheckRule2(int[,] matrix, int[] n)
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
                    if (matrix[n[0] + i, n[1] + j] == 1)
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
            matrix[n[0], n[1]] = 1; // Let N be 1 to check for violations
            if ((!r1vio) && (matrix[n[0] - 1, n[1]] == 2))
            {
                r1vio = CheckRule1(matrix, new int[] { n[0] - 1, n[1] });
            }
            if ((!r1vio) && (matrix[n[0] + 1, n[1]] == 2))
            {
                r1vio = CheckRule1(matrix, new int[] { n[0] + 1, n[1] });
            }
            if ((!r1vio) && (matrix[n[0], n[1] - 1] == 2))
            {
                r1vio = CheckRule1(matrix, new int[] { n[0], n[1] - 1 });
            }
            if ((!r1vio) && (matrix[n[0], n[1] + 1] == 2))
            {
                r1vio = CheckRule1(matrix, new int[] { n[0], n[1] + 1 });
            }
            matrix[n[0], n[1]] = 0;

            return ((ones < 1) || r1vio);
        }
    }
}
}
