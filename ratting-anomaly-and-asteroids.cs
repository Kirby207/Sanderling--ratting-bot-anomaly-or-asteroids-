/*V 1.04 This bot ratting anomaly and/or asteroids; warp to asteroids at 20km; anomaly at 50km; configurable from settings for almost everything.
+New: TIMERS !! Safe logout for smaller value of: DT, Timer session.  you go home and logout and stop bot before DT
	(see notes on testing chapter), measured when you finish a site( belts or anomaly). 
	+you can see the time span to closing the game and bot stop on stats messages and the time when you started the session ( botting) in you local time
+New: Offload LIMIT count
+New: When you dont have minim 5 drones in window drones you go home ( and bot stop) This will be measured only when you take anomaly
+New: Omnidirectional supported ( is better to fill in the complet name and remember is in tests)
+NEW delayed undock: fill in the values in seconds
- changing the tab to the one for  combat anomaly (so don't forget to fill your tab name, I've fill in an generic "combat")
- you have to fill in how many armor repairers you have , and afterburners; Hardeners and Omni you have(if you use them dont forget to set them true). 
	This function is for measure module, to be sure you have all measured ( Thx to terpla!!)
- orbit at distance (W + click) or around selected rat. Alternatively, by overview menu at max 30 km ...I let the code there.
- run from reds( on grid - but you have 99% chances to die if they are on you.)
- activate /stop Armor repairer automatically 
- activate afterburner
- take the rats in order
- Loot wrecks – all, or not, REMEMBER to set the empty wrecks to not be visibles in overview.  when is (x) % full, warp home
- When are hostiles in local, warp “home”( for now, make your window local chat biggest possible)
- The Commanders or officers are already in commanderNameWreck ( + wrecks, to take all loot)
- Set in overview neutrals background in red; ( everybody who is not your corp/ally etc); ( see for a picture on https://forum.botengine.org/t/ratting-bot-anomaly-and-or-asteroids-with-sanderling/1206/43)
!! The symbol ♦ is from drifters; they appear in hordes near stations and asteroids (also is in test, sometimes is work, sometimes not)
!! Set your own overview : tab for combat pve and also everybody else than fleet, corp, ally, militia, god standings in red( that means : bad standings, neutral etc are their background in RED)
!! do not overlap the windows :d :)) !!
!! see the guide
###################
Testing :
! Timers: To be clear about this feature I explain here the story
	1.  the script contain already set the real DT-1min of server and this value is of reference  and calcs are made for EVE time (UTC);
	2.	firstly we talk about a safe logout before downtime. 
	2.a. you want to logout safe  with 10 min before DT. If you do anomalies then you have TO TAKE IN ACCOUNT ONE LAST ANOMALY, simply because you can finish a site right at safe - 1 min and start a new anomaly. For asteroids is similar but the time is smaller
	2.b. Example: you need 45 min / anomaly but you have to warp home, dock , etc so you need 2-3 min in +. Overall, 1h 15m
	2.c; fill in the 
				minutesToDT = 15; //value in minutes before the DT of server
				hoursToDT = 1;//value in h before the DT
	3. The logic is similar for session timer: you wanna play 5h and 15m; Then you fill in :
				hoursToSession = 5;
				minutesToSession = 15;
! The smaller value between DT and session timer is taken in account ( and you can see this value in stats
	the stats show the closest in days: h : m. to be sure the maths are not messed, for now I let the days.
!!Now the sketch is like that: the values, even if are measured all time, are taken in account ONLY when you finish a site ( anomaly or not)
	BUT the script is made so you can change that and react at the time when this values are reached. ( you have to find inside the code where, or contact me)
!!!! if an red appear in local and he get out fast, the bot is comming back at killing rats.
+ attention, because the local can do some false allarms (some "friends" had some bad standings even if he is blue)
+ the window local chat must be biggest possible? if in you system are 50 persons and your window have enter for 10 persons  you are kill meat
+ added microwarpdrive 
+ owning anomaly should be reliable
+ updated code for warp home
+ simplified the code for attacking with drones
Update: now the belts are taken in order, without crash, anomalies  - updated code. It could generate an crash when you move the mouse in the same time when he click
-loot wrecks : he need to click only once on "open cargo", still looking at a solution( the one with var click stop = true / false  , not so reliable)
-The symbol ♦ from types

To do:
-distinguish ewar web/scramble etc from targetpainting ( for example)
local chat  if is scrollable 
###################
Thx to:
Viir
Terpla
pikacuq
the others from https://forum.botengine.org/ who contribued with or without their ideas /knowledges /code lines to create this bot/script.
############
GUIDE:
 fill in the values:
 -retreat from neutrals 
 -if you like to do anomalies or asteroids
 -the timers ( see on TESTING) + values in seconds to delay undock
 -name anomalies etc etc
 -your home
 -the name of your tab for ratting
 -how many afterburners/hardeners/omni/armor repairers you have
 -your home bookmark
 -container into station for unload
 -the name of your omni 
 -the value for your emergency warp out ( only for armor supported)
 -the value of your armor hp to start the repairer
 -the value of your offload cargo ( you can have a real big last wreck with more than 100m3), default value is 75% now
 -and offload count ( limit)
 -make reds background in overview ( see forum my post)
 - hide the empty wrecks ( see photo on forum, my post)
 -DO NOT CHAT !! ( better hide the windos, except local, look on the photo on forum hot to do that in a manner 70%safe
 -IF you warp/dock etc manual is better to stop the bot
 -some features could not work on older versions of sanderling!!
 !!not at the end but firstly , say thx to Viir, you have no ideea how much this man helped a noob like me to make and improve this script!!
*/

using BotSharp.ToScript.Extension;
using Parse = Sanderling.Parse;
using MemoryStruct = Sanderling.Interface.MemoryStruct;

//	begin of configuration section ->

var RetreatOnNeutralOrHostileInLocal =true;   // warp to RetreatBookmark when a neutral or hostile is visible in local.
var RattingAnomaly = true;	//	when this is set to true, you take anomaly
var RattingAsteroids = false;	//	when this is set to true, you take asteroids
// SESSION/DT TImers

var minutesToDT = 15; //value in minutes before the DT of server ( -1min)
var hoursToDT = 1;//value in h before the DT-1min

var hoursToSession = 5;
var minutesToSession = 11;
////
var MinimDelayUndock = 2;//in seconds
var MaximDelayUndock = 45;//in seconds

var LimitOffloadCount = 100;

/////settings anomaly
string AnomalyToTakeColumnHeader = "name";  // the column header from table ex : name
string AnomalyToTake = "forsaken hub"; // his name , ex:  "forsaken hub"  " combat"
string IgnoreAnomalyName = "haven|Belt|asteroid|drone|forlorn|rally|sanctum|blood hub|serpentis hub|hidden|port|den";// what anomaly to ignore
string IgnoreColumnheader = "Name";//the head  of anomaly to ignore
// you have to run from this rats:
string runFromRats = "♦|Titan|Dreadnought|Autothysian|Autothysian lancer|punisher|bestower|harbringer";// you run from him

//celestial to orbit
string celestialOrbit = "broken|pirate gate";

string CelestialToAvoid = "Chemical Factory"; // this one make difference between haven rock and gas
// wrecks commander etc
string commanderNameWreck = "Commander|Dark Blood|true|Shadow Serpentis|Dread Gurista|Domination Saint|Gurista Distributor|Sentient|Overseer|Spearhead|Dread Guristas|Estamel|Vepas|Thon|Kaikka|True Sansha|Chelm|Vizan|Selynne|Brokara|Dark Blood|Draclira|Ahremen|Raysere|Tairei|Cormack|Setele|Tuvan|Brynn|Domination|Tobias|Gotan|Hakim|Mizuro|wreck";


////////
//new!! ratting tab, fill in with your own ratting tab
string rattingTab = "combat";

int DroneNumber = 5;// set number of drones in space

