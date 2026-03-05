// exec("core/ohmtal/server/Goal.cs");
/* -------------------- evo goals --------------------------------
 * Base Classes:
 *    - Goal
 *    - Goal_Composite
 *    - Goal_Evaluator
 *
 *
 * I don't understand why there is goal and goal_composite ....
 * I think I only need One Object! => Goal
 *
 * !!!!!!!!!!! subgoals: !!!!!
 * Mat put the subgoals on front and call front first. But Simgroup can only
 * add objects so I use the "last" 
*/ 

/* 




gah torquescript is sometime very tricky!!
==>
$bla1 = new simgroup(test ) {class = "goal"; num=1;};
$bla2 = new simgroup(test ) {class = "goal"; num=2;};
function test::process(%this) { echo ("testnum:" SPC %this.num); }
$bla1.process();
$bla2.process();

!!!!!!!!!!!!!!!!!!! WHILE IT WORKS IN OGE IN OGE3D IT DOESNT !!!!!!!!!!!!!!!!!!!
$bla2.process(); does not call the function !!!

$bla1 = new simgroup(  ) {class = "test"; superclass="huhu"; num=1;};
$bla2 = new simgroup( ) {class = "test"; superclass="huhu"; num=2;};
function test::process(%this) { echo ("process....testnum:" SPC %this.num); }
function huhu::info(%this) { echo ("info.....testnum:" SPC %this.num); }
$bla1.process();$bla1.info();
$bla2.process();$bla1.info();




ID-TEST 
function ui() { $bla = new SimGroup(); schedule(10,0,ui); $i++; if ($i==1000) {echo($bla);$i=0;} }
function ui() { }



/* ----------------------------------------------------------------
  *  Goal
  
  
   
  
    Implemented as SimGroup NOOOOOOOOOOOOOO SimSet!!! 
   properties: 
      int type
      class ? entity_type => owner
      int(enum) status : 0=inactive, 1=active, 2=completed  3=failed
  
   [X]  void AddSubgoal(Goal<entity_type>* g)
   [X] virtual bool HandleMessage(const Telegram& msg){return false;}

   [X] void ActivateIfInactive();
   [X] void ReactivateIfFailed();
   [X] virtual void Activate() = 0;
   [X] virtual int  Process() = 0;
   [X] virtual void Terminate() = 0;
   
   [X] int  ProcessSubgoals();
   [X] bool ForwardMessageToFrontMostSubgoal(const Telegram& msg);
         ** function Goal::forwardMessageToSubgoal(%this,%msgObj)
         not front!!
  
   [X]  bool         isComplete()const{return m_iStatus == completed;} 
   [X]  bool         isActive()const{return m_iStatus == active;}
   [X]  bool         isInactive()const{return m_iStatus == inactive;}
   [X]  bool         hasFailed()const{return m_iStatus == failed;}
   [X]  int          GetType()const{return m_iType;}
  
   [~] typedef std::list<Goal<entity_type>* > SubgoalList;
   [~] SubgoalList   m_SubGoals;
   [~] Goal_Composite(entity_type* pE, int type):Goal<entity_type>(pE,type){}
*/

//globals:
$gs_inactive   = 0;
$gs_active     = 1;
$gs_completed  = 2;
$gs_failed     = 3;
 
/* EXAMPLE ONLY
function create_Goal(%owner,%type)
{
   %result = new SimSet!!!(Goal) {
      //class       = Goal;
      owner       = %owner;
      type        = %type;

      status      = $gs_inactive; 
   };

   return %result;
}
*/
function Goal::addSubGoal(%this, %goalObj)
{
   %this.add(%goalObj);
}

function Goal::message(%this,%msgObj) { /*virtual*/ return false;}

function Goal::handleMessage(%this,%msgObj) {

   if (%this.getCount() > 0) {
      return %this.forwardMessageToSubgoal(%msgObj);
   } else {
      return %this.message(%msgObj);
   }
      
}

function Goal::forwardMessageToSubgoal(%this,%msgObj) 
{
   // in book it's used the front most object but we can only add so we use the last!
   
   if (%this.getCount() > 0) {
      return %this.getObject(%this.getCount() - 1).handleMessage(%msgObj);
   }
   return false;
}


function Goal::isComplete( %this )  {return %this.status == $gs_completed; } 
function Goal::isActive( %this )    {return %this.status == $gs_active;    }
function Goal::isInactive( %this )  {return %this.status == $gs_inactive;  }
function Goal::hasFailed( %this )   {return %this.status == $gs_failed;    }
function Goal::getType( %this )     {return %this.type;}


function Goal::ActivateIfInactive(%this)
{
   if (%this.isInactive()) {
      %this.activate();
   }
   return true;
}
function Goal::ReactivateIfFailed(%this)
{
   if (%this.hasFailed()) {
      %this.status = $gs_inactive;
   }
   return true;
}
function Goal::activate(%this) { /*virtual*/ return false;}

/**
 * This is used to reduce redundant source on Goals
 * It's for subgoal handling ! 
 * 
 * return true => proceed , false => return %this.status.
 * so the call is 
   if (!%this.preProcess()) {
      return %this.status;
   }

 */
function Goal::preProcess(%this) {
   %this.ActivateIfInactive();
   %SubgoalStatus = %this.ProcessSubgoals();
   if (%SubgoalStatus == $gs_active)
      return false; //%this.status;

   if (%SubgoalStatus == $gs_failed) {
      %this.status = $gs_failed;
      return false; // %this.status;
   }  
   return true;
}

function Goal::process(%this) { /*virtual*/  return false;}
function Goal::terminate(%this) { /*virtual*/ return true;}

function Goal::RemoveAllSubgoals(%this)
{
   /* simset rewite
   while (%this.getCount() > 0) {
      %this.getObject(0).RemoveAllSubgoals();
      %this.getObject(0).terminate();
      %this.getObject(0).delete();
    }
   */ 
   
   while (%this.getCount() > 0) {
      %this.getObject(0).RemoveAllSubgoals();
      %this.getObject(0).terminate();
      %this.getObject(0).status = 0;
      %this.remove(%this.getObject(0));
    }

   
}

function Goal::ProcessSubgoals(%this)
{ 
  //remove all completed and failed goals from the end of the  list
  
  while (  %this.getCount() > 0 && //!m_SubGoals.empty() &&
        (   %this.getObject(%this.getCount() - 1).isComplete() 
            || %this.getObject(%this.getCount() - 1).hasFailed()))
  {    
    %obj = %this.getObject(%this.getCount() - 1);
    %obj.terminate();
    %this.remove(%obj);
    %obj.status = 0;
    
  }

  //if any subgoals remain, process the one at the front of the list
  if (%this.getCount() > 0)
  { 
    //grab the status of the front-most subgoal
    %StatusOfSubGoals = %this.getObject(%this.getCount() - 1).process();

    //we have to test for the special case where the front-most subgoal
    //reports 'completed' *and* the subgoal list contains additional goals.When
    //this is the case, to ensure the parent keeps processing its subgoal list
    //we must return the 'active' status.
    if (%StatusOfSubGoals == $gs_completed  && %this.getCount() > 0) 
    {
      return $gs_active;
    }
    return %StatusOfSubGoals;
  }
  
  //no more subgoals to process - return 'completed'
  else
  {
    return $gs_completed;
  }
}

/* ----------------------------------------------------------------
  *  Goal_Evaluator
   
   Implemented as ScriptObject
  
   [*] double       m_dCharacterBias;
   [*] virtual double CalculateDesirability(Raven_Bot* pBot)=0;
   [*] virtual void  SetGoal(Raven_Bot* pBot) = 0;
*/ 

//=============================================================================
// GoalBrain
// Usually The Brain is the same source so we put it here: 
//=============================================================================
function GoalBrain::Arbitrate(%this)
{
   %best = 0;   
   %mostDesirable = 0;
   for (%i = 0; %i < %this.evas.getCount(); %i++)
   {
      %desirabilty = %this.evas.getObject(%i).calculateDesirability();
      if (%desirabilty > %best)
      {
         %best = %desirabilty;
         %mostDesirable = %this.evas.getObject(%i);
      }
   }

   if (!isObject(%mostDesirable)) {
      // maybe we have nothing todo :P error("GoalThink assert! Cant find a evaluation for my brain!!" SPC %this.getId());

      return false;
   }
   
   %mostDesirable.setGoal();
   
   return true;
}


function GoalBrain::activate(%this) 
{
   if (!%this.Arbitrate())
      return false;

    %this.status = $gs_active;
    return true;
}

function GoalBrain::process(%this) 
{

   %this.ActivateIfInactive();
   %SubgoalStatus = %this.ProcessSubgoals();

  if ( %SubgoalStatus == $gs_completed || %SubgoalStatus == $gs_failed)
  {
      %this.status  = $gs_inactive;
  }

  return %this.status;   
}

function GoalBrain::onRemove(%this)
{
   safeDelete(%this.evas);
}


// CalculateDesirability
function EvaluatorIdle::calculateDesirability(%this)
{
   %multi = 0.01;
   return %this.charBias * %multi; 
}

function EvaluatorIdle::setGoal(%this)
{
   // idle sleeping whatever
}

