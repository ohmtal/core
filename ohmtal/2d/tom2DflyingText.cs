/*
Flying Text tom2D
 XXTH 2024-02-09
 based on the flying_text resouce on  garagegames.com
 WARNING OGE3D uses colorI not colorF

 
exec("core/ohmtal/2d/tom2dFlyingText.cs");

$t = tom2dFlyingText_wrapper("You found a secret...",-1);$t.AddToQueue("Go west..\nUNTIL YOU SEE..."); $t.AddToQueue("A BIG HEAD!\nThen go east.."); $t.AddToQueue("good luck"); 

--------------------------------------------------------------------------------
//short 
$test = new ScriptGroup() {class = "tom2dFlyingText"; color = -1; text = "Hello World!\nMY FRIEND";};
~~~~
// Appearance Methods (appearance)
// ==================
// -1 - Random Method (any but 'None' method)
//  0 - None
//  1 - Rise Up
//  2 - Zoom from the left
//  3 - Drop Down
//  4 - Zoom from the right
//  5 - Diagonal to straight
//  6 - Four Corners
//  7 - Circle in from right
//
//
// Show Methods (showMethod) (bits set - more than one method can be selected)
// ============
// -1 - Random Method (any but 'None' method) 
//  0 - Steady
//  1 - Sin bounce
//  2 - Cos side-to-side
//  4 - Whole word sin bounce
//  8 - Whole word cos side-to-side


$test = new ScriptGroup() {
   class = "tom2dFlyingText";
   text = "Hello World!\nMY FRIEND";
   parent="Game2DHud";
   //appearance
   appearance=-1; 
   appearanceTime = 600;
   //showing
   showMethod = -1;
   showTime   = 2000;
   showCycle  = 900;
   
   color = "50 90 255 255";
   fontPath = "common/ui/letters/default/"; // traling slash!! 

   onReset = ""; //callback
   deleteOnDone = true; //DEFAULT - else you can use it again
   startOnAdd   = true; //DEFAULT - else you can use .start
   
   sizeModulation = 0.8;
   spaceModualtion = 0.9;
   
}; 
~~~~
with callback :

function newTestText() { $i++; $test = new ScriptGroup() {class = "tom2dFlyingText"; color = -1; text = "Hello World!\n" @ $i; onReset="newTestText();";};}
newTestText();

~~~~
with wrapper

tom2dFlyingText_wrapper("HELLO WORLD");


*/


//~~~~~~~~~~ DEFAULT GLOBALS :: can be overwritten after exec! ~~~~~~~~~~~~~~~~~
// COLORS: red green blue yellow
$tom2dFlyingText::colorCount = 7;
$tom2dFlyingText::color[0] = "255 0 0 255"; //red
$tom2dFlyingText::color[1] = "0 255 0 255"; //green
$tom2dFlyingText::color[2] = "0 0 255 255"; //blue
$tom2dFlyingText::color[3] = "255 255 0 255"; //yellow
$tom2dFlyingText::color[4] = "255 255 255 255"; //white
$tom2dFlyingText::color[5] = "0 255 255 255"; //cyan
$tom2dFlyingText::color[6] = "255 0 255 255"; //magenta

//$tom2dFlyingText::LetterImages = "core/art/gui/letters/default/";
$tom2dFlyingText::LetterImages = "core/art/gui/letters/comic/";
$tom2dFlyingText::LetterImages = "core/art/gui/letters/nice/";
$tom2dFlyingText::ParentObject = "Game2DHud";
$tom2dFlyingText::GuiBaseWidth = 1024; //640
$tom2dFlyingText::GuiBaseHeight = 786; //480;



//------------------------------------------------------------------------------
// CONST 
$PI = 3.14159265;
//------------------------------------------------------------------------------
//simple STATIC  wrapper  
function tom2dFlyingText_wrapper(%text, %color, %time)
{
   %result  = new ScriptGroup() {
      class = "tom2dFlyingText"; 
      color = %color; 
      text = %text;
      showTime = %time;
      sizeModulation = 0.6;
      spaceModualtion = 1.5;
      
      /* test static  
      appearance = 0;
      appearanceTime = 0;
      showMethod = 0;
      */
      
   };
   return %result;
}

