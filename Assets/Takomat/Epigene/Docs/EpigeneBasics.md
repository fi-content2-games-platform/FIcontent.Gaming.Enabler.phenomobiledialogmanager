Epigene Basics
==============


Introduction
------------

This document aimed for help to understand the basics of Epigene.

Epigene is a game engine(frame work) built inside Unity3D and it's goal 
to support and help the creation of certain type of games.
It incorporates many technologies and 
provides most of the toolsets you need to create new games using it.


The system
----------

Epigene is built from various components, each designed for one key function.
Many parts of the game can be configured via json files.

Core components:

- Game system 		Low level game setup, trigger, event systems
- Event system 		Event system is responsible to provide 
                    a communication layer between core and game components
- UI system 		Graphics, buttons, images, huds, texts, videos 
- Localization		Multi language (i18n) localization system 
                    with text, sprites and audio support
- Dialog systems 	trigger driven dialog system 
                    with "bubbles" and "multi answers"
- Networking		TCP/IP, UPD, HTTP network support 
                    to connect to network game servers
- Audio System  	Sound and background music support
- Video system 		playback of videos
- Virtual Studio 	support of a built in virtual studio

Next to the core components, Epigene also contains an Editor,
which has been designed to create new games from scratch, 
create or manage ui screens.
This editor is built in plugin, 
which also helps on importing 

The complete system also includes several other components, such as:

- Game server 		Game server written in C++ and 
                    can communicate with Epigene via network.
- AI exporter		A script for Adobe Illustrator 
                    to export a complete UI design, 
                    which can be imported into Epigene based games.
- json files 		Most of the game logic is written in json config files. 
                    While it's part of the core functionality, 
                    it is also the format we use over each system. 


Since multiple games and many developers share the same files, 
we try our best to keep things organised. 
The layout of the Epigene framework should 
be followed layout within each game. 
Epigene itself should be apart from the game and 
the preferred way of integrate it is as git submodule. 
This allows each game to be synchronised 
with any specific version of the core.
When you are develop with epigene, 
you can also use symbolic links to link Epigene to your project.

Just as with organising the files, 
the source code of Epigene is also organised and 
grouped into namespaces, each starts with "Epigene".
(We might need to change this latter to Takomat.Epigene)

Project layout:

Each project has an Assets folder which is the root of everything.
Each assets which are related, developed, 
owned by Takomat is located under the 
Assets/Takomat folder. Third party assets should be placed under Assets, 
but outside of Takomat folder.

A typical game layout would looks like this:

Assets/	Takomat
			Epigene
				Doc 		Documentations
				Sources 	Source file for the core system.
				Editor 		Built-in editor integration
				Resources	basic stuff for editor and low level ui
			Game
				Resources	
					Sprites
					Audio


		/3rd 		3rd party stuff (we should organise like this)
		/custom-name	3rd party stuff (this is atm)

To integrate Epigene simply add it / link to your project under the location or use submodule if possible.
Assets/Takomat/Epigene

Dynamically shared assets and resources:
Make sure, that if you use the same assets in multiple location dynamically, you place them under a subfolder to avoid conflicts.
ALL Resources folder will be merged as one, and your game will access to all.
Let's say you load a prefab or sprites using it's location, and you want to have the same stuff shared with other projects, then use the game name as a subfolder:
Assets/Takomat/Game/Resources/MyGame1/bubble1.mat
Assets/Takomat/Game/Resources/MyGame2/bubble1.mat


Quick Start
-----------

To quickly start a game you have 3 options:
# use an existing game as template, then clear the project from not needed assets (sprites, etc) and start to add the new ones
# use an empty template 
# Use the built in Game / UI Editor to create a new game

