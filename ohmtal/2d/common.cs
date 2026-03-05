//-----------------------------------------------------------------------------
// Global Const
//-----------------------------------------------------------------------------


  $DefaultAudioType = 0;
  $GuiAudioType = 1;
  $SimAudioType = 2;
  $MusicAudioType = 3;

  $textAlignLeft = 0;
  $textAlignMiddle = 1;
  $textAlignRight = 2;
  $textAlignCenter = 3;

  //MATH
  $M_PI = 3.14159265358979323846;
  
  //box2d BodyTypes
   $staticBody       = 0; //does not move and receive collisions
	$kinematicBody    = 1; //manually move and receive collisions
 	$dynamicBody      = 2; //dynamic move, send and receive collisions
 
  //box2d ShapeTypes
    $PolygonShape = 0;
    $CircleShape  = 1;
    $EdgeShape    = 2;
    $ChainShape   = 3;

  //Box2d JointTypes
   $unknownJoint    =  0;
   $revoluteJoint   =  1;
   $prismaticJoint  =  2;
   $distanceJoint   =  3; 
   $pulleyJoint     =  4;
   $mouseJoint      =  5;
   $gearJoint       =  6;
   $wheelJoint      =  7;
   $weldJoint       =  8;
   $frictionJoint   =  9;
   $ropeJoint       = 10;
   $motorJoint      = 11;
  
//-----------------------------------------------------------------------------
exec("./canvas.cs");  
//-----------------------------------------------------------------------------
// Datadirectory
//-----------------------------------------------------------------------------
function initDataDirectory() 
{
       
  //I dont want that for development envirorment!
   if (isfile("core/ohmtal/2d/common.cs") && $platform !$= "Android")
        return false;
       
       
   %dir = getUserDataDirectory() @ "/" @ $Game::Company @ "/" @ $Game::Caption ;
   dEcho ("initDataDirectory" SPC %dir);
   $Game::DataDirectory ="";
   
   //ok this is tricky since we dont have normal access there!
   %lFile =  %dir @ "/dataaccess.cs";
   $foo::bar=1;
   export("$foo::*", %lFile, false);
   $Scripts::ignoreDSOs=true;
   %success = exec(%lFile, true);
   $Scripts::ignoreDSOs=false;
   if ( %success )
   {
        $Game::DataDirectory = %dir;
        //OGE 3D fileDelete(%lFile,true);
        fileDelete(%lFile);
        
   } else {
       error("Unable to access data Directory!");
       return false;
   }
   
    
   return true;

}
//-----------------------------------------------------------------------------
// Sound
//-----------------------------------------------------------------------------
function initSound() 
{
   %p = "core/scripts/client/";
   exec(%p @ "audio.cs");
   exec(%p @ "audioEnvironments.cs" );
   exec(%p @ "audioDescriptions.cs" );
   exec(%p @ "audioStates.cs" );
   exec(%p @ "audioAmbiences.cs" );

   sfxStartup();

   %desc = new AudioDescription(AD_EFFECT)
     {
      volume = 0.7;
      isLooping= false;
      is3D = false;
      type = $SimAudioType;
    };
    GameScreen.cleanup.add(%desc);
    
    
     %desc = new AudioDescription(AD_LOOP)
     {
      volume = 0.7;
      isLooping= true;
      is3D = false;
      type = $MusicAudioType;
    };
    
     %desc = new AudioDescription(AD_STREAM_LOOP)
     {
      volume = 0.7;
      isLooping= true;
      isStreaming = false; //sucks: true;
      is3D = false;
      type = $MusicAudioType;
    };
    GameScreen.cleanup.add(%desc);
}
//----------------------------------------------------------
// Usage: loadSound(OBJECTNAME,FILENAME,[AD_EFFECT] || AD_LOOP) 
// WAV SOUNDS MUST BE CD-Quality: 44.100 kHz, stereo, 16 Bit!
function loadSound(%name,%filename,%desc, %preload) 
{

       if (%desc $= "")
                %desc = "AD_EFFECT";
       if (%preload $= "") 
                %preload = true;
 
       %result = new AudioProfile(%name)
       {
         filename = $Game::SoundPath @ "/" @ %filename;
         description = %desc;
         preload = %preload;
       };
       
       GameScreen.cleanup.add(%result);
       
       return %result;
       
}

//-----------------------------------------------------------------------------
// Graphics
//-----------------------------------------------------------------------------
function loadTexture(%filename, %setTransparent) {
  %result = new tom2Dtexture();
  %result.setBitmap($Game::ImagePath @ "/" @ %filename, false); //OGE3D!, %setTransparent);
  if (!%result) {
    error("Loading Texture:" SPC %filename SPC "failed!");
    return 0;
  }
  GameScreen.cleanup.add(%result);
  return %result;
}

function loadTextureAbsolutPath(%filename, %setTransparent) {
  %result = new tom2Dtexture();
  %result.setBitmap( %filename, false, %setTransparent);
  if (!%result) {
    error("Loading Texture:" SPC %filename SPC "failed!");
    return 0;
  }
  GameScreen.cleanup.add(%result);
  return %result;
}


