using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Media; //Sounds
using System.Runtime.InteropServices; //Keyboard

namespace Pong
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll")]
        static extern ushort GetAsyncKeyState(int vKey);
        public static bool IsKeyPushedDown(Keys keyData)  // For fast keyboard input
        {
            return 0 != (GetAsyncKeyState((int)keyData) & 0x8000);
        }
        Point Ball;
        Point Ball_Direction;

        int Paddle_Left_Y = 128;
        int Paddle_Right_Y = 128;
        int Right_score = 0;
        int Left_score = 0;

        bool Game_paused = true;

        public Form1()
        {
            InitializeComponent();
            Ball = new Point(25, 13);
            Ball_Direction = new Point(3, 3);
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (IsKeyPushedDown(Keys.W)) Paddle_Left_Y -= 10;
            if (IsKeyPushedDown(Keys.X)) Paddle_Left_Y += 10;
            if (IsKeyPushedDown(Keys.P)) Paddle_Right_Y -= 10;
            if (IsKeyPushedDown(Keys.M)) Paddle_Right_Y += 10;

            if (IsKeyPushedDown(Keys.Space)) Game_paused = false; // Start ball motion following a <space>

            if (Paddle_Left_Y < 5) Paddle_Left_Y = 0;  // Clamp paddle positions
            if (Paddle_Left_Y > 251) Paddle_Left_Y = 251;
            if (Paddle_Right_Y < 5) Paddle_Right_Y = 0;
            if (Paddle_Right_Y > 251) Paddle_Right_Y = 251;
            if (IsKeyPushedDown(Keys.Escape)) Application.Exit();

            // Ball/Paddle collision detection on left paddle
            if ((Ball.X < 30) && (Math.Abs(Ball.Y - Paddle_Left_Y) < 50) && (Ball_Direction.X < 0))  
            {  
                Ball_Direction.X = -Ball_Direction.X;  
            }
             
            if ((Ball.X > 482) && (Math.Abs(Ball.Y - Paddle_Right_Y) < 50) && (Ball_Direction.X > 0))  
            {  
                Ball_Direction.X = -Ball_Direction.X;  
            }

            if (!Game_paused)
            {

                Ball.X += Ball_Direction.X;
                if ((Ball.X > 511) && (Ball_Direction.X > 0))   // Ball hit back wall travelling left
                {
                    //Ball_Direction.X = -Ball_Direction.X;    // Reverse horizontal motion
                    Ball.X = 30;                             // Restart ball in front of paddle
                    Ball.Y = Paddle_Left_Y;
                    Game_paused = true;
                    Left_score++;
                }
                if ((Ball.X < 1) && (Ball_Direction.X < 0)) // Ball hit back wall travelling right
                {
                    //Ball_Direction.X = -Ball_Direction.X;    // Reverse horizontal motion
                    Ball.X = 480;                            // Restart ball in front of paddle
                    Ball.Y = Paddle_Right_Y;
                    Game_paused = true;
                    Right_score++;
                }
                Ball.Y += Ball_Direction.Y;
                if ((Ball.Y > 255) && (Ball_Direction.Y > 0)) // Ball hit bottom wall travelling down
                {
                    Ball_Direction.Y = -Ball_Direction.Y;      // Reverse horizontal motion
                }
                if ((Ball.Y < 1) && (Ball_Direction.Y < 0))   // Ball hit top wall travelling down
                {
                    Ball_Direction.Y = -Ball_Direction.Y;   // Reverse horizontal motion
                }
            }

            Bitmap image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            Graphics g = Graphics.FromImage(image);
            g.FillEllipse(new SolidBrush(Color.Red), Ball.X - 10, Ball.Y - 10, 20, 20);
            g.FillRectangle(new SolidBrush(Color.Blue), 10, Paddle_Left_Y - 50, 10, 100);
            g.FillRectangle(new SolidBrush(Color.Blue), 490, Paddle_Right_Y - 50, 10, 100);
            g.Dispose();
            label1.Text = Left_score.ToString("0.#");
            label2.Text = Right_score.ToString("0.#");

            if ((Left_score > 2) || (Right_score > 2))  // Identify a winner
            {
                timer1.Stop();  // Stop game loop
                String text = "Right";
                if (Left_score > Right_score) text = "Left";
                DialogResult reply = MessageBox.Show(text + " player wins\rDo you wish to play again?", "Winner", MessageBoxButtons.YesNo);
                if (reply == DialogResult.Yes)
                {
                    Left_score = 0;     // Reset game values
                    Right_score = 0;
                    Paddle_Left_Y = 128;
                    Paddle_Right_Y = 128;
                    Game_paused = true;
                    Ball.X = 25; Ball.Y = 13;
                    Ball_Direction.X = 3; Ball_Direction.Y = 3;
                    timer1.Start();  // Restart game loop
                }
                else
                {
                    Application.Exit(); // Quit game
                }
            }


            pictureBox1.Image = image;

        }
    }
}
