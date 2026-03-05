// exec("core/ohmtal/shared/misc.cs");
//------------------------------------------------------------------------------
function togglePause(%val) {
  if (%val && $Server::ServerType $= "SinglePlayer") { 
     if ($timescale == 0) {
          if ($SavTimeScale < 1 ) $SavTimeScale = 1;
          $timescale = $SavTimeScale;
          clientCmdClearCenterPrint();
          if (PlayGuiCountDown.isVisible()) PlayGuiCountDown.PauseTime(false);
     } else {
          $SavTimeScale = $timescale; 
          clientCmdCenterPrint( "Pause", 0);
          $timescale = 0;
          if (PlayGuiCountDown.isVisible()) PlayGuiCountDown.PauseTime(true);
     }
  }
}
//------------------------------------------------------------------------------
// Lang tool!
//------------------------------------------------------------------------------
/*
initLangTable
==============
  Init LangTable and set $GLOBALS::LANGUAGES for use in gui.

Example
   exec("common/shared/misc.cs");
   initLangTable("towers/shared/lang","english deutsch");

*/
function initLangTable(%path,%languages)
{
   $GLOBALS::LANGUAGES = %languages;
   %cnt = getWordCount(%languages);
   if (%cnt == 0)
      return false;

   // Init Language tool:
   $I18N::DEFAULT = new LangTable();
   
   %default = getWord(%languages,0);
   
   exec(%path @ "/" @ %default @ ".cs");

   for (%i=0; %i<%cnt; %i++)
        $I18N::DEFAULT.addLanguage(%path @ "/" @ getWord(%languages,%i) @ ".lso", getWord(%languages,%i));
        
        
   $I18N::DEFAULT.setDefaultLanguage(0);
   $I18N::DEFAULT.setCurrentLanguage(0);
   
   if ($pref::langID $= "" || $pref::langID >= %cnt)
        $pref::langID = 0;
   
   if ($pref::langID != 0 )
        $I18N::DEFAULT.setCurrentLanguage($pref::langID);


}
//------------------------------------------------------------------------------
function setNewCurrentLanguage(%id)
{
  $pref::langID = %id;
  $I18N::DEFAULT.setCurrentLanguage(%id);
}
//------------------------------------------------------------------------------
// Translate! Stupid short Name *lol*
function Tr(%id)
{
  $I18N::DEFAULT.getString(%id);
}

function T(%string)
{
  if (!isObject($I18N::DEFAULT))
        return %string;
  %id = getLangIDfromString(%string);
  if (%id >= 0)
      return $I18N::DEFAULT.getString(%id);
  return %string;
}
//------------------------------------------------------------------------------
function getLangIDfromString(%string)
{
  return $I18N::DEFAULT.getIdFromString(%string);
}
//------------------------------------------------------------------------------
// THIS MUST HAVE ALL VARIABLES IN STYLE $L_[NO] starting with 1!
// can be called on GuiControl::OnAdd :D
function LangPrepareSingleGuiControl(%gui)
{
  %gui.langTableMod = "DEFAULT";
  if (%gui.text !$= "")
  {
    %id = getLangIdfromString(%gui.text);
    if (%id >=0)
      %gui.TextID = "L_" @ (%id+1);
  }
  // now look at tool tip:
  if (%gui.tooltip !$= "")
  {
    %id = getLangIdfromString(%gui.tooltip);
    if (%id >=0)
      %gui.tooltip = Tr(%id);
  }
  

}
//------------------------------------------------------------------------------
// THIS MUST HAVE ALL VARIABLES IN STYLE $L_[NO] starting with 1!
function LangPrepareGui(%gui, %initial)
{
  if (%initial)
      %gui.langTableMod = "DEFAULT";
      
  if (%gui.text !$= "")
  {
    %id = getLangIdfromString(%gui.text);
    if (%id >=0)
      %gui.TextID = "L_" @ (%id+1);
  }

  if (%gui.getCount() > 0) //recursive walk the gui!
    for (%i = 0; %i< %gui.getCount(); %i++)
        LangPrepareGui(%gui.getObject(%i), false);

}
//------------------------------------------------------------------------------
// %APPDATA% Datadirectory
/*
  Depends on:
        $Game::Company = "Ohmtal Game Studio";
        $Game::Caption = "Ohmtal TEMPLATE";
  
  Setup 
        $Game::DataDirectory  

        
=> MOVED TO common/game.cs!        
*/
//------------------------------------------------------------------------------