(Atm of writing this document, we only have the #1 option available.)

quick steps:
------------

# create new project /open one in Unity
# get epigenecore from a git "EpigeneCore" https://username@bitbucket.org/takomatgmbh/epigenecore.git
# create a new MyGame.cs file inherited fro the Base.
# setup the initialisation, based on the templates, other games setup (we miss lots of docs here, so use what we have: games)
# Add your sprites, files into the project under Assets/Takomat/Game/Resources/
# Either create manually or copy existing json screen files to get start up, or use the built in Editor
# Create a screen class to have canvas with camera like MainScreen.cs inheriting from core Epigene.UI.Screen
# Make sure your json screen file is valid and it refer to the same c# script you just created above. (you can use an existing game or template to create these two files)
# Click Play

Game System
-----------

The core system which manage the trigger system for a game. It is designed to use multiple running game, where it is possible to switch between them, pause or resume each one of them individually. 

The game system built up from a GameManager and Game classes, which defines the configuration and game related setups, initialisations.

These initialisations, setups are divided into two parts:

# MainGame class 	every new game must implement a game class from this base and setup the basic initialisation for each game.
# Assets			json files, sprites, sounds, etc.

Accessing to the game manager is via it's singleton instance:

GameManager gm = GameManager.Instance;


================================================================================
UI System
================================================================================

UI System is responsible for manage and handle buttons, sprites, user events, etc.
The system also connected with other core components for more flexibility and ease of use.
(i.e.. is use the audio system to play the required sounds based on user events)

As many other parts of the system, you can access 
to the ui system via the UIManager singleton instance.

UIManager ui = UIManager.Instance;

UI system designed from scratch due to limitations 
of existing / available systems and our requirements. 
The ui systems was designed to support the ui elements 
in a game and also to create them quickly or 
transport them from one game to other.

Screens ------------------------------------------------------------------------

Screen is a representation of a canvas in Epigene. 
A way of organise ui elements within a game and make 
it easy to switch between them. You can have 
multiple screens and navigate between 
then easily with a "SwitchScreen" function.
You can define a screen with a json file and 
epigene can built up the ui element dynamically for you based on the json file.

A UIScreen defined in JSON is called by the main Game.
A UIScreen has a script with the same name as the JSON file.
This CS script handles ... 

HUD  --------------------------------------------------------------------------- 

1. HUDs are like any other ui elements, but those are more complex 
then basic ui elements like buttons or scrolls, etc.
HUDs are usually has game dependent logics and built up 
from multiple ui components or even from multiple huds.
Within Epigene HUDs are slightly separated, 
because they cannot be make out from json file. 
Instead it have to be made by a programmer. 
The simple reason behind this decision is to don't repeat ourselves. 
Epigene provide everything you need except the game specific parts, 
you we leave and give the freedom to every game, 
developers and designers to play.
Using HUDs there is no limitation what and 
how to integrate custom UI items into a game.

2. It is basically defined by a behavior for handling the logic
of one or more UI objects,
a set of ui objects defined in a UIScreen/JSON for 
a heads up display,
and a dedicated layer HUD in the UIManager,
which can be set active or passive.

3. Example and Creation

3.1 An Example is a SimulationMenu prefab,
which has a script HUD.cs for the logic.

3.2 Creating a complete HUD system description: TODO



================================================================================
Event System
================================================================================


Most of the elements, not only UI uses Epigene event system. 
This way different parts of the system does not have 
to know about each-other but can listen or send messages, events.

Usage:
 GameManager.Instance.Event(..)

Register/deregister events

Every component who want to capture or listen 
on events need to implement 
a listener function and register it via the game manager.
When the object does not care about the events anymore 
it should deregister those.

Use OnEnable()/OnDisable() to register / deregister event handlers, so the object will be always valid based on it's state (SetActive(true/false) would also register/deregister it)
If you forget to deregister an event and you register it again, you could miss events or some events might arrives into not existing objects. (probably exception)

Localization
--------------

A localization system helps to localise every part of the game, 
including texts, sprites or images or audio files. 
All of the texts are located and defined 
for each language in a localization db, 
which is a plain text file in an simple format:

[LANG/MODUL]
[ID]="my-text"

In the code you can access to the localization system via a singleton and get the right text via the id as "MODUL.ID". The language part is managed automatically by the system to make sure you always get the last selected language by user.

Dialog systems
--------------

Dialog system was designed to support multiple scenarios. 
Each scenarios can contains any number of dialogs or multi answers.
This system will drive the user through a dialog with NPC characters and can also be used to create quiz within the game.

Using the dialog manager you can have access to this system and you can load scenarios or switch between them.

All scenarios with dialogs and answers are defined in json configuration files.
These configuration files does NOT includes any texts, but only localization IDs.
Each text located in the localisation db.

Networking
------------

Networking module implement a layer to TCP/UPD and HTTP protocols.
The system hides which protocol is used and make it simple to connect to other servers.

Audio System
--------------

Audio system supports background music (loop) and "any" number of sound effects to play simultaneously. 


Virtual Studio
--------------

Virtual studio module features a built-in solution to represents 
3D objects on 2D/3D screens like they would be in a studio. The object can be rotated and each parameters, such as visual effects, lights, size, shaders, etc can be separated from the rest of the game.


Nuts and Bolts about unity3d
----------------------------

1) WINDOW is not Fullscreen in Windows 8

When you start a Unity app in windows with the display option enabled, 
so we can set the resolution and fullscreen mode by hand at start, 
then this settings are SAVED to your LOCAL PC.

So, if you start a unity app (test app, any app) where you have this options,
then your settings will be used later not the defaults. 
Defaults only used the very first time the app used.
Unity saves this settings by the app name (on windows),
so if you have another game with same name, then it will override your settings.
Since we don't have a display resolution dialog (because we don't want it),
the game will start as it was set LAST time, and not as it is set as default in Unity.
It make sense, but due to development it can cause strange issues.. like now.

We do testing other apps, so it's easy to set this to something else. 
Just as it was set to an iphone size on my windows due to one 
of my last test for the automatic upgrade. 
(I used a small app instead of a big eezaos, 
but with the same name so the scripts would all work)

Solution / workaround
1. You have to build the eezaos (or another smaller app 
with same name I guess), which has the Display Resolution 
Dialog option enabled (in build->player settings).
2. Run the game once with this option, set the settings 
to fullscreen and the proper resolution. Run the game and exit.
3. Rebuild the game with no resolution dialog, and it will work perfectly