int TargetCountMax = 2; //target numbers
//set  hardeners, repairer, set true if you want to run them all time, if not, there is set StartArmorRepairerHitPoints
var ActivateHardener = true;
var ActivateOmni = true;
var ActivateArmorRepairer = false;
string OmniSup = "Omnidirectional Tracking Link I";



// 	Number of ArmorRepairers and afterburners; Thx Terpla. Both armor repairers are managed in same time, if you have 2. 
const int ArmorRepairsCount = 1;
const int AfterburnersCount = 1;
// 	Number of Afterburners, Thx Terpla. Also be carefull, I cannot manage 1 afterburner and 1 MWD in same time. this function is to be sure I have measurements in measure all modules tooltip
//fill in carefull also the bot will keep measuring 

const int HardenersCount = 0; 
const int OmniCount = 0;
//	warpout emergency armor

var EmergencyWarpOutHitpointPercent = 40; // just in case, when you warp on emergency
var StartArmorRepairerHitPoints = 95; // armor value in % , when it starts armor repairer
//

bool returnDronesToBayOnRetreat = true;    // when set to true, bot will attempt to dock back the drones before retreating
//
//	Bookmark of location where ore should be unloaded.
string UnloadBookmark = "home"; //supposed your bookmark is named home

//	Name of the container to unload to as shown in inventory.
string UnloadDestContainerName = "Item Hangar"; //supposed it is Item Hangar


//	Bookmark of place to retreat to to prevent ship loss.
string RetreatBookmark = UnloadBookmark;

//register the visited locations
Queue<string> visitedLocations = new Queue<string>();

//diverses
var lockTargetKeyCode = VirtualKeyCode.LCONTROL;// lock target

var targetLockedKeyCode = VirtualKeyCode.SHIFT;//locked target

var orbitKeyCode = VirtualKeyCode.VK_W;

var attackDrones = VirtualKeyCode.VK_F;

var EnterOffloadOreHoldFillPercent = 75;	//	percentage of ore hold fill level at which to enter the offload process and warp home.

const string StatusStringFromDroneEntryTextRegexPattern = @"\((.*)\)";
static public string StatusStringFromDroneEntryText(this string droneEntryText) => droneEntryText?.RegexMatchIfSuccess(StatusStringFromDroneEntryTextRegexPattern)?.Groups[1]?.Value?.RemoveXmlTag()?.Trim();
var startSession = DateTime.Now; // alternative: DateTime.UtcNow;
var playSession = DateTime.UtcNow.AddHours(hoursToSession).AddMinutes(minutesToSession);



//	<- end of configuration section


Func<object> BotStopActivity = () => null;

Func<object> NextActivity = MainStep;

for(;;)
{

MemoryUpdate();

Host.Log(
	"Session started at: " +  startSession.ToString(" HH:mm") +// alternative (" dd/MM/yyyy HH:mm") 
	" ; armor.hp: " + ArmorHpPercent + "%" +
	" ; retreat: " +(chatLocal?.ParticipantView?.Entry?.Count(IsNeutralOrEnemy)-1)+ " # "  + RetreatReason + 
	" ; overview.rats: " + ListRatOverviewEntry?.Length +
	" ; drones in space( from total): " + DronesInSpaceCount + "(" +(DronesInSpaceCount + DronesInBayCount)+ ")"+
	" ; targeted rats :  " + Measurement?.Target?.Length+
	" ; cargo percent : " + OreHoldFillPercent + "%" +
	" ; Closer logout in(days /h/ m) : "  + TimeSpan.FromMinutes(logoutgame).ToString(@"dd\:hh\:mm")+
	" ; current offload count (max limit): " + OffloadCount + "("+ LimitOffloadCount+")" +
	" ; nextAct  : " + NextActivity?.Method?.Name);

CloseModalUIElement();

if(0 < RetreatReason?.Length && !(Measurement?.IsDocked ?? false))
{
	if (listOverviewDreadCheck?.Length > 0)
	{		Host.Log("I'm a chicken and I'm run from dread");
	if (RattingAsteroids) {	StopAfterburner(); 	ActivateArmorRepairerExecute(); InitiateWarpToMiningSite(); }
	}
	Host.Log("Tactical retreat !! Better be a living cicken than a roasted bull! ");
	Console.Beep(500, 200);
	StopAfterburner();
	ActivateArmorRepairerExecute();
	 if (Measurement?.ShipUi?.Indication?.ManeuverType == ShipManeuverTypeEnum.Orbit)
	{
	 ClickMenuEntryOnPatternMenuRoot(Measurement?.InfoPanelCurrentSystem?.ListSurroundingsButton, UnloadBookmark, "align");
	}

	if (!returnDronesToBayOnRetreat || (returnDronesToBayOnRetreat && 0 == DronesInSpaceCount))
	{
	Host.Log("Picard : Yes, I warping home( I know ... I know ... Is an Miracle!!) ");
	 ClickMenuEntryOnPatternMenuRoot(Measurement?.InfoPanelCurrentSystem?.ListSurroundingsButton, UnloadBookmark, "dock");
	}
	else 
		{ DroneEnsureInBay();}
	continue;
}
if(Measurement?.WindowOther != null) CloseWindowOther();
if(Measurement?.WindowTelecom != null) CloseWindowTelecom();
NextActivity = NextActivity?.Invoke() as Func<object>;

if(BotStopActivity == NextActivity)
	break;	
if(null == NextActivity)
	NextActivity = MainStep;
Host.Delay(1111);
}

int? ShieldHpPercent => ShipUi?.HitpointsAndEnergy?.Shield / 10;
int? ArmorHpPercent => ShipUi?.HitpointsAndEnergy?.Armor / 10;

bool DefenseExit =>
    (Measurement?.IsDocked ?? false) ||
    !(0 < ListRatOverviewEntry?.Length);

bool DefenseEnter =>
    !DefenseExit;

string RetreatReasonTemporary = null;
string RetreatReasonPermanent = null;
string RetreatReason => RetreatReasonPermanent ?? RetreatReasonTemporary;
int? LastCheckOreHoldFillPercent = null;

int OffloadCount = 0;
bool OreHoldFilledForOffload => Math.Max(0, Math.Min(100, EnterOffloadOreHoldFillPercent)) <= OreHoldFillPercent;

Func<object> MainStep()
{
	
   // Host.Log("enter mainstep"); // used for debug
    if (Measurement?.IsDocked ?? false)
    {		InInventoryUnloadItems();

        if (0 < RetreatReasonPermanent?.Length || OffloadCount > LimitOffloadCount
		||   logoutme==true )
        { Host.Log("permanent retreat = bot stop");
        	Sanderling.KeyboardPressCombined(new[]{ VirtualKeyCode.LMENU, VirtualKeyCode.SHIFT, VirtualKeyCode.VK_Q});
		Host.Delay(1111);
			if (reasonDrones == true) 
			{ Host.Log(" Until you refill your drones = bot stop");
				return BotStopActivity;
			}
		return BotStopActivity; 
		}

        if (0 < RetreatReason?.Length)
			{ Host.Log("Hostiles on local, but I'm safe into Station ... taking a nap");
			Host.Delay(4111);
			return MainStep;
			}
		Random rnd = new Random();
		int DelayTime = rnd.Next(MinimDelayUndock, MaximDelayUndock);
			Host.Log("Keep your horses for :  " + DelayTime+ " s ");
			Host.Delay( DelayTime*1000);
        Undock();
    }

    if (ReadyForManeuver)
    {
        DroneEnsureInBay(); 

        Host.Log("Refreshing news: I'm ready for rats");
        if (0 == DronesInSpaceCount && 0 == ListRatOverviewEntry?.Length)
        {
           // Host.Log("drones in space 0 going to asteroids or anomaly"); //used for debug

            if (ReadyForManeuver)
            {
                if ((!OreHoldFilledForOffload) && (0 == ListRatOverviewEntry?.Length) && (listOverviewCommanderWreck?.Length > 0) && (ListCelestialObjects?.Length > 0))
                    return InBeltMineStep;
                if (OreHoldFilledForOffload)
                {
                    ClickMenuEntryOnPatternMenuRoot(Measurement?.InfoPanelCurrentSystem?.ListSurroundingsButton, UnloadBookmark, "dock");
                    return MainStep;
                }

				if ((!OreHoldFilledForOffload) && (0 == ListRatOverviewEntry?.Length)
					&& (listOverviewCommanderWreck?.Length > 0)
					&& (ListCelestialObjects?.Length > 0))
				return InBeltMineStep;

                if (RattingAnomaly)
                {
						Host.Log("I would like to spin around rocks");
                    return TakeAnomaly;
                }
                if (RattingAsteroids)
                {
						Host.Log("Maybe some Asteroids bring some cool rats? :)");
                    InitiateWarpToMiningSite();
                    return MainStep;
                }
            }
        }

    }

	ModuleMeasureAllTooltip();

	if (ActivateHardener)
		ActivateHardenerExecute();
	if (ActivateOmni)	
		ActivateOmniExecute();
	return InBeltMineStep;
}

