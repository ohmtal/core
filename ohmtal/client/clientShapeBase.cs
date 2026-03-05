//exec("core/ohmtal/client/clientShapeBase.cs");


function getClientPlayer()
{
  return (ServerConnection.getcontrolobject());
}

function getClientPlayerIsDead()
{
  return (getclientPlayer().isdisabled());
}

//-----------------------------------------------
function clientSearchAll() {
   ContainerClearSearch(true);
   initContainerALLSearch(true);
   while ((%targetObject = containerSearchNext(true)) != 0) {
      echo("You found: " SPC %targetObject.getid() ); //SPC %targetObject.getShapeName()); 
   }

}

/**
 * getClientCloseObject
 * Return the closed object 
 * Defaults: 
 * %vectorAhead = 2; 
 * %radius = 2;  
 * %typeMask = $TypeMasks::PlayerObjectType | $TypeMasks::ItemObjectType
*/
function getClientCloseObject(%vectorAhead, %radius, %typeMask) {
    
   if (  %vectorAhead * 1 == 0) 
   {
      %vectorAhead = 2; 
   }
   if (  %radius * 1 == 0) 
   {
      %radius = 2;  
   }
   if (  %typeMask * 1 == 0) 
   {
      %typeMask = $TypeMasks::PlayerObjectType | $TypeMasks::ItemObjectType;    
   }
   
   %player = getClientPlayer();

   %pos = %player.getTransform();
   %x = getWord(%pos, 0);
   %y = getWord(%pos, 1);
   %z = getWord(%pos, 2);
   // adjust z value a bit...
   %z += 2.0;
   %finalPos = %x SPC %y SPC %z;

   %eye = %player.getEyeVector();
   %vec = vectorScale(%eye, %vectorAhead);
   %finalPos = vectorAdd(%finalPos, %vec);
   
   ContainerClearSearch(true);
   InitContainerRadiusSearch(%finalPos, %radius, %typeMask, true);
   %dist = 1000000; %obj = 0;
   while ((%targetObject = containerSearchNext(true)) != 0) {
      
       if (%targetObject.getid() == %player.getId() || %targetObject.isMounted())
         continue;
       %curDist = VectorDist(%targetObject.getPosition(), %player.getPosition());
       if (%curDist < %dist) {
         %obj = %targetObject;
         %dist = %curDist;  
       }
      dEcho("YOU SEE : " SPC %targetObject.getid() SPC %targetObject.getShapeName() SPC "DIST:" SPC %curDist); 
   }
   if (%obj)
      dEcho("YOU SELECTED: " SPC %obj.getid() SPC %obj.getShapeName()); 
   return %obj;
}