function tom2Dtexture::replaceTexture(%this,%filename)
{
 %this.setBitmap($Game::ImagePath @ "/" @ %filename);

}


function startBitmapSheet(%filename)
{
  %result = new tom2Dtexture();
  %result.addToBitmapsheet($Game::ImagePath @ "/" @ %filename);
  return %result;
}

function addToBitmapSheet(%textureObj,%filename)
{
  %textureObj.addToBitmapsheet($Game::ImagePath @ "/" @ %filename);
}

function generateBitmapsheet(%textureObj, %maxColumns)
{
   %textureObj.generateBitmapsheet(%maxColumns);
}

//-----------------------------------------------------------------------------
// Init
//-----------------------------------------------------------------------------
function initClient() 
{
      //2021-03-14 added $pref::Game::ScreenResolution prior it was always 800x600
      if ($Game::ScreenResolution $= "")
         $Game::ScreenResolution = "800 600";
         
      if ($Game::ScaleContent $= "")
            $Game::ScaleContent = true;
      

 
         new GuiControl(PlayGui) {
           Profile = "GuiContentProfile";
           HorizSizing = "right";
           VertSizing = "bottom";
           position = "0 0";
           Extent = $Game::ScreenResolution;
           MinExtent = "8 8";
           Visible = "1";
              applyFilterToChildren = "1";
              noCursor = $Game::DisableMouseCursor; 
              cameraZRot = "0";
              helpTag = "0";
              forceFOV = "0";
        
           new tom2DCtrl(GameScreen) {
              canSaveDynamicFields = "0";
              Profile = "PlayCanvasProfile";
              HorizSizing = "width";
              VertSizing = "height";
              position = "0 0";
              Extent = $Game::ScreenResolution;
              BaseExtent = $Game::ScreenResolution;
              ScaleContent = $Game::ScaleContent;
              MinExtent = "8 2";
              canSave = "1";
              Visible = "1";
              hovertime = "1000";
              applyFilterToChildren = "1";
              cameraZRot = "0";
              forceFOV = "30";
           };
        };
        
        GameScreen.cleanup = new SimGroup();
        
        new ScriptObject(GameObject) {
          class = Game;
          screen = GameScreen;
        };
        
        GameScreen.game = GameObject;
        initSound();
        
        if (isObject(GameObject))
            GameObject.onLoad();
        
        Canvas.setContent( PlayGui );
        Canvas.setCursor("DefaultCursor");
        GameScreen.setFirstResponder();
        
        if ($platform $= "Android")
        	hideSplashScreen();
         
   // As we know at this point that the initial load is complete,
   // we can hide any splash screen we have, and show the canvas.
   // This keeps things looking nice, instead of having a blank window
  //moved to core/game.cs Canvas.showWindow();
         
        
}
//-----------------------------------------------------------------------------
// Common Tools
//-----------------------------------------------------------------------------
function DoToggleFullScreen(%val) {
 if ($pref::Video::denyFullScreen)
        return;

 if (%val == 1) 
        toggleFullScreen();
}


function onStart() {}
function onExit() {}

//-----------------------------------------------------------------------------
// package
//-----------------------------------------------------------------------------
package game2DPkg {
        
        function onStart()
        {
           dEcho("game2DPkg::onStart >>>>>>>>>>>>>>>>>>>>>>>>>"); 
           // Parent::onStart();
           setRandomSeed();
           if ($Pref::Video::Resolution !$= "") {
                $width = getWord($pref::Video::resolution,0);
                if ($width < 800) {
                        $pref::Video::resolution = "800 600" SPC getWord($pref::Video::resolution,2);
                        echo("Forcing resolution to 800 x 600");
                }
           }
           if ($Game::ScreenResolution $= "")
           {
               $Game::ScreenResolution = getWords($pref::Video::resolution,0,1);
           }
           
           dEcho("game2DPkg::onStart :: initCanvas...."); 
           initCanvas($Game::Caption);
           
           if ($Game::VersionString !$= "")
                setversionstring($Game::VersionString);
           if ($Game::VersionNumber !$= "")                
                setversionnumber(mFloor($Game::VersionNumber));
           
           
          if (isDebugBuild() || $ALPHAVERSION)
           { 
                exec("core/art/gui/console.gui");
                exec("core/art/gui/consoleVarDlg.gui");
                GlobalActionMap.bind(keyboard, "tilde", toggleConsole);
           }
           
           if (!$pref::Video::denyFullScreen)
                      GlobalActionMap.bind(keyboard, "alt return", DoToggleFullScreen);
           exec($Game::Path @ "/" @ $Game::MainScript);
           dEcho("game2DPkg::onStart :: initClient...."); 
           initClient();
           dEcho("<<<<<<<<<<<<<<<< game2DPkg::onStart");
        }
        
        function onExit()
        {
           //Parent::onExit();
           alxStopAll();
           GameScreen.cleanup.delete();
           PlayGui.delete();
           GameObject.onExit();
           GameObject.delete();
           
        }
}; 
activatePackage(game2DPkg);