void CloseModalUIElement()
{
    var ButtonClose =
        ModalUIElement?.ButtonText?.FirstOrDefault(button => (button?.Text).RegexMatchSuccessIgnoreCase("close|no|ok"));
    Sanderling.MouseClickLeft(ButtonClose);
}
void CloseWindowTelecom()
{
    var WindowTelecom = Measurement?.WindowTelecom?.FirstOrDefault(w => (w?.Caption.RegexMatchSuccessIgnoreCase("Information") ?? false));
    var CloseButton = WindowTelecom?.ButtonText?.FirstOrDefault(text => text.Text.RegexMatchSuccessIgnoreCase("Close"));
    if (CloseButton != null)
        Sanderling.MouseClickLeft(CloseButton);
}
public void CloseWindowOther()//thx Terpla
{
    var windowOther = Sanderling?.MemoryMeasurementParsed?.Value?.WindowOther?.FirstOrDefault();

    //	if close button not visible then move mouse to the our window
    if (!windowOther?.HeaderButtonsVisible ?? false)
        Sanderling.MouseMove(windowOther.LabelText.FirstOrDefault());

    Sanderling.InvalidateMeasurement(); //	make sure we have new measurement
    if (windowOther?.HeaderButton != null)
    {
        //	we have 3 buttons and looking with HintText "Close"
        var closeButton = windowOther.HeaderButton?.FirstOrDefault(x => x.HintText == "Close");
        if (closeButton != null)
            Sanderling.MouseClickLeft(closeButton);
    }
}
void DroneLaunch()
{
    Host.Log("Launching my Vipers");
    Sanderling.MouseClickRight(DronesInBayListEntry);
    Sanderling.MouseClickLeft(Menu?.FirstOrDefault()?.EntryFirstMatchingRegexPattern("launch", RegexOptions.IgnoreCase));
}

void DroneEnsureInBay()
{
    if (0 == DronesInSpaceCount)
        return;
    DroneReturnToBay();
    Host.Delay(4444);
}

void DroneReturnToBay()
{
    Host.Log("Inconceivable to forget my Vipers here");
    //Sanderling.MouseClickRight(DronesInSpaceListEntry);
    //Sanderling.MouseClickLeft(Menu?.FirstOrDefault()?.EntryFirstMatchingRegexPattern("return.*bay", RegexOptions.IgnoreCase));
     Sanderling.KeyboardPressCombined(new[]{ targetLockedKeyCode, VirtualKeyCode.VK_R });//if you like 
}


Func<object> DefenseStep()
{

    var NPCtargheted = Measurement?.Target?.Length;
    var shouldAttackTarget = ListRatOverviewEntry?.Any(entry => entry?.MeActiveTarget ?? false) ?? false;
    var targetSelected = Measurement?.Target?.FirstOrDefault(target => target?.IsSelected ?? false);

    var Broken = ListCelestialObjects?.FirstOrDefault();

    var droneListView = Measurement?.WindowDroneView?.FirstOrDefault()?.ListView;

    var droneGroupWithNameMatchingPattern = new Func<string, DroneViewEntryGroup>(namePattern =>
        droneListView?.Entry?.OfType<DroneViewEntryGroup>()?.FirstOrDefault(group => group?.LabelTextLargest()?.Text?.RegexMatchSuccessIgnoreCase(namePattern) ?? false));

    var overviewEntryLockTarget =
        ListRatOverviewEntry?.FirstOrDefault(entry => !((entry?.MeTargeted ?? false) || (entry?.MeTargeting ?? false)));

    var droneGroupInLocalSpace = droneGroupWithNameMatchingPattern("local space");

    var setDroneInLocalSpace = droneListView?.Entry?.OfType<DroneViewEntryItem>()
        ?.Where(drone => droneGroupInLocalSpace?.RegionCenter()?.B < drone?.RegionCenter()?.B)
        ?.ToArray();
    var droneInLocalSpaceSetStatus =
        setDroneInLocalSpace?.Select(drone => drone?.LabelText?.Select(label => label?.Text?.StatusStringFromDroneEntryText()))?.ConcatNullable()?.WhereNotDefault()?.Distinct()?.ToArray();

    var droneInLocalSpaceIdle =
        droneInLocalSpaceSetStatus?.Any(droneStatus => droneStatus.RegexMatchSuccessIgnoreCase("idle")) ?? false;

    var droneGroupInBay = droneGroupWithNameMatchingPattern("bay");

    //Host.Log("enter defense from defense step.");

    if (ActivateArmorRepairer == true || ArmorHpPercent < StartArmorRepairerHitPoints)
    {
        Host.Log("Armor integrity < "  + StartArmorRepairerHitPoints + "%");
        ActivateArmorRepairerExecute();
    }

    if (ArmorHpPercent > StartArmorRepairerHitPoints && ActivateArmorRepairer == false)
    { StopArmorRepairer(); }

    if (DefenseExit)
    { 
	//	Host.Log("exit defense."); // used for debug
	return null; 
	}
    if (Measurement?.ShipUi?.Indication?.ManeuverType != ShipManeuverTypeEnum.Orbit)
		return InBeltMineStep;
	
    if (null == targetSelected)
        LockTarget();

    if (0 < DronesInBayCount && DronesInSpaceCount < DroneNumber)
        DroneLaunch();

    if (!(0 < DronesInSpaceCount))
        DroneLaunch();

    if (null != targetSelected)
    {
        if (shouldAttackTarget)
        {
		if (Measurement?.Target?.FirstOrDefault(target => target?.IsSelected ?? false).DistanceMax < WeaponRange)
            ActivateWeaponExecute();
		if (droneInLocalSpaceIdle && (Measurement?.Target?.Length > 0))
			{
				//Host.Log("My drones are lazy now ...");//used for debug
				Sanderling.KeyboardPress(attackDrones);
				Host.Log("Vipers message: Sir! Yes Sir! We engage the target");
			}
        }
        else
            UnlockTarget();
    }
    if (Measurement?.Target?.Length < TargetCountMax && 1 < ListRatOverviewEntry?.Count())
        LockTarget();

    if (EWarToAttack?.Count() > 0)//thx pikacuq
    {
        var EWarSelected = EWarToAttack?.FirstOrDefault(target => target?.IsSelected ?? false);
        var EWarLocked = EWarToAttack?.FirstOrDefault(target => target?.MeTargeted ?? false);

        if (EWarLocked == null)
        {
            Sanderling.KeyDown(lockTargetKeyCode);
            Sanderling.MouseClickLeft(EWarToAttack?.FirstOrDefault(entry => !((entry?.MeTargeted ?? false))));
            Sanderling.KeyUp(lockTargetKeyCode);
        }
        else if (EWarSelected == null)
        { Sanderling.MouseClickLeft(EWarToAttack?.FirstOrDefault()); }

        else
        {
            //Host.Log("drones change to ewar target"); // it was for debug
            Sanderling.KeyboardPress(attackDrones);
            Host.Log("Some nasty rats, engaging them ");
        }
    }
    if (0 == ListRatOverviewEntry?.Count())
    {
	StopAfterburner();
	DroneEnsureInBay();
    }
    return DefenseStep;
}