//------------------------------------------------------------------------------
function tom2dFlyingText::AddToQueue(%this, %text, %color)
{  
   if (%text $= "")
      return false;
      
   if (%color $= "")
      %color = %this.color;   
      
   %obj = new SimObject() { text = %text; color = %color; };
   %this.add(%obj); 
   return true;
}
//------------------------------------------------------------------------------
function tom2dFlyingText::onAdd(%this)
{
   
   if (!%this.parent) 
      %this.parent = $tom2dFlyingText::ParentObject;

      
   if (!isObject(%this.parent))
   {
      error("tom2dFlyingText invalid parent !! " SPC %this.parent);
      return;
   }
      
   // Get the extent of our play window
   %ext = %this.parent.getExtent();
   %this.parentWidth = getWord(%ext,0);
   %this.parentHeight =  getWord(%ext,1);
      
   // ... appearance ...   
   if (%this.appearance $= "")    
      %this.appearance = -1;
      
   if (%this.appearance < 0)
      %this.appearance = getRandom(6) + 1;
      
      
   if (%this.appearanceTime $= "") 
      %this.appearanceTime = 800;
      
   if (%this.appearanceTime < 1)
      %this.appearanceTime = 1;
      
   // ... method ...   
   %this.setMethod(%this.showMethod, %this.showTime, %this.showCycle);
   // ... font ...
   if (%this.fontPath $= "")
      %this.fontPath = $tom2dFlyingText::LetterImages;

      
   if (%this.sizeModulation $= "")   
      %this.sizeModulation = 1.0;      
      
   if (%this.spaceModualtion $= "")
      %this.spaceModualtion = 1.0;
      
   // ... text ...   
   %this.numChars = 0;
   %this.setText(%this.text, %this.color);   
   
   if (%this.deleteOnDone $= "")
      %this.deleteOnDone = true;
      
   if (%this.startOnAdd $= "") 
      %this.startOnAdd = true;
      
   if (%this.startOnAdd) 
      %this.start();
      
      
      
      
   dEcho("tom2dFlyingText::onAdd::"
      @ "\n %this.appearance=" SPC %this.appearance
      @ "\n %this.appearanceTime=" SPC %this.appearanceTime
      @ "\n %this.showMethod=" SPC %this.showMethod
      @ "\n %this.showTime=" SPC %this.showTime
      @ "\n %this.showCycle=" SPC %this.showCycle
   );
      
}
//------------------------------------------------------------------------------
function tom2dFlyingText::onRemove(%this)
{
   %this.removeControls();
   dEcho("tom2dFlyingText::onRemove(%this)");
}
//------------------------------------------------------------------------------
function tom2dFlyingText::setMethod(%this, %method, %time, %cycle)
{

   %this.showMethod = %method;
   
   if (%this.showMethod $= "")
      %this.showMethod = -1;
      
   if (%this.showMethod < 0)
   {
      %randVal = getRandom(3);
      switch (%randVal) 
      {
        case 0:
            %Method = 1;
        case 1:
            %Method = 2;
        case 2:
            %Method = 4;
        default:
            %Method = 8;
      }
   } else {
      %Method = %this.showMethod;
   }

   %this._showMethod = %Method & 15;
   %this._AlwaysShow = %Method & 16;

   %this.showTime = %time;
   if (%this.showTime $= "")
      %this.showTime = 3000;
      
   %this.showCycle = %cycle;   
   if (%this.showCycle $= "") 
      %this.showCycle = 900;
      
   dEcho("tom2dFlyingText::setMethod(%this, " SPC %this.showMethod SPC %this.showTime SPC  %this.showCycle);
      
}
//------------------------------------------------------------------------------
function tom2dFlyingText::setText(%this, %text, %color)
{
   dEcho("tom2dFlyingText::setText \n" SPC %text @ "\n" @  %color);
   if ( %this.numChars  > 0)
   {
      %this.removeControls();
   }
   %this.text = %text;
   %this.color = %color;
   
   %this.numLines = getRecordCount(%this.text);
   %fixedTextToShow = "";
   for (%i = 0; %i < %this.numLines; %i++)
   {
      %fieldText = getRecord(%this.text,%i);
      %fixedTextToShow = %fixedTextToShow @ %fieldText;
      %nextLineChar[%i] = strlen(%fixedTextToShow)-1;
   }
      
   // Some initial setup
   %this.numChars = strlen(%fixedTextToShow);
   %curLine = 0;
   %this.totalWidth[0] = 0;
   
   %maxHeight = 0;
   
   // Give spaces a width of 40 pixels
   %spaceWidth = mFloor(40 * %this.parentWidth / $tom2dFlyingText::GuiBaseWidth 
                  * %this.sizeModulation *  %this.spaceModualtion); 

   
   // Create a BitmapCtrl for each non-space character
   for (%i=0;%i<%this.numChars;%i++)
   {
      %curChar = getSubStr(%fixedTextToShow,%i,1);
      
      // Skip all the spaces
      if (%curChar !$= " ")
      {
         // Check for special characters
         if (%curChar $= "!")
            %curChar = "ep";
         else if (%curChar $= ".")
            %curChar = "period";
         else if (%curChar $= "?")
            %curChar = "question";
         else if (%curChar $= ":")
            %curChar = "colon";
         else if (%curChar $= "-")
            %curChar = "minus";
         else if (%curChar $= ",")
            %curChar = "comma";
         else if (%curChar $= "\"")
            %curChar = "doublequote";
         else if (%curChar $= "=")
            %curChar = "equal";
         else if (%curChar $= "+")
            %curChar = "plus";
         else if (%curChar $= "\'")
            %curChar = "quote";
         else if (%curChar $= ";")
            %curChar = "semicolon";

         // All our alphanumeric bitmaps are in the folder
         %bitmap = %this.fontPath @ %curChar;


         // create the texture and the sprite
         %texture = new Tom2DTexture(); 
         %texture.setBitMap(%bitmap, true);
         %this.control[%i] = new tom2DSprite();
         %this.control[%i].setTexture(%texture);   
         
         //need to calculate against Basesize to get the same effect:
         %w = mFloor(%texture.getWidth() * %this.parentWidth / $tom2dFlyingText::GuiBaseWidth * %this.sizeModulation); 
         %h = mFloor(%texture.getHeight() * %this.parentHeight / $tom2dFlyingText::GuiBaseHeight * %this.sizeModulation); 

         %this.control[%i].size = %w SPC %h;
         //%this.control[%i].size = %texture.getWidth() SPC %texture.getHeight();
         %this.control[%i].visible = true;
         %this.parent.addRenderObject(%this.control[%i]);
         

         
         if (%this.color) 
         {
            if (getWordCount(%this.color) == 4 ) //onecolor
            {
               %this.control[%i].getTexture().color = %this.color;
               
            } else if (%this.color $= "-2") //all random
            {
               %c = getRandom(0,255) SPC getRandom(0,255) SPC getRandom(0,255) SPC 255;
               %this.control[%i].getTexture().color = %c; 
            } else if (%this.color $= "-1" && $tom2dFlyingText::colorCount * 1 > 0) //random from list
            {
               %c = $tom2dFlyingText::color[getRandom($tom2dFlyingText::colorCount - 1)];
               %this.control[%i].getTexture().color = %c;
            }
            //else it's invalid keep default color
         }
         %this.control[%i].w = getWord(%this.control[%i].size,0) * %this.spaceModualtion ;
         %this.control[%i].h = getWord(%this.control[%i].size,1);


         if (%maxHeight < %this.control[%i].h)
            %maxHeight = %this.control[%i].h;
         
         %width = %this.control[%i].w ;
      }
      else
      {
         // Give spaces a width of 40 pixels
         %width = %spaceWidth; 
         %this.control[%i] = 0;
      }

      // Update our total width
      %this.totalWidth[%curLine] += %width; 
      if (%i == %nextLineChar[%curLine])
      {
         %curLine++;
         %this.totalWidth[%curLine] = 0;
      }
   }

   // Set our beginning x location so our text is centered on the screen
   %x = (%this.parentWidth / 2) - (%this.totalWidth[0] / 2);
   %y = (%this.parentHeight / 2) - %maxHeight - (%this.numLines-1) * %maxHeight;
   %curLine = 0;

   // Now that we know the total width we can set each chars final position
   for (%i=0;%i<%this.numChars;%i++)
   {
      if (%this.control[%i])
      {
         // PuPt2 is the final ending position
         %this.PuPt2x[%i] = %x;
         %this.PuPt2y[%i] = %y;

         // The starting position will depend upon our appearance method
         %this.setPuStartPos(%i);

         // Width of this bitmap
         %width = %this.control[%i].w; 
      }
      else
      {
         // Spaces have a width of 40
         %width = %spaceWidth;
      }

      // Set where our next character will be positioned
      %x += %width;
      if (%i == %nextLineChar[%curLine])
      {
         %curLine++;
         %x = (%this.parentWidth / 2) - (%this.totalWidth[%curLine] / 2);
         %y += mFloor(%maxHeight * 1.5); //120;
      }
   }
}
//------------------------------------------------------------------------------
function tom2dFlyingText::start(%this)
{
   %this.startTime =  GetRealTime();  

   // Schedule the removal of this message
   %this.removeSchedule = %this.schedule( %this.appearanceTime + %this.showTime
      , "reset");
   
   // Set all the chars of the message to their starting positions
   %this.updatePopupMessage();
}   
//------------------------------------------------------------------------------
function tom2dFlyingText::stop(%this)
{
   cancel(%this.removeSchedule);
   %this.removeSchedule = 0;
   cancel(%this.updateSchedule);
   %this.removeSchedule = 0;
}
//------------------------------------------------------------------------------
function tom2dFlyingText::removeControls(%this)
{
  if (%this.numChars > 0 )
   {
      for ( %i=0; %i < %this.numChars; %i++)
      {
         if (%this.control[%i])
         {
            %this.parent.removeRenderObject(%this.control[%i]);
            %t = %this.control[%i].getTexture(); //save texture to delete it later 
            %this.control[%i].delete();
            
            %this.control[%i] = 0;
            if (isObject(%t))
               %t.delete();
         }
      } //for 
   }
}
//------------------------------------------------------------------------------
function tom2dFlyingText::reset(%this, %ignoreDelete)
{
  
   %this.stop();
   %this.removeControls();
   
   //callback
   if (%this.onReset !$= "")
      eval(%this.onReset);
         
   //queue
   if (%this.getCount() > 0)
   {
      %text = %this.getObject(0).text;
      %color = %this.getObject(0).color;
      %this.getObject(0).delete();
      //also update method ... maybe random
      %this.setMethod(%this.showMethod, %this.showTime, %this.showCycle);
      %this.setText(%text, %color);
      %this.start();
      return;
   }
   
   if (!%ignoreDelete && %this.deleteOnDone)
      %this.schedule(0,"delete");
      
}
//------------------------------------------------------------------------------
function tom2dFlyingText::updatePopupMessage(%this)
{
   %diffTime = GetRealTime() - %this.startTime;
   %appearancePercent = %diffTime / %this.appearanceTime;
   %showPercent = %diffTime / %this.showCycle;
   for (%i=0;%i<%this.numChars;%i++)
   {
      if (%this.control[%i])
      {
         %pos = %this.computePopupPosition(%i,%appearancePercent,%showPercent);
         //%this.control[%i].position = %pos;
         %this.control[%i].x = getWord(%pos, 0);
         %this.control[%i].y = getWord(%pos, 1);
      }
   }
   %this.updateSchedule = %this.schedule( 16, "updatePopupMessage");
}
//------------------------------------------------------------------------------
function tom2dFlyingText::computePopupPosition( %this, %i, %percent, %percentShow )
{
   // Special case for the Circle in method
   if (%this.appearance == 7)
   {
      %percent -= %i * 0.04;
      if (%percent < 0)
         %percent = 0;
   }

   // Get our show adjust - %percent will always be set to 1.0 or
   // below after doing this code fragment
   if (%this._AlwaysShow & 16)
   {
      %adjust = %this.computePopupShowAdjust( %i, %percentShow, 1);
      if (%percent > 1)
         %percent = 1;
   }
   else
   {
      if (%percent > 1)
      {
         if (%percent > 1.2)
            %percentToUse = 1;
         else
            %percentToUse = (%percent - 1) / 0.2;

         %adjust = %this.computePopupShowAdjust( %i, %percentShow, %percentToUse);
         %percent = 1;
      }
      else
      {
         %adjust = 0;
      }
   }

   // Break out the x and y components of the adjust value
   %adjustX = getWord(%adjust,0);
   %adjustY = getWord(%adjust,1);

   // Special case for Circle in method
   if (%this.appearance == 7)
   {
      if (%percent <= 0.25)
      {
         // Adjust percent so it covers the range of 0.0 to 0.25
         %percent /= 0.25;

         // Get the interpolated position between pt1 and ptMid
         %workPtX = %this.interpolatePopup(%percent,%this.PuPt1x[%i],%this.PuPtMidx[%i]);
         %workPtY = %this.interpolatePopup(%percent,%this.PuPt1y[%i],%this.PuPtMidy[%i]);
      }
      else if (%percent <= 0.9)
      {
         // Adjust percent so it covers the range of 0.25 to 0.9
         %percent = (%percent - 0.25) / 0.65;
         %piVal = %percent * $PI * 2;
         %sinVal = mSin(%piVal);
         %cosVal = mCos(%piVal);

         // Get our position around the circle
         %workPtX = %this.parentWidth/2 - %sinVal * %this.PuPtMidy[%i];
         %workPtY = %this.parentHeight/2 - %cosVal * %this.PuPtMidy[%i];
      }
      else
      {
         // Adjust percent so it covers the range of 0.9 to 1.0
         %percent = (%percent - 0.9) / 0.1;

         // Get the interpolated position between ptMid and pt2
         %workPtX = %this.interpolatePopup(%percent,%this.PuPtMidx[%i],%this.PuPt2x[%i]);
         %workPtY = %this.interpolatePopup(%percent,%this.PuPtMidy[%i],%this.PuPt2y[%i]);
      }
   }
   else
   {
      // Get the interpolated position between pt1 and pt2
      %workPtX = %this.interpolatePopup(%percent,%this.PuPt1x[%i],%this.PuPt2x[%i]);
      %workPtY = %this.interpolatePopup(%percent,%this.PuPt1y[%i],%this.PuPt2y[%i]);
   }

   %workPtX = mFloor(%workPtX+%adjustX);
   %workPtY = mFloor(%workPtY+%adjustY);

   return (%workPtX SPC %workPtY);
}

//------------------------------------------------------------------------------
function tom2dFlyingText::setPuStartPos( %this, %i )
{
   // Set our starting position based upon our appearance method
   if (%this.appearance == 0)
   {
      %this.PuPt1x[%i] = %this.PuPt2x[%i];
      %this.PuPt1y[%i] = %this.PuPt2y[%i];
   }
   else if (%this.appearance == 1)
   {
      %this.PuPt1x[%i] = %this.PuPt2x[%i];
      %this.PuPt1y[%i] = %this.parentHeight + (%i * 80);
   }
   else if (%this.appearance == 2)
   {
      
      %pos = %this.PuPt2x[%i] - (%this.parentWidth/2) - (%this.totalWidth[0]/2) - 40;
      %this.PuPt1x[%i] = %pos * 3;
      %this.PuPt1y[%i] = %this.PuPt2y[%i];
   }
   else if (%this.appearance == 3)
   {
      %this.PuPt1x[%i] = %this.PuPt2x[%i];
      %this.PuPt1y[%i] = 0 - (%i * 80) - 80;
   }
   else if (%this.appearance == 4)
   {
      %pos = %this.PuPt2x[%i] - ((%this.parentWidth / 2) - (%this.totalWidth[0] / 2));
      %this.PuPt1x[%i] = %this.parentWidth + %pos * 3;
      %this.PuPt1y[%i] = %this.PuPt2y[%i];
   }
   else if (%this.appearance == 5)
   {
      %this.PuPt1x[%i] = (%this.parentWidth/2) - ((%this.parentWidth / 2) - %this.PuPt2x[%i]) * 2;
      %this.PuPt1y[%i] = %this.parentHeight - (%i * 80) - 80;
   }
   else if (%this.appearance == 6)
   {
      switch (%i % 8)
      {
         case 0:
            %this.PuPt1x[%i] = %this.parentWidth;
            %this.PuPt1y[%i] = %this.parentHeight - %this.control[%i].h;

         case 1:
            %this.PuPt1x[%i] = %this.parentWidth - %this.control[%i].w; 
            %this.PuPt1y[%i] = %this.parentHeight;

         case 2:
            %this.PuPt1x[%i] = %this.parentWidth;
            %this.PuPt1y[%i] = 0;

         case 3:
            %this.PuPt1x[%i] = %this.parentWidth - %this.control[%i].w; 
            %this.PuPt1y[%i] = 0 - %this.control[%i].h; 

         case 4:
            %this.PuPt1x[%i] = 0;
            %this.PuPt1y[%i] = %this.parentHeight;

         case 5:
            %this.PuPt1x[%i] = 0 - %this.control[%i].w; 
            %this.PuPt1y[%i] = %this.parentHeight - %this.control[%i].h;

         case 6:
            %this.PuPt1x[%i] = 0;
            %this.PuPt1y[%i] = 0 - %this.control[%i].h; 

         case 7:
            %this.PuPt1x[%i] = 0 - %this.control[%i].w; 
            %this.PuPt1y[%i] = 0;
      }
   }
   else if (%this.appearance == 7)
   {
      %this.PuPt1x[%i] = %this.parentWidth;
      %this.PuPt1y[%i] = %this.parentHeight/4;
      %this.PuPtMidx[%i] = %this.parentWidth/2;
      %this.PuPtMidy[%i] = %this.PuPt1y[%i];
   }
}


