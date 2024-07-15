using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace actual_project
{
    public class Boardstate
    {
        public int generation;
        public int[,] board = new int[8, 8];
        public int points = 0;
        public bool hasKilled = false;
        public bool CPUturn=true;
        public int pieceMoved;
        public int MoveX;
        public int MoveY;
        public int MoveNo;
        public List<int> kingPieces = new List<int>();
        public List<int> piecesKilled = new List<int>(); 

        public List<Boardstate> nextGeneration = new List<Boardstate>();

        public Boardstate CreateChildNode(Boardstate ParentNode, Piece MovedPiece, int piecenum, int NewX, int NewY, bool hasKilled, List<Piece> piecesKilled, int count) //creates the boardstate of a possible move
        {
            Boardstate boardHolder = new Boardstate()
            {
                generation = ParentNode.generation + 1,
                kingPieces = new List<int>(ParentNode.kingPieces),
                board = ParentNode.board.Clone() as int[,],
                pieceMoved = piecenum,
                points = 0,
                MoveX = NewX,
                MoveY = NewY,
                MoveNo = count
            };
            boardHolder.hasKilled = hasKilled;
            if (boardHolder.generation % 2 == 1)
            {
                boardHolder.CPUturn = true;
            }
            else
            {
                boardHolder.CPUturn = false;
            }
            if (hasKilled)
            {
                foreach (var item in piecesKilled)
                {
                    boardHolder.piecesKilled.Add(ParentNode.board[item.x, item.y] - 1);
                    boardHolder.board[item.x, item.y] = 0;
                    if (boardHolder.CPUturn)
                    {
                        if (item.king)
                        {
                            boardHolder.points += 15;
                        }
                        else
                        {
                            boardHolder.points += 10;
                        }
                    }
                    else
                    {
                        if (item.king)
                        {
                            boardHolder.points -= 15;
                        }
                        else

                        {
                            boardHolder.points -= 10;
                        }
                    }
                }
            }
            else
            {
                boardHolder.piecesKilled.Clear();
            }
            if ((NewY == 0 && boardHolder.CPUturn) || (NewY == 7 && !boardHolder.CPUturn))
            {
                bool king = true;
                foreach (var item in boardHolder.kingPieces)
                {
                    if (ParentNode.board[MovedPiece.x, MovedPiece.y] - 1 == item)
                    {
                        king = false;
                    }
                }
                if (king)
                {
                    boardHolder.kingPieces.Add(ParentNode.board[MovedPiece.x, MovedPiece.y] - 1);
                    if (boardHolder.CPUturn)
                    {
                        boardHolder.points += 5;
                    }
                    else
                    {
                        boardHolder.points -= 5;
                    }
                }
            }
            boardHolder.points += ParentNode.points;
            boardHolder.board[NewX, NewY] = boardHolder.board[MovedPiece.x, MovedPiece.y];
            boardHolder.board[MovedPiece.x, MovedPiece.y] = 0;
            return boardHolder;
        }

    }
}