var SiteFinished =false;
Func<object> InBeltMineStep()
{
var LootButton = Measurement?.WindowInventory?[0]?.ButtonText?.FirstOrDefault(text => text.Text.RegexMatchSuccessIgnoreCase("Loot All"));
    if (RattingAnomaly && (0 < listOverviewEntryFriends?.Length || ListCelestialToAvoid?.Length>0 ) && 0 < ListRatOverviewEntry?.Length && ReadyForManeuver )
	{       if (Measurement?.ShipUi?.Indication?.ManeuverType != ShipManeuverTypeEnum.Orbit)
   	    {
		Host.Log("Presence of friends on site! I'm doing you a favor and ignore this anomaly and a second favor to take another one(anomaly)");
		ActivateArmorRepairerExecute();//to be sure I stay alive, rats can target me
        return TakeAnomaly;
		}
	}
    if ((ReadyForManeuver) && (Measurement?.ShipUi?.Indication?.ManeuverType != ShipManeuverTypeEnum.Orbit) && (0 < ListRatOverviewEntry?.Length))
    {
        Orbitkeyboard();

        if (DefenseEnter)
        {
            //Host.Log("enter defense.");
            return DefenseStep;
        }
    }

    EnsureWindowInventoryOpen();
    if ((!OreHoldFilledForOffload) && 0 == ListRatOverviewEntry?.Length && 0 < listOverviewCommanderWreck?.Length)
	{
		if(!(listOverviewCommanderWreck?.FirstOrDefault()?.DistanceMax > 10000))
        StopAfterburner();
		else 		
		ActivateAfterburnerExecute();
        if (LootButton != null)
            Sanderling.MouseClickLeft(LootButton);
        if ((listOverviewCommanderWreck?.FirstOrDefault()?.DistanceMax > 100) )
            ClickMenuEntryOnMenuRoot(listOverviewCommanderWreck?.FirstOrDefault(), "open cargo");
	}
    else if (( OreHoldFilledForOffload || 0 < listOverviewCommanderWreck?.Length ) && 0 == ListRatOverviewEntry?.Length)
 	{
        Host.Log("Im coolest! Site finished! "); 
		SiteFinished =true;	
        return MainStep;
	}
    return InBeltMineStep;
}


Sanderling.Parse.IMemoryMeasurement Measurement =>
    Sanderling?.MemoryMeasurementParsed?.Value;

IWindow ModalUIElement =>
    Measurement?.EnumerateReferencedUIElementTransitive()?.OfType<IWindow>()?.Where(window => window?.isModal ?? false)
    ?.OrderByDescending(window => window?.InTreeIndex ?? int.MinValue)
    ?.FirstOrDefault();

IEnumerable<Parse.IMenu> Menu => Measurement?.Menu;

Parse.IShipUi ShipUi => Measurement?.ShipUi;

Sanderling.Interface.MemoryStruct.IMenuEntry MenuEntryLockTarget =>
    Menu?.FirstOrDefault()?.Entry?.FirstOrDefault(entry => entry.Text.RegexMatchSuccessIgnoreCase("^lock"));

Sanderling.Interface.MemoryStruct.IMenuEntry MenuEntryUnLockTarget =>
    Menu?.FirstOrDefault()?.Entry?.FirstOrDefault(entry => entry.Text.RegexMatchSuccessIgnoreCase("^unlock"));

Sanderling.Parse.IWindowOverview WindowOverview =>
    Measurement?.WindowOverview?.FirstOrDefault();

Sanderling.Parse.IWindowInventory WindowInventory =>
    Measurement?.WindowInventory?.FirstOrDefault();

IWindowDroneView WindowDrones =>
    Measurement?.WindowDroneView?.FirstOrDefault();
	

Tab OverviewTabActive =>
	Measurement?.WindowOverview?.FirstOrDefault()?.PresetTab
	?.OrderByDescending(tab => tab?.LabelColorOpacityMilli ?? 1500)
	?.FirstOrDefault();
Tab combatTab => WindowOverview?.PresetTab
	?.OrderByDescending(tab => tab?.Label.Text.RegexMatchSuccessIgnoreCase(rattingTab))
	?.FirstOrDefault();
	
	
	
var inventoryActiveShip = WindowInventory?.ActiveShipEntry;
var inventoryActiveShipEntry = WindowInventory?.ActiveShipEntry;
var ShipHasHold = inventoryActiveShipEntry?.TreeEntryFromCargoSpaceType(ShipCargoSpaceTypeEnum.General) != null;
var hasHold = ShipHasHold;

ITreeViewEntry InventoryActiveShipContainer
{
    get
    {
        var hasHold = ShipHasHold;
        return
        WindowInventory?.ActiveShipEntry?.TreeEntryFromCargoSpaceType( hasHold ? ShipCargoSpaceTypeEnum.OreHold : ShipCargoSpaceTypeEnum.General);
    }
}
IInventoryCapacityGauge OreHoldCapacityMilli =>
    (InventoryActiveShipContainer?.IsSelected ?? false) ? WindowInventory?.SelectedRightInventoryCapacityMilli : null;

int? OreHoldFillPercent => (int?)((OreHoldCapacityMilli?.Used * 100) / OreHoldCapacityMilli?.Max);
Sanderling.Accumulation.IShipUiModule[] SetModuleWeapon =>
	Sanderling.MemoryMeasurementAccu?.Value?.ShipUiModule?.Where(module => module?.TooltipLast?.Value?.IsWeapon ?? false)?.ToArray();

int?		WeaponRange => SetModuleWeapon?.Select(module =>
	module?.TooltipLast?.Value?.RangeOptimal ?? module?.TooltipLast?.Value?.RangeMax ?? module?.TooltipLast?.Value?.RangeWithin ?? 0)?.DefaultIfEmpty(0)?.Min();
	
string OverviewTypeSelectionName =>
    WindowOverview?.Caption?.RegexMatchIfSuccess(@"\(([^\)]*)\)")?.Groups?[1]?.Value;

Parse.IOverviewEntry[] ListRatOverviewEntry => WindowOverview?.ListView?.Entry?.Where(entry =>
    (entry?.MainIconIsRed ?? false))
    ?.OrderBy(entry => entry?.Name?.RegexMatchSuccessIgnoreCase(@"battery|tower|sentry|web|strain|splinter|render|raider|friar|reaver")) //Frigate
    ?.OrderBy(entry => entry?.Name?.RegexMatchSuccessIgnoreCase(@"coreli|centi|alvi|pithi|corpii|gistii|cleric|engraver")) //Frigate
    ?.OrderBy(entry => entry?.Name?.RegexMatchSuccessIgnoreCase(@"corelior|centior|alvior|pithior|corpior|gistior")) //Destroyer
    ?.OrderBy(entry => entry?.Name?.RegexMatchSuccessIgnoreCase(@"corelum|centum|alvum|pithum|corpum|gistum|prophet")) //Cruiser
    ?.OrderBy(entry => entry?.Name?.RegexMatchSuccessIgnoreCase(@"corelatis|centatis|alvatis|pithatis|copatis|gistatis|apostle")) //Battlecruiser
    ?.OrderBy(entry => entry?.Name?.RegexMatchSuccessIgnoreCase(@"core\s|centus|alvus|pith\s|corpus|gist\s")) //Battleship
    ?.ThenBy(entry => entry?.DistanceMax ?? int.MaxValue)
    ?.ToArray();