//------------------------------------------------------------------------------
// Fonts!
//------------------------------------------------------------------------------
function makeMyFonts()
{
   CreateFontRanges("Arial","9 10 12 14 16 18 20 22 24 26 28 30 32 36 48");
   CreateFontRanges("Ardagh","9 10 12 14 16 18 20 22 24 26 28 30 32 36 48");
//   CreateFontRanges("Arial Bold","14 16 18 20 22 24 36 48");
//   CreateFontRanges("Arial Italic","14 16 18 20 22 24 36");
//   CreateFontRanges("Arial Bold Italic","14 16 18 20 22 24 36");
   CreateFontRanges("Comic Sans MS","14 16 18 20 22 24 36 48");
   CreateFontRanges("Terminator Two","20");
}

function CreateFontRanges(%fontName, %sizes)
{
    for (%i = 0; %i < getWordCount(%sizes); %i++)            
            populateFontCacheRange(%fontName, getWord(%sizes, %i), 32, 126);      
     writeFontCache();  
}


//------------------------------------------------------------------------------
// debug output IMPLEMENTED as  engine function .. also dWarn
//------------------------------------------------------------------------------
/*
function dEcho(%msg)
{
  if (isDebugBuild())
        echo("DEBUG:" SPC %msg);
}
//------------------------------------------------------------------------------
function dError(%msg)
{
  if (isDebugBuild())
        error("DEBUG:" SPC %msg);
}
*/
//------------------------------------------------------------------------------
// string stuff
//------------------------------------------------------------------------------

function leading_chars(%data,%len,%char)
{
   while (strlen(%data)<%len)
   {
      %data = %char @ %data;
   }
   return(%data);
}

//------------------------------------------------------------------------------
function fillLeadingChar(%base,%len, %char )
{
   %result = %base;
   while (strlen(%result) < %len)
       %result = %char @ %result;
   
   return %result;
}
//------------------------------------------------------------------------------
function fillTrailingChar(%base,%len, %char )
{
   %result = %base;
   while (strlen(%result) < %len)
       %result = %result @ %char;
   
   return %result;
}


/*-----------------------------------------------------------------------------
 weighted random curved
 
 lets say getrandom(1,5) 
 where 1 should be called much more than 5 
  
 weights would be 
   1 => 5
   2 => 4
   3 => 3
   4 => 2
   5 => 1
  weight sum =  15    
   RAND  RESULT 
     1   1
     2   1
     3   1
     4   1
     5   1
     6   2
     7   2
     8   2
     9   2  
    10   3
    11   3
    12   3
    13   4  
    14   4
    15   1
   
  new rewritten ... now it rocks :D
  
  echo(getWeightedCurveRandom(1,5));
  for ($i=0; $i<100; $i++) getWeightedCurveRandom(1,5);
-----------------------------------------------------------------------------*/
function getWeightedCurveRandom(%min, %max) 
{
   %wMax =  %max-%min +1 ;
   
   //dError("getWeightedCurveRandom min/max:" SPC %min SPC %max SPC "WMAX=" SPC %wmax);
   %j = %min;
   %randValue = 0;   
   for (%i = 0; %i <%Wmax; %i++)
   {
      %weight = %wMax - %i;
      for (%k = 0; %k<%weight; %k++)
      {
         %randTable[%k+%randValue]=%j;
      }
      %randValue += %weight;
      //dEcho(%i SPC %weight SPC %j SPC %randValue);
      %j++;
      
   }
   %randValue--;
/*   
dError("------------------------------");
   for (%i=0; %i<=%randValue; %i++)
   {
         dEcho(%i SPC %randTable[%i]); 
   }
dError("------------------------------");
*/
   %randNeedle = getRandom(%randValue-1);
   %result = %randTable[%randNeedle];

 //dEcho("randValue:" SPC %randValue SPC "needle" SPC %randNeedle SPC "result:" SPC %result);   
  if (%result * 1 < %min) {
   error("getWeightedCurveRandom FAAAAAAIILLL !!!");
   %result = getRandom(%min,%max); 
  }


   return %result;
 
   
}


//------------------------------------------------------------------------------
// misc
//------------------------------------------------------------------------------
function safeDelete(%obj)
{
   if (isObject(%obj))
   {
     %obj.schedule(0,"delete");
     %obj=0;
   }

}

//special round
function tomRound(%val, %prec)
{
   if (!%prec)
      return mRound(%val);
      
   %n = "1";   
   for(%i=0; %i<%prec; %i++)
   {
      %n = %n @ "0"; 
   }   
   return mRound(%val * %n) / %n;
   
}
//special round words like vectors
function tomRoundWords(%string, %prec)
{
   %result = "";
   for(%i = 0; %i<getWordCount(%string); %i++)
   {
      %val = tomRound(getWord(%string,%i),%prec);
      %result = %result SPC %val;
   }
   return %result;
}