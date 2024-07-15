
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace actual_project
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const int boardsize = 8;
        int piecenum; //used to store the piece that has been selected's x value
        const int tilesize = 100; //constant so i can easily adjust the size and width of the tiles and pieces
        bool CPUturn = false; //boolean to show if it is the CPU's or the players turn
        int PlayerMoveX;
        int PlayerMoveY;
        int count;
        bool[] validmove = new bool[8];
        int[] killPoint = new int[8];
        bool[] storer = new bool[4];
        bool killOpportunity = false;
        bool playerChecker;
        bool currentlyMoving = false;
        List<int> haveChecked = new List<int>();
        List<Piece> piecesKilled = new List<Piece>();
        Boardstate originalBoardstate = new Boardstate();
        bool check;
        DispatcherTimer CPUmove = new DispatcherTimer();
        Rectangle highlight = new Rectangle() //rectangle to highlight the tile a user selects
        {
            Height = tilesize,
            Width = tilesize,
            Fill = Brushes.Blue
        };
        Rectangle[] possibleMove = new Rectangle[4]; // to highlight the tiles where a selected piece could possibly move
        Rectangle[,] board = new Rectangle[8, 8]; //holds all 64 tiles of the board
        Piece[] pieces = new Piece[24]; // computer's and player's pieces stored here

        public MainWindow()
        {
            InitializeComponent();
            GameStart();
        }
        public void GameStart() // draws the boars, pieces and sets up the variables for the original boarstate 
        {
            CPUmove.Interval = new TimeSpan(0, 0, 0, 1, 00);
            CPUmove.Start();
            CPUmove.Tick += CPUmove_Tick;
            originalBoardstate.generation = 0;
            Height = 838;
            Width = 815;
            for (int i = 0; i < 4; i++)
            {
                possibleMove[i] = new Rectangle
                {
                    Width = tilesize,
                    Height = tilesize,
                    Fill = Brushes.Green
                };
                MyCanvas.Children.Add(possibleMove[i]);
            }
            for (int i = 0; i < boardsize; i++)
            {
                for (int j = 0; j < boardsize; j++)
                {
                    board[i, j] = new Rectangle();
                    board[i, j].Width = tilesize;
                    board[i, j].Height = tilesize;
                    if ((i + j) % 2 == 1)
                    {
                        board[i, j].Fill = Brushes.SaddleBrown;
                    }
                    else
                    {
                        board[i, j].Fill = Brushes.Peru;
                    }
                    Canvas.SetLeft(board[i, j], tilesize * i);
                    Canvas.SetTop(board[i, j], tilesize * j);
                    MyCanvas.Children.Add(board[i, j]); // creates the board
                }
            }
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    pieces[i + j * 4] = new Piece(tilesize);
                    if (j > 2)
                    {
                        pieces[i + j * 4].CPUpiece = true;
                        pieces[i + j * 4].circle.Fill = Brushes.Gray;
                        pieces[i + j * 4].x = (((j + 1) % 2) + i * 2);
                        pieces[i + j * 4].y = (j + 2);
                        Canvas.SetLeft(pieces[i + j * 4].circle, pieces[i + j * 4].x * tilesize);
                        Canvas.SetTop(pieces[i + j * 4].circle, pieces[i + j * 4].y * tilesize);
                        originalBoardstate.board[pieces[i + j * 4].x, pieces[i + j * 4].y] = i + j * 4 + 1;
                    }
                    else
                    {
                        pieces[i + j * 4].CPUpiece = false;
                        pieces[i + j * 4].circle.Fill = Brushes.PapayaWhip;
                        pieces[i + j * 4].x = ((j + 1) % 2) + i * 2;
                        pieces[i + j * 4].y = j;
                        Canvas.SetLeft(pieces[i + j * 4].circle, pieces[i + j * 4].x * tilesize);
                        Canvas.SetTop(pieces[i + j * 4].circle, pieces[i + j * 4].y * tilesize);
                        originalBoardstate.board[pieces[i + j * 4].x, pieces[i + j * 4].y] = i + j * 4 + 1;
                    }
                    MyCanvas.Children.Add(pieces[i + j * 4].circle);
                }
            }
            for (int i = 0; i < 24; i++)
            {
                if (pieces[i].CPUpiece == false && CPUturn == false)
                {
                    pieces[i].circle.MouseDown += Piece_MouseDown;
                }

            }
            possibleMove[0].MouseDown += MainWindow_MouseUp;
            possibleMove[2].MouseDown += MainWindow_MouseUp;
            possibleMove[1].MouseDown += MainWindow_MouseUp1;
            possibleMove[3].MouseDown += MainWindow_MouseUp1;
            CPUturn = false;
        }

        private void CPUmove_Tick(object sender, EventArgs e) //decides the computer move 
        {
            if (CPUturn)
            {
                count = 0;
                List<int> Holder = originalBoardstate.kingPieces.ToList();
                FindNextGeneration(originalBoardstate); //finds all the possible moves to the current boardstate
                if (originalBoardstate.nextGeneration.Count == 0)
                {
                    MessageBox.Show("player wins");
                    GameStart();
                }
                else
                {
                    foreach (var gen1Boardstate in originalBoardstate.nextGeneration) // finds all the possible responses to possible moves
                    {
                        FindNextGeneration(gen1Boardstate);
                        foreach (var gen2Boardstate in gen1Boardstate.nextGeneration) //finds the responses possible to that
                        {
                            FindNextGeneration(gen2Boardstate);
                            foreach (var gen3Boardstate in gen2Boardstate.nextGeneration) //finds the responses possilbe to that
                            {
                                FindNextGeneration(gen3Boardstate);
                                foreach (var gen4Boardstate in gen3Boardstate.nextGeneration) //finds the responses possilbe to that
                                {
                                    FindNextGeneration(gen4Boardstate);
                                    foreach (var gen5Boardstate in gen4Boardstate.nextGeneration) //finds the responses possilbe to that
                                    {
                                        FindNextGeneration(gen5Boardstate);
                                        foreach (var gen6Boardstate in gen5Boardstate.nextGeneration) //finds the responses possilbe to that
                                        {
                                            FindNextGeneration(gen6Boardstate);
                                        }
                                    }
                                }
                            }
                        }

                    }
                    int BestPoints = MiniMax(originalBoardstate, -180, 180);
                    bool breakLoop = false;
                    int count = 0;
                    do
                    {
                        if (BestPoints == originalBoardstate.nextGeneration.ElementAt(count).points)
                        {
                            originalBoardstate = originalBoardstate.nextGeneration.ElementAt(count);
                            breakLoop = true;
                        }
                        count++;
                    } while (!breakLoop);
                    Canvas.SetLeft(pieces[originalBoardstate.pieceMoved].circle, originalBoardstate.MoveX * tilesize);
                    Canvas.SetTop(pieces[originalBoardstate.pieceMoved].circle, originalBoardstate.MoveY * tilesize);
                    pieces[originalBoardstate.pieceMoved].x = originalBoardstate.MoveX;
                    pieces[originalBoardstate.pieceMoved].y = originalBoardstate.MoveY;
                    originalBoardstate.nextGeneration.Clear();
                    originalBoardstate.generation = 0;
                    originalBoardstate.kingPieces = Holder.ToList();
                    bool king = false;
                    foreach (var item in originalBoardstate.kingPieces)
                    {
                        if (originalBoardstate.pieceMoved == item)
                        {
                            king = true;
                        }
                    }
                    if (originalBoardstate.MoveY == 0 && !king)
                    {
                        kingPiece(originalBoardstate.pieceMoved);
                    }
                    if (originalBoardstate.hasKilled)
                    {
                        foreach (var item in originalBoardstate.piecesKilled)
                        {
                            pieces[item].x = -10;
                            pieces[item].y = -10;
                            pieces[item].circle.Visibility = Visibility.Collapsed;
                        }
                    }
                }
                originalBoardstate.points = 0;
                pieces = pieces[0].UpdateValues(originalBoardstate, pieces, originalBoardstate.piecesKilled);
                foreach (var item in originalBoardstate.nextGeneration)
                {
                    originalBoardstate.nextGeneration.Remove(item);
                }
                CPUturn = false;
            }
        } 

        public int MiniMax(Boardstate CurrentNode, int alphaPointer, int betaPointer) //calculates the best move
        {
            int comparer;
            bool breakLoop = false;
            if (CurrentNode.generation % 2 == 1)
            { //minimum points found
                if (CurrentNode.generation == 7)
                {
                    comparer = CurrentNode.points;
                }
                else
                {
                    CurrentNode.points = 180;
                }
                foreach (var item in CurrentNode.nextGeneration)
                {
                    if (!breakLoop)
                    {
                        comparer = MiniMax(item, alphaPointer, betaPointer); // this causes the function to be recurisve until the final generation 
                        if (comparer < CurrentNode.points)
                        {
                            CurrentNode.points = comparer;
                        }
                        if (betaPointer < comparer)
                        {
                            betaPointer = comparer;
                        }
                        if (betaPointer <= alphaPointer)
                        {
                            breakLoop = true;
                        }
                    }
                }
                breakLoop = false;
                return CurrentNode.points;
            }
            else //max points found
            {
                if (CurrentNode.generation == 7)
                {
                    comparer = CurrentNode.points;
                }
                else
                {
                    CurrentNode.points = -180;
                }
                foreach (var item in CurrentNode.nextGeneration)
                {
                    if (!breakLoop)
                    {
                        comparer = MiniMax(item, alphaPointer, betaPointer); // this causes the function to be recurisve until the final generation 
                        if (comparer > CurrentNode.points)
                        {
                            CurrentNode.points = comparer;
                        }
                        if (alphaPointer > comparer)
                        {
                            alphaPointer = comparer;
                        }
                        if (betaPointer <= alphaPointer)
                        {
                            breakLoop = true;
                        }
                    }
                }
                breakLoop = false;
                return CurrentNode.points;
            }
        }

        public void FindNextGeneration(Boardstate currentGeneration)//finds all the possible boardstates of a subsequent move
        {
            Piece[] GenerationPieces = pieces.Clone() as Piece[];
            GenerationPieces = GenerationPieces[0].UpdateValues(currentGeneration, GenerationPieces, currentGeneration.piecesKilled);
            count = 0;
            MoveChecker(currentGeneration, GenerationPieces);
            if (killOpportunity)
            {
                int constant = currentGeneration.nextGeneration.Count;
                int numRemoved = 0;
                for (int i = 0; i < constant; i++)
                {
                    if (currentGeneration.nextGeneration.ElementAt(i - numRemoved).piecesKilled.Count == 0)
                    {
                        currentGeneration.nextGeneration.RemoveAt(i - numRemoved);
                        numRemoved++;
                    }
                }
            }
        }

        public void kingPiece(int piecenum) //sets a normal piece to a king piece
        {
            pieces[piecenum].king = true;
            if (pieces[piecenum].CPUpiece == false)
            {
                pieces[piecenum].circle.Fill = Brushes.Gold;
            }
            else
            {
                pieces[piecenum].circle.Fill = Brushes.Black;
            }
            originalBoardstate.kingPieces.Add(piecenum);
        }

        public void PieceMove(int x, int y, Piece currentPiece, int ychange, bool king, bool CPUMoveMap, int piecenum, Boardstate currentBoard) //finds all the possible moves for a piece in a boardstate
        {
            int repeats = 0;
            bool haveKilled = false;
            if (king)
            {
                repeats = 1;
            }
            else
            {
                for (int i = 0; i < 4; i++)
                {
                    validmove[4 + i] = false;
                }
            }
            for (int i = 0; i < repeats + 1; i++)
            {
                if (i == 1)
                {
                    ychange = ychange * -1;
                }
                if (x > -1)
                {
                    validmove[0 + 4 * i] = true;
                    validmove[1 + 4 * i] = true;
                }
                validmove[2 + 4 * i] = false;
                validmove[3 + 4 * i] = false;
                if ((x == 7) || ((y < 1) && (ychange == -1)) || ((y > 6) && (ychange == 1)) || x < 0)
                {
                    validmove[0 + 4 * i] = false;
                }
                else if (ychange + y < 8)
                {
                    if (currentBoard.board[x + 1, y + ychange] > 0) // y starts at 0 when it should start at 8
                    {
                        validmove[0 + 4 * i] = false;
                        if ((x < 6)
                            && (((y >= 2) && (ychange == -1)) || ((y <= 5) && (ychange == 1)))
                            && (currentBoard.board[x + 2, y + 2 * ychange] == 0)
                            && ((currentBoard.board[x + 1, y + ychange] > 12 && !CPUMoveMap) || (currentBoard.board[x + 1, y + ychange] < 13 && CPUMoveMap))
                            && x > -1)
                        {
                            validmove[2 + 4 * i] = true;
                            killOpportunity = true;
                            killPoint[0] = currentBoard.board[x + 1, y + ychange] - 1;
                            if (check)
                            {
                                foreach (var item in haveChecked)
                                {
                                    if (item == killPoint[0])
                                    {
                                        haveKilled = true;
                                        validmove[2 + 4 * i] = false;
                                    }
                                }
                            }
                            if (!haveKilled && !currentlyMoving)
                            {
                                piecesKilled.Add(pieces[killPoint[0]]);
                                haveChecked.Add(killPoint[0]);
                                DoubleKillCheck(x + 2, y + 2 * ychange, currentPiece, ychange, piecenum, CPUMoveMap, x, y, currentBoard);
                            }
                        }
                    }
                }
                if ((x < 1) || ((y < 1) && (ychange == -1)) || ((y > 6) && (ychange == 1)))
                {
                    validmove[1 + 4 * i] = false;
                }
                else
                {
                    if (ychange + y < 8)
                    {
                        if (currentBoard.board[x - 1, y + ychange] > 0)
                        {
                            validmove[1 + 4 * i] = false;
                            if ((x > 1)
                                && (((y >= 2) && (ychange == -1)) || ((y <= 5) && (ychange == 1)))
                                && (currentBoard.board[x - 2, y + 2 * ychange] == 0)
                                && ((currentBoard.board[x - 1, y + ychange] > 12 && !CPUMoveMap) || (currentBoard.board[x - 1, y + ychange] < 13 && CPUMoveMap)))
                            {
                                validmove[3 + 4 * i] = true;
                                killOpportunity = true;
                                killPoint[1] = currentBoard.board[x - 1, y + ychange] - 1;
                                if (check)
                                {
                                    foreach (var item in haveChecked)
                                    {
                                        if (item == killPoint[1])
                                        {
                                            haveKilled = true;
                                            validmove[3 + 4 * i] = false;
                                        }
                                    }
                                }
                                if (!haveKilled && !currentlyMoving)
                                {
                                    haveChecked.Add(killPoint[1]);
                                    piecesKilled.Add(pieces[killPoint[1]]);
                                    DoubleKillCheck(x - 2, y + 2 * ychange, currentPiece, ychange, piecenum, CPUMoveMap, x, y, currentBoard);
                                }
                            }
                        }
                    }
                }
                if (CPUturn && !killOpportunity && !check)
                {
                    if (validmove[0 + 4 * i])
                    {
                        currentBoard.nextGeneration.Add(currentBoard.CreateChildNode(currentBoard, currentPiece, piecenum, x + 1, y + ychange, false, piecesKilled, count));
                        if (currentBoard.generation == 0)
                        {
                            count++;
                        }
                    }
                    if (validmove[1 + 4 * i])
                    {

                        currentBoard.nextGeneration.Add(currentBoard.CreateChildNode(currentBoard, currentPiece, piecenum, x - 1, y + ychange, false, piecesKilled, count));
                        if (currentBoard.generation == 0)
                        {
                            count++;
                        }
                    }
                }
            }
        }

        public void MoveChecker(Boardstate currentBoard, Piece[] currentPositions)//finds all the possible moves that can be made in one turn using a boardstate as a reference
        {
            killOpportunity = false;
            check = false;
            int piecetype;
            int ychange; ;
            bool CPUMoveMap = true;
            if (!CPUturn || currentBoard.generation % 2 == 1)
            {
                CPUMoveMap = false;
            }
            if (CPUMoveMap)
            {
                piecetype = 12;
                ychange = -1;
            }
            else
            {
                piecetype = 0;
                ychange = 1;
            }
            for (int i = 0; i < 12; i++)
            {
                if (pieces[i + piecetype].x >= 0)
                {
                    PieceMove(currentPositions[i + piecetype].x, currentPositions[i + piecetype].y, currentPositions[i + piecetype], ychange, currentPositions[i + piecetype].king, CPUMoveMap, i + piecetype, currentBoard);
                    piecesKilled.Clear();
                    haveChecked.Clear();
                }
            }
        }

        public void DoubleKillCheck(int X, int Y, Piece currentPiece, int ychange, int piecenum, bool CPUMoveMap, int originalX, int originalY, Boardstate currentBoard)//checks for chain kill moves 
        {
            check = true;
            PieceMove(X, Y, currentPiece, ychange, pieces[piecenum].king, CPUMoveMap, piecenum, currentBoard);
            if (check)
            {
                check = false;
                if (CPUturn)
                {
                    currentBoard.nextGeneration.Add(currentBoard.CreateChildNode(currentBoard, currentPiece, piecenum, X, Y, true, piecesKilled, count));
                    piecesKilled.Clear();
                    count++;
                }
            }

        }

        private void Piece_MouseDown(object sender, MouseButtonEventArgs e) //takes the x and y of a click and converting that into the coordinate style of the board
        {
            if (CPUturn == false && !playerChecker)
            {
                playerChecker = false;
                for (int i = 0; i < 4; i++)
                {
                    MyCanvas.Children.Remove(possibleMove[i]);
                }
                highlight.Visibility = Visibility.Visible;
                Point click = e.GetPosition(this);
                int x = Convert.ToInt32(click.X);
                int y = Convert.ToInt32(click.Y);
                x = x / tilesize;
                y = y / tilesize;
                Canvas.SetLeft(highlight, x * tilesize);
                Canvas.SetTop(highlight, y * tilesize);
                PlayerTurn(x, y);
            }
        }

        private void PlayerTurn(int x, int y) //calculates the possible moves for a clicked piece and highlights the relevant tiles
        {
            int ychange;
            if (!playerChecker)
            {
                MoveChecker(originalBoardstate, pieces);
            }
            ychange = 1;
            piecenum = originalBoardstate.board[x, y] - 1;
            int repeats = 0;
            if (pieces[piecenum].king)
            {
                repeats = 1;
            }
            else
            {
                for (int i = 0; i < 4; i++)
                {
                    validmove[4 + i] = false;
                }
            }
            for (int i = 0; i < repeats + 1; i++)
            {
                if (i == 1)
                {
                    ychange = ychange * -1;
                }
                validmove[2 + 4 * i] = false;
                validmove[3 + 4 * i] = false;
                if (playerChecker)
                {
                    highlight.Visibility = Visibility.Visible;
                }
                else
                {
                    validmove[2 + 4 * i] = false;
                    validmove[3 + 4 * i] = false;
                    if (killOpportunity == false)
                    {
                        validmove[0 + 4 * i] = true;
                        validmove[1 + 4 * i] = true;
                    }
                    else
                    {
                        validmove[0 + 4 * i] = false;
                        validmove[1 + 4 * i] = false;
                    }
                    if (x < 7 && y + ychange < 8 && y + ychange >= 0)
                    {
                        if (originalBoardstate.board[x + 1, y + ychange] != 0)
                        {
                            validmove[0 + 4 * i] = false;
                        }
                    }
                    else
                    {
                        validmove[0 + 4 * i] = false;
                    }
                    if (x > 0 && y + ychange < 8 && y + ychange >= 0)
                    {
                        if (originalBoardstate.board[x - 1, y + ychange] != 0)
                        {
                            validmove[1 + 4 * i] = false;
                        }
                    }
                    else
                    {
                        validmove[1 + 4 * i] = false;
                    }
                }
                if (x < 6 && y + 2 * ychange < 8 && y + 2 * ychange >= 0)
                {
                    if (originalBoardstate.board[x + 1, y + ychange] > 12 && originalBoardstate.board[x + 2, y + 2 * ychange] == 0)
                    {
                        validmove[2 + 4 * i] = true;
                        killPoint[0] = originalBoardstate.board[x + 1, y + ychange] - 1;
                    }
                }
                if (x > 1 && y + 2 * ychange < 8 && y + 2 * ychange >= 0)
                {
                    if (originalBoardstate.board[x - 1, y + ychange] > 12 && originalBoardstate.board[x - 2, y + 2 * ychange] == 0)
                    {
                        validmove[3 + 4 * i] = true;
                        killPoint[1] = originalBoardstate.board[x - 1, y + ychange] - 1;
                    }
                }
                if ((validmove[0 + 4 * i] || validmove[2 + 4 * i]))
                {
                    MyCanvas.Children.Add(possibleMove[0 + 2 * i]);
                    if (validmove[2 + 4 * i])
                    {
                        Canvas.SetLeft(possibleMove[0 + 2 * i], (x + 2) * tilesize);
                        Canvas.SetTop(possibleMove[0 + 2 * i], (y + 2 * ychange) * tilesize);
                    }
                    else
                    {
                        Canvas.SetLeft(possibleMove[0 + 2 * i], (x + 1) * tilesize);
                        Canvas.SetTop(possibleMove[0 + 2 * i], (y + ychange) * tilesize);
                    }
                }
                if (validmove[1 + 4 * i] || validmove[3 + 4 * i])
                {
                    MyCanvas.Children.Add(possibleMove[1 + 2 * i]);
                    if (validmove[3 + 4 * i])
                    {
                        Canvas.SetLeft(possibleMove[1 + 2 * i], (x - 2) * tilesize);
                        Canvas.SetTop(possibleMove[1 + 2 * i], (y + 2 * ychange) * tilesize);
                    }
                    else
                    {
                        Canvas.SetLeft(possibleMove[1 + 2 * i], (x - 1) * tilesize);
                        Canvas.SetTop(possibleMove[1 + 2 * i], (y + ychange) * tilesize);
                    }
                }
                PlayerMoveX = x;
                PlayerMoveY = y;
            }
        }
        private void MainWindow_MouseUp1(object sender, MouseButtonEventArgs e) //moves the player piece when a move is selected if the piece is moving towards x = 0
        {
            if (CPUturn == false)
            {
                int ychange;
                Point click = e.GetPosition(this);
                if (Convert.ToInt32(click.Y) > pieces[piecenum].y * tilesize)
                {
                    ychange = 1;
                }
                else
                {
                    ychange = -1;
                }
                highlight.Visibility = Visibility.Collapsed;
                for (int i = 0; i < 4; i++)
                {
                    MyCanvas.Children.Remove(possibleMove[i]);
                }
                originalBoardstate.board[pieces[piecenum].x, pieces[piecenum].y] = 0;
                if (validmove[3] || validmove[7])
                {
                    pieces[piecenum].x = PlayerMoveX - 2;
                    pieces[piecenum].y = PlayerMoveY + 2 * ychange;
                    Canvas.SetLeft(pieces[piecenum].circle, (PlayerMoveX - 2) * tilesize);
                    Canvas.SetTop(pieces[piecenum].circle, (PlayerMoveY + 2 * ychange) * tilesize);
                    originalBoardstate.board[pieces[killPoint[1]].x, pieces[killPoint[1]].y] = 0;
                    pieces[killPoint[1]].circle.Visibility = Visibility.Collapsed;
                    originalBoardstate.board[pieces[piecenum].x, pieces[piecenum].y] = piecenum + 1;
                    pieces[killPoint[1]].x = -10;
                    pieces[killPoint[1]].y = -10;
                    playerChecker = true;
                    PlayerTurn(PlayerMoveX - 2, PlayerMoveY + 2 * ychange);
                }
                else
                {
                    pieces[piecenum].x = PlayerMoveX - 1;
                    pieces[piecenum].y = PlayerMoveY + ychange;
                    Canvas.SetLeft(pieces[piecenum].circle, (PlayerMoveX - 1) * tilesize);
                    Canvas.SetTop(pieces[piecenum].circle, (PlayerMoveY + ychange) * tilesize);
                    originalBoardstate.board[pieces[piecenum].x, pieces[piecenum].y] = piecenum + 1;
                }
                if (!(validmove[2] || validmove[3] || validmove[6] || validmove[7]))
                {
                    CPUturn = true;
                    playerChecker = false;
                }
                if (pieces[piecenum].y == 7)
                {
                    kingPiece(piecenum);
                }
            }

        }

        private void MainWindow_MouseUp(object sender, MouseButtonEventArgs e) //moves the player piece when a move is selected if the piece is moving towards x = 7
        {
            if (!CPUturn)
            {
                int ychange;
                Point click = e.GetPosition(this);
                if (Convert.ToInt32(click.Y) > pieces[piecenum].y * tilesize)
                {
                    ychange = 1;
                }
                else
                {
                    ychange = -1;
                }
                highlight.Visibility = Visibility.Collapsed;
                for (int i = 0; i < 4; i++)
                {
                    MyCanvas.Children.Remove(possibleMove[i]);
                }
                originalBoardstate.board[pieces[piecenum].x, pieces[piecenum].y] = 0;
                if (validmove[2] || validmove[6])
                {
                    pieces[piecenum].x = PlayerMoveX + 2;
                    pieces[piecenum].y = PlayerMoveY + 2 * ychange;
                    Canvas.SetLeft(pieces[piecenum].circle, (PlayerMoveX + 2) * tilesize);
                    Canvas.SetTop(pieces[piecenum].circle, (PlayerMoveY + 2 * ychange) * tilesize);
                    originalBoardstate.board[pieces[killPoint[0]].x, pieces[killPoint[0]].y] = 0;
                    pieces[killPoint[0]].circle.Visibility = Visibility.Collapsed;
                    pieces[killPoint[0]].x = -10;
                    pieces[killPoint[0]].y = -10;
                    originalBoardstate.board[pieces[piecenum].x, pieces[piecenum].y] = piecenum + 1;
                    playerChecker = true;
                    PlayerTurn(PlayerMoveX + 2, PlayerMoveY + 2 * ychange);
                }
                else
                {
                    pieces[piecenum].x = PlayerMoveX + 1;
                    pieces[piecenum].y = PlayerMoveY + ychange;
                    Canvas.SetLeft(pieces[piecenum].circle, (PlayerMoveX + 1) * tilesize);
                    Canvas.SetTop(pieces[piecenum].circle, (PlayerMoveY + ychange) * tilesize);
                    originalBoardstate.board[pieces[piecenum].x, pieces[piecenum].y] = piecenum + 1;
                }
                if (!(validmove[2] || validmove[3] || validmove[6] || validmove[7]))
                {
                    CPUturn = true;
                    playerChecker = false;
                }
                if (pieces[piecenum].y == 7)
                {
                    kingPiece(piecenum);
                }
            }
        }
    }
}