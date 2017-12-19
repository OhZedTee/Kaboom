/*-----------------------------------Header---------------------------------------
  
  Project Name	     : Kaboom
  File Name		     : Game1.cs
  Author		     : Ori Talmor
  Modified Date      : Wednesday, February 11, 2015
  Due Date		     : Wednesday, February 11, 2015
  Program Description: This XNA program is an imitation of the original atari based
                       Kaboom game. Most of the images were taken from the original
                       game, credits to activision for most of the images. The game
                       is a fun, interactive, AI game to pass time. The point of
                       the game is to catch all bombs that the burglar throws at
                       you in your water buckets. Getting the highest score is
                       the point of this arcade game.
 --------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Kaboom_
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {

        #region Drawing:
        GraphicsDeviceManager graphics;  //The manager needed in order to draw the game
        SpriteBatch spriteBatch;         //The sprite batch which draws out the game
        #endregion

        #region Input:
        MouseState mouse = new MouseState();      //The current state of the mouse
        KeyboardState key = new KeyboardState();  //The current state of the keyboard
        #endregion

        #region AI:
        Burglar burglar;  //An entity which creates bombs and drops it towards the player based on a randomized location
        #endregion

        #region User:
        Bucket bucket = new Bucket(); //Create an instance of the bucket
        #endregion

        #region Important Info:
        int screenW;  //Create an integer that stores the width of the players screen
        int screenH;  //Create an integer that stores the width of the players screen
        int prevMouse; //Create an integer that stores the previous mouse state
        SpriteFont scoreFont; //The font for the score
        Vector2 scorePos; //The position of the score
        int flash = 1;    //The flash of the screen when the player loses
        #endregion

        #region Game Changer:
        bool levelStarted;  //If the player is ready to begin the next level
        int level = 1;      //If the player has reached a new level, this variable is manipulated
        int prevLevel = 1;  //The players current level
        bool levelCompleted = false; //Is the level complete?
        bool allBombsDrawn = false;  //Are all the bombs drawn?
        bool isMoreDifficult = false; //Is the game set to a harder difficulty?
        #endregion

        #region Bomb:
        Bomb[] droppedBombs = new Bomb[Bomb.numBombs];  //An array of entities that hold the bombs' information
        #endregion

        #region Menu:
        Texture2D menu;        //The texture holding the image of the menu
        Rectangle menuBounds;  //The boundaries of the menu
        Rectangle playBounds;  //The boundaries of the play button
        int playBoundsH = 50;  //The Height of the play button
        int playBoundsW = 50;  //The width of the play button
        int playBoundsX = 750; //The X coordinate of the boundaries of the play button
        int playBoundsY = 430; //The Y coordinate of the boundaries of the play button
        Texture2D mouseImg;    //The texture for the mouse
        Rectangle mouseBounds; //The boundaries of the mouse
        #endregion

        #region Instructions:
        Texture2D instImg;         //The instructions image
        private float scrollY;     //The factor by which the image is scrolling
        Texture2D scrollerBtn;     //The texture of the scrolling button
        Texture2D scrollImg;       //The texture of the scrolling bar
        Rectangle scrollerBounds;  //The boundaries of the scrolling button
        Rectangle scrollImgBounds; //The boundaries of the scrolling bar
        Texture2D instPlayButton;  //The texture of the play button
        Rectangle instPlayBounds;  //The boundaries of the play button
        int instScale;             //Create the scale that stores the ratio between the instructions image and the scroll bar length
        bool reachedMin = false;   //Has the scroll bar reached its minimum length at the top?
        bool reachedMax = false;   //Has the scroll bar reached its maximum length at the bottom?
        #endregion

        #region Sound:
        SoundEffect bombTick;                  //The sound effect of the bomb ticking
        SoundEffectInstance bombTickInstance;  //An instance of the bomb ticking sound effect
        SoundEffect bombCaught;                //The bomb caught sound effect
        SoundEffectInstance bombCaughtInstance;//An instance of that bomb caught sound effect
        SoundEffect bombExplodes;              //The exploding bomb sound effect
        SoundEffect bombExplodesFinal;         //The exploding sound of the last bomb exploding
        bool isGameMuted = false;              //Is the game muted?
        #endregion

        #region Game States:
        //Gamestates:
        private const byte STATE_MENU = 0;          //The state in which the menu is shown to the player
        private const byte STATE_INSTRUCTIONS = 1;  //The state where the instructions are shown to the player
        private const byte STATE_PLAY = 2;          //The state in which the game is being manipulated and displayed
        private const byte STATE_RESET = 3;         //The state in which the game is being manipulated and displayed
        private static int state = STATE_MENU;      //What state the game is currently in
        private const int STATE_ENDOFBOMB = 4;      //The state in which the bombs need to explode
        #endregion

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            //Get the size of the screen
            screenW = GraphicsDevice.Viewport.Width;
            screenH = GraphicsDevice.Viewport.Height;

            //Set the burglar's screen width to screen width from above
            Burglar.screenWidth = screenW;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //Images: 
            Bucket.bucketImg = Content.Load<Texture2D>(@"Images\bucket1");
            Burglar.burglarImage = Content.Load<Texture2D>(@"Images\burglar");
            Bomb.bombImgRight = Content.Load<Texture2D>(@"Images\bombspriteright");
            Bomb.bombImgLeft = Content.Load<Texture2D>(@"Images\bombspriteleft");
            Bomb.explodeImg = Content.Load<Texture2D>(@"Images\explodingAnimation");
            menu = Content.Load<Texture2D>(@"Images\menu");
            mouseImg = Content.Load<Texture2D>(@"Images\mouse");
            instImg = Content.Load<Texture2D>(@"Images\instructionsfullP2");
            scrollerBtn = Content.Load<Texture2D>(@"Images\scrollBtn");
            scrollImg = Content.Load<Texture2D>(@"Images\scrollbar");
            instPlayButton = Content.Load<Texture2D>(@"Images\playbutton");
            Bucket.bombCaughtImg = Content.Load<Texture2D>(@"Images\waterAnim");

            //Boundaries
            mouseBounds = new Rectangle(0, 0, mouseImg.Width / 2, mouseImg.Height/2);
            playBounds = new Rectangle(playBoundsX, playBoundsY, playBoundsW, playBoundsH);
            menuBounds = new Rectangle(0, 0, screenW, screenH);
            scrollerBounds = new Rectangle(737, 80, scrollerBtn.Width, scrollerBtn.Height);
            scrollImgBounds = new Rectangle(scrollerBounds.X, scrollerBounds.Height - 15, scrollImg.Width, scrollImg.Height);
            instPlayBounds = new Rectangle(scrollerBounds.X, scrollImgBounds.Height + instPlayButton.Height, instPlayButton.Width, instPlayButton.Height);

            //Set the scale of the instructions screen to the image divided by the length of the scroll bar path
            instScale = instImg.Height / (scrollImgBounds.Height - scrollerBtn.Height);

            //Audio:
            bombTick = Content.Load<SoundEffect>(@"Audio\bombtick");
            bombTickInstance = bombTick.CreateInstance();
            bombCaught = Content.Load<SoundEffect>(@"Audio\bombCatch");
            bombCaughtInstance = bombCaught.CreateInstance();
            bombExplodes = Content.Load<SoundEffect>(@"Audio\bombexplode");
            bombExplodesFinal = Content.Load<SoundEffect>(@"Audio\bombexplodefinal");

            //The font and position of the score
            scoreFont = Content.Load<SpriteFont>(@"Fonts\scoreFont");
            scorePos = new Vector2(screenW / 2 + 100, 15);           

            //Initialize the buckets location and size
            bucket.bucketBounds.Y = 391;

            //Initialize the burglar instance
            burglar = new Burglar(new Vector2(GraphicsDevice.Viewport.Width / 2, 0), 10.0f, 0.75f);

            //Set the burglar's bounds tp his start position and size
            burglar.burglarBounds = new Rectangle((screenW / 2 - Burglar.burglarImage.Width / 2), 0,
                Burglar.burglarImage.Width, Burglar.burglarImage.Height);

            //Get first random time for direction switch
            burglar.SetReverseTimer();

            //Call the LoadContent subprogram from the bomb class to initialize the bomb animation requirements
            Bomb.LoadContent();

            //Call the LoadAnimContent subprogram from the bucket class to initialize the bucket animation requirements 
            bucket.LoadAnimContent();

            //Set the release time for each bomb to an initial time
            Bomb.RandomizeReleaseTime();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            //Get the current state of the keyboard
            key = Keyboard.GetState();

            //If the escape key is pressed, the game exits
            if (key.IsKeyDown(Keys.Escape))
                this.Exit();

            //Get the current state of the mouse
            mouse = Mouse.GetState();

            //Update only the current gamestate that the player is on:
            switch (state)
            {
                //If the bomb's need to explode
                case STATE_ENDOFBOMB:

                    //Create an array to which bombs are still on the screen
                   int[] arrayIndex = BombExplodes(true);

                   //Create a boolean to check if the bombs have all exploded
                   bool finishedPlaying = false;
                   //Boolean to force an exit
                   bool doExit = false;

                   //Create a temporary variable to store the amount of bombs currently on screen
                   int numOfBombs = 0;

                    //For every index in the droppedBombs array:
                    for (int k = 0; k < droppedBombs.Length; k++)
                    {
                        //If the value of that index in the array is not null:
                        if (droppedBombs[k] != null)
                        {
                            //If the bomb was not caught:
                            if (!droppedBombs[k].bombCaught)
                            {
                                //Increment the number of bombs variable
                                numOfBombs++;
                            }
                        }
                    }

                   //While the bombs have not all exploded, and no forced exit:
                   while (!finishedPlaying || !doExit)
                   {
                       //For ever index in the bombs that are still on the map
                       for (int i = 0; i < arrayIndex.Length; i++)
                       {
                           //If the index is not the first one (the first one has already exploded)
                           if (i > 0)
                           {
                               //If the the element in the array is not equal to 0 (only equals to zero if the amount of bombs
                               //on the screen is less then the length of the array)
                               if (arrayIndex[i] != 0)
                               {
                                   //If the previous bomb has exploded:
                                   if (droppedBombs[arrayIndex[i - 1]].isBombExploded)
                                   {
                                       //If the current bomb has not exploded:
                                       if (!droppedBombs[arrayIndex[i]].isBombExploded)
                                       {
                                           //Create a bomb from the dropped bombs array at the index of the bomb that needs to explode
                                           Bomb bomb = droppedBombs[arrayIndex[i]];

                                           //If the bomb is not waiting to explode yet, and has not exploded:
                                           if (bomb.state != Bomb.STATE_WAITINGTOEXPLODE &&
                                               bomb.state != Bomb.STATE_BOMBEXPLODE)
                                           {
                                               //Set the bomb's state to waiting to explode
                                               bomb.state = Bomb.STATE_WAITINGTOEXPLODE;
                                           }

                                           //If the bomb has waited 1 frame:
                                           if (Bomb.bombExplodedCounter == 1)
                                           {
                                               //Set the counter back to 0:
                                               Bomb.bombExplodedCounter = 0;

                                               //If the bomb counter has reached 2:
                                               if (bomb.stateChangeCounter == 2)
                                               {
                                                   //Set the exploded counter to 0
                                                   Bomb.bombExplodedCounter = 0;
                                               }

                                               //If its the last bomb being exploded:
                                               if (i + 1 == numOfBombs)
                                               {
                                                   //Call the bomb state change subprogram to animate the exploding bomb and play its respective sound 
                                                   //effect if the game isn't muted
                                                   bomb.BombStateChange(bombExplodes, bombExplodesFinal, true, isGameMuted);
                                               }

                                               //Otherwise, if any other bomb is being exploded:
                                               else
                                               {
                                                   //Call the bomb state change subprogram to animate the exploding bomb and play its respective sound 
                                                   //effect if the game isn't muted
                                                   bomb.BombStateChange(bombExplodes, bombExplodesFinal, false, isGameMuted);
                                               }

                                               //If the current bomb state is not exploding
                                               if (bomb.state != Bomb.STATE_BOMBEXPLODE)
                                               {
                                                   //Set the finished playing to false, because all the bombs have not finished exploding
                                                   finishedPlaying = false;
                                                   //Break from the for loop
                                                   break;
                                               }

                                               //If the bomb has not finished exploding:
                                               if (!bomb.isBombExploded)
                                               {
                                                   //Break from the for loop
                                                   break;
                                               }
                                           }

                                           //If the bomb has not finished waiting a frame:
                                           else
                                           {
                                               //Increment the counter
                                               Bomb.bombExplodedCounter++;
                                               //Set the  forced exit to true;
                                               doExit = true;
                                               //Break out of the loop
                                               break;
                                           }
                                       }
                                   }
                               }
                           }
                       }

                       //If the forced exit is true:
                       if (doExit)
                       {
                           //Break out of the while loop
                           break;
                       }

                       //Set finished to true
                       finishedPlaying = true;
                       //For every index in the array:
                       for (int k = 0; k < arrayIndex.Length; k++)
                       {
                           //If its not the first index:
                           if (arrayIndex[k] != 0)
                           {
                               //Create a bomb instance equal to the bomb at the array index
                               Bomb bomb = droppedBombs[arrayIndex[k]];

                               //If the bomb has not exploded:
                               if (!bomb.isBombExploded)
                               {
                                   //Not all the bombs have been animated (Set finished playing to false)
                                   finishedPlaying = false;
                                   //Break out of the for loop
                                   break;
                               }

                           }
                       }

                       //If all the bombs have animated:
                       if (finishedPlaying)
                       {
                           //Force an exit out of the while loop
                           doExit = true;
                       }
                   }

                   //If all the bombs have animated:
                   if (finishedPlaying)
                   {
                       //Set the state of the game to reset
                       state = STATE_RESET;
                   }

                        break;
                    

                case STATE_MENU:

                    //Call the Menu and Instructions subprogram with the mouse location
                    MenuAndInstructionsOptions(prevMouse);

                    break;

                case STATE_INSTRUCTIONS:

                    //Call the Menu and Instructions subprogram with the mouse location
                    MenuAndInstructionsOptions(prevMouse);
                    //Set the previous mouse y location to the current mouse location
                    prevMouse = mouse.Y;

                    break;

                case STATE_RESET:

                    //Update the buckets location to the current location of the mouse
                    bucket.mouseX = mouse.X;
                    //Check the buckets collision detection
                    bucket.Collision(GraphicsDevice);

                    //If the bomb sound effect instance is playing:
                    if (bombTickInstance.State == SoundState.Playing)
                    {
                        //Pause the bomb sound effect instance
                        bombTickInstance.Pause();
                    }

                    //If the player has no lives left:
                    if (bucket.lives == 0)
                    {
                        //Set the number of bombs on the screen to 0
                        Bomb.numBombsOnScreen = 0;

                        //Set the speed of the burglar back to its original spped
                        burglar.speed = Burglar.startSpeed;

                        //Set the burglar's range back to its original range
                        burglar.range = Burglar.startRange;

                        //Set the minimum and maximum amount of time needed for the burglar to switch directions back to the original amount
                        burglar.minLevelTimer = Burglar.startMinLevelTimer;
                        burglar.maxLevelTimer = Burglar.startMaxLevelTimer;

                        //Set the release time range back to its original amount
                        Bomb.bombReleaseTimeRange = 25;

                        //Set the number of bombs back to its original amount
                        Bomb.numBombs = 10;

                        //Set the amount of bombs allowed on the screen (capacity) back to its original amount
                        droppedBombs = new Bomb[Bomb.numBombs];

                        //Set the level failed to false
                        Bomb.levelFailed = false;

                        //Reset the score
                        bucket.score = 0;

                        //Reset the score multiplier
                        bucket.scoreMultiplier = 1;

                        //Reset the lives
                        bucket.lives = 3;
                    }

                    //Reset the burglars location with the games screen width
                    burglar.ResetLoc(screenW);

                    //If the burglar has finished reseting:
                    if (Burglar.resetFinished)
                    {
                        //If the level was failed:
                        if (Bomb.levelFailed)
                        {
                            //If the score multiplier is greater than the starting anount
                            if (bucket.scoreMultiplier > 1)
                            {
                                //Decrement the multiplier
                                bucket.scoreMultiplier--;
                            }

                            //Set the number of bombs on the screen to 0
                            Bomb.numBombsOnScreen = 0;

                            //If the burglar's speed is greater than the minimum speed
                            if (burglar.speed > Burglar.startSpeed)
                            {
                                //Decrement the speed of the burglar
                                burglar.speed -= 0.2f;
                            }

                            //If the burglar's range is less then the start range
                            if (burglar.range < Burglar.startRange)
                            {
                                //Increment the burglar's range
                                burglar.range += 0.5f;
                            }

                            //If the burglar's min. level timer is less then the starting, 
                            //and that the burglars max. level timer is less then the starting value
                            if (burglar.minLevelTimer < Burglar.startMinLevelTimer && burglar.maxLevelTimer < Burglar.startMaxLevelTimer)
                            {
                                //Increment the minimum and maximum amount of time needed for the burglar to switch directions
                                burglar.minLevelTimer += 10;
                                burglar.maxLevelTimer += 20;
                            }

                            //If the bomb's release time range is greater than its original amount:
                            if (Bomb.bombReleaseTimeRange > 25)
                            {
                                //Decrement the range by 5;
                                Bomb.bombReleaseTimeRange -= 5;
                            }

                            //If the number of bomb's drawn is greater than or equal to 100:
                            if (Bomb.numBombs >= 100)
                            {
                                //Decrement the number of bombs by 50
                                Bomb.numBombs -= 50;
                            }

                            //Else if the number of bomb's drawn is greater than or equal to 50 but less than 100:
                            else if (Bomb.numBombs >= 50 && Bomb.numBombs < 100)
                            {
                                //Decrement the number of bomb's by 25
                                Bomb.numBombs -= 25;
                            }

                            //Else if the number of bomb's drawn is greater than 10 but less than 50:
                            else if (Bomb.numBombs > 10 && Bomb.numBombs < 50)
                            {
                                //Decrement the number of bomb's by 10
                                Bomb.numBombs -= 10;
                            }

                            //Otherwise, if the number of bomb's drawn to the screen is less than 10:
                            else
                            {
                                //Set the number of bomb's 5
                                Bomb.numBombs = 5;
                            }

                            //Decrement the amount of bombs allowed on the screen (capacity) to the new amout
                            droppedBombs = new Bomb[Bomb.numBombs];

                            //Set the level failed to false
                            Bomb.levelFailed = false;
                        }

                        //If the level was completed:
                        if (levelCompleted)
                        {
                            //Increment the multiplier
                            bucket.scoreMultiplier++;

                            //Set the number of bombs on the screen to 0
                            Bomb.numBombsOnScreen = 0;

                            //If the burglars speed is less than 3.5 and the speed is greater than or equal to the minimum speed:
                            if (burglar.speed < 3.5 && burglar.speed >= Burglar.startSpeed)
                            {
                                //Increment the speed of the burglar
                                burglar.speed += 0.25f;
                            }

                            //If the burglar's range is greater than or equal to the minimum range but less than the maximum range:
                            if (burglar.range >= Burglar.startRange && burglar.range < 35)
                            {
                                //Decrement the burglar's range
                                burglar.range += 0.55f;
                            }

                            //if the bomb's release time is greater than or equal to its original amount:
                            if (Bomb.bombReleaseTimeRange >= 25)
                            {
                                //Increment the reease time of the bomb
                                Bomb.bombReleaseTimeRange += 5;
                            }

                            //If the burglar's minimum level timer is greater than 30, and the maximum level timer is greater than 40:
                            if (burglar.minLevelTimer > 30 && burglar.maxLevelTimer > 40)
                            {
                                //Decrement the minimum and maximum amount of time needed for the burglar to switch directions
                                burglar.minLevelTimer -= 30;
                                burglar.maxLevelTimer -= 40;
                            }

                            //If the number of bomb's drawn is greater than or equal to 100:
                            if (Bomb.numBombs >= 100)
                            {
                                //Increment the number of bombs by 50
                                Bomb.numBombs += 50;
                            }

                            //Else if the number of bomb's drawn is greater than or equal to 50 but less than 100:
                            else if (Bomb.numBombs >= 50 && Bomb.numBombs < 100)
                            {
                                //Increment the number of bombs by 25
                                Bomb.numBombs += 25;
                            }

                            //Else if the number of bomb's drawn is greater than or equal to 10 but less than 50:
                            else if (Bomb.numBombs >= 10 && Bomb.numBombs < 50)
                            {
                                //Increment the number of bombs by 10
                                Bomb.numBombs += 10;
                            }

                            //Otherwise if the number of bomb's drawn is less than 10:
                            else
                            {
                                //Set the number of bombs to 10
                                Bomb.numBombs = 10;
                            }

                            //Set the amount of bombs allowed on the screen (capacity) by the amount set above
                            droppedBombs = new Bomb[Bomb.numBombs];

                            //Set the level completed back to false
                            levelCompleted = false;
                        }

                        //Set the state to play
                        state = STATE_PLAY;
                        //Set all bombs drawn to false
                        allBombsDrawn = false;
                        //Set level started to false
                        levelStarted = false;
                        //Set the reset finished to false
                        Burglar.resetFinished = false;
                    }

                    break;

                //If the player is in the play state:
                case STATE_PLAY:

                    //Update the buckets location to the current location of the mouse
                    bucket.mouseX = mouse.X;
                    //Check the buckets collision detection
                    bucket.Collision(GraphicsDevice);

                    //If the level isn't started:
                    if (!levelStarted)
                    {
                        //If the bomb tick sound effect is playing:
                        if (bombTickInstance.State == SoundState.Playing)
                        {
                            //Pause the sound effect
                            bombTickInstance.Pause();
                        }
                    }

                    //If the number 1 key is pressed:
                    if (key.IsKeyDown(Keys.D1))
                    {
                        //Set the bucket's lives to 0
                        bucket.lives = 0;
                        //Set the state to reset
                        state = STATE_RESET;
                    }

                    //If the number 3 key is pressed:
                    if (key.IsKeyDown(Keys.D3))
                    {
                        //Set the game to muted
                        isGameMuted = true;

                        //If the bomb tick sound effect is playing:
                        if (bombTickInstance.State == SoundState.Playing)
                        {
                            //Pause the sound effect
                            bombTickInstance.Pause();
                        }
                    }

                    //If the number 4 key is pressed:
                    if (key.IsKeyDown(Keys.D4))
                    {
                        //Set the game to not muted
                        isGameMuted = false;
                    }

                    //If the number 5 key is pressed:
                    if (key.IsKeyDown(Keys.D5))
                    {
                        //Make the game less difficult
                        isMoreDifficult = false;
                    }

                    //If the number 6 key is pressed:
                    if (key.IsKeyDown(Keys.D6))
                    {
                        //Make the game more difficult
                        isMoreDifficult = true;
                    }


                    //If the left mouse button is pressed:
                    if (mouse.LeftButton == ButtonState.Pressed)
                    {
                        //set the start level boolean to true;
                        levelStarted = true;
                    }

                    //If the player is ready to start the level:
                    if (levelStarted)
                    {
                        //Incrememnt the burglar's direction switch counter
                        burglar.dirCounter++;

                        //Call the burglar instance to generate a new location
                        burglar.GenerateLoc();

                        if (!allBombsDrawn)
                        {
                            //Increment the bomb's timer so that it knows when it needs to add a new bomb
                            Bomb.timer++;
                        }

                        //For every bomb in the dropped bomb array:
                        for (int i = 0; i < droppedBombs.Length; i++)
                        {
                            //If the current bomb isn't null:
                            if (droppedBombs[i] != null)
                            {
                                //If the current bomb wasn't caught:
                                if (!droppedBombs[i].bombCaught)
                                {
                                    //Create a temporary boolean to test if its the end of the players life
                                    bool testEndOfLife = false;
                                    //Set the boolean to the bomb's collision subprogram to check if the bomb hit the side of the screen
                                    testEndOfLife =  droppedBombs[i].Collision(screenH, testEndOfLife);

                                    //If the end of life boolean returns true:
                                    if (testEndOfLife)
                                    {
                                        //Call the bomb explodes subprogram
                                        BombExplodes();

                                        //Decrement the player's lives
                                        bucket.lives--;
                                        //Set the state to the end of bomb state
                                        state = STATE_ENDOFBOMB;

                                        //Break the For loop
                                        break;
                                    }
                                }
                            }
                        }

                        //If not all the bomb's were drawn:
                        if (!allBombsDrawn)
                        {
                            //If its time for the next bomb to be drawn:
                            if (Bomb.timer == Bomb.bombReleaseTime)
                            {
                                //Creates a new instance of the bomb class with the burglars current location
                                Bomb bomb = new Bomb((burglar.burglarBounds.X + (burglar.burglarBounds.Width / 2) - (Bomb.bombImgRight.Width / 2)),
                                    burglar.burglarBounds.Y + Burglar.burglarImage.Height);
                                //Set the specified index in the array of bombs to the bomb instance created above
                                droppedBombs[Bomb.numBombsOnScreen] = bomb;

                                //If the bomb ticking sound effect instance isn't playing and the game isn't muted:
                                if (bombTickInstance.State == SoundState.Stopped && !isGameMuted)
                                {
                                    //Set it's volume to 75 percent of the original
                                    bombTickInstance.Volume = 0.75f;
                                    //Set the sound effect instance to loop
                                    bombTickInstance.IsLooped = true;
                                    //Play the sound effect instance 
                                    bombTickInstance.Play();
                                }

                                //If the bomb ticking sound effect instance is currently playing and the game isn't muted:
                                else if (!isGameMuted)
                                {
                                    //Resume the sound effect
                                    bombTickInstance.Resume();
                                }

                                //Randomize the bomb's next release time
                                Bomb.RandomizeReleaseTime();

                                //If the amount of bombs needed to be on the screen hasn't reached its limit:
                                if (Bomb.numBombsOnScreen != droppedBombs.Length - 1)
                                    Bomb.numBombsOnScreen++;  //Increment the index for the array of bombs

                                //Otherwise, if the max number of bombs drawn on the screen has been reached:
                                else
                                {
                                    //Set all the bombs drawn to true
                                    allBombsDrawn = true;
                                }

                                //Call the subprogram in the instance of the bomb that calculates the direction in which the bomb 
                                //should be drawn based on the burglars direction
                                bomb.CalcDirection(burglar);
                                //Reset the bomb timer
                                Bomb.timer = 0;
                            }
                        }
                    }

                    //For every bomb in the droppedBombs array:
                    for (int i = 0; i < droppedBombs.Length; i++)
                    {
                        //If the bomb exists:
                        if (droppedBombs[i] != null)
                        {
                            if (!droppedBombs[i].bombCaught)
                            {
                                //Update its Y velocity
                                droppedBombs[i].UpdateY();
                                //Animate the bomb
                                droppedBombs[i].FrameAnimation();
                            }
                        }
                    }

                    #region BombToBucketCollision

                    //For every bomb in the droppedBombs array:
                    for (int i = 0; i < droppedBombs.Length; i++)
                    {
                        //If the bomb exists:
                        if (droppedBombs[i] != null)
                        {
                            //If the bomb wasn't caught:
                            if (!droppedBombs[i].bombCaught)
                            {
                                //Call the bucket subprogram bomb caught to check which bucket the bomb hit and to animate that bucket and 
                                //make sure the game isn't muted
                                bucket.bombCaught(droppedBombs[i], bombCaught, isGameMuted);
                                //Set the bucket to not colliding
                                bucket.isCollided = false;
                            }
                        }
                    }

                    #endregion

                    //Temporary number to check if all the bombs were caught
                    int num = 0;
                    //If the player caught the total amount of bombs drawn to the screen:
                    for(int i = 0; i < droppedBombs.Length; i++)
                    {
                        //If the bomb at that index isn't null:
                        if (droppedBombs[i] != null)
                        {
                            //If the bomb at that index was caught
                            if (droppedBombs[i].bombCaught)
                            {
                                //Increment num
                                num++;
                            }
                        }
                    }
                    
                    //If the num equals to the length of the bomb array:
                    if (num == droppedBombs.Length)
                    {
                        //Increment level
                        level++;
                    }

                    //If the level the player needs to be on is different then the one it is currently on:
                    if (level != prevLevel)
                    {
                        //Set the state to reset
                        state = STATE_RESET;

                        //Set level complete to true
                        levelCompleted = true;
                    }

                    

                    //Set the level that the player is on to the level that the player needs to be on
                    prevLevel = level;

                    //Get out of the switch statement
                    break;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            //Clear the screen and set the colour to that of the background needed (a dark green)
            GraphicsDevice.Clear(new Color(79, 116, 32));

            //Draw only the current gamestate that the player is on:
            switch (state)
            {
                case STATE_MENU:

                    //Begin the spriteBatch
                    spriteBatch.Begin();

                    //Draw the menu image
                    spriteBatch.Draw(menu, menuBounds, Color.White);
                    //Draw the mouse
                    spriteBatch.Draw(mouseImg, mouseBounds, Color.White);

                    //End the spriteBatch
                    spriteBatch.End();

                    break;

                case STATE_INSTRUCTIONS:
                    
                    //Begin the spriteBatch with the following properties, specifically the wrap
                    spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.LinearWrap, null, null);

                    //Draw the instructions image to the screen
                    spriteBatch.Draw(instImg, Vector2.Zero, new Rectangle(100, (int)(scrollY), instImg.Width, instImg.Height), Color.White);

                    //End the special spriteBatch
                    spriteBatch.End();
                    
                    //Begin the standard spriteBatch
                    spriteBatch.Begin();

                    //Draw the scroller to the screen:
                    spriteBatch.Draw(scrollImg, scrollImgBounds, Color.White);
                    spriteBatch.Draw(scrollerBtn, scrollerBounds, Color.White);
                    spriteBatch.Draw(instPlayButton, instPlayBounds, Color.White);

                    //Draw the mouse to the screen
                    spriteBatch.Draw(mouseImg, mouseBounds, Color.White);

                    //End the standard spriteBatch
                    spriteBatch.End();

                    break;

                case STATE_ENDOFBOMB:

                    //Call the Draw state subprogram, to draw the all screen items, including the burglar, bucket, and background
                    DrawState(spriteBatch);

                    break;

                //If the player is in the play state:
                case STATE_PLAY:

                    //Call the Draw state subprogram, to draw the all screen items, including the burglar, bucket, and background
                    DrawState(spriteBatch);

                    //If the bucket caught a bomb
                    if (bucket.bombCaughtDraw)
                    {
                        //Begin the spriteBatch
                        spriteBatch.Begin();

                        //If the bomb hits the top bucket:
                        if (Bucket.isHitTopBucket)
                        {
                            //Animate the bucket's water splash
                            bucket.AnimWater();

                            //Set the water's bounds to that of the top bucket
                            Bucket.waterBounds = new Rectangle(bucket.bucketBounds.X, bucket.bucketBounds.Y - Bucket.bombCaughtImg.Height,
                                bucket.bucketBounds.Width, bucket.bucketBounds.Height);

                            //Draw the water animation to the bucket bounds created above ^^^
                            spriteBatch.Draw(Bucket.bombCaughtImg, Bucket.waterBounds, Bucket.waterSrcRec, Color.White);
                        }

                        //If the bomb hits the middle bucket:
                        if (Bucket.isHitMiddleBucket)
                        {
                            //Animate the bucket's water splash
                            bucket.AnimWater();

                            //Set the water's bounds to that of the middle bucket
                            Bucket.waterBounds = new Rectangle(bucket.bucketBounds.X, bucket.bucketBounds.Y + 36 - Bucket.bombCaughtImg.Height,
                                bucket.bucketBounds.Width, bucket.bucketBounds.Height);

                            //Draw the water animation to the bucket bounds created above ^^^
                            spriteBatch.Draw(Bucket.bombCaughtImg, Bucket.waterBounds, Bucket.waterSrcRec, Color.White);
                        }

                        //If the bomb hits the bottom bucket:
                        if (Bucket.isHitBottomBucket)
                        {
                            //Animate the bucket's water splash
                            bucket.AnimWater();

                            //Set the water's bounds to that of the bottom bucket
                            Bucket.waterBounds = new Rectangle(bucket.bucketBounds.X, bucket.bucketBounds.Y + 72 - Bucket.bombCaughtImg.Height,
                                bucket.bucketBounds.Width, bucket.bucketBounds.Height);

                            //Draw the water animation to the bucket bounds created above ^^^
                            spriteBatch.Draw(Bucket.bombCaughtImg, Bucket.waterBounds, Bucket.waterSrcRec, Color.White);
                        }

                        //End the spriteBatch
                        spriteBatch.End();
                        //Increment the draw counter
                        bucket.caughtDrawCounter++;

                        //If the counter gets to 10
                        if (bucket.caughtDrawCounter == 10)
                        {
                            //The bomb is no longer caught (no more animation)
                            bucket.bombCaughtDraw = false;
                            //The bomb is no longer hitting the top bucket
                            Bucket.isHitTopBucket = false;
                            //The bomb is no longer hitting the middle bucket
                            Bucket.isHitMiddleBucket = false;
                            //The bomb is no longer hitting the bottom bucket
                            Bucket.isHitBottomBucket = false;
                            //Reset the counter back to 0
                            bucket.caughtDrawCounter = 0;
                        }
                    }
                    
                    //Get out of the switch statement
                    break;

                case STATE_RESET:

                    //Call the Draw state subprogram, to draw the all screen items, including the burglar, bucket, and background
                    DrawState(spriteBatch);

                    break;
            }

            base.Draw(gameTime);
        }

        /// <summary>
        /// Pre: A spritebatch that helps the program draw to the screen
        /// Post: Draw out the important things to the screen (ie Burglar, bucket, bombs, etc)
        /// Description: Set the background colour to a custom color and then draw the burglar, the bucket, and the bombs to the screen
        /// </summary>
        /// <param name="sb">>A spritebatch that allows drawing to the screen</param>
        private void DrawState(SpriteBatch sb)
        {
            //Begin the spriteBatch
            spriteBatch.Begin();

            //If the game is resetting and if the level is failed:
            if (state == STATE_RESET && Bomb.levelFailed)
            {
                //Increment flash
                flash++;
            }

            //If the game is resetting, if flash divided by 2 leaves no remainder, and if the level is failed:
            if (state == STATE_RESET && flash % 3 == 0 && Bomb.levelFailed)
            {
                //Clear the screen and set the colour to that of the background needed (a dark green)
                GraphicsDevice.Clear(new Color(229, 229, 229) * 0.5f);
            }

            //If the game is resetting, if flash divided by 4 leaves no remainder and if the level is failed:
            if (state == STATE_RESET && flash % 6 == 0 && Bomb.levelFailed)
            {
                //Create a new rectangle texture set to the burglars height and the screen width
                Texture2D yellowRect = new Texture2D(GraphicsDevice, GraphicsDevice.Viewport.Width, Burglar.burglarImage.Height);
                Color[] color = new Color[yellowRect.Width * yellowRect.Height]; //set the color to the amount of pixels in the textures
                for (int i = 0; i < color.Length; i++)  //loop through all the colors setting them to the yellow that matches the burglar
                {
                    color[i] = new Color(179, 174, 64) * 1.5f;
                }

                yellowRect.SetData(color);  //Set each pixel of the rectangle texture's color to the color choosen above
                //Draw out the yellow rectange
                spriteBatch.Draw(yellowRect, new Rectangle(0, 0, yellowRect.Width, Burglar.burglarImage.Height), Color.White);
            }

            //Otherwise, if the game is not resetting or if flash divided by 4 leaves a remainder
            else
            {
                //Clear the screen and set the colour to that of the background needed (a dark green)
                GraphicsDevice.Clear(new Color(79, 116, 32));

                //Create a new rectangle texture set to the burglars height and the screen width
                Texture2D grayRect = new Texture2D(GraphicsDevice, GraphicsDevice.Viewport.Width, Burglar.burglarImage.Height);
                Color[] color = new Color[grayRect.Width * grayRect.Height]; //set the color to the amount of pixels in the textures
                for (int i = 0; i < color.Length; i++)  //loop through all the colors setting them to the grey that matches the burglar
                {
                    color[i] = new Color(171, 171, 171);
                }

                grayRect.SetData(color);  //Set each pixel of the rectangle texture's color to the color choosen above
                //Draw out the gray rectange
                spriteBatch.Draw(grayRect, new Rectangle(0, 0, grayRect.Width, Burglar.burglarImage.Height), Color.White);
            }

            bucket.Draw(spriteBatch, isMoreDifficult);  //Call the draw subprogram in the bucket class to draw the bucket to the screen

            burglar.Draw(spriteBatch); //Call the draw subprogram in the burglar instance's class to draw the burglar instance to the screen

            //Draw score
            string output = bucket.score.ToString();

            //Find the center of the string
            Vector2 fontOrigin = scoreFont.MeasureString(output) / 2;

            //Draw the string
            spriteBatch.DrawString(scoreFont, output, scorePos, new Color(255, 214, 74) , 0, fontOrigin, 1.0f, SpriteEffects.None, 0.5f);

            //If the game is in the play state:
            if (state == STATE_PLAY)
            {
                //For every bomb:
                for (int i = 0; i < droppedBombs.Length; i++)
                {
                    //If the bomb was created:
                    if (droppedBombs[i] != null)
                    {
                        //If the bomb wasn't caught:
                        if (!droppedBombs[i].bombCaught)
                        {
                            droppedBombs[i].Draw(spriteBatch);  //Draw the bomb to the screen
                        }
                    }
                }
            }

            //If the game is in the end of life state: 
            if (state == STATE_ENDOFBOMB)
            {
                //For every bomb on memory:
                for (int i = 0; i < droppedBombs.Length; i++)
                {
                    //If the bomb is not null and that it wasn't caught
                    if (droppedBombs[i] != null && !droppedBombs[i].bombCaught)
                    {
                        //Draw the bomb to the screen
                        droppedBombs[i].Draw(spriteBatch);
                    }
                }
            }

            //End the spriteBatch
            spriteBatch.End();
        }

        /// <summary>
        /// Pre: An integer value of the mouse's previous y coordinate
        /// Post: Display mouse on screen, update menu, and instructions page
        /// Description: Update the location of the mouse on screen, control the scroller and instructions, and update the menu
        /// </summary>
        /// <param name="prevMouseY">An integer representing the previous Y coordinate of the mouse</param>
        private void MenuAndInstructionsOptions(int prevMouseY)
        {
            //Update the buckets location to the current location of the mouse
            mouseBounds.X = mouse.X;
            mouseBounds.Y = mouse.Y;

            #region Burglar Mouse Collision

            //If the mouse is not within the right boundary of the screen:
            if (mouseBounds.X + mouseBounds.Width >= screenW)
            {
                //Set the mouse to be near the right boundary of the screen
                mouseBounds.X = screenW - mouseBounds.Width - 1;
            }

            //If the mouse is not within the left boundary of the screen:
            if (mouseBounds.X <= 0)
            {
                //Set the mouse to be near the left boundary of the screen
                mouseBounds.X = 1;
            }

            //If the mouse is not within the bottom boundary of the screen:
            if (mouseBounds.Y + mouseBounds.Height >= screenH)
            {
                //Set the mouse to be near the bottom boundary of the screen
                mouseBounds.Y = screenH - mouseBounds.Height - 1;
            }

            //If the mouse is not within the top boundary of the screen:
            if (mouseBounds.Y <= 0)
            {
                //Set the mouse to be near the top boundary of the screen
                mouseBounds.Y = 1;
            }

            #endregion

            #region Scroll Bar Collision

            //If the state of the game is set to the instructions state:
            if (state == STATE_INSTRUCTIONS)
            {
                //If the player's Y coordinate of the mouse is not were it was before:
                if (mouseBounds.Y != prevMouseY)
                {
                    //Set the displacement to the change in mouse location
                    int displacement = mouseBounds.Y - prevMouseY;

                    //If the left mouse button was pressed:
                    if (mouse.LeftButton == ButtonState.Pressed)
                    {
                        //If the mouse is within the boundaries of the scroll bar on the X coordinates:
                        if (mouseBounds.X + mouseBounds.Width > scrollerBounds.X && mouseBounds.X < scrollerBounds.X + scrollerBounds.Width)
                        {
                            //If the mouse is within the boundaries of the scroll bar on the Y coordinates:
                            if (mouseBounds.Y < scrollerBounds.Y + scrollerBounds.Height && mouseBounds.Y + mouseBounds.Height > scrollerBounds.Y)
                            {
                                //If the minimum or maximum wasn't reached:
                                if (!reachedMin && !reachedMax)
                                {
                                    //Increement th scroll bar's Y coordinate by the displacement
                                    scrollerBounds.Y += displacement;

                                    //Set the displacement of the instructions image to the previous displacement multiplied by the scale
                                    int imageDisplacement = displacement * instScale;

                                    //If the mouse's new location is greater than the old location (moving down)
                                    if (mouseBounds.Y - prevMouseY > 0)
                                    {
                                        //If the scrollY location is less than the image's height minus the screen Height:
                                        if (scrollY < instImg.Height - screenH)
                                        {
                                            //Increment the scrollY location by the image displacement
                                            scrollY += imageDisplacement;
                                        }
                                    }

                                    //If the mouse's new location is less than the old location (moving up)
                                    if (mouseBounds.Y - prevMouseY < 0)
                                    {
                                        //If the scrollY location is greater than 0:
                                        if (scrollY > 0)
                                        {
                                            //Increment the scrollY location by the image displacement
                                            scrollY += imageDisplacement;
                                        }
                                    }
                                }

                                //If the scrollY is less than or equal to 0:
                                if (scrollY <= 0)
                                {
                                    //The scroll bar has reached it's minumum
                                    reachedMin = true;
                                }

                                //If the scrollY is greater than the instruction image height minus the screen's height:
                                else if (scrollY >= instImg.Height - screenH)
                                {
                                    //The scroll bar has reached it's maximum
                                    reachedMax = true;
                                }

                                //Otherwise:
                                else
                                {
                                    //Set reached Max and Min back to false
                                    reachedMax = false;
                                    reachedMin = false;
                                }

                                //If the scroll bar reached the bottom of the screen (its max):
                                if (reachedMax)
                                {
                                    //Create a new integer to store a temporary location
                                    int locY;

                                    //Set the temporary location to the scroll bar's current location plus the displacement
                                    locY = scrollerBounds.Y + displacement;

                                    //If the temporary location is less than the scroll bar's original location:
                                    if (locY < scrollerBounds.Y)
                                    {
                                        //Set the scroll bar to the temporary location
                                        scrollerBounds.Y = locY;
                                        //Reset reached maximum back to false
                                        reachedMax = false;
                                    }
                                }

                                //If the scroll bar reached the top of the screen (its min):
                                if (reachedMin)
                                {
                                    //Create a new integer to store a temporary location
                                    int locY;

                                    //Set the temporary location to the scroll bar's current location plus the displacement
                                    locY = scrollerBounds.Y + displacement;

                                    //If the temporary location is greater than the scroll bar's original location:
                                    if (locY > scrollerBounds.Y)
                                    {
                                        //Set the scroll bar to the temporary location
                                        scrollerBounds.Y = locY;
                                        //Reset reached maximum back to false
                                        reachedMin = false;
                                    }
                                }
                            }
                        }
                    }
                }

                //If the left button on the mouse was pressed:
                if (mouse.LeftButton == ButtonState.Pressed)
                {
                    //If the mouse is within the boundaries of the play button on the instructions screen on the X coordinates:
                    if (mouseBounds.X + mouseBounds.Width > instPlayBounds.X && mouseBounds.X < instPlayBounds.X + instPlayBounds.Width)
                    {
                        //If the mouse is within the boundaries of the play button on the instructions screen on the Y coordinates:
                        if (mouseBounds.Y < instPlayBounds.Y + instPlayBounds.Height && mouseBounds.Y > instPlayBounds.Y)
                        {
                            //Set the state of the game to the play state
                            state = STATE_PLAY;
                        }

                    }
                }
            }

            #endregion

            //If the left button on the mouse is clicked
            if (mouse.LeftButton == ButtonState.Pressed)
            {
                //If the mouse intersects with the play button on the x axis
                if (mouseBounds.X >= playBounds.X && mouseBounds.X <= playBounds.X + playBounds.Width)
                {
                    //If the mouse intersects with the play button on the y axis
                    if (mouseBounds.Y >= playBounds.Y && mouseBounds.Y <= playBounds.Y + playBounds.Height)
                    {
                        //If the game is currently in the menu:
                        if (state == STATE_MENU)
                        {
                            //Set the state of the game over to the instructions stage
                            state = STATE_INSTRUCTIONS;
                        }
                    }
                }

            }
        }

        /// <summary>
        /// Post: Creates the list of bomb's that need to explode and plays the sound of the first bomb
        /// Description: Go through all the bombs and store the indexs where the bomb has been drawn, but has not been caught, in an integer array
        ///              This subprogram also animates the explosion of the first bomb and plays its explosion sound
        /// </summary>
        public void BombExplodes()
        {
            //Create a temporary integer to store index slots
            int k = 0;
            //Create a temporary array to hold the element numbers
            int[] indexs = new int[droppedBombs.Length];

            //For every bomb in the droppedBombs' array:
            for (int i = 0; i < droppedBombs.Length; i++)
            {
                //If the bomb isn't null:
                if (droppedBombs[i] != null)
                {
                    //If the bomb was not caught:
                    if (!droppedBombs[i].bombCaught)
                    {
                        //Set the temporary array to the specified element number
                        indexs[k] = i;
                        //Incrememnt the temporary index slot variable
                        k++;
                    }
                }
            }

            //If the bomb ticking sound effect is playing:
            if (bombTickInstance.State == SoundState.Playing)
            {
                //Pause the sound effect
                bombTickInstance.Pause();
            }  

            //Set the first bomb that is drawn on the screen to the exploding state:
            droppedBombs[indexs[0]].state = Bomb.STATE_BOMBEXPLODE;

            //While the first bomb isn't yet finished exploding:
            while (!droppedBombs[indexs[0]].isBombExploded)
            {
                //Call the subprogram in the bomb class to animate, explode the first bomb, and play the sound effect of the 
                //bomb exploding, if the game isn't muted
                droppedBombs[indexs[0]].BombStateChange(bombExplodes, bombExplodesFinal, false, isGameMuted);
            }           
        }

        /// <summary>
        /// Pre: A boolean that allows to call this subprogram instead of the one right above which does a very similar task
        /// Post: Creates the list of bomb's that need to explode
        /// Description: Goes through all the bombs and store the indexs where the bomb has been drawn, but has not been caught, in an 
        ///              integer array
        /// </summary>
        /// <param name="bypass">A boolean that if set to true or false, the subprogram doesn't check the first bomb and doesn't animate it</param>
        /// <returns>Returns an integer array were each element is the next index in the droppedBombs array that needs to explode</returns>
        public int[] BombExplodes(bool bypass)
        {
            //Create a temporary integer to store index slots
            int k = 0;
            //Create a temporary array to hold the element numbers
            int[] indexs = new int[droppedBombs.Length];

            //For every bomb in the droppedBombs' array:
            for (int i = 0; i < droppedBombs.Length; i++)
            {
                //If the bomb isn't null:
                if (droppedBombs[i] != null)
                {
                    //If the bomb wasn't caught:
                    if (!droppedBombs[i].bombCaught)
                    {
                        //Set the temporary array to the specified element number
                        indexs[k] = i;
                        //Incrememnt the temporary index slot variable
                        k++;
                    }
                }
            }

            //Return the indexs array
            return indexs;
        }
    }
}