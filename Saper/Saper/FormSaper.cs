using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Saper
{
    public partial class FormSaper : Form
    {
        enum GameSize
        {
            Small, Medium, Big
        };
        const int fieldSize = 30;
        Random generator = new Random();

        int widthMap;
        int heightMap;
        int minesMap;
        int openToWin;
        Button[,] fieldsMap;

        GameSize gameSize;

        public FormSaper()
        {
            InitializeComponent();

            gameSize = GameSize.Small;
            małaToolStripMenuItem_Click(null, null);
        }

        private void Generate(int widthMap, int heightMap, int minesMap)
        {
            panelGameButtons.Controls.Clear();
            this.widthMap = widthMap;
            this.heightMap = heightMap;
            this.minesMap = minesMap;
            this.openToWin = widthMap * heightMap - minesMap;
            fieldsMap = new Button[widthMap, heightMap];

            //generowanie buttonów
            for (int x = 0; x < widthMap; x++)
            {
                for (int y = 0; y < heightMap; y++)
                {
                    Button b = new Button();
                    b.Size = new Size(fieldSize, fieldSize);
                    b.Location = new Point(fieldSize * x, fieldSize * y);
                    fieldsMap[x, y] = b;
                    b.Click += Field_Click;
                    panelGameButtons.Controls.Add(b);
                }
            }
            //losowanie min
            do
            {
                int x = generator.Next(widthMap);
                int y = generator.Next(heightMap);
                if (fieldsMap[x, y].Name != "@")
                {
                    fieldsMap[x, y].Name = "@";
                    minesMap--;
                }
            } while (minesMap > 0);
            //obliczanie ilosci min w sąsiedztwie
            for (int x = 0; x < widthMap; x++)
            {
                for (int y = 0; y < heightMap; y++)
                {
                    if (fieldsMap[x, y].Name != "@")
                    {
                        int mineCount = 0;
                        for (int xx = x - 1; xx <= x + 1; xx++)
                        {
                            for (int yy = y - 1; yy <= y + 1; yy++)
                            {
                                if (xx >= 0 && xx < widthMap &&
                                   yy >= 0 && yy < heightMap &&
                                   fieldsMap[xx, yy].Name == "@")
                                {
                                    mineCount++;
                                }
                            }
                        }
                        if (mineCount > 0)
                        {
                            fieldsMap[x, y].Name = mineCount.ToString();
                        }
                    }
                }
            }
        }

        private void Field_Click(object sender, EventArgs e)
        {
            Button b = sender as Button;
            if (b.Name == "@")
            {
                EndGame("Niestety przegrałeś");
            }
            else
            {
                for (int x = 0; x < widthMap; x++)
                {
                    for (int y = 0; y < heightMap; y++)
                    {
                        if (fieldsMap[x, y] == b)
                        {
                            OpenField(x, y);
                        }
                    }
                }
            }
            if (openToWin == 0)
            {
                EndGame("Brawo - wygrałeś!");
            }
        }

        private void EndGame(string info)
        {
            OpenAllFields();
            if (MessageBox.Show("Czy chcesz jeszce jeden raz?",
                               info,
                               MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                switch (gameSize)
                {
                    case GameSize.Small: małaToolStripMenuItem_Click(null, null); break;
                    case GameSize.Medium: średniaToolStripMenuItem_Click(null, null); break;
                    case GameSize.Big: dużaToolStripMenuItem_Click(null, null); break;
                }
            }
            else
            {
                Close();
            }
        }

        private void OpenField(int x, int y)
        {
            if (fieldsMap[x, y].BackColor != Color.White)
            {
                fieldsMap[x, y].Text = fieldsMap[x, y].Name;
                fieldsMap[x, y].BackColor = Color.White;
                openToWin--;
                if (fieldsMap[x, y].Name == "")
                {
                    for (int xx = x - 1; xx <= x + 1; xx++)
                    {
                        for (int yy = y - 1; yy <= y + 1; yy++)
                        {
                            if (xx >= 0 && xx < widthMap &&
                               yy >= 0 && yy < heightMap)
                            {
                                OpenField(xx, yy);
                            }
                        }
                    }
                }
            }
        }

        private void OpenAllFields()
        {
            for (int x = 0; x < widthMap; x++)
            {
                for (int y = 0; y < heightMap; y++)
                {
                    fieldsMap[x, y].Text = fieldsMap[x, y].Name;
                    if (fieldsMap[x, y].Name == "@")
                    {
                        if (openToWin == 0)
                        {
                            fieldsMap[x, y].BackColor = Color.LightSalmon;
                        }
                        else
                        {
                            fieldsMap[x, y].BackColor = Color.Red;
                        }
                    }
                    else
                    {
                        if (openToWin == 0)
                        {
                            fieldsMap[x, y].BackColor = Color.White;
                        }
                        else
                        {
                            fieldsMap[x, y].BackColor = Color.LightSalmon;
                        }
                    }
                }
            }
        }
        private void małaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            gameSize = GameSize.Small;
            Generate(8, 8, 10);
        }

        private void średniaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            gameSize = GameSize.Medium;
            Generate(16, 16, 40);
        }

        private void dużaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            gameSize = GameSize.Big;
            Generate(30, 16, 99);
        }
    }
}