Parse.IOverviewEntry[] ListCelestialObjects => WindowOverview?.ListView?.Entry
    ?.Where(entry => entry?.Name?.RegexMatchSuccessIgnoreCase(celestialOrbit) ?? false)
    ?.OrderBy(entry => entry?.DistanceMax ?? int.MaxValue)
    ?.ToArray();
	
Parse.IOverviewEntry[] ListCelestialToAvoid => WindowOverview?.ListView?.Entry
    ?.Where(entry => entry?.Name?.RegexMatchSuccessIgnoreCase(CelestialToAvoid ) ?? false)
    ?.OrderBy(entry => entry?.DistanceMax ?? int.MaxValue)
    ?.ToArray();

Parse.IOverviewEntry[] listOverviewDreadCheck => WindowOverview?.ListView?.Entry
    ?.Where(entry => (entry?.Name?.RegexMatchSuccess(runFromRats) ?? true) || (entry?.Type?.RegexMatchSuccess(runFromRats) ?? true))
    .ToArray();

Parse.IOverviewEntry[] listOverviewEntryFriends =>
    WindowOverview?.ListView?.Entry
    ?.Where(entry => entry?.ListBackgroundColor?.Any(IsFriendBackgroundColor) ?? false)
    ?.ToArray();

Parse.IOverviewEntry[] listOverviewEntryEnemy =>
    WindowOverview?.ListView?.Entry
    ?.Where(entry => entry?.ListBackgroundColor?.Any(IsEnemyBackgroundColor) ?? false)
    ?.ToArray();
// this is for ewar - not used for the momment
EWarTypeEnum[] listEWarPriorityGroupTeamplate =
{
    EWarTypeEnum.WarpDisrupt, EWarTypeEnum.WarpScramble, EWarTypeEnum.ECM, EWarTypeEnum.Web, EWarTypeEnum.EnergyNeut, EWarTypeEnum.EnergyVampire, EWarTypeEnum.TrackingDisrupt
};

Parse.IOverviewEntry[] EWarToAttack =>
    WindowOverview?.ListView?.Entry
	?.Where(entry => entry != null && (!entry?.EWarType?.IsNullOrEmpty() ?? false) && (entry?.EWarType).Any())
	?.ToArray(); 
	// tests: with listEWarPriorityGroupTeamplate !=null will ignore any ewar, without = argument null exception at first argument intersect  ( the list of enums) 
 /*   ?.Where(entry => (!(entry?.EWarType?.IsNullOrEmpty() ?? false)) && listEWarPriorityGroupTeamplate !=null && listEWarPriorityGroupTeamplate.Intersect(entry.EWarType).Any())
    ?.ToArray();*/

	
Parse.IOverviewEntry[] listOverviewCommanderWreck =>
    WindowOverview?.ListView?.Entry
    ?.Where(entry => entry?.Name?.RegexMatchSuccessIgnoreCase(commanderNameWreck) ?? true)
    ?.OrderBy(entry => entry?.DistanceMax ?? int.MaxValue)
    .ToArray();

DroneViewEntryGroup DronesInBayListEntry =>
    WindowDrones?.ListView?.Entry?.OfType<DroneViewEntryGroup>()?.FirstOrDefault(Entry => null != Entry?.Caption?.Text?.RegexMatchIfSuccess(@"Drones in bay", RegexOptions.IgnoreCase));

DroneViewEntryGroup DronesInSpaceListEntry =>
    WindowDrones?.ListView?.Entry?.OfType<DroneViewEntryGroup>()?.FirstOrDefault(Entry => null != Entry?.Caption?.Text?.RegexMatchIfSuccess(@"Drones in Local Space", RegexOptions.IgnoreCase));

int? DronesInSpaceCount => DronesInSpaceListEntry?.Caption?.Text?.AsDroneLabel()?.Status?.TryParseInt();
int? DronesInBayCount => DronesInBayListEntry?.Caption?.Text?.AsDroneLabel()?.Status?.TryParseInt();


public bool ReadyForManeuverNot =>
    Measurement?.ShipUi?.Indication?.LabelText?.Any(indicationLabel =>
        (indicationLabel?.Text).RegexMatchSuccessIgnoreCase("warp|docking")) ?? false;

public bool ReadyForManeuver => !ReadyForManeuverNot && !(Measurement?.IsDocked ?? true);

Sanderling.Interface.MemoryStruct.IListEntry WindowInventoryItem =>
    WindowInventory?.SelectedRightInventory?.ListView?.Entry?.FirstOrDefault();


WindowChatChannel chatLocal =>
     Sanderling.MemoryMeasurementParsed?.Value?.WindowChatChannel
     ?.FirstOrDefault(windowChat => windowChat?.Caption?.RegexMatchSuccessIgnoreCase("local") ?? false);
//    assuming that own character is always visible in local
public bool hostileOrNeutralsInLocal => 1 < chatLocal?.ParticipantView?.Entry?.Count(IsNeutralOrEnemy);

void ClickMenuEntryOnMenuRoot(IUIElement MenuRoot, string MenuEntryRegexPattern)
{
    Sanderling.MouseClickRight(MenuRoot);

    var Menu = Measurement?.Menu?.FirstOrDefault();

    var MenuEntry = Menu?.EntryFirstMatchingRegexPattern(MenuEntryRegexPattern, RegexOptions.IgnoreCase);

    Sanderling.MouseClickLeft(MenuEntry);
}

void EnsureWindowInventoryOpen()
{
    if (null != WindowInventory)
        return;
   // Host.Log("open Inventory.");
    Sanderling.MouseClickLeft(Measurement?.Neocom?.InventoryButton);
    Host.Delay(1111);
}
void EnsureWindowInventoryOpenActiveShip()
{
    EnsureWindowInventoryOpen();

    var inventoryActiveShip = WindowInventory?.ActiveShipEntry;

    if (!(inventoryActiveShip?.IsSelected ?? false))
        Sanderling.MouseClickLeft(inventoryActiveShip);
}


//	sample label text: Intensive Reprocessing Array <color=#66FFFFFF>1,123 m</color //
string InventoryContainerLabelRegexPatternFromContainerName(string containerName) =>
    @"^\s*" + Regex.Escape(containerName) + @"\s*($|\<)";

void InInventoryUnloadItems() => InInventoryUnloadItemsTo(UnloadDestContainerName);

void InInventoryUnloadItemsTo(string DestinationContainerName)
{
    Host.Log("unload items to '" + DestinationContainerName + "'.");

    EnsureWindowInventoryOpenActiveShip();

    for (; ; )
    {
        var oreHoldListItem = WindowInventory?.SelectedRightInventory?.ListView?.Entry?.ToArray();

        var oreHoldItem = oreHoldListItem?.FirstOrDefault();

        if (null == oreHoldItem)
            break;    //    0 items in Cargo

        if (1 < oreHoldListItem?.Length)
            ClickMenuEntryOnMenuRoot(oreHoldItem, @"select\s*all");

        var DestinationContainerLabelRegexPattern =
            InventoryContainerLabelRegexPatternFromContainerName(DestinationContainerName);

        var DestinationContainer =
            WindowInventory?.LeftTreeListEntry?.SelectMany(entry => new[] { entry }.Concat(entry.EnumerateChildNodeTransitive()))
            ?.FirstOrDefault(entry => entry?.Text?.RegexMatchSuccessIgnoreCase(DestinationContainerLabelRegexPattern) ?? false);

        if (null == DestinationContainer)
            Host.Log("Houston, we have a problem: '" + DestinationContainerName + "' not found");

        Sanderling.MouseDragAndDrop(oreHoldItem, DestinationContainer);
    }
}

bool InitiateWarpToMiningSite()	=>
	InitiateDockToOrWarpToLocationInSolarSystemMenu("asteroid belts", PickNextMiningSiteFromSystemMenu);

