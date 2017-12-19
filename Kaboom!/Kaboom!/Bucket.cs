/*-----------------------------------Header---------------------------------------
  
  Project Name	     : Kaboom
  File Name		     : Bucket.cs
  Author		     : Ori Talmor
  Modified Date      : Wednesday, February 11, 2015
  Due Date		     : Wednesday, February 11, 2015
  Program Description: This file contains the bucket class which maintains
                       everything the bucket might need.This class creates an 
                       instance of the bucket, draws it, animates it, and updates
                       its location based on the mouse location
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
    class Bucket
    {
        #region Images:
        public static Texture2D bucketImg; //The texture of the bucket
        public Rectangle bucketBounds;  //The bounds of the bucket
        public static Texture2D bombCaughtImg; //The picture when the bucket catches a bomb
        public static Rectangle waterSrcRec; //The source bounds for the water sprite sheet
        public static Rectangle waterBounds; //The bounds of the water animation
        private static int imgW;  //The width of each frame in the water sprite sheet
        private static int imgH;  //The height of each frame  in the water sprite sheet
        private static int imgFramesWide = 3; //The amount of frames wide for the water sprite sheet
        private static int imgFramesHigh = 1; //The amount of frames Hight for the water sprite sheet
        private int imgNumFrames = 3;    //The total number of frames in the spritesheet
        private int imgFrameNum = 0;  //The current frame being drawn
        private int animSmoothness = 4;  //The smoothness of the animation
        private int animDelayCount = 1;  //The delay of the animation
        #endregion

        #region Game Changers:
        public static bool isHitTopBucket = false;  //Did the bomb hit the top bucket?
        public static bool isHitMiddleBucket = false;  //Did the bomb hit the Middle bucket?
        public static bool isHitBottomBucket = false;  //Did the bomb hit the bottom bucket?
        public bool bombCaughtDraw = false;  //Does the image for catching the bomb need to be drawn?
        public bool isCollided = false;  //Is the bucket colliding with anything?
        #endregion

        #region Important Info:
        public Vector2 mouseLoc;  //The location of the mouse        
        public byte caughtDrawCounter = 0;  //Counter for how long the image of the bucket stays        
        public int lives = 3;  //The lives of the player
        public int score = 0;  //The players score
        public int prevLifeUp = 0;  //The previous score were the player got an extra life
        public int scoreMultiplier = 1;  //The multiplier for the score
        #endregion

        /// <summary>
        /// Post: set the player's mouse x coordinate location to that of the bucket
        /// Description: Bucket's Mouse Property: Get the location of the player's mouse. Set the location of the players mouse
        /// </summary>
        public float mouseX
        {
            //Get the buckets current location
            get { return mouseLoc.X; }
            //Set the buckets location to whatever value this subprogram was called to with.
            //Update the mouse's location by calling the UpdateFrame method
            set { mouseLoc.X = value; UpdateFrame(); }
        }
        
        /// <summary>
        /// Post: Update the location of the buckets.
        /// Descripition: Set the location on the bucket to the mouses current location
        /// </summary>
        public void UpdateFrame()
        {
            //Sret the bucket's x bounds to that of the mouse
            bucketBounds.X = (int)mouseLoc.X;
        }

        /// <summary>
        /// Pre: A spritebatch that helps the program draw to the screen
        /// Post: Draw the required buckets based on the amount of lives the player has.
        /// Description: Calculate the location of the buckets based on the amount of lives the player has, 
        /// and then draw the buckets to the screen
        /// </summary>
        /// <param name="sb">A spritebatch to allow drawing to the screen</param>
        /// <param name="isDifficult">A boolean that returns if the player wants to play in a higher difficulty setting</param>
        public void Draw(SpriteBatch sb, bool isDifficult)
        {
            //If the player wants to play in a harder difficulty setting
            if (isDifficult)
            {
                //Set the width of the bucket to the more difficult setting
                bucketBounds.Width = Convert.ToInt32(bucketImg.Width / 1.5f);
            }

            //Otherwise, if the player wants to player in an easier difficulty setting
            else
            {
                //Set the buckets width to that of the image
                bucketBounds.Width = bucketImg.Width;
            }

            //Set the buckets height to that of the image
            bucketBounds.Height = bucketImg.Height;

            //If the player has more than or equal to 1 life:
            if (lives >= 1)
            {
                //Draw the top bucket.
                sb.Draw(bucketImg, new Rectangle(bucketBounds.X, bucketBounds.Y, bucketBounds.Width, bucketBounds.Height), Color.White);
            }

            //If the player has more than or equal to 2 lives:
            if (lives >= 2)
            {
                //Draw the middle bucket
                sb.Draw(bucketImg, new Rectangle(bucketBounds.X, bucketBounds.Y + 36, bucketBounds.Width, bucketBounds.Height), Color.White);
            }

            //If the player has more than or equal to 3 lives:
            if (lives >= 3)
            {
                //Draw the top bucket
                sb.Draw(bucketImg, new Rectangle(bucketBounds.X, bucketBounds.Y + 72, bucketBounds.Width, bucketBounds.Height), Color.White);
            }
        }

        /// <summary>
        /// Pre: The graphics device to get the width of the screen
        /// Post: Offset the buckets if they go off screen
        /// Description: Checks to see if the bucket is off screen, and if it is it offsets it
        /// </summary>
        /// <param name="gd">Graphics Device needed to get the width of the screen</param>
        public void Collision(GraphicsDevice gd)
        {
            //If the bucket is colliding with the right side of the screen:
            if (bucketBounds.Width + bucketBounds.X >= gd.Viewport.Width - 5)
            {
                //Offset the bucket by a little to the left of its current location
                bucketBounds.X = gd.Viewport.Width - bucketBounds.Width - 5;
            }

            //If the bucket is colliding with the right side of the screen:
            else if(bucketBounds.X <= 5)
            {
                //Offset the bucket by a little to the right of the left side of the screen.
                bucketBounds.X = 5;
            }
        }

        /// <summary>
        /// Pre: A bomb instance used to check if the bomb was caught by the bucket, and a soundeffect to play when such happens
        /// Post: If a bomb is caught, the bomb is then dumped and the sound effect is played
        /// Description: If the bomb is within the colliding range of a bucket, the bomb is "collected" and a sound effect is played
        /// </summary>
        /// <param name="bomb">An instance of the bomb class</param>
        /// <param name="bombEffect">A sound effect for when the bomb is caught</param>
        /// <param name="gameMuted">A boolean returning whether the game has been muted</param>
        public void bombCaught(Bomb bomb, SoundEffect bombEffect, bool gameMuted)
        {
             //If the bomb intersects with the buckets on the X axis
            if ((bomb.bombRec.X >= bucketBounds.X) && (bomb.bombRec.X +
            bomb.bombRec.Width <= bucketBounds.X + bucketBounds.Width))
            {
                //If a bomb hasn't collided with the bucket
                if (!isCollided)
                {
                    //If the player has more than or equal to one life
                    if (lives >= 1)
                    {
                        //If the bomb intersects with the top bucket on the Y axis
                        if (((bomb.bombRec.Y + bomb.bombRec.Height) > bucketBounds.Y) &&
                        (bomb.bombRec.Y < bucketBounds.Y + bucketBounds.Height))
                        {
                            //If the game isn't muted:
                            if (!gameMuted)
                            {
                                //Play the soundeffect
                                bombEffect.Play();
                            }

                            //The bomb has been caught and an image needs to be displayed
                            bombCaughtDraw = true;
                            //The bomb hit the top bucket
                            isHitTopBucket = true;
                            //The bomb has been caught (needed to "dispose" of the bomb)
                            bomb.bombCaught = true;
                            //The bucket is now colliding with the bomb
                            isCollided = true;
                            //Increment the score by the multiplier
                            score += scoreMultiplier;

                            //If the score is greater than the previous required amount by at least 1000:
                            if (score - prevLifeUp >= 1000)
                            {
                                //If the player has less than 3 lives:
                                if (lives < 3)
                                {
                                    //Set the previous required score to the current score
                                    prevLifeUp = score;
                                    //Give the player an extra life
                                    lives++;
                                }
                            }
                        }
                    }
                }

                //If the bucket isn't colliding with a bomb:
                if (!isCollided)
                {
                    //If the player has more than or equal to 2 lives:
                    if (lives >= 2)
                    {
                        //If the bomb hasn't hit the top bucket already:
                        if (!isHitTopBucket)
                        {
                            //If the bomb intersects with the middle bucket on the Y axis
                            if (((bomb.bombRec.Y + bomb.bombRec.Height) > bucketBounds.Y + 36) &&
                            (bomb.bombRec.Y < bucketBounds.Y + 36 + bucketBounds.Height))
                            {

                                //If the game isn't muted:
                                if (!gameMuted)
                                {
                                    //Play the soundeffect
                                    bombEffect.Play();
                                }

                                //The bomb has been caught and an image needs to be displayed
                                bombCaughtDraw = true;
                                //The bomb hit the middle bucket
                                isHitMiddleBucket = true;
                                //The bomb has been caught (needed to "dispose" of the bomb)
                                bomb.bombCaught = true;
                                //The bucket is now colliding with the bomb
                                isCollided = true;
                                //Increment the score by the multiplier
                                score += scoreMultiplier;

                                //If the score is greater than the previous required amount by at least 1000:
                                if (score - prevLifeUp >= 1000)
                                {
                                    //If the player has less than 3 lives:
                                    if (lives < 3)
                                    {
                                        //Set the previous required score to the current score
                                        prevLifeUp = score;
                                        //Increment the amount of lives the player has
                                        lives++;
                                    }
                                }
                            }
                        }
                    }
                }

                //If the bucket isn't colliding with a bomb:
                if (!isCollided)
                {

                    //If the player has more than or equal to 3 lives:
                    if (lives >= 3)
                    {
                        //If the bomb hasn't hit the top or middle bucket yet:
                        if (!isHitTopBucket || !isHitMiddleBucket)
                        {
                            //If the bomb intersects with the bottom bucket on the Y axis
                            if (((bomb.bombRec.Y + bomb.bombRec.Height) > bucketBounds.Y + 72) &&
                            (bomb.bombRec.Y < bucketBounds.Y + 72 + bucketBounds.Height))
                            {
                                //If the game isn't muted:
                                if (!gameMuted)
                                {
                                    //Play the soundeffect
                                    bombEffect.Play();
                                }

                                //The bomb has been caught and an image needs to be displayed
                                bombCaughtDraw = true;
                                //The bomb hit the bottom bucket
                                isHitBottomBucket = true;
                                //The bomb has been caught (needed to "dispose" of the bomb)
                                bomb.bombCaught = true;
                                //The bucket is now colliding with the bomb
                                isCollided = true;
                                //Increment the score by the multiplier
                                score += scoreMultiplier;

                                //If the score is greater than the previous required amount by at least 1000:
                                if (score - prevLifeUp >= 1000)
                                {
                                    //If the player has less than 3 lives:
                                    if (lives < 3)
                                    {
                                        //Set the previous required score to the current score
                                        prevLifeUp = score;
                                        //Increment the amount of lives the player has
                                        lives++;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Post: Set the required values for the later animation
        /// Description: Calculate the required information needed to animate
        /// </summary>
        public void LoadAnimContent()
        {
            //Set the width of the image to the entire image width divided by # of frames wide
            imgW = bombCaughtImg.Width / imgFramesWide;
            //Set the height of the image to the entire image height divided by the # of frames high
            imgH = bombCaughtImg.Height / imgFramesHigh;

            //Set the source rectangle to the image width and height
            waterSrcRec = new Rectangle(0, 0, imgW, imgH);
        }

        /// <summary>
        /// Post: Animate the water when the bomb is caught in the bucket
        /// Description: Animate the water spritesheet for each frame of the picture
        /// </summary>
        public void AnimWater()
        {
            //If enough frames have passed to update then set up next frame
            if (animDelayCount == 0)
            {
                //Calculate the next Source rectangle based on which frame we are currently on
                //Based on the current frame number and the number for frames wide the spread 
                //sheet is determine which column on the spreadsheet we are on
                int srcX = (imgFrameNum % imgFramesWide) * imgW;

                //Update the Source rectangle
                waterSrcRec = new Rectangle(srcX, waterSrcRec.Y, imgW, imgH);

                //Update the Frame number
                imgFrameNum = (imgFrameNum + 1) % imgNumFrames;
            }

            //Update the frames passed until next allowable update
            animDelayCount = (animDelayCount + 1) % animSmoothness;
        }
    }
}
