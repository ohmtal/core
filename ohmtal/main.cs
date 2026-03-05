//-----------------------------------------------------------------------------
// Ohmtal Game Engine
// common stuff
//-----------------------------------------------------------------------------

   exec("./globals.cs");
   exec("./shared/envcache.cs");
   exec("./shared/filefunc.cs");
   exec("./shared/misc.cs");
   exec("./shared/t3dmediation.cs");
   
   exec("./server/admin.cs");
   exec("./server/Goal.cs");
   exec("./client/clientShapeBase.cs");


   //define behaviour on  loaded objects with same name:  
   $Con::redefineBehavior="renameNew";
   

