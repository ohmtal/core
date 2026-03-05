//-----------------------------------------------------------------------------
// Canvas
//-----------------------------------------------------------------------------
function initCanvas(%windowName)
{
   dEcho("initCanvas >>>>>>>>>>>>>>>>>>>>>>>>>"); 
   exec($Game::Path @ "/ui/defaultProfiles.cs");
   exec($Game::Path @ "/ui/cursor.cs");

   dEcho("initCanvas :: createCanvas....."); 
   if (!createCanvas(%windowName)) {
      quit();
      return;
   }
   
   
   
  Canvas.centerX = getWord(Canvas.getCenter(), 0);
  Canvas.centerY = getWord(Canvas.getCenter(), 1);
  Canvas.width   = getWord(Canvas.getExtent(), 0);
  Canvas.height = getWord(Canvas.getExtent(), 1);
   
  Canvas.scaleW = Canvas.width / getWord($Game::ScreenResolution,0);
  Canvas.scaleH = Canvas.height / getWord($Game::ScreenResolution,1);
   dEcho("<<<<<<<<<<<<<<<<<<<<<<<< initCanvas"); 

}

function resetCanvas()
{
   if (isObject(Canvas))
   {
      Canvas.repaint(); 
   }
}



function GuiCanvas::onResize(%this)
{
  warn("******************* CANVAS onRESIZE !!!!***********************");
  %this.centerX = getWord(%this.getCenter(), 0);
  %this.centerY = getWord(%this.getCenter(), 1);
  %this.width   = getWord(%this.getExtent(), 0);
  %this.height = getWord(%this.getExtent(), 1);

  %this.scaleW = Canvas.width / getWord($Game::ScreenResolution,0);
  %this.scaleH = Canvas.height / getWord($Game::ScreenResolution,1);
  
  if (isObject(GameObject) && GameObject.isMethod("onResize"))
      GameObject.onResize(); 
}



