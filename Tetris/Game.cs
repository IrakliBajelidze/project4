﻿// Emin Nuriyev - 819768073
// Irakli Bajelidze - 819818981

using System;
using System.Collections.Generic;
using System.Media;
using Newtonsoft.Json;
using System.IO;

namespace Tetris
{
    class Game
    {
        // sounds used in game
        public SoundPlayer clearSound = new SoundPlayer();
        public SoundPlayer stopSound = new SoundPlayer();
        public SoundPlayer rotationSound = new SoundPlayer();
        public SoundPlayer loss = new SoundPlayer();

        public int score = 0;
        public int level = 1;
        public int lines = 0;
        public int lines2 = 0;
        public int[,] nextPiece = new int[4, 4];    // represents 4x4 next piece which is displayd top right
        public int[,] board = new int[21, 10];  // represents board (only 18/21 are actually drawn)
        public int color;   // color of current moving piece

        public bool pieceExists;

        static Random rnd = new Random();

        public int roll = 0;    // roll determines which piece is next (0-27)
        public int prevRoll = 0;

        // These is the list of all pieces with all possible rotations
        // ! Here some pieces are added twice so that they have increased chance to be chosen
        int[,,] pieces = new int[28, 4, 4] {
            {                                // | piece
                { 0, 0, 0, 0 },
                { 0, 0, 0, 0 },
                { 1, 1, 1, 1 },
                { 0, 0, 0, 0 },
            },
            {
                { 0, 1, 0, 0 },
                { 0, 1, 0, 0 },
                { 0, 1, 0, 0 },
                { 0, 1, 0, 0 },
            },
            {                       // |- piece
                { 0, 0, 0, 0 },
                { 2, 2, 2, 0 },
                { 0, 2, 0, 0 },
                { 0, 0, 0, 0 },
            },
            {
                { 0, 0, 2, 0 },
                { 0, 2, 2, 0 },
                { 0, 0, 2, 0 },
                { 0, 0, 0, 0 },
            },
            {
                { 0, 0, 0, 0 },
                { 0, 2, 0, 0 },
                { 2, 2, 2, 0 },
                { 0, 0, 0, 0 },
            },
            {
                { 0, 2, 0, 0 },
                { 0, 2, 2, 0 },
                { 0, 2, 0, 0 },
                { 0, 0, 0, 0 },
            },
            {                       // |_ piece
                { 0, 0, 0, 0 },
                { 3, 3, 3, 0 },
                { 3, 0, 0, 0 },
                { 0, 0, 0, 0 },
            },
            {
                { 0, 3, 3, 0 },
                { 0, 0, 3, 0 },
                { 0, 0, 3, 0 },
                { 0, 0, 0, 0 },
            },
            {
                { 0, 0, 0, 0 },
                { 0, 0, 3, 0 },
                { 3, 3, 3, 0 },
                { 0, 0, 0, 0 },
            },
            {
                { 0, 3, 0, 0 },
                { 0, 3, 0, 0 },
                { 0, 3, 3, 0 },
                { 0, 0, 0, 0 },
            },
            {                   // _| piece
                { 0, 0, 0, 0 },
                { 4, 4, 4, 0 },
                { 0, 0, 4, 0 },
                { 0, 0, 0, 0 },
            },
            {
                { 0, 0, 4, 0 },
                { 0, 0, 4, 0 },
                { 0, 4, 4, 0 },
                { 0, 0, 0, 0 },
            },
            {
                { 0, 0, 0, 0 },
                { 4, 0, 0, 0 },
                { 4, 4, 4, 0 },
                { 0, 0, 0, 0 },
            },
            {
                { 0, 4, 4, 0 },
                { 0, 4, 0, 0 },
                { 0, 4, 0, 0 },
                { 0, 0, 0, 0 },
            },
            {                   // Z piece
                { 0, 0, 5, 0 },
                { 0, 5, 5, 0 },
                { 0, 5, 0, 0 },
                { 0, 0, 0, 0 },
            },
            {
                { 0, 0, 0, 0 },
                { 5, 5, 0, 0 },
                { 0, 5, 5, 0 },
                { 0, 0, 0, 0 },
            },
            {                   // S piece
                { 0, 6, 0, 0 },
                { 0, 6, 6, 0 },
                { 0, 0, 6, 0 },
                { 0, 0, 0, 0 },
            },
            {
                { 0, 0, 0, 0 },
                { 0, 6, 6, 0 },
                { 6, 6, 0, 0 },
                { 0, 0, 0, 0 },
            },
            {                   // square piece
                { 0, 0, 0, 0 },
                { 0, 7, 7, 0 },
                { 0, 7, 7, 0 },
                { 0, 0, 0, 0 },
            },
            {                   // square piece
                { 0, 0, 0, 0 },
                { 0, 7, 7, 0 },
                { 0, 7, 7, 0 },
                { 0, 0, 0, 0 },
            },
            {                   // square piece
                { 0, 0, 0, 0 },
                { 0, 7, 7, 0 },
                { 0, 7, 7, 0 },
                { 0, 0, 0, 0 },
            },
            {                   // square piece
                { 0, 0, 0, 0 },
                { 0, 7, 7, 0 },
                { 0, 7, 7, 0 },
                { 0, 0, 0, 0 },
            },
            {                                // | piece
                { 0, 0, 0, 0 },
                { 0, 0, 0, 0 },
                { 1, 1, 1, 1 },
                { 0, 0, 0, 0 },
            },
            {
                { 0, 1, 0, 0 },
                { 0, 1, 0, 0 },
                { 0, 1, 0, 0 },
                { 0, 1, 0, 0 },
            },
            {                   // S piece
                { 0, 6, 0, 0 },
                { 0, 6, 6, 0 },
                { 0, 0, 6, 0 },
                { 0, 0, 0, 0 },
            },
            {
                { 0, 0, 0, 0 },
                { 0, 6, 6, 0 },
                { 6, 6, 0, 0 },
                { 0, 0, 0, 0 },
            },
            {                   // Z piece
                { 0, 0, 5, 0 },
                { 0, 5, 5, 0 },
                { 0, 5, 0, 0 },
                { 0, 0, 0, 0 },
            },
            {
                { 0, 0, 0, 0 },
                { 5, 5, 0, 0 },
                { 0, 5, 5, 0 },
                { 0, 0, 0, 0 },
            }
        };

        public Game()
        {   
        }

        // sets everything to starting point
        public void StartOver()
        {
            for (int i = 0; i < 21; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    board[i, j] = 0;
                }
            }
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    nextPiece[i, j] = 0;
                }
            }


            score = 0;
            level = 1;
            lines = 0;
            lines2 = 0;
            color = 0;
            pieceExists = false;
            roll = 0;
            prevRoll = 0;
        }

        // saves current game member's states to json files
        public void SaveGame()
        {
            File.WriteAllText(Environment.CurrentDirectory + @"\JsonFiles\Save\board.json", JsonConvert.SerializeObject(board));
            File.WriteAllText(Environment.CurrentDirectory + @"\JsonFiles\Save\level.json", JsonConvert.SerializeObject(level));
            File.WriteAllText(Environment.CurrentDirectory + @"\JsonFiles\Save\score.json", JsonConvert.SerializeObject(score));
            File.WriteAllText(Environment.CurrentDirectory + @"\JsonFiles\Save\lines.json", JsonConvert.SerializeObject(lines));
            File.WriteAllText(Environment.CurrentDirectory + @"\JsonFiles\Save\lines2.json", JsonConvert.SerializeObject(lines2));
            File.WriteAllText(Environment.CurrentDirectory + @"\JsonFiles\Save\nextPiece.json", JsonConvert.SerializeObject(nextPiece));
            File.WriteAllText(Environment.CurrentDirectory + @"\JsonFiles\Save\color.json", JsonConvert.SerializeObject(color));
            File.WriteAllText(Environment.CurrentDirectory + @"\JsonFiles\Save\prevRoll.json", JsonConvert.SerializeObject(prevRoll));
            File.WriteAllText(Environment.CurrentDirectory + @"\JsonFiles\Save\pieceExists.json", JsonConvert.SerializeObject(pieceExists));
            File.WriteAllText(Environment.CurrentDirectory + @"\JsonFiles\Save\roll.json", JsonConvert.SerializeObject(roll));
            File.WriteAllText(Environment.CurrentDirectory + @"\JsonFiles\sfxMuted.json", JsonConvert.SerializeObject(Form1.sfxMuted));
        }

        // loads from json files, if they don't exist creates new
        public void LoadGame()
        {
            try
            {
                board = JsonConvert.DeserializeObject<int[,]>(File.ReadAllText(Environment.CurrentDirectory + @"\JsonFiles\Save\board.json"));
                score = JsonConvert.DeserializeObject<int>(File.ReadAllText(Environment.CurrentDirectory + @"\JsonFiles\Save\score.json"));
                lines = JsonConvert.DeserializeObject<int>(File.ReadAllText(Environment.CurrentDirectory + @"\JsonFiles\Save\lines.json"));
                lines2 = JsonConvert.DeserializeObject<int>(File.ReadAllText(Environment.CurrentDirectory + @"\JsonFiles\Save\lines2.json"));
                nextPiece = JsonConvert.DeserializeObject<int[,]>(File.ReadAllText(Environment.CurrentDirectory + @"\JsonFiles\Save\nextPiece.json"));
                color = JsonConvert.DeserializeObject<int>(File.ReadAllText(Environment.CurrentDirectory + @"\JsonFiles\Save\color.json"));
                prevRoll = JsonConvert.DeserializeObject<int>(File.ReadAllText(Environment.CurrentDirectory + @"\JsonFiles\Save\prevRoll.json"));
                pieceExists = JsonConvert.DeserializeObject<bool>(File.ReadAllText(Environment.CurrentDirectory + @"\JsonFiles\Save\pieceExists.json"));
                roll = JsonConvert.DeserializeObject<int>(File.ReadAllText(Environment.CurrentDirectory + @"\JsonFiles\Save\roll.json"));
                level = JsonConvert.DeserializeObject<int>(File.ReadAllText(Environment.CurrentDirectory + @"\JsonFiles\Save\level.json"));
                Form1.sfxMuted = JsonConvert.DeserializeObject<bool>(File.ReadAllText(Environment.CurrentDirectory + @"\JsonFiles\sfxMuted.json"));
            }
            catch
            {
                SaveGame();
            }
        }

        // chooses piece from pieces[,,] array at random
        public void RandomizeNextPiece()
        {
            prevRoll = roll;
            roll = rnd.Next(0, 28); // random number from 0 to 27 (including)
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    nextPiece[i, j] = 0;
                }
            }

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    nextPiece[i, j] = pieces[roll, i, j];
                }
            }
        }

        // places new piece (next piece) on board
        // returns false if piece can't be placed (game over)
        public bool AddNewPiece()
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (nextPiece[i, j] != 0)
                    {
                        color = nextPiece[i, j];
                        break;
                    }
                }
            }

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (nextPiece[i, j] != 0)
                    {
                        if(board[i, j + 3] != 0)
                        {
                            return false;
                        }
                        board[i, j + 3] = 8;
                    }
                }
            }
            pieceExists = true;
            return true;
        }

        // rotates piece if it is possible
        public void RotatePiece(bool clockwise)
        {
            try
            {
                for (int i = 0; i < 21; i++)
                {
                    for (int j = 0; j < 10; j++)
                    {
                        if (board[i, j] == 8)
                        {
                            if (prevRoll == 18 || prevRoll == 19 || prevRoll == 20 || prevRoll == 21) // square
                            {
                                return;
                            }
                            //
                            // l piece
                            //
                            else if (prevRoll == 0 || prevRoll == 22) // horizontal l
                            {
                                if (board[i - 1, j + 2] == 0 && board[i - 2, j + 2] == 0 && board[i + 1, j + 2] == 0)
                                {
                                    board[i, j] = 0;
                                    board[i, j + 1] = 0;
                                    board[i, j + 3] = 0;

                                    board[i - 1, j + 2] = 8;
                                    board[i - 2, j + 2] = 8;
                                    board[i + 1, j + 2] = 8;

                                    prevRoll = 1;
                                    return;
                                }
                                return;
                            }
                            else if (prevRoll == 1 || prevRoll == 23)  // vertical l
                            {
                                if (board[i + 2, j + 1] == 0 && board[i + 2, j - 1] == 0 && board[i + 2, j - 2] == 0)
                                {
                                    board[i, j] = 0;
                                    board[i + 1, j] = 0;
                                    board[i + 3, j] = 0;

                                    board[i + 2, j + 1] = 8;
                                    board[i + 2, j - 1] = 8;
                                    board[i + 2, j - 2] = 8;

                                    prevRoll = 0;
                                    return;
                                }
                                return;
                            }
                            //
                            // Z piece
                            //
                            else if (prevRoll == 14 || prevRoll == 26)
                            {
                                if (board[i + 2, j] == 0 && board[i + 1, j - 2] == 0)
                                {
                                    board[i, j] = 0;
                                    board[i + 1, j] = 0;

                                    board[i + 2, j] = 8;
                                    board[i + 1, j - 2] = 8;

                                    prevRoll = 15;
                                    return;
                                }
                                return;
                            }
                            else if (prevRoll == 15 || prevRoll == 27)
                            {
                                if (board[i - 1, j + 2] == 0 && board[i, j + 2] == 0)
                                {
                                    board[i, j] = 0;
                                    board[i + 1, j + 2] = 0;

                                    board[i - 1, j + 2] = 8;
                                    board[i, j + 2] = 8;

                                    prevRoll = 14;
                                    return;
                                }
                                return;
                            }
                            //
                            // S piece
                            //
                            else if (prevRoll == 17 || prevRoll == 25)
                            {
                                if (board[i - 1, j] == 0 && board[i + 1, j + 1] == 0)
                                {
                                    board[i + 1, j] = 0;
                                    board[i + 1, j - 1] = 0;

                                    board[i - 1, j] = 8;
                                    board[i + 1, j + 1] = 8;

                                    prevRoll = 16;
                                    return;
                                }
                                return;
                            }
                            else if (prevRoll == 16 || prevRoll == 24)
                            {
                                if (board[i + 2, j] == 0 && board[i + 2, j - 1] == 0)
                                {
                                    board[i, j] = 0;
                                    board[i + 2, j + 1] = 0;

                                    board[i + 2, j] = 8;
                                    board[i + 2, j - 1] = 8;

                                    prevRoll = 17;
                                    return;
                                }
                                return;
                            }

                            if (clockwise == true)
                            {
                                //
                                // T piece
                                //
                                if (prevRoll == 2) // t flipped over
                                {
                                    if (board[i - 1, j + 1] == 0)
                                    {
                                        board[i, j + 2] = 0;

                                        board[i - 1, j + 1] = 8;

                                        prevRoll = 3;
                                        return;
                                    }
                                    return;
                                }
                                else if (prevRoll == 3)
                                {
                                    if (board[i + 1, j + 1] == 0)
                                    {
                                        board[i + 2, j] = 0;

                                        board[i + 1, j + 1] = 8;

                                        prevRoll = 4;
                                        return;
                                    }
                                    return;
                                }
                                else if (prevRoll == 4)
                                {
                                    if (board[i + 2, j] == 0)
                                    {
                                        board[i + 1, j - 1] = 0;

                                        board[i + 2, j] = 8;

                                        prevRoll = 5;
                                        return;
                                    }
                                    return;
                                }
                                else if (prevRoll == 5)
                                {
                                    if (board[i + 1, j - 1] == 0)
                                    {
                                        board[i, j] = 0;

                                        board[i + 1, j - 1] = 8;

                                        prevRoll = 2;
                                        return;
                                    }
                                    return;
                                }
                                //
                                // L piece
                                //
                                else if (prevRoll == 6)
                                {
                                    if (board[i - 1, j] == 0 && board[i - 1, j + 1] == 0 && board[i + 1, j + 1] == 0)
                                    {
                                        board[i, j] = 0;
                                        board[i + 1, j] = 0;
                                        board[i, j + 2] = 0;

                                        board[i - 1, j] = 8;
                                        board[i - 1, j + 1] = 8;
                                        board[i + 1, j + 1] = 8;

                                        prevRoll = 7;
                                        return;
                                    }
                                    return;
                                }
                                else if (prevRoll == 7)
                                {
                                    if (board[i, j + 2] == 0 && board[i + 1, j] == 0 && board[i + 1, j + 2] == 0)
                                    {
                                        board[i, j] = 0;
                                        board[i, j + 1] = 0;
                                        board[i + 2, j + 1] = 0;

                                        board[i, j + 2] = 8;
                                        board[i + 1, j] = 8;
                                        board[i + 1, j + 2] = 8;

                                        prevRoll = 8;
                                        return;
                                    }
                                    return;
                                }
                                else if (prevRoll == 8)
                                {
                                    if (board[i + 2, j] == 0 && board[i + 2, j - 1] == 0 && board[i, j - 1] == 0)
                                    {
                                        board[i, j] = 0;
                                        board[i + 1, j] = 0;
                                        board[i + 1, j - 2] = 0;

                                        board[i + 2, j] = 8;
                                        board[i + 2, j - 1] = 8;
                                        board[i, j - 1] = 8;

                                        prevRoll = 9;
                                        return;
                                    }
                                    return;
                                }
                                else if (prevRoll == 9)
                                {
                                    if (board[i + 1, j - 1] == 0 && board[i + 1, j + 1] == 0 && board[i + 2, j - 1] == 0)
                                    {
                                        board[i, j] = 0;
                                        board[i + 2, j] = 0;
                                        board[i + 2, j + 1] = 0;

                                        board[i + 1, j - 1] = 8;
                                        board[i + 1, j + 1] = 8;
                                        board[i + 2, j - 1] = 8;

                                        prevRoll = 6;
                                        return;
                                    }
                                    return;
                                }
                                //
                                // _| piece
                                //
                                else if (prevRoll == 10)
                                {
                                    if (board[i - 1, j + 1] == 0 && board[i + 1, j + 1] == 0 && board[i + 1, j] == 0)
                                    {
                                        board[i, j] = 0;
                                        board[i, j + 2] = 0;
                                        board[i + 1, j + 2] = 0;

                                        board[i - 1, j + 1] = 8;
                                        board[i + 1, j + 1] = 8;
                                        board[i + 1, j] = 8;

                                        prevRoll = 11;
                                        return;
                                    }
                                    return;
                                }
                                else if (prevRoll == 11)
                                {
                                    if (board[i, j - 1] == 0 && board[i + 1, j - 1] == 0 && board[i + 1, j + 1] == 0)
                                    {
                                        board[i, j] = 0;
                                        board[i + 2, j] = 0;
                                        board[i + 2, j - 1] = 0;

                                        board[i, j - 1] = 8;
                                        board[i + 1, j - 1] = 8;
                                        board[i + 1, j + 1] = 8;

                                        prevRoll = 12;
                                        return;
                                    }
                                    return;
                                }
                                else if (prevRoll == 12)
                                {
                                    if (board[i, j + 1] == 0 && board[i, j + 2] == 0 && board[i + 2, j + 1] == 0)
                                    {
                                        board[i, j] = 0;
                                        board[i + 1, j] = 0;
                                        board[i + 1, j + 2] = 0;

                                        board[i, j + 1] = 8;
                                        board[i, j + 2] = 8;
                                        board[i + 2, j + 1] = 8;

                                        prevRoll = 13;
                                        return;
                                    }
                                    return;
                                }
                                else if (prevRoll == 13)
                                {
                                    if (board[i + 1, j - 1] == 0 && board[i + 1, j + 1] == 0 && board[i + 2, j + 1] == 0)
                                    {
                                        board[i, j] = 0;
                                        board[i, j + 1] = 0;
                                        board[i + 2, j] = 0;

                                        board[i + 1, j - 1] = 8;
                                        board[i + 1, j + 1] = 8;
                                        board[i + 2, j + 1] = 8;

                                        prevRoll = 10;
                                        return;
                                    }
                                    return;
                                }
                            }
                            else
                            {
                                //
                                // T piece
                                //
                                if (prevRoll == 2)
                                {
                                    if (board[i - 1, j + 1] == 0)
                                    {
                                        board[i, j] = 0;

                                        board[i - 1, j + 1] = 8;

                                        prevRoll = 5;
                                        return;
                                    }
                                    return;
                                }
                                else if (prevRoll == 3)
                                {
                                    if (board[i + 1, j + 1] == 0)
                                    {
                                        board[i, j] = 0;

                                        board[i + 1, j + 1] = 8;

                                        prevRoll = 2;
                                        return;
                                    }
                                    return;
                                }
                                else if (prevRoll == 4)
                                {
                                    if (board[i + 2, j] == 0)
                                    {
                                        board[i + 1, j + 1] = 0;

                                        board[i + 2, j] = 8;

                                        prevRoll = 3;
                                        return;
                                    }
                                    return;
                                }
                                else if (prevRoll == 5)
                                {
                                    if (board[i + 1, j - 1] == 0)
                                    {
                                        board[i + 2, j] = 0;

                                        board[i + 1, j - 1] = 8;

                                        prevRoll = 4;
                                        return;
                                    }
                                    return;
                                }
                                //
                                // L piece
                                //
                                else if (prevRoll == 6)
                                {
                                    if (board[i - 1, j + 1] == 0 && board[i + 1, j + 1] == 0 && board[i + 1, j + 2] == 0)
                                    {
                                        board[i, j] = 0;
                                        board[i + 1, j] = 0;
                                        board[i, j + 2] = 0;

                                        board[i - 1, j + 1] = 8;
                                        board[i + 1, j + 1] = 8;
                                        board[i + 1, j + 2] = 8;

                                        prevRoll = 9;
                                        return;
                                    }
                                    return;
                                }
                                else if (prevRoll == 7)
                                {
                                    if (board[i + 1, j] == 0 && board[i + 2, j] == 0 && board[i + 1, j + 2] == 0)
                                    {
                                        board[i, j] = 0;
                                        board[i, j + 1] = 0;
                                        board[i + 2, j + 1] = 0;

                                        board[i + 1, j] = 8;
                                        board[i + 2, j] = 8;
                                        board[i + 1, j + 2] = 8;

                                        prevRoll = 6;
                                        return;
                                    }
                                    return;
                                }
                                else if (prevRoll == 8)
                                {
                                    if (board[i, j - 1] == 0 && board[i, j - 2] == 0 && board[i + 2, j - 1] == 0)
                                    {

                                        board[i, j] = 0;
                                        board[i + 1, j] = 0;
                                        board[i + 1, j - 2] = 0;

                                        board[i, j - 1] = 8;
                                        board[i, j - 2] = 8;
                                        board[i + 2, j - 1] = 8;

                                        prevRoll = 7;
                                        return;
                                    }
                                    return;
                                }
                                else if (prevRoll == 9)
                                {
                                    if (board[i, j + 1] == 0 && board[i + 1, j + 1] == 0 && board[i + 1, j - 1] == 0)
                                    {
                                        board[i, j] = 0;
                                        board[i + 2, j] = 0;
                                        board[i + 2, j + 1] = 0;

                                        board[i, j + 1] = 8;
                                        board[i + 1, j + 1] = 8;
                                        board[i + 1, j - 1] = 8;

                                        prevRoll = 8;
                                        return;
                                    }
                                    return;
                                }
                                //
                                // _| piece
                                //
                                else if (prevRoll == 10)
                                {
                                    if (board[i - 1, j + 1] == 0 && board[i - 1, j + 2] == 0 && board[i + 1, j + 1] == 0)
                                    {
                                        board[i, j] = 0;
                                        board[i, j + 2] = 0;
                                        board[i + 1, j + 2] = 0;

                                        board[i - 1, j + 1] = 8;
                                        board[i - 1, j + 2] = 8;
                                        board[i + 1, j + 1] = 8;

                                        prevRoll = 13;
                                        return;
                                    }
                                    return;
                                }
                                else if (prevRoll == 11)
                                {
                                    if (board[i + 1, j - 1] == 0 && board[i + 1, j + 1] == 0 && board[i + 2, j + 1] == 0)
                                    {
                                        board[i, j] = 0;
                                        board[i + 2, j] = 0;
                                        board[i + 2, j - 1] = 0;

                                        board[i + 1, j - 1] = 8;
                                        board[i + 1, j + 1] = 8;
                                        board[i + 2, j + 1] = 8;

                                        prevRoll = 10;
                                        return;
                                    }
                                    return;
                                }
                                else if (prevRoll == 12)
                                {
                                    if (board[i, j + 1] == 0 && board[i + 2, j] == 0 && board[i + 2, j + 1] == 0)
                                    {
                                        board[i, j] = 0;
                                        board[i + 1, j] = 0;
                                        board[i + 1, j + 2] = 0;

                                        board[i, j + 1] = 8;
                                        board[i + 2, j] = 8;
                                        board[i + 2, j + 1] = 8;

                                        prevRoll = 11;
                                        return;
                                    }
                                    return;
                                }
                                else if (prevRoll == 13)
                                {
                                    if (board[i, j - 1] == 0 && board[i + 1, j - 1] == 0 && board[i + 1, j + 1] == 0)
                                    {
                                        board[i, j] = 0;
                                        board[i, j + 1] = 0;
                                        board[i + 2, j] = 0;

                                        board[i, j - 1] = 8;
                                        board[i + 1, j - 1] = 8;
                                        board[i + 1, j + 1] = 8;

                                        prevRoll = 12;
                                        return;
                                    }
                                    return;
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                return;
            }
        }

        // moves piece, parameter determines direction, ex. if "left" string is passed the piece moves left
        public void MovePiece(string direction)
        {
            int row = 0, col = 0;
            if (direction == "down")
            {
                row = 1;
            }
            else if (direction == "right")
            {
                col = 1;
            }
            else if (direction == "left")
            {
                col = -1;
            }
            else
            {
                throw new NotImplementedException();
            }


            if (PieceCanMove(direction) == true)
            {
                for (int i = 0; i < 21; i++)
                {
                    for (int j = 0; j < 10; j++)
                    {
                        if (board[i, j] == 8)
                        {
                            if (board[i + row * 1, j + col * 1] == 0)
                            {
                                board[i + row * 1, j + col * 1] = 9;
                                board[i, j] = 0;
                            }
                            else if (board[i + row * 1, j + col * 1] == 8)
                            {
                                if (board[i + row * 2, j + col * 2] == 0)
                                {
                                    board[i + row * 2, j + col * 2] = 9;
                                    board[i + row * 1, j + col * 1] = 9;
                                    board[i, j] = 0;
                                }
                                else if (board[i + row * 2, j + col * 2] == 8)
                                {
                                    if (board[i + row * 3, j + col * 3] == 0)
                                    {
                                        board[i + row * 3, j + col * 3] = 9;
                                        board[i + row * 2, j + col * 2] = 9;
                                        board[i + row * 1, j + col * 1] = 9;
                                        board[i, j] = 0;
                                    }
                                    else if (board[i + row * 3, j + col * 3] == 8)
                                    {
                                        board[i + row * 4, j + col * 4] = 9;
                                        board[i + row * 3, j + col * 3] = 9;
                                        board[i + row * 2, j + col * 2] = 9;
                                        board[i + row * 1, j + col * 1] = 9;
                                        board[i, j] = 0;
                                    }
                                    else
                                    {
                                        pieceExists = false;
                                    }
                                }
                                else
                                {
                                    pieceExists = false;
                                }
                            }
                            else
                            {
                                pieceExists = false;
                            }
                        }
                    }
                }
            }
            else
            {
                if (direction == "down")
                {
                    pieceExists = false;
                }
            }

            for (int i = 0; i < 21; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if (board[i, j] == 9)
                    {
                        board[i, j] = 8;
                    }
                }
            }
        }

        // is checker function for MovePiece() checks if requested move is valid, returns false if it's not
        private bool PieceCanMove(string direction)
        {
            int row = 0, col = 0, count = 0;

            if (direction == "down")
            {
                row = 1;
            }
            else if (direction == "right")
            {
                col = 1;
            }
            else if (direction == "left")
            {
                col = -1;
            }
            else
            {
                throw new NotImplementedException();
            }


            if (direction == "left")
            {
                try
                {
                    for (int i = 20; i >= 0; i--)
                    {
                        for (int j = 9; j >= 0; j--)
                        {
                            if (count == 4)
                            {
                                for (int r = 0; r < 21; r++)
                                {
                                    for (int c = 0; c < 10; c++)
                                    {
                                        if (board[r, c] == 9)
                                        {
                                            board[r, c] = 8;
                                        }
                                    }
                                }

                                return true;
                            }
                            else if (board[i, j] == 8)
                            {
                                count++;
                                board[i, j] = 9;

                                if (board[i + row * 1, j + col * 1] == 0)
                                {
                                    continue;
                                }
                                else if (board[i + row * 1, j + col * 1] == 8)
                                {
                                    count++;
                                    board[i + row * 1, j + col * 1] = 9;
                                    if (board[i + row * 2, j + col * 2] == 0)
                                    {
                                        continue;
                                    }
                                    else if (board[i + row * 2, j + col * 2] == 8)
                                    {
                                        count++;
                                        board[i + row * 2, j + col * 2] = 9;
                                        if (board[i + row * 3, j + col * 3] == 0)
                                        {
                                            continue;
                                        }
                                        else if (board[i + row * 3, j + col * 3] == 8)
                                        {
                                            count++;
                                            board[i + row * 3, j + col * 3] = 9;
                                            if (board[i + row * 4, j + col * 4] == 0)
                                            {
                                                continue;
                                            }
                                            else
                                            {
                                                for (int r = 0; r < 21; r++)
                                                {
                                                    for (int c = 0; c < 10; c++)
                                                    {
                                                        if (board[r, c] == 9)
                                                        {
                                                            board[r, c] = 8;
                                                        }
                                                    }
                                                }

                                                return false;

                                            }
                                        }
                                        else
                                        {
                                            for (int r = 0; r < 21; r++)
                                            {
                                                for (int c = 0; c < 10; c++)
                                                {
                                                    if (board[r, c] == 9)
                                                    {
                                                        board[r, c] = 8;
                                                    }
                                                }
                                            }

                                            return false;
                                        }
                                    }
                                    else
                                    {
                                        for (int r = 0; r < 21; r++)
                                        {
                                            for (int c = 0; c < 10; c++)
                                            {
                                                if (board[r, c] == 9)
                                                {
                                                    board[r, c] = 8;
                                                }
                                            }
                                        }
                                        return false;
                                    }
                                }
                                else
                                {
                                    for (int r = 0; r < 21; r++)
                                    {
                                        for (int c = 0; c < 10; c++)
                                        {
                                            if (board[r, c] == 9)
                                            {
                                                board[r, c] = 8;
                                            }
                                        }
                                    }
                                    return false;
                                }

                            }
                        }
                    }
                }
                catch
                {
                    return false;
                }
            }
            else
            {
                try
                {
                    for (int i = 0; i < 21; i++)
                    {
                        for (int j = 0; j < 10; j++)
                        {
                            if (count == 4)
                            {
                                for (int r = 0; r < 21; r++)
                                {
                                    for (int c = 0; c < 10; c++)
                                    {
                                        if (board[r, c] == 9)
                                        {
                                            board[r, c] = 8;
                                        }
                                    }
                                }
                                return true;
                            }
                            else if (board[i, j] == 8)
                            {
                                count++;
                                board[i, j] = 9;

                                if (board[i + row * 1, j + col * 1] == 0)
                                {
                                    continue;
                                }
                                else if (board[i + row * 1, j + col * 1] == 8)
                                {
                                    count++;
                                    board[i + row * 1, j + col * 1] = 9;
                                    if (board[i + row * 2, j + col * 2] == 0)
                                    {
                                        continue;
                                    }
                                    else if (board[i + row * 2, j + col * 2] == 8)
                                    {
                                        count++;
                                        board[i + row * 2, j + col * 2] = 9;
                                        if (board[i + row * 3, j + col * 3] == 0)
                                        {
                                            continue;
                                        }
                                        else if (board[i + row * 3, j + col * 3] == 8)
                                        {
                                            count++;
                                            board[i + row * 3, j + col * 3] = 9;
                                            if (board[i + row * 4, j + col * 4] == 0)
                                            {
                                                continue;
                                            }
                                            else
                                            {
                                                for (int r = 0; r < 21; r++)
                                                {
                                                    for (int c = 0; c < 10; c++)
                                                    {
                                                        if (board[r, c] == 9)
                                                        {
                                                            board[r, c] = 8;
                                                        }
                                                    }
                                                }

                                                return false;

                                            }
                                        }
                                        else
                                        {
                                            for (int r = 0; r < 21; r++)
                                            {
                                                for (int c = 0; c < 10; c++)
                                                {
                                                    if (board[r, c] == 9)
                                                    {
                                                        board[r, c] = 8;
                                                    }
                                                }
                                            }

                                            return false;
                                        }
                                    }
                                    else
                                    {
                                        for (int r = 0; r < 21; r++)
                                        {
                                            for (int c = 0; c < 10; c++)
                                            {
                                                if (board[r, c] == 9)
                                                {
                                                    board[r, c] = 8;
                                                }
                                            }
                                        }
                                        return false;
                                    }
                                }
                                else
                                {
                                    for (int r = 0; r < 21; r++)
                                    {
                                        for (int c = 0; c < 10; c++)
                                        {
                                            if (board[r, c] == 9)
                                            {
                                                board[r, c] = 8;
                                            }
                                        }
                                    }
                                    return false;
                                }

                            }
                        }
                    }
                }
                catch
                {
                    return false;
                }
            }
            return false;
        }

        // checks all collumns for full lines and returns List of ints where lines' indexes are stored
        public List<int> CheckColumns()
        {
            int count = 0, count2 = 0;
            var lines = new List<int>();

            for (int i = 20; i >= 3; i--)
            {
                for (int j = 9; j >= 0; j--)
                {
                    if (board[i, j] != 0)
                    {
                        count++;
                    }
                    else
                    {
                        break;
                    }

                    if (count == 10)
                    {
                        lines.Add(i);
                        count2++;
                    }
                }

                if (count2 == 4)
                {
                    break;
                }
                count = 0;
            }
            return lines;
        }

        // uses CheckColumns() above to remove corresponding column and award points
        // also increases lines, scores count and increases level every 10 lines
        public bool RemoveFullColumns()
        {
            var linesCleared = CheckColumns();
            int count = linesCleared.Count;

            if (count == 0)
            {
                return false;
            }
            else
            {          
                for (int z = 0; z < count; z++)
                {
                    for (int i = 0; i < linesCleared[count - 1] - 3; i++)
                    {
                        for (int j = 0; j < 10; j++)
                        {
                            board[linesCleared[count - 1] - i + z, j] = board[linesCleared[count - 1] - i + z - 1, j];
                        }
                    }
                }

                if (Form1.sfxMuted == false)
                {
                    clearSound.Play();
                }

                score += level * linesCleared.Count * 100;
                if(linesCleared.Count > 1)
                {
                    score += level * (linesCleared.Count - 1) * 50;
                }

                lines += linesCleared.Count;
                lines2 += linesCleared.Count;
                if(lines2 >= 10)
                {
                    level++;
                    lines2 -= 10;
                }

                return true;
            }
        }
    }
}
