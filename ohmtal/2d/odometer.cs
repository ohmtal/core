/**
 * Odometer
 *
 Example: 
                
  $odometer = createOdometer(GameScreen, 9,250,180,20,50,9,%this.img_digits);
  $odometer.mode=1;
  
 Modes:
   0: Like car odometer count up one by one
   1: All digits roll simultan to their position
   2: Single Digit rolling starting by the right walking to left
  
  
*/
function createOdometer(%Screen, %x,%y,%w,%h,%layer,%digits,%digitImage)
{

  %obj = new tom2DScriptRenderObject()
  {
     class = "Odometer";
     image  = %digitImage;
     digits = %digits;
     x      = %x;
     y      = %y;
     w      = %w; 
     h      = %h;
     layer  = %layer;
     speed  = 5;
     value  = 0;
     targetValue = 0;
     mode  = 0; //0 like a car odometer always counts +1, 1=like a flipper odometer each digit is rotated
                //2 like 1 but starting with left to right
     sound = "";
  };
  %obj.init();
  %screen.addRenderObject(%obj);
  return %obj;
}
//-----------------------------------------------------------------------------
function Odometer::init(%this)
{
  %this.setDigits(%this.digits);
  //down from setDigits ! %obj.setDimension(%this.w,%this.h);
  %this.progress = 0; // 0..1
  
  %this.textureWidth = %this.image.getWidth();
  %this.textureHeight = %this.image.getHeight();
  %this.setDigitStrValues();
}
//-----------------------------------------------------------------------------
function Odometer::setDigits(%this,%digits)
{
  %this.digits = %digits;
  %this.limit="";
  for (%i = 0 ; %i < %digits; %i++)
  {
     %this.limit = %this.limit @ "9";
     %this.digitAnim[%i] = false;
  }
  %this.limit++;
  %this.setDimension(%this.w,%this.h);
 
}
//-----------------------------------------------------------------------------
function Odometer::setDimension(%this,%w,%h)
{
   %this.digitWidth = %w / %this.digits;
   %this.digitheight= %h;
   %this.w = %w; 
   %this.h = %h;
}
//-----------------------------------------------------------------------------
function Odometer::setValue(%this, %value, %animate)
{
   if (%mode == 0 && %value >= %this.limit)
        %value %= %this.limit;
   if (%animate) {
       %this.targetValue = %value;
       if (%this.mode > 0)
       {
         %this.setStrTargetValue();
         if (%this.mode == 2)
            %this.mode2Rolling = %this.digits-1;
       }
   } else {
      %this.value = %value;
      %this.targetValue = %value;
      %this.setDigitStrValues();
   }
}
//-----------------------------------------------------------------------------
function Odometer::onUpdate(%this,%dt)
{
  if (%this.value != %this.targetValue)
  {
     %this.progress += %dt*%this.speed;
  
     switch (%this.mode)
     {
       case 1:
             if (%this.progress >= 1) 
             {
                  %this.progress=0;
                  %foundValue = false;
                  for (%i=0; %i < %this.digits; %i++)
                  {
                    %cv =  getSubstr(%this.strValue,%i,1);
                    %tv =  getSubstr(%this.strTargetValue,%i,1); 
                    if (%cv != %tv) 
                    {
                      %cv = (%cv+1) % 10;
                      %tmpstr = "";
                      if (%i>0)
                        %tmpstr = getSubStr(%this.strValue, 0,%i);
                      %tmpstr = %tmpstr @ %cv;
                      %tmpstr = %tmpstr @ getSubStr(%this.strValue, %i+1,255);
                      %this.strValue = %tmpstr;
                      %foundValue = true;
                    }
                  }
  
                if (!%foundValue)
                {
                 %this.value = %this.strValue;
                 %this.setDigitStrValues();
                 
                }
                if (isObject(%this.sound))
                        alxPlay(%this.sound);
               
                
             }
       case 2: //left to right
             if (%this.progress >= 1) 
             {
                  %this.progress=0;
                  %foundValue = false;


                    %i = %this.mode2Rolling;    
                    %cv =  getSubstr(%this.strValue,%i,1);
                    %tv =  getSubstr(%this.strTargetValue,%i,1);
                    
                    if (%cv != %tv) 
                    {
                      %cv = (%cv+1) % 10;
                      %tmpstr = "";
                      if (%i>0)
                        %tmpstr = getSubStr(%this.strValue, 0,%i);
                      %tmpstr = %tmpstr @ %cv;
                      %tmpstr = %tmpstr @ getSubStr(%this.strValue, %i+1,255);
                      %this.strValue = %tmpstr;
                      %foundValue = true;
                    } 

                if (!%foundValue)
                {
                   %this.mode2Rolling--;
                }
                if (%this.mode2Rolling < 0)
                {
                   %this.value = %this.strValue;
                   %this.setDigitStrValues();
                } else if (!%foundValue)   //skip rolls which are equal
                {
                   %i = %this.mode2Rolling;
                   
                   %cv =  getSubstr(%this.strValue,%i,1);
                   %tv =  getSubstr(%this.strTargetValue,%i,1);
                   
                   if (%cv == %tv)
                        %this.progress = 1;
                } else {
                   if (isObject(%this.sound))
                        alxPlay(%this.sound);
                }
             }
       default:
             if (%this.progress >= 1) 
             {
                while ( %this.progress >= 1 && %this.value < %this.targetValue)
                {
                  %this.progress--;
                  %this.value++;
                  if (isObject(%this.sound))
                        alxPlay(%this.sound);
                  
                }
               %this.setDigitStrValues();
             }
     }
  } else {
    %this.progress = 0;
  }
   
  

}
//-----------------------------------------------------------------------------
function Odometer::setStrTargetValue(%this)
{
   %this.strTargetValue= %this.targetvalue;
   while (strlen(%this.strTargetValue) < %this.digits)
       %this.strTargetValue = "0"  @ %this.strTargetValue;
}
//-----------------------------------------------------------------------------
function Odometer::setDigitStrValues(%this)
{
   %this.strValue= %this.value;
   while (strlen(%this.strValue) < %this.digits)
       %this.strValue = "0"  @ %this.strValue;
   
   %this.setStrTargetValue();
}
//-----------------------------------------------------------------------------
function Odometer::onRender(%this,%screen)
{
  %savAlign = %screen.ImageAlign; 
  %screen.ImageAlign=1;

  for (%i = 0; %i< %this.digits; %i++)
        %this.drawDigit(%screen,%i);

/*        
// DEBUG output:

%screen.writeText(%this.x,
                 %this.y+%this.h,
                 "STRVALUE:" SPC %this.strValue 
                 SPC "Value:" SPC %this.value 
                 SPC "TARGETVALUE:" SPC %this.targetValue
                 ,$textAlignLeft);
*/        
 %screen.ImageAlign= %savAlign;
}
//-----------------------------------------------------------------------------
function Odometer::drawDigit(%this,%screen, %id)
{
  %value = getSubstr(%this.strValue,%id,1);  
  %doAnim=false;  
  
  if (%this.progress > 0)
  {
     switch (%this.mode)
     {
       case 1: // like flipper
           %targetvalue = getSubstr(%this.strTargetValue,%id,1); 
           %doAnim = %targetvalue != %value; 
       case 2: // like 1 but one by one
           if (%id == %this.mode2Rolling)
           {
             %targetvalue = getSubstr(%this.strTargetValue,%id,1);
//echo("Odometer::drawDigit ID:" SPC %id SPC "val/target:" SPC %value SPC %targetValue);           
             %doAnim = %targetvalue != %value;
           }
       default:  //like car
          //set all digits to animate on progress by default
          %doAnim = %this.progress != 0;
          //check if the right neightbour is not a 9 so we dont need to animate! 
          if (%doAnim && %id < %this.digits-1)
          {
             
             for (%i = %id+1; %i<%this.digits; %i++)
             {
                %nValue = getSubstr(%this.strValue,%i,1);
                if (%nValue != 9)
                {
                   %doAnim = false;
                   break;
                }
             }
             %nValue = getSubstr(%this.strValue,%id+1,1);
             if (%nValue != 9)
                %doAnim = false;
          }
     }
  }
  %x = %this.x+%this.digitWidth*(%id+1);
  

  if (%doAnim)
  {
  
  
     %h1 = %this.textureHeight * (1-%this.progress);
     %h2 = %this.textureHeight * %this.progress;
     %rect1 = "0" SPC %h2 SPC %this.textureWidth SPC (%this.textureHeight-%h2);
     %rect2 = "0 0" SPC %this.textureWidth SPC %h2;
     //ConsoleMethod( tom2DCtrl, drawRect, void, 10, 11, "(tom2DTexture,imgId,x,y,layer,w,h,srcRect,(optimizetransparent)"

     %rh1 = mFloor(%this.digitHeight * (1-%this.progress));
     %rh2 = %this.digitHeight - %rh1;

     
     %value2 = (%value+1) % 10;
     
      %screen.drawRect(%this.image,%value,%x,%this.y,%this.layer,%this.digitWidth,%rh1,%rect1);
      %screen.drawRect(%this.image,%value2,%x,%this.y+%rh1,%this.layer,%this.digitWidth,%rh2,%rect2);
      
  } else {
     // echo("Odometer::drawDigit " SPC %x SPC %this.y SPC %this.digitWidth SPC %this.digitHeight);
     %screen.drawstretch(%this.image,%value,%x,%this.y,%this.layer,%this.digitWidth,%this.digitHeight);
  
  }
  
  
}