//------------------------------------------------------------------------------
function tom2dFlyingText::interpolatePopup( %this, %percentage, %pt1, %pt2)
{
    %result = %pt1 + (%pt2 - %pt1) * %percentage;

    return %result;
}



//------------------------------------------------------------------------------
function tom2dFlyingText::computePopupShowAdjust( %this, %i, %percent, %percentToUse )
{
   // Default to adjusting neither 'x' nor 'y'
   %adjustX = 0;
   %adjustY = 0;

   if (%this._showMethod  & 3)
   {
      // Modify percent by char position
      %percent1 = %percent + (%i/10);

      if (%this._showMethod  & 1)
      {
         %offset = 12 * mSin($PI * 2 * %percent1);
         %adjustY = %offset * %percentToUse;
      }

      if (%this._showMethod  & 2)
      {
         %offset = 12 * mCos($PI * 2 * %percent1);
         %adjustX = %offset * %percentToUse;
      }
   }

   if (%this._showMethod  & 4)
   {
      %offset = 12 * mSin($PI * 2 * %percent);
      %adjustY += %offset * %percentToUse;
   }

   if (%this._showMethod  & 8)
   {
      %offset = 12 * mCos($PI * 2 * %percent);
      %adjustX += %offset * %percentToUse;
   }

   return (%adjustX SPC %adjustY);
}





