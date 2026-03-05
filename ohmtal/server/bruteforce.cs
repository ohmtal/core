//------------------------------------------------------------------------------
// Bruteforce protection
// (c) huehn-software
//------------------------------------------------------------------------------

function initBruteForce(%maxposts, %ResetSec, %blockSec) {
  
  if (isObject($BruteProtector)) $BruteProtector.delete();
  $BruteProtector = new SimObject() {
     maxPosts = %maxPosts;
     resetTime = %ResetSec * 1000;
     blockTime = %blockSec * 1000;
  };
}

//------------------------------------------------------------------------------
function BruteForceDeleteObj(%obj) {
  if (!isObject($BruteProtector) || !isObject(%obj)) {
    return;
  }  
  %ip = %obj.ip;
  %obj.delete();
// error("DELETE IP =" SPC %ip);  
  $BruteProtector.ip[%ip]="";
}
//------------------------------------------------------------------------------
// checkBrute
// handles brute objects 
// return 0 if BLOCKED and 1 if OK
function checkBrute(%addr, %addinfo) {
  
  if (!isObject($BruteProtector)) {
     error ("Calling CheckBrute without object - i'll do a init for you :P !!!");
     initBruteForce(5, 3, 300); 
  }
  
   if (getSubStr(%addr , 0, 3) $= "IP:" ) { 
      %needle=strpos( %addr , ":", 3 ) - 3;
      %ip = getSubStr(%addr, 3, %needle);
// error("NEEDLE / IP =" SPC %needle @ " *"@%ip@"*");      
   } else {
      //we only want to check ip addresses :P
      return 1;
   }
      error("CHECK : " SPC %addr SPC %ip);
   
 
  
  
  if (!isObject($BruteProtector.ip[%ip])) {  //no current protector object
     %obj = new SimObject() {
       ip = %ip;
       count = 1;
       blocked = 0;
     };
     %obj.expireschedule = schedule($BruteProtector.resetTime,0,BruteForceDeleteObj,%obj);
     $BruteProtector.ip[%ip] = %obj;
     return 1;
  } else { 
     %obj = $BruteProtector.ip[%ip];
     if (%obj.blocked) {
      cancel(%obj.expireschedule);
      %obj.expireschedule = schedule($BruteProtector.blockTime,0,BruteForceDeleteObj,%obj);
      echo("BRUTEPROTECTOR: BLOCK TIME ENHANCED FOR IP/INFO:" SPC %addr SPC %addinfo); 
      return 0;
     }
     
     %obj.count ++;
     if ( %obj.count > $BruteProtector.maxPosts) {
        cancel(%obj.expireschedule);
        %obj.expireschedule = schedule($BruteProtector.blockTime,0,BruteForceDeleteObj,%obj);
        %obj.blocked = 1;
        echo("BRUTEPROTECTOR: BLOCKED IP/INFO:" SPC %addr SPC %addinfo); 
     } else {
        return 1;
     }
  }
  return 0;
}
