/*-----------------------------------Header---------------------------------------
  
  Project Name	     : Kaboom
  File Name		     : Burglar.cs
  Author		     : Ori Talmor
  Modified Date      : Wednesday, February 11, 2015
  Due Date		     : Wednesday, February 11, 2015
  Program Description: This file contains the burglar class which maintains
                       everything the burglar might need.This class creates an 
                       instance of the burglar, draws it, animates it, and updates
                       its location based on a range created for the burglar
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
    class Burglar
    {
        #region Images:
        public static Texture2D burglarImage;  //The texture of the burglar
        public Rectangle burglarBounds;  //The location of the burglar image
        #endregion

        #region Game Changers:
        public static bool resetFinished;  //Is the burglar finished reseting to its original position
        public static float startSpeed = 0.75f;  //the minimum speed of the burglar
        public static float startRange = 10;  //The minimum speed of the burglar
        public static int startMinLevelTimer = 90;  //The minimum for the minimum time that the burglar can be spending in a direction
        public static int startMaxLevelTimer = 180; //The minimum for the maximum time that the burglar can be spending in a direction
        public int minLevelTimer = 90;  //The minimun time that the burglar can be spending in a direction
        public int maxLevelTimer = 180; //The maximum time that the burglar can be spending in a direction
        #endregion

        #region Important Info:
        public Vector2 loc;  //The location of the burglar
        public float range;  //The range that the burglar can move in
        public float speed;  //The speed at which the burglar can move at
        public int dirCounter;  //The counter that keeps track of next direction change        
        public float reverseTime;  //The Actual stored time for the next direction change
        public sbyte dir = 1;  //The direction
        private float testLocX;  //the location used for testing collision detection
        public static int screenWidth; //The width of the screen
        private static int resetCounter;  //Stabalize the speed at which the burglar resets
        #endregion

        /// <summary>
        /// Pre: The location of the burglar as an integer, its range, and it's speed both as a decimal
        /// Post: Saves the pre ^ data into the class
        /// Description: The constructor which stores the location of the burglar its speed and range when creating an instance of it
        /// <param name="location">The location of the burglar</param>
        /// <param name="burglarSpeed">The speed of the burglar</param>
        /// <param name="burglarRange">The range of the burglar</param>
        /// </summary>
        public Burglar(Vector2 location, float burglarRange, float burglarSpeed)
        {
            loc = location;  //Set the instance of the burglar class to the given location
            range = burglarRange; //Give the instance of the burglar class the given range
            speed = burglarSpeed; //Give the instance of the burglar class the given speed
        }

        /// <summary>
        /// Post: Updates the bounds of the burglar
        /// Description: When called, the subprogram updates the location of the burglar
        private void UpdateFrame()
        {
            //Update the bounds of the rectangle
            burglarBounds = new Rectangle((int)loc.X, (int)loc.Y+4, burglarImage.Width, burglarImage.Height);
        }

        /// <summary>
        /// Pre: The width of the screen as an integer.
        /// Post: Gives the burglar a new location based on his range
        /// Description: This subprogram recalculates the location of the burglar
        /// in hopes of getting it closer to its original position, based on its range
        /// <param name="screenWidth">The width of the screen</param>
        /// </summary>
        public void ResetLoc(int screenWidth)
        {
            //Increment the reset counter
            resetCounter++;

            //If the counter is equal to two:
            if (resetCounter == 2)
            {
                //Set the targeted position to the middle of the screen
                int startPositionX = (screenWidth / 2) - (burglarImage.Width / 2);
                //Create and initialize resetRange to equal the range of the burglar
                float resetRange = range;

                //If the burglar is not at its target position:
                if (!resetFinished)
                {
                    //Create a random generator
                    Random rnd = new Random();

                    //If the absolute value of the displacement between the burglar and the final position is less than the range:
                    if (Math.Abs(startPositionX - loc.X) < range)
                    {
                        //Set the range to the absolute difference between the target position and the burglar's current position
                        resetRange = Math.Abs(startPositionX - loc.X);
                    }

                    //Create and intitialize displacement to be a random number between 0 and the range
                    float displacement = rnd.Next(0, (int)resetRange + 1);
                    //Multiply the displacement by the burglar's speed and by two
                    displacement *= speed * 2;

                    //If the burglar's location is greater than the target location:
                    if (loc.X > startPositionX)
                    {
                        //Subtract the displacement from the burglars location
                        loc.X -= displacement;
                        //Reset the counter
                        resetCounter = 0;
                    }

                    //If the burglar's location is less than the target location:
                    if (loc.X < startPositionX)
                    {
                        //Add the displacement from the burglars location
                        loc.X += displacement;
                        //Reset the counter
                        resetCounter = 0;
                    }

                    //If the reset range is less than 3 and the displacement is equal to a decimal number less than 1 
                    //(any decimal gets cut of because its an integer variable):
                    if(resetRange <= 3  && displacement == 0)
                    {
                        //Set the burglar's location to the target location
                        loc.X = startPositionX;
                    }
                }

                //If the burglar's location is equal to the target position:
                if (loc.X == startPositionX)
                {
                    //The burglar has finished reseting
                    resetFinished = true;
                }
            }

            //Update the location of the burglar by calling the subprogram that does just that.
            UpdateFrame();
        }

        /// <summary>
        /// Pre: A spritebath that lets the program draw to the screen
        /// Post: Draws the burglar to the screen
        /// Description: Using the burglars texture and bounds, it draws it to the screen
        /// <param name="sb">A spritebatch</param>
        /// </summary>
        public void Draw(SpriteBatch sb)
        {
            //Draw the burglar to the screen
            sb.Draw(burglarImage, burglarBounds, Color.White);
        }

        /// <summary>
        /// Post: Sets the time needed to reverse the direction
        /// Description: Gets a random time within the timer range that will turn the direction of the burglar based on it.
        /// </summary>
        public void SetReverseTimer()
        {
            //Create a new random generator
             Random random = new Random();

            //Set the time needed to reverse direction to a random number between the minimum timer and the maximum timer values
            reverseTime = random.Next(minLevelTimer, maxLevelTimer);
        }

        /// <summary>
        /// Post: Switches the direction of the burglar and helps generate its next location
        /// Description: Checks to see if its time to reverse the direction, if so it does that and then generates the burglars new location
        /// </summary>
        private void CheckTimer()
        {
            //If it's time to switch directions
            if (dirCounter >= reverseTime)
            {
               //Switch directions
               dir *= -1;

                //Call the subprogram to set the next time needed to switch directions
               SetReverseTimer();

                //Reset the timer back down to 0
               dirCounter = 0;

               //Call the subprogram to calculate the burglars next position
               GenerateLoc();
            }
        }

        /// <summary>
        /// Pre: If the direction needs to be switched without checking to see if its time.
        /// Post: Switches direction without waiting for the cue
        /// Description: Doesn't wait to see if its time to switch directions, it switches directions, 
        /// resets the time needed to switch again, and generates the players next location.
        /// <param name="bypass">If switching directions is needed immediately without waiting</param>
        /// </summary>
        private void CheckTimer(bool bypass)
        {
            //If its needed to switch directions immediately:
            if (bypass)
            {
               //Switch directions
               dir *= -1;

               //Call the subprogram to set the next time needed to switch directions
               SetReverseTimer();

               //Call the subprogram to calculate the burglars next position
               GenerateLoc();
            }
        }

        /// <summary>
        /// Post: Generates the burglars next location
        /// Description: Based on the speed, range, and the burglar's current location, a new location for the
        /// burglar is generated
        /// </summary>
        public void GenerateLoc()
        {
            //Create a random generator
            Random rdm = new Random();

            //Create and initialize a displacement to a random location between the burglars current location and his range
            float displacement = rdm.Next(0, (int)range + 1);

            //Manipulate the displacement based on the speed and direction
            displacement *= speed;
            displacement *= dir;

            //Set the test location to the burglars current position plus his displacement
            testLocX = loc.X + displacement;

            //Call the subprogram to test collision with the test location
            Collision(testLocX);

            //Call the subprogram to update the burglars bounds
            UpdateFrame();
        }

        /// <summary>
        /// Pre: An x coordinate for the test location of the burglar
        /// Post: If colliding with anything, (specifically walls), offset the player and force a change in direction
        /// Description: Checks if the test location collides with any of the walls and if it does, it updates its location and forces
        /// the burglar to change directions.
        /// <param name="locX">The burglar's X co-ordinate</param>
        /// </summary>
        public void Collision(float locX)
        {
            //If the test location collides with the right side of the screen:
            if (locX + burglarImage.Width >= screenWidth - range - burglarImage.Width)
            {
                //Offset the player by a bit to the left
                burglarBounds.X = screenWidth - 10;
                //Force the burglar to switch directions
                CheckTimer(true);
            }

            //Else if the test location collides with the left side of the screen:
            else if (locX <= range + burglarImage.Width)
            {
                //Offset the player from the left side of the screen, a little bit to the right.
                burglarBounds.X = burglarImage.Width + 5;
                //Force the burglar to switch directions
                CheckTimer(true);
            }

            //Else, if the burglar does not collide with any of the walls:
            else
            {
                //Set the burglar's location to the test locations
                loc.X = locX;
            }
        }
    }
}