MemoryStruct.IMenuEntry PickNextMiningSiteFromSystemMenu(IReadOnlyList<MemoryStruct.IMenuEntry> availableMenuEntries)
{
	Host.Log("Hubble says he saw  " + availableMenuEntries?.Count.ToString() + " mining sites to choose from.");

	var nextSite =
		availableMenuEntries
		?.OrderBy(menuEntry => visitedLocations.ToList().IndexOf(menuEntry?.Text))
		?.FirstOrDefault();

	Host.Log("I pick in order '" + nextSite?.Text + "' as next mining site. You like it or not, c'est la vie :p");
	return nextSite;
}

bool InitiateDockToOrWarpToLocationInSolarSystemMenu(
	string submenuLabel,
	Func<IReadOnlyList<MemoryStruct.IMenuEntry>, MemoryStruct.IMenuEntry> pickPreferredDestination = null)
{
	Host.Log("Preparing engines for '" + submenuLabel + "'");
	
	var listSurroundingsButton = Measurement?.InfoPanelCurrentSystem?.ListSurroundingsButton;
	
	Sanderling.MouseClickRight(listSurroundingsButton);

	var submenuEntry = Measurement?.Menu?.FirstOrDefault()?.EntryFirstMatchingRegexPattern("^" + submenuLabel + "$", RegexOptions.IgnoreCase);

	if(null == submenuEntry)
	{
		Host.Log("Failure on telemetry systems: Submenu '" + submenuLabel + "' not found.");
		return true;
	}

	Sanderling.MouseClickLeft(submenuEntry);

	var submenu = Measurement?.Menu?.ElementAtOrDefault(1);

	var destinationMenuEntry = pickPreferredDestination?.Invoke(submenu?.Entry?.ToList()) ?? submenu?.Entry?.FirstOrDefault();

	if(destinationMenuEntry == null)
	{
		Host.Log("My fingers failed to open submenu '" + submenuLabel + "' in the solar system menu.");
		return true;
	}

	Sanderling.MouseClickLeft(destinationMenuEntry);

	var actionsMenu = Measurement?.Menu?.ElementAtOrDefault(2);

	if(destinationMenuEntry == null)
	{
		Host.Log("I'm drunk? Failed to open actions menu for '" + destinationMenuEntry.Text + "' in the solar system menu.");
		return true;
	}
	var menuResultaction = actionsMenu?.Entry.ToArray();
	var menuResultSelectWarpMenu= menuResultaction?[1];
	var maneuverMenuEntry = menuResultSelectWarpMenu;

	if (maneuverMenuEntry?.Text != "Warp to Within")
	{
	// Host.Log("not a good menu");//used for debug
	return true;
	}
	if (maneuverMenuEntry?.Text == "Warp to Within")
	{
		Host.Log("Prepare your engines for '" + maneuverMenuEntry.Text + "' on '" + destinationMenuEntry?.Text + "'");
		
		Sanderling.MouseClickRight(maneuverMenuEntry);
		
		var menuResultats = Measurement?.Menu?.ElementAtOrDefault(3);
		var menuResultWarpDestination = menuResultats?.Entry.ToArray();
		if (menuResultWarpDestination[0].Text !=  "Within 0 m")
		{
		Host.Log("Failed to open the kinder egg '" + destinationMenuEntry.Text + "' in the solar system menu.");
		return true;
		}
		else
		{
		Host.Log("'Engines' to Picard:initiating  warp on '" + destinationMenuEntry?.Text + "'");
		
		ClickMenuEntryOnMenuRoot(menuResultWarpDestination[0], "within 0 m");
   		Host.Delay(8000);
		return false;
		}		
	}

	Host.Log("no suitable menu entry found on '" + destinationMenuEntry?.Text + "'");
	return true;
}
void LockTarget()
{
    Sanderling.KeyDown(lockTargetKeyCode);
    Sanderling.MouseClickLeft(ListRatOverviewEntry?.FirstOrDefault(entry => !((entry?.MeTargeted ?? false) || (entry?.MeTargeting ?? false))));
    Sanderling.KeyUp(lockTargetKeyCode);
}
void UnlockTarget()
{
    var targetSelected = Measurement?.Target?.FirstOrDefault(target => target?.IsSelected ?? false);
    Sanderling.MouseClickRight(targetSelected);
    Sanderling.MouseClickLeft(MenuEntryUnLockTarget);
    Host.Log("sorry, this is not a target");
}
void Undock()
{
    while (Measurement?.IsDocked ?? true)
    {
        Sanderling.MouseClickLeft(Measurement?.WindowStation?.FirstOrDefault()?.UndockButton);
        Host.Log("waiting for undocking to complete.");
        Host.Delay(8000);
    }
    Host.Delay(4444);
    Sanderling.InvalidateMeasurement();
}

void ModuleMeasureAllTooltip()
{
	Host.Log("Your modules have to be somewhere");
	
		var armorRapairCount = Sanderling.MemoryMeasurementAccu?.Value?.ShipUiModule.Count(m => m?.TooltipLast?.Value?.IsArmorRepairer ?? false);
		var afterburnersCount = Sanderling.MemoryMeasurementAccu?.Value?.ShipUiModule.Count((module => (module?.TooltipLast?.Value?.IsAfterburner ?? false) || (module?.TooltipLast?.Value?.IsMicroWarpDrive?? false)));
		var hardenersCount = Sanderling.MemoryMeasurementAccu?.Value?.ShipUiModule.Count(m => m?.TooltipLast?.Value?.IsHardener ?? false);
		var omniCount  = Sanderling.MemoryMeasurementAccu?.Value?.ShipUiModule.Count(module => module?.TooltipLast?.Value?.LabelText?.Any(
					label => label?.Text?.RegexMatchSuccess(OmniSup, System.Text.RegularExpressions.RegexOptions.IgnoreCase) ?? false) ?? false);
	while( (armorRapairCount < ArmorRepairsCount) || (afterburnersCount <  AfterburnersCount)
			|| (hardenersCount <  HardenersCount)|| (omniCount <  OmniCount)	)
	{
		if(Sanderling.MemoryMeasurementParsed?.Value?.IsDocked ?? false)
			break;
		foreach(var NextModule in Sanderling.MemoryMeasurementAccu?.Value?.ShipUiModule)
		{
			if(null == NextModule)
				break;
	
			Host.Log("This circuit is all right? let's measure this module");
			//	take multiple measurements of module tooltip to reduce risk to keep bad read tooltip.
			Sanderling.MouseMove(NextModule);
			Sanderling.WaitForMeasurement();
			Sanderling.MouseMove(NextModule);
		}		
	omniCount  = Sanderling.MemoryMeasurementAccu?.Value?.ShipUiModule.Count(module => module?.TooltipLast?.Value?.LabelText?.Any(
					label => label?.Text?.RegexMatchSuccess(OmniSup, System.Text.RegularExpressions.RegexOptions.IgnoreCase) ?? false) ?? false);
		hardenersCount = Sanderling.MemoryMeasurementAccu?.Value?.ShipUiModule.Count(m => m?.TooltipLast?.Value?.IsHardener ?? false);
		armorRapairCount = Sanderling.MemoryMeasurementAccu?.Value?.ShipUiModule.Count(m => m?.TooltipLast?.Value?.IsArmorRepairer ?? false);
		afterburnersCount = Sanderling.MemoryMeasurementAccu?.Value?.ShipUiModule.Count((module => (module?.TooltipLast?.Value?.IsAfterburner ?? false) || (module?.TooltipLast?.Value?.IsMicroWarpDrive?? false)));
		Host.Log(  " Armor Repair count = " + armorRapairCount + "; Afterburners count = " + afterburnersCount + " ;     Hardeners count = " + hardenersCount + " ;  Omni count = " + omniCount + " " );

	}
}

void ActivateHardenerExecute()
{
    var SubsetModuleHardener =
        Sanderling.MemoryMeasurementAccu?.Value?.ShipUiModule
        ?.Where(module => module?.TooltipLast?.Value?.IsHardener ?? false);
 
    var SubsetModuleToToggle =
        SubsetModuleHardener
        ?.Where(module => !(module?.RampActive ?? false));

    foreach (var Module in SubsetModuleToToggle.EmptyIfNull())
        ModuleToggle(Module);
}
void ActivateArmorRepairerExecute()
{
    var SubsetModuleArmorRepairer =
        Sanderling.MemoryMeasurementAccu?.Value?.ShipUiModule
        ?.Where(module => module?.TooltipLast?.Value?.IsArmorRepairer ?? false);

    var SubsetModuleToToggle =
        SubsetModuleArmorRepairer
        ?.Where(module => !(module?.RampActive ?? false));

    foreach (var Module in SubsetModuleToToggle.EmptyIfNull())
        ModuleToggle(Module);
}
void StopArmorRepairer()
{
    var SubsetModuleArmorRepairer =
        Sanderling.MemoryMeasurementAccu?.Value?.ShipUiModule
        ?.Where(module => module?.TooltipLast?.Value?.IsArmorRepairer ?? false);

    var SubsetModuleToToggle =
        SubsetModuleArmorRepairer
        ?.Where(module => (module?.RampActive ?? false));

    foreach (var Module in SubsetModuleToToggle.EmptyIfNull())
        ModuleToggle(Module);
}
void ActivateWeaponExecute()
{
    var SubsetModuleWeapon =
        Sanderling.MemoryMeasurementAccu?.Value?.ShipUiModule
        ?.Where(module => module?.TooltipLast?.Value?.IsWeapon ?? false);

    var SubsetModuleToToggle =
        SubsetModuleWeapon
        ?.Where(module => !(module?.RampActive ?? false));

    foreach (var Module in SubsetModuleToToggle.EmptyIfNull())
        ModuleToggle(Module);
}
void ActivateAfterburnerExecute()
{
    var SubsetModuleAfterburner =
        Sanderling.MemoryMeasurementAccu?.Value?.ShipUiModule
        ?.Where(module => (module?.TooltipLast?.Value?.IsAfterburner ?? false) || (module?.TooltipLast?.Value?.IsMicroWarpDrive?? false));

    var SubsetModuleToToggle =
        SubsetModuleAfterburner
        ?.Where(module => !(module?.RampActive ?? false));

    foreach (var Module in SubsetModuleToToggle.EmptyIfNull())
        ModuleToggle(Module);
}
void StopAfterburner()
{

    var SubsetModuleAfterburner =
        Sanderling.MemoryMeasurementAccu?.Value?.ShipUiModule
        ?.Where(module => (module?.TooltipLast?.Value?.IsAfterburner ?? false) || (module?.TooltipLast?.Value?.IsMicroWarpDrive?? false));

    var SubsetModuleToToggle =
        SubsetModuleAfterburner
        ?.Where(module => (module?.RampActive ?? false));

    foreach (var Module in SubsetModuleToToggle.EmptyIfNull())
    { ModuleToggle(Module); //Host.Log("stop afterburner "); //used for debug
	}
}
void ActivateOmniExecute()
{
    var SubsetModuleOmni =
		Sanderling.MemoryMeasurementAccu?.Value?.ShipUiModule?.Where(module => module?.TooltipLast?.Value?.LabelText?.Any(
		label => label?.Text?.RegexMatchSuccess(OmniSup, System.Text.RegularExpressions.RegexOptions.IgnoreCase) ?? false) ?? false);

    var SubsetModuleToToggle =
        SubsetModuleOmni
        ?.Where(module => !(module?.RampActive ?? false));

    foreach (var Module in SubsetModuleToToggle.EmptyIfNull())
        ModuleToggle(Module);
}
void ModuleToggle(Sanderling.Accumulation.IShipUiModule Module)
{
    var ToggleKey = Module?.TooltipLast?.Value?.ToggleKey;

    Host.Log("toggle module using " + (null == ToggleKey ? "mouse" : Module?.TooltipLast?.Value?.ToggleKeyTextLabel?.Text));

    if (null == ToggleKey)
        Sanderling.MouseClickLeft(Module);
    else
        Sanderling.KeyboardPressCombined(ToggleKey);
}


void MemoryUpdate()
{
 	RetreatUpdate();
	UpdateLocationRecord();
	OffloadCountUpdate();
	Timers ();

}
var logoutme= false;
var eveServerDT = DateTime.Today.AddDays(0).AddHours(11).AddMinutes(0);
var logoutgame = (eveServerDT-DateTime.UtcNow ).TotalMinutes;
void Timers ()
{
var now = DateTime.UtcNow;
	//Host.Log("utc time Start Session at  :  " + now.ToString(" dd/MM/yyyy hh:mm:ss") + " "); //used for debug
var CloseGameSession = (playSession - now).TotalMinutes;
//	Host.Log("Total minutes until session Is CLOSE :  " + CloseGameSession + " "); //used for debug
//	Host.Log("Ttimespan until session Is CLOSE :  " +TimeSpan.FromMinutes(CloseGameSession).ToString(@"dd\:hh\:mm")+ " "); //used for debug	

if (now >eveServerDT)
	{//	Host.Log("The DT was today at :  " + eveServerDT.ToString(" dd/MM/yyyy hh:mm:ss") + " "); //used for debug
		eveServerDT = DateTime.Today.AddDays(1).AddHours(11).AddMinutes(0);
	}
var eveSafeDT = eveServerDT.AddHours(-hoursToDT).AddMinutes(-minutesToDT);
	// Host.Log("Next safe logout at:  " + eveSafeDT.ToString(" dd/MM/yyyy hh:mm:ss")  + " "); // used for debug
var CloseGameDT = (eveSafeDT - now).TotalMinutes;
var LogoutGame = Math.Min(CloseGameDT,CloseGameSession);
	//	Host.Log("Closer Logout Game in :   " + TimeSpan.FromMinutes(LogoutGame).ToString(@"dd\:hh\:mm") + " ; if I do not reach the offload limit count "); //used for debug
if (playSession !=DateTime.UtcNow)
{
logoutgame = LogoutGame;
//Host.Log("Total minutes until session Is CLOSE :  " + logoutgame + " ");
}
	if (LogoutGame<0) 
		logoutme = true;
}

bool MeasurementEmergencyWarpOutEnter =>
    !(Measurement?.IsDocked ?? false) && !(EmergencyWarpOutHitpointPercent < ArmorHpPercent);

void RetreatUpdate()
{
    RetreatReasonTemporary = (RetreatOnNeutralOrHostileInLocal && hostileOrNeutralsInLocal) 
	|| (listOverviewDreadCheck?.Length > 0) || (listOverviewEntryEnemy?.Length > 0) || reasonDrones == true 
	|| (logoutme == true && SiteFinished ==true ) ? "reds in local or session time elapsed" : null;
    if (!MeasurementEmergencyWarpOutEnter)
        return;

    //	measure multiple times to avoid being scared off by noise from a single measurement. 
    Sanderling.InvalidateMeasurement();

    if (!MeasurementEmergencyWarpOutEnter)
        return;

    RetreatReasonPermanent = "They messed my Armor hp or session is expired??";
}
void UpdateLocationRecord()
{
    //	I am not interested in locations which are only close during warp.
    if (Measurement?.ShipUi?.Indication?.ManeuverType == ShipManeuverTypeEnum.Warp)
        return;

    // Purpose of recording locations is to prioritize our next destination when warping to mining site.

    var currentSystemLocationLabelText =
        Measurement?.InfoPanelCurrentSystem?.ExpandedContent?.LabelText
        ?.OrderByCenterVerticalDown()?.FirstOrDefault()?.Text;

    if (currentSystemLocationLabelText == null)
        return;

    // 2018-03 observed label text: <url=showinfo:15//40088644 alt='Nearest'>Amsen V - Asteroid Belt 1</url>

    var currentLocationName = RegexExtension.RemoveXmlTag(currentSystemLocationLabelText)?.Trim();
    var lastRecordedLocation = visitedLocations.LastOrDefault();

    if (lastRecordedLocation == currentLocationName)
        return;

    visitedLocations.Enqueue(currentLocationName);
    Host.Log("Recorded transition from location '" + lastRecordedLocation + "' to location '" + currentLocationName + "'");

    if (100 < visitedLocations.Count)
        visitedLocations.Dequeue();
}

