/*-----------------------------------Header---------------------------------------
  
  Project Name	     : Kaboom
  File Name		     : Bomb.cs
  Author		     : Ori Talmor
  Modified Date      : Wednesday, February 11, 2015
  Due Date		     : Wednesday, February 11, 2015
  Program Description: This file contains the bomb class which maintains
                       everything the bomb might need.This class creates an 
                       instance of the bomb, draws it, animates it, updates
                       its location based on the burglars location, checks if the
                       bomb needs to explode, animates it and much much more.
 --------------------------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Kaboom_
{
    class Bomb
    {
        #region Images:
        public static Texture2D bombImgRight; //The texture that holds the right bomb spritesheet
        public static Texture2D bombImgLeft;  //The texture that holds the left bomb spritesheet
        public Texture2D bombDisplayed;       //The texture that holds the spritesheet to be displayed
        public static Rectangle bombSrcRec;   //The source bounds for the bomb spritesheet
        public Rectangle bombRec;             //The bounds for the bomb
        public static Texture2D explodeImg;   //The texture that holds the exploding spritesheet
        public static Rectangle explodeSrcRec;//The source rectangle for the sprite sheet
        public Rectangle explodeRec;          //The bounds for the explosion
        #endregion

        #region Explosion Animation:
        private static int imgW;              //The width of a single frame from the bomb sprite sheet
        private static int imgH;              //The Height of a single frame from the bomb sprite sheet
        private static int imgFramesWide = 3; //The amount of frames wide that the sprite sheet is
        private static int imgFramesHigh = 1; //The amount of frames high that the sprite sheet is
        private int imgNumFrames = 3;         //The total number of frames in the sprite sheet
        private int imgFrameNum = 0;          //The current frame number being drawn
        private int animSmoothness = 6;       //The smoothness of the animation
        private int animDelayCount = 1;       //The delay between each frame
        private static int explodeW;          //The width of a single frame from the explosion sprite sheet
        private static int explodeH;          //The Height of a single frame from the explosion sprite sheet
        private static int explodingFramesWide = 3;  //The amount of frames wide that the explosion sprite sheet is
        private static int explodingFramesHigh = 1;  //The amount of frames high that the explosion sprite sheet is
        private int explodingFrameNum = 0;           //The current frame number being drawn
        private int explodingSmoothness = 2;         //The smoothness of the animation
        private int explodingDelayCount = 1;         //The delay between each frame
        #endregion

        #region Game Changers:
        public static int bombReleaseTime;              //The amount of time until each bomb is released
        public static int bombReleaseTimeRange = 25;    //The range for the time in which the bomb must be released
        public static short startBombReleaseTime = 10;  //The minimum range for the time in which the bomb must be released
        public static short  timer;                     //The timer that keeps track of time between each event
        #endregion

        #region Important Info:
        private const float GRAVITY = 9.81f / 2f;    //The factor of gravity that increments the bombs towards the buckets
        private Vector2 pos;                         //The bombs position
        public static int numBombsOnScreen = 0;      //The number of bombs that are currently on the screen
        public static int numBombs = 10;             //The total number of bombs in the current level
        public static bool levelFailed =  false;     //Is the level failed?
        public bool bombCaught = false;              //Was the bomb caught by the bucket?
        public bool isBombExploded = false;          //Has the bomb exploded?
        public static byte bombExplodedCounter = 0;  //The amount of time between each explosion
        #endregion

        #region Game States:
        public const int STATE_DRAW = 0;               //The draw state for the bombs
        public const int STATE_WAITINGTOEXPLODE = 1;   //The state in which the bomb waits to explode
        public const int STATE_BOMBEXPLODE = 2;        //The state in which the bomb explodes and animates
        public int state = 0;                          //The current state of the bomb instance
        public int stateChangeCounter = 0;             //The timer that checks if the proper time has
                                                       //passed between a change in state (waiting to explode and exploding specifically)
        #endregion

        /// <summary>
        /// Pre: The bombs position as an x and y value as a decimal float
        /// Post: Sets the position of the bomb to that of the given location
        /// Description: Gets the location of the bomb and sets it to the bombs new starting position
        /// </summary>
        /// <param name="x">The co-ordinate on the X-axis that corresponds to the bomb's location</param>
        /// <param name="y">The co-ordinate on the Y-axis that corresponds to the bomb's location</param>
        public Bomb(float x, float y)
        {
            //Set the bomb's position to that of the given position
            pos = new Vector2(x, y);

        }

        /// <summary>
        /// Post: Set the information needed inorder to animate
        /// Description: Set the required information in order to animate the bomb and explosion
        /// </summary>
        public static void LoadContent()
        {
            //Set the bomb image width
            imgW = bombImgRight.Width / imgFramesWide;
            //Set the bomb image height
            imgH = bombImgRight.Height / imgFramesHigh;

            //Set the source of the bomb to the image width and height of a single frame
            bombSrcRec = new Rectangle(0, 0, imgW, imgH);

            //Set the exploding image width
            explodeH = explodeImg.Height / explodingFramesHigh;
            //Set the bomb image height
            explodeW = explodeImg.Width / explodingFramesWide;

            //Set the source of the bomb to the image width and height of a single frame
            explodeSrcRec = new Rectangle(0, 0, explodeW, explodeH);
        }

        /// <summary>
        /// Pre: An instance of the burglar class
        /// Post: Calculate the image to display based on the burglar's direction
        /// Description: Change the image to be displated based on the direction of the burglar
        /// </summary>
        /// <param name="burglar">An instance of the burglar class</param>
        public void CalcDirection(Burglar burglar)
        {
            //If the burglar is moving in the positive direction:
            if (burglar.dir == 1)
            {
                //Set the texture to be drawn to that of the left sided bomb sprite sheet
                bombDisplayed = bombImgLeft;
            }

            //If the burglar is moving in the negative direction:
            else
            {
                //Set the texture to be drawn to that of the right sided bomb sprite sheet
                bombDisplayed = bombImgRight;
            }
        }

        /// <summary>
        /// Post: Animate the Bomb being drawn
        /// Description: Calculate the next frame needed to be drawn
        /// </summary>
        public void FrameAnimation()
        {
            //If enough frames have passed to update then set up next frame
            if (animDelayCount == 0)
            {
                //Calculate the next Source rectangle based on which frame we are currently on
                //Based on the current frame number and the number for frames wide the spread 
                //sheet is determine which column on the spreadsheet we are on
                int srcX = (imgFrameNum % imgFramesWide) * imgW;

                //Update the Source rectangle
                bombSrcRec = new Rectangle(srcX, bombSrcRec.Y, imgW, imgH);

                //Update the Frame number
                imgFrameNum = (imgFrameNum + 1) % imgNumFrames;
            }


            //Update the frames passed until next allowable update
            animDelayCount = (animDelayCount + 1) % animSmoothness;
        }

        /// <summary>
        /// Pre:  A batch of draw commands
        /// Post: Draw the bomb onto the screen
        /// Description: If the bomb isn't exploding, draw a normal bomb, otherwise draw the bomb exploding
        /// </summary>
        /// <param name="sb">A batch of draw commands</param>
        public void Draw(SpriteBatch sb)
        {
            //If the bomb isn't exploding:
            if (state != STATE_BOMBEXPLODE)
            {
                //Set the rectangle of the bomb to its needed position
                bombRec = new Rectangle((int)pos.X, (int)pos.Y, Bomb.imgW, Bomb.imgH);
                //Draw the bomb to the screen
                sb.Draw(bombDisplayed, bombRec, bombSrcRec, Color.White);
            }

            //If the bomb is exploding:
            else
            {
                //Set the rectangle of the exploding bomb to its needed position
                explodeRec = new Rectangle((int)pos.X, (int) pos.Y, Bomb.explodeW, Bomb.explodeH);
                //Draw the explosion to the screen
                sb.Draw(explodeImg, explodeRec, explodeSrcRec, Color.White);
            }
        }

        /// <summary>
        /// Post: Update the bomb's position based on gravity]
        /// Description: Increment the bomb's Y position by gravity
        /// </summary>
        public void UpdateY()
        {
            //Set the positiion of the bomb, to its previous position, where the Y coordinate is incremented by gravity
            pos = new Vector2(pos.X, pos.Y + GRAVITY);
        }

        /// <summary>
        /// Pre: The screen's height as an integer, and whether a bomb has hit the bottom of the screen, as a boolean
        /// Post: If a bomb has hit the bottom of the screen: the level is failed
        /// Description: Check if a bomb has hit the bottom of the screen, and if it has, the level is failed and the bomb's start waiting to explode
        /// </summary>
        /// <param name="screenH">The height of the screen</param>
        /// <param name="endOfLife">A temporary boolean that checks if a bomb hits the bottom of the screen</param>
        /// <returns>Returns a boolean that states whether or not a bomb hit the bottom of the screen</returns>
        public bool Collision(int screenH, bool endOfLife)
        {
            //If the bomb hits the bottom of the screen:
            if (bombRec.Y + bombRec.Height >= screenH)
            {
                //Set level to failed
                levelFailed = true;
                //Set the state of the bomb to waiting to explode
                state = STATE_WAITINGTOEXPLODE;
                //Set the end of the bomb's life to true
                endOfLife = true;
            }

            //Return the end of the bomb's life state
            return endOfLife;
        }

        /// <summary>
        /// Pre: A soundeffect of the explosion
        /// Post: Animate the bomb if it's set to explode and then play an exploding sound
        /// Description: Check if the bomb should be exploding, if so animate it and then play its sound
        /// </summary>
        /// <param name="explosion">A sound effect to be played when a bomb explodes</param>
        /// <param name="finalExplosion">A sound effect to be played when the last bomb explodes</param>
        /// <param name="lastBomb">A boolean returning whether it is the last bomb being exploded</param>
        /// <param name="gameMuted">A boolean returning whether the game has been muted</param>
        public void BombStateChange(SoundEffect explosion, SoundEffect finalExplosion ,bool lastBomb, bool gameMuted)
        {
            //If the bomb is waiting to explode:
            if (state == STATE_WAITINGTOEXPLODE)
            {
                //If the bomb has waited two frames:
                if (stateChangeCounter == 2)
                {
                    //Set the bomb's state to exploding
                    state = STATE_BOMBEXPLODE;
                    //Reset the counter to wait two seconds for the next bomb
                    stateChangeCounter = 0;
                }

                //If the bomb hasn't yet finished waiting two seconds:
                else
                {
                    //Increment the counter
                    stateChangeCounter++;
                }
            }

            //If the bomb should be exploding:
            if (state == STATE_BOMBEXPLODE)
            {
                //If enough frames have passed to update then set up next frame
                if (explodingDelayCount == 0)
                {
                    //Calculate the next Source rectangle based on which frame we are currently on
                    //Based on the current frame number and the number for frames wide the spread 
                    //sheet is determine which column on the spreadsheet we are on
                    int srcX = (explodingFrameNum % explodingFramesWide) * explodeW;

                    //Update the Source rectangle
                    explodeSrcRec = new Rectangle(srcX, explodeSrcRec.Y, explodeW, explodeH);

                    //If the bomb has animated through all its frames:
                    if (explodingFrameNum == explodingFramesWide - 1)
                    {
                        //If the last bomb has exploded:
                        if (lastBomb && !gameMuted)
                        {
                            //Play the final exploding sound
                            finalExplosion.Play();
                        }

                        //Otherwise, if any other bomb has exploded:
                        else if (!gameMuted)
                        {
                            //Play the exploding sound
                            explosion.Play();
                        }

                        //Set the bomb to finished exploding
                        isBombExploded = true;
                    }

                    //Update the Frame number
                    explodingFrameNum = (explodingFrameNum + 1) % imgNumFrames;
                }

                //Update the frames passed until next allowable update
                explodingDelayCount = (explodingDelayCount + 1) % explodingSmoothness;
            }
        }

        /// <summary>
        /// Post: Set the next time for the bomb to draw
        /// Description: Create a random number between a range of the release time, and set the bomb release time to the created random number
        /// </summary>
        public static void RandomizeReleaseTime()
        {
            //Create a random generator
            Random rnd = new Random();

            //Create a release time based on the range
            int release = rnd.Next(startBombReleaseTime, bombReleaseTimeRange);

            //Set the bomb's release time to that of the randomly generated one
            bombReleaseTime = release;
        }
    }
}
