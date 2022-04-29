using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ConwayGameOfLife
{
    public partial class Form1 : Form
    {
        // The universe array
        bool[,] universe = new bool[10, 10];
        bool[,] scratchpad = new bool[10, 10];

        bool countType;
        bool nCount = true;
        // Drawing colors
        Color gridColor = Color.Black;
        Color cellColor = Color.Gray;

        // The Timer class
        Timer timer = new Timer();

        // Generation count
        int generations = 0;
        int numAlive = 0;

        public Form1()
        {
            InitializeComponent();

            // Setup the timer
            timer.Interval = 100; // milliseconds
            timer.Tick += Timer_Tick;
            timer.Enabled = false; // start timer running
        }

        private void Clear()
        {
            timer.Enabled = false;
            for (int i = 0; i < universe.GetLength(1); i++)
            {
                for (int j = 0; j < universe.GetLength(0); j++)
                {
                    universe[j, i] = false;
                }
            }
            graphicsPanel1.Invalidate();
        }
        private void Randomize(bool rngType, int seed)
        {
            Clear();
            Random rng = new Random();
            if (!rngType)
            {
                rng = new Random(seed);
            }
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    if (rng.Next(0, 3) == 0)
                    {
                        universe[x, y] = true;
                    }
                }
            }
        }

        // Calculate the next generation of cells
        private void NextGeneration()
        {
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    int neighborCount = 0;
                    scratchpad[x, y] = false;
                    if (countType)
                    {
                        neighborCount = CountNeighborsToroidal(x, y);
                    }
                    else
                    {
                        neighborCount = CountNeighborsFinite(x, y);
                    }
                    if (universe[x, y])
                    {
                        if (neighborCount < 2)
                        {
                            scratchpad[x, y] = false;
                        }
                        if (neighborCount > 3)
                        {
                            scratchpad[x, y] = false;
                        }
                        if (neighborCount == 2 || neighborCount == 3)
                        {
                            scratchpad[x, y] = true;
                        }
                    }
                    else
                    {
                        if (neighborCount == 3)
                        {
                            scratchpad[x, y] = true;
                        }
                    }
                }
            }
            bool[,] temp = universe;
            universe = scratchpad;
            scratchpad = temp;

            //sets numAlive back to 0
            numAlive = 0;

            //loops through new universe to check alive count
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    if (universe[x, y])
                    {
                        numAlive++;

                    }
                }
            }

            // Increment generation count
            generations++;

            // Update status strip generations
            toolStripStatusLabelGenerations.Text = "Generations = " + generations.ToString();

            //updates status strip number alive
            toolStripStatusLabel1.Text = "Alive = " + numAlive.ToString();

            graphicsPanel1.Invalidate();
        }

        // The event called by the timer every Interval milliseconds.
        private void Timer_Tick(object sender, EventArgs e)
        {
            NextGeneration();
        }

        private int CountNeighborsFinite(int x, int y)
        {
            int count = 0;
            int xLen = universe.GetLength(0);
            int yLen = universe.GetLength(1);

            for (int yOffset = -1; yOffset <= 1; yOffset++)
            {
                for (int xOffset = -1; xOffset <= 1; xOffset++)
                {
                    int xCheck = x + xOffset;
                    int yCheck = y + yOffset;

                    if (yOffset == 0 && xOffset == 0)
                    {
                        continue;
                    }
                    if (xCheck < 0)
                    {
                        continue;
                    }
                    if (yCheck < 0)
                    {
                        continue;
                    }
                    if (xCheck >= xLen)
                    {
                        continue;
                    }
                    if (yCheck >= yLen)
                    {
                        continue;
                    }

                    if (universe[xCheck, yCheck] == true)
                    {
                        count++;
                    }
                }
            }

            return count;
        }

        private int CountNeighborsToroidal(int x, int y)
        {
            int count = 0;
            int xLen = universe.GetLength(0);
            int yLen = universe.GetLength(1);

            for (int yOffset = -1; yOffset <= 1; yOffset++)
            {
                for (int xOffset = -1; xOffset <= 1; xOffset++)
                {
                    int xCheck = x + xOffset;
                    int yCheck = y + yOffset;

                    if (xOffset == 0 && yOffset == 0)
                    {
                        continue;
                    }

                    if (xCheck < 0)
                    {
                        xCheck = xLen - 1;
                    }
                    if (yCheck < 0)
                    {
                        yCheck = yLen - 1;
                    }
                    if (xCheck >= xLen)
                    {
                        xCheck = 0;
                    }
                    if (yCheck >= yLen)
                    {
                        yCheck = 0;
                    }

                    if (universe[xCheck, yCheck] == true)
                    {
                        count++;
                    }
                }
            }
            return count;
        }

        private void graphicsPanel1_Paint(object sender, PaintEventArgs e)
        {
            // Calculate the width and height of each cell in pixels
            // CELL WIDTH = WINDOW WIDTH / NUMBER OF CELLS IN X
            int cellWidth = graphicsPanel1.ClientSize.Width / universe.GetLength(0);
            // CELL HEIGHT = WINDOW HEIGHT / NUMBER OF CELLS IN Y
            int cellHeight = graphicsPanel1.ClientSize.Height / universe.GetLength(1);

            // A Pen for drawing the grid lines (color, width)
            Pen gridPen = new Pen(gridColor, 1);

            // A Brush for filling living cells interiors (color)
            Brush cellBrush = new SolidBrush(cellColor);

            // Iterate through the universe in the y, top to bottom
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                // Iterate through the universe in the x, left to right
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    // A rectangle to represent each cell in pixels
                    Rectangle cellRect = Rectangle.Empty;
                    cellRect.X = x * cellWidth;
                    cellRect.Y = y * cellHeight;
                    cellRect.Width = cellWidth;
                    cellRect.Height = cellHeight;

                    // Fill the cell with a brush if alive
                    if (universe[x, y] == true)
                    {
                        e.Graphics.FillRectangle(cellBrush, cellRect);

                        if (nCount)
                        {
                            Font font = new Font("Arial", 20f);

                            StringFormat stringFormat = new StringFormat();
                            stringFormat.Alignment = StringAlignment.Center;
                            stringFormat.LineAlignment = StringAlignment.Center;

                            int neighbors = CountNeighborsFinite(x, y);

                            if (neighbors > 3 || neighbors < 2)
                            {
                                e.Graphics.DrawString(neighbors.ToString(), font, Brushes.Crimson, cellRect, stringFormat);
                            }
                            else
                            {
                                e.Graphics.DrawString(neighbors.ToString(), font, Brushes.Green, cellRect, stringFormat);
                            }
                        }

                    }

                    // Outline the cell with a pen
                    e.Graphics.DrawRectangle(gridPen, cellRect.X, cellRect.Y, cellRect.Width, cellRect.Height);

                }
            }

            // Cleaning up pens and brushes
            gridPen.Dispose();
            cellBrush.Dispose();
        }

        private void graphicsPanel1_MouseClick(object sender, MouseEventArgs e)
        {
            // If the left mouse button was clicked
            if (e.Button == MouseButtons.Left)
            {
                // Calculate the width and height of each cell in pixels
                int cellWidth = graphicsPanel1.ClientSize.Width / universe.GetLength(0);
                int cellHeight = graphicsPanel1.ClientSize.Height / universe.GetLength(1);

                // Calculate the cell that was clicked in
                // CELL X = MOUSE X / CELL WIDTH
                int x = e.X / cellWidth;
                // CELL Y = MOUSE Y / CELL HEIGHT
                int y = e.Y / cellHeight;

                // Toggle the cell's state
                universe[x, y] = !universe[x, y];

                // Tell Windows you need to repaint
                graphicsPanel1.Invalidate();
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            timer.Enabled = true; //starts the timer
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            timer.Enabled = false; //stops the timer
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            //advances one generation
            NextGeneration();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //closes the program
            this.Close();
        }

        private void newToolStripButton_Click(object sender, EventArgs e)
        {
            Clear();
        }

        private void cellColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog dlg = new ColorDialog();
            dlg.Color = cellColor;

            if (DialogResult.OK == dlg.ShowDialog())
            {
                cellColor = dlg.Color;

                graphicsPanel1.Invalidate();
            }
        }

        private void gridColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog dlg = new ColorDialog();
            dlg.Color = gridColor;

            if (DialogResult.OK == dlg.ShowDialog())
            {
                gridColor = dlg.Color;

                graphicsPanel1.Invalidate();
            }
        }

        private void backColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog dlg = new ColorDialog();
            dlg.Color = graphicsPanel1.BackColor;

            if (DialogResult.OK == dlg.ShowDialog())
            {
                graphicsPanel1.BackColor = dlg.Color;

                graphicsPanel1.Invalidate();
            }
        }

        private void finiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            countType = false;
            finiteToolStripMenuItem.Checked = true;
            torodialToolStripMenuItem.Checked = false;
        }

        private void torodialToolStripMenuItem_Click(object sender, EventArgs e)
        {
            countType = true;
            finiteToolStripMenuItem.Checked = false;
            torodialToolStripMenuItem.Checked = true;
        }

        private void gridToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (gridToolStripMenuItem1.Checked == true)
            {
                gridColor = Color.Black;
                graphicsPanel1.Invalidate();
            }
            else
            {
                gridColor = graphicsPanel1.BackColor;
                graphicsPanel1.Invalidate();
            }
        }

        private void seedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SeedDialogue dlg = new SeedDialogue();

            if (DialogResult.OK == dlg.ShowDialog())
            {
                Randomize(false, dlg.Seed);
            }

            graphicsPanel1.Invalidate();
        }

        private void timeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Randomize(true, 0);
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OptionsDialog dlg = new OptionsDialog();

            dlg.timerInterval = timer.Interval;

            if (DialogResult.OK == dlg.ShowDialog())
            {
                timer.Interval = dlg.timerInterval;
                universe = new bool[dlg.universeWidth, dlg.universeHeight];
                scratchpad = new bool[dlg.universeWidth, dlg.universeHeight];
            }

            graphicsPanel1.Invalidate();
        }

        private void neighborCountToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (neighborCountToolStripMenuItem.Checked == true)
            {
                nCount = true;
            }
            else
            {
                nCount = false;
            }

            graphicsPanel1.Invalidate();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "All Files|*.*|Cells|*.cells";
            dlg.FilterIndex = 2; dlg.DefaultExt = "cells";

            if (DialogResult.OK == dlg.ShowDialog())
            {
                StreamWriter writer = new StreamWriter(dlg.FileName);

                writer.WriteLine("!" + DateTime.Now);

                for (int y = 0; y < universe.GetLength(1); y++)
                {
                    String currentRow = string.Empty;
                    for (int x = 0; x < universe.GetLength(0); x++)
                    {
                        if (universe[x,y])
                        {
                            currentRow += 'O';
                        }
                        else
                        {
                            currentRow += '.';
                        }
                    }
                    writer.WriteLine(currentRow);
                }
                writer.Close();
            }
        }
    }
}