// Orbit asteroid at 30km  Orbit("Crokite") will orbit first asteroid named Crokite at 10 km, stolen from forum :d
void Orbit(string whatToOrbit, string distance = "30 km")
{
    var ToOrbit = Measurement?.WindowOverview?.FirstOrDefault()?.ListView?.Entry?.Where(entry => entry?.Name?.RegexMatchSuccessIgnoreCase(whatToOrbit) ?? false)?.ToArray();
    ClickMenuEntryOnPatternMenuRoot(ToOrbit.FirstOrDefault(), "Orbit", distance);
}
// Stolen and modified from MineOre.cs
// modified to click on Submenu if SubMenuEntryRegexPattern not null
void ClickMenuEntryOnPatternMenuRoot(IUIElement MenuRoot, string MenuEntryRegexPattern, string SubMenuEntryRegexPattern = null)
{
    Sanderling.MouseClickRight(MenuRoot);
    var Menu = Sanderling?.MemoryMeasurementParsed?.Value?.Menu?.FirstOrDefault();
    var MenuEntry = Menu?.EntryFirstMatchingRegexPattern(MenuEntryRegexPattern, RegexOptions.IgnoreCase);
    Sanderling.MouseClickLeft(MenuEntry);
    if (SubMenuEntryRegexPattern != null)
    {
        // Using the API explorer when we click on the top menu we get another menu that has more options
        // So skip the MenuRoot and click on Submenu
		//var subMenu = Sanderling?.MemoryMeasurementParsed?.Value?.Menu?.Skip(1).First();
		var subMenu = Sanderling?.MemoryMeasurementParsed?.Value?.Menu?.ElementAtOrDefault(1);
        var subMenuEntry = subMenu?.EntryFirstMatchingRegexPattern(SubMenuEntryRegexPattern, RegexOptions.IgnoreCase);
        Sanderling.MouseClickLeft(subMenuEntry);
    }
}

var reasonDrones = false;
Func<object> TakeAnomaly()
{
    var probeScannerWindow = Measurement?.WindowProbeScanner?.FirstOrDefault();
    
    var scanActuallyAnomaly = probeScannerWindow?.ScanResultView?.Entry?.FirstOrDefault(ActuallyAnomaly);

    var UndesiredAnomaly = probeScannerWindow?.ScanResultView?.Entry?.FirstOrDefault(IgnoreAnomaly);

    var scanResultCombatSite = probeScannerWindow?.ScanResultView?.Entry?.FirstOrDefault(AnomalySuitableGeneral);

    Host.Log("Telemetry instruments: working at ignoring anomalies :) be patient");
   if ( (DronesInSpaceCount + DronesInBayCount ) < DroneNumber)
	{
	reasonDrones = true;

	}
   if (combatTab != OverviewTabActive)
	{ 
	Sanderling.MouseClickLeft(combatTab);
		Host.Delay(1111);
	}

    if (probeScannerWindow == null)
        Sanderling.KeyboardPressCombined(new[] { VirtualKeyCode.LMENU, VirtualKeyCode.VK_P });

    if (null != scanActuallyAnomaly)
    {
        ClickMenuEntryOnMenuRoot(scanActuallyAnomaly, "Ignore Result");
        return TakeAnomaly;
    }
    if (null != UndesiredAnomaly)
    {
        ClickMenuEntryOnMenuRoot(UndesiredAnomaly, "Ignore Result");
        return TakeAnomaly;
    }

    if (null == scanResultCombatSite)
        Host.Log(" Hubble: no more anomalies! If you dont like the asteroids then admire the Space. ");
    if ((null != scanResultCombatSite) && (null == UndesiredAnomaly))
    {
        Sanderling.MouseClickRight(scanResultCombatSite);
        var menuResult = Measurement?.Menu?.ToList();
        if (null == menuResult)
        { Host.Log(" Telemety calibration: not expected resultats. "); return TakeAnomaly; }
		else
		{
        var menuResultWarp = menuResult?[0].Entry.ToArray();
        var menuResultSelectWarpMenu = menuResultWarp?[1];
        Sanderling.MouseClickLeft(menuResultSelectWarpMenu);
        var menuResultats = Measurement?.Menu?.ToList();
		if (Measurement?.Menu?.ToList() ? [1].Entry.ToArray()[4].Text !=  "Within 50 km")
			{ 
			return TakeAnomaly;
			}
			else
			{        
			var menuResultWarpDestination = Measurement?.Menu?.ToList() ? [1].Entry.ToArray();
			Host.Log(" Hooray, warping to anomaly  ");
			ClickMenuEntryOnMenuRoot(menuResultWarpDestination[4], "within 50 km");
		if (probeScannerWindow != null)
			Sanderling.KeyboardPressCombined(new[] { VirtualKeyCode.LMENU, VirtualKeyCode.VK_P });
			}
		}
        return MainStep;
    }
    return MainStep;
}
void Orbitkeyboard()
{
    //Orbit(celestialOrbit); // to use for orbit celestial objects from overview 
    Sanderling.KeyDown(orbitKeyCode);

    if (0 < ListCelestialObjects?.Length)
        Sanderling.MouseClickLeft(ListCelestialObjects?.FirstOrDefault());
    if (0 == ListCelestialObjects?.Length)
    { Sanderling.MouseClickLeft(ListRatOverviewEntry?.FirstOrDefault(entry => (entry?.MainIconIsRed ?? false))); }

    Sanderling.KeyUp(orbitKeyCode);

    ActivateAfterburnerExecute();
    Host.Delay(1111);
    Host.Log("I smell roses ... better to Orbit arround here");
}
void OffloadCountUpdate()
{
    var OreHoldFillPercentSynced = OreHoldFillPercent;

    if (!OreHoldFillPercentSynced.HasValue)
        return;

    if (0 == OreHoldFillPercentSynced && OreHoldFillPercentSynced < LastCheckOreHoldFillPercent)
        ++OffloadCount;

    LastCheckOreHoldFillPercent = OreHoldFillPercentSynced;
}


bool AnomalySuitableGeneral(MemoryStruct.IListEntry scanResult) =>
    scanResult?.CellValueFromColumnHeader(AnomalyToTakeColumnHeader)?.RegexMatchSuccessIgnoreCase(AnomalyToTake) ?? false;

bool ActuallyAnomaly(MemoryStruct.IListEntry scanResult) =>
       scanResult?.CellValueFromColumnHeader("Distance")?.RegexMatchSuccessIgnoreCase("km") ?? false;

bool IgnoreAnomaly(MemoryStruct.IListEntry scanResult) =>
scanResult?.CellValueFromColumnHeader(IgnoreColumnheader)?.RegexMatchSuccessIgnoreCase(IgnoreAnomalyName) ?? false;

bool IsEnemyBackgroundColor(ColorORGB color) =>
    color.OMilli == 500 && color.RMilli == 750 && color.GMilli == 0 && color.BMilli == 0;


bool IsFriendBackgroundColor(ColorORGB color) =>
    (color.OMilli == 500 && color.RMilli == 0 && color.GMilli == 150 && color.BMilli == 600) || (color.OMilli == 500 && color.RMilli == 100 && color.GMilli == 600 && color.BMilli == 100);

bool IsNeutralOrEnemy(IChatParticipantEntry participantEntry) =>
   !(participantEntry?.FlagIcon?.Any(flagIcon =>
     new[] { "good standing", "excellent standing", "Pilot is in your (fleet|corporation|alliance)", "Pilot is an ally in one or more of your wars", }
     .Any(goodStandingText =>
        flagIcon?.HintText?.RegexMatchSuccessIgnoreCase(goodStandingText) ?? false)) ?? false);

