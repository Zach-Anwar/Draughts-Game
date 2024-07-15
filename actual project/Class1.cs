using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace actual_project
{
    public class Piece
    {
        public Ellipse circle;
        public bool CPUpiece;
        public bool king = false;
        public int x;
        public int y;

        public Piece(int tilesize)
        {
            circle = new Ellipse
            {
                Height = tilesize,
                Width = tilesize
            };
            king = false;
        }
        public Piece[] UpdateValues(Boardstate currentBoard, Piece[] pieces, List<int> piecesKilled) //updates the values of the tempory pieces in findNextGeneration
        {
            foreach (var item in piecesKilled)
            {
                    pieces[item].x = -10;
                    pieces[item].y = -10;
            }
            for (int i = 0; i < 24; i++)
            {
                pieces[i].king = false;
            }
            foreach (var item in currentBoard.kingPieces)
            {
                pieces[item].king = true;
            }
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (currentBoard.board[i,j] > 0 )
                    {
                        pieces[currentBoard.board[i, j] - 1].x = i;
                        pieces[currentBoard.board[i, j] - 1].y = j;
                    }
                }
            }
            return pieces;
        }
    }
}
