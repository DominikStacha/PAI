using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace PA1
{
    public class NQueen
    {
        public readonly int N;

        public NQueen(int N)
        {
            this.N = N;
        }

        public void PrintBoard(bool[,] board)
        {
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    Console.Write(board[i, j] ? " X " : " O ");
                }

                Console.WriteLine();
            }

            Console.WriteLine();
        }

        private bool IsSafe(bool[,] board, int row, int col)
        {
            for (int i = 0; i < col; i++)
            {
                if (board[row, i])
                    return false;
            }

            for (int i = row, j = col; i >= 0 && j >= 0; i--, j--)
            {
                if (board[i, j])
                    return false;
            }

            for (int i = row, j = col; i < N && j >= 0; i++, j--)
            {
                if (board[i, j])
                    return false;
            }

            return true;
        }

        public ConcurrentBag<bool[,]> Solve()
        {
            ConcurrentBag<bool[,]> solutions = new ConcurrentBag<bool[,]>();
            List<Thread> threads = new List<Thread>();
            for (int i = 0; i < N; i++)
            {
                bool[,] board = new bool[N, N];
                board[i, 0] = true;
                Thread thread = new Thread(() =>
                {
                    Backtracking(board, 1, solutions);
                });
                thread.Start();
                threads.Add(thread);
            }

            threads.ForEach(t => t.Join());
            return solutions;
        }

        private bool Backtracking(bool[,] board, int col, ConcurrentBag<bool[,]> solutions)
        {
            for (int i = 0; i < N; i++)
            {
                if (!IsSafe(board, i, col)) continue;
                board[i, col] = true;
                // last column
                if (col == N - 1)
                {
                    bool[,] boardClone = (bool[,]) board.Clone();
                    solutions.Add(boardClone);
                }

                if (Backtracking(board, col + 1, solutions))
                {
                    return true;
                }

                board[i, col] = false;
            }

            return false;
        }
        public List<bool[,]> SolveSingleThread()
        {
            List<bool[,]> solutions = new List<bool[,]>();
            bool[,] board = new bool[N, N];
            BacktrackingSingleThread(board, 0, solutions);
            return solutions;
        }

        private bool BacktrackingSingleThread(bool[,] board, int col, List<bool[,]> solutions)
        {
            for (int i = 0; i < N; i++)
            {
                if (!IsSafe(board, i, col)) continue;
                board[i, col] = true;
                // last column
                if (col == N - 1)
                {
                    bool[,] boardClone = (bool[,])board.Clone();
                    solutions.Add(boardClone);
                }

                if (BacktrackingSingleThread(board, col + 1, solutions))
                {
                    return true;
                }

                board[i, col] = false;
            }

            return false;
        }
    }
}