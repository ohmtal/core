/*
exec("core/ohmtal/shared/t3dmediation.cs");
*/

function alxStopAll() 
{
    for (%i = 0; %i < getWordCount($Client::soundTracks); %i++)
    {
      sfxStop(getWord($Client::soundTracks, %i));
    }

   //dError("sfxStopAll does NOT exists anymore!");
   //sfxStopAll(0);
   //sfxStopAll(1);
   //sfxStopAll(2);
   //sfxStopAll(3);
   //sfxStopAll(4);
}

function alxStop(%object)
{
  sfxStop(%object);
}


function alxplay(%trackname,%x,%y,%z)
{
   %result = sfxPlay(%trackname,%x,%y,%z);
   $Client::soundTracks =  trim($Client::soundTracks SPC %result);
   return result;
}
