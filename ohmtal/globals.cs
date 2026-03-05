//-----------------------------------------------------------------------------
// Ohmtal Game Engine
// global definitions 
//-----------------------------------------------------------------------------


$Behavior["Seek"]       = BIT(1);
$Behavior["Flee"]       = BIT(2);
$Behavior["Arrive"]     = BIT(3);
$Behavior["Wander"]     = BIT(4);
$Behavior["Cohesion"]   = BIT(5);  
$Behavior["Separation"] = BIT(6); 
$Behavior["Alignment"]  = BIT(7); 
$Behavior["unused"]     = BIT(8);  
$Behavior["Wall_Avoidance"]  = BIT(9);
$Behavior["Path"]       = BIT(10);
$Behavior["Pursuit"]    = BIT(11);
$Behavior["Evade"]      = BIT(12);
$Behavior["Interpose"]  = BIT(13);
$Behavior["Hide"]       = BIT(14);
$Behavior["Flock"]      = BIT(15);  //=> cohesion+separation+aligment
$Behavior["Offset_Pursuit"] =  BIT(16); 
$Behavior["PursuitPathBehaviourType"] = BIT(17); 

