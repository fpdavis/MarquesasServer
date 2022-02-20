# Marquesas Server
Small Footprint HTTP Server Plugin for LaunchBox. Utilizes the Launchbox API found at https://pluginapi.launchbox-app.com/

Example Uses

   * Display the current game marque full screen on a seperate system such as a tablet or smart phone.
   * View the current game manual on a seperate system such as a tablet, laptop, or smart phone.
   * Retrieve LaunchBox and game information through a standard HTTP JSON API.

Installation

   To install unzip and copy the MarquesasServer folder into your LaunchBox/Plugins folder.
   The server will begin running whenever LaunchBox is started. The first time you run 
   it with the plugin a Windows security dialog will pop up asking if you want to allow 
   it to connect to the network. You will need to select one of the two options presented, 
   the option "Private networks, such as my home or work network will suffice". Once in
   LaunchBox you will see a new "Marquesas Server Admin" option under the Tools menu to
   manage the server. The server can then be accessed from any computer, phone, or tablet
   located on (or with access to) the same network as the machine running LaunchBox/BigBox.

   Requires .NET Framework Version 4.8 from https://dotnet.microsoft.com/download/dotnet-framework/net48

   **See the default index (LaunchBox->Tools->Marquesas Server Admin->Ellipses Button) for a 
   comprehensive list of dynamically generated direct links and detailed information.**

Auto Refreshing Web Pages

   These pages (specifically the Marque page) were designed to be brought up on a separate
   computer such as an old tablet to display the marque of the game currently being played
   in BigBox. It is recommended that the page be loaded and then the browser set to full
   screen (kiosk) mode. In most browser on Windows you can toggle kiosk mode by hitting F11
   when in your browser. Each page will refresh automatically every few seconds to insure
   that the game being played will be reflected in the browser.
   
   Works via a standard HTTP interface allowing the retrieval of HTML pages for the current
   game. Any *ImagePath property can be retrieved from the IGame object
   (Unbroken.LaunchBox.Plugins.Data.IGame.*) by specifying /Image/ followed by the Image
   descriptor (the part preceeding ImagePath)

   * /Image/Back
   * /Image/Background
   * /Image/Box3D
   * /Image/Cart3D
   * /Image/CartBack
   * /Image/CartFront
   * /Image/ClearLogo
   * /Image/Front
   * /Image/Marque
   * /Image/PlatformClearLogo
   * /Image/Screenshot
   <br><br>
   * /Manual

JSON API for retrieving data

   * /StateManager (Unbroken.LaunchBox.Plugins.PluginHelper.StateManager)
   * /StateManager/IsInGame
   * /StateManager/* (Unbroken.LaunchBox.Plugins.PluginHelper.StateManager.*)
   <br><br>
   * /SelectedGame (Unbroken.LaunchBox.Plugins.Data.IGame.Properties)
   * /SelectedGames (Unbroken.LaunchBox.Plugins.Data.IGame.Properties)
   * /SelectedGames/* (Unbroken.LaunchBox.Plugins.Data.IGame.Properties.*)
   * /SelectedGameMethods/* (Unbroken.LaunchBox.Plugins.Data.IGame.Methods.*)

JSON API for retrieving raw binaries

   * /Binary/ScreenshotImage
   * /Binary/FrontImage
   * /Binary/MarqueeImage
   * /Binary/BackImage
   * /Binary/Box3DImage
   * /Binary/BackgroundImage
   * /Binary/Cart3DImage
   * /Binary/CartFrontImage
   * /Binary/CartBackImage
   * /Binary/ClearLogoImage
   * /Binary/PlatformClearLogoImage
   * /Binary/Application
   * /Binary/Configuration
   * /Binary/DosBoxConfiguration
   * /Binary/Manual
   * /Binary/Music
   * /Binary/ScummVmGameDataFolder
   * /Binary/Video
   * /Binary/ThemeVideo

See the default index (LaunchBox->Tools->Marquesas Server Admin->Ellipses) for a 
comprehensive list of dynamically generated direct links and detailed information.

Source & Binaries

   * Source: https://github.com/fpdavis/MarquesasServer
   * Binaries: https://forums.launchbox-app.com/files/file/675-marquesas-http-server
   * Binaries: https://github.com/fpdavis/MarquesasServer/releases

Todo (in order of importance):

   * Add working examples for the new Game & Platform Calls in the Index 
   * Add support for SuperSockets
   * Memory/CPU profile

Changes (oldest to newest):

   * Moved the spin up of the http server into its own class
   * Created new placeholder object for game data
   * Commented out the ProcessMonitor for now as it won't be needed as a plugin
   * Added Base64 encoded image serving with caching
   * Identified HTML to use for serving Base64 encoded image as maximum sized
     background image that maintains aspect ratio and does not exceed the
     bounds of the browser window.
   <br><br>
   * Added settings form
   <br><br>
   * Tied in form settings to backend code
   * Split out abstract HTTP Server class.
   * Created list object to store HttpServer references.
   * Changed Update.cs to use Application Name from Resource string.
   <br><br>
   * Implemented server side and client side caching. Client side caching uses 
     Etag/If-None-Match. This has been tested in Chrome and Edge. The If-None-Match
     sounds promising but may not be as consistant and predictable as a simple 
     javascript ajax call to check if a change has occured.
   * Converted to a Windows executable to be able to test form.
   * Settings Form now loads and saves data.
   * Added Readme.md file.
   <br><br>
   * Upgraded HTTPServer for better threading, stopping, SSL support, and many more options for future expansion.
   * Removed option to select IP Address, can now bind to all available addresses without extra thread overhead.
     This should not be an issue in most cases.
   * Added opton for Secure port number.
   * Added ability to enable or disable each of the ports.
   <br><br>
   * Added API Interface for retrieving StateManager information.
   * Removed unused files.
   * Centered the status form.
   * Added communications pathway between ISystemMenuItemPlugin and ISystemEventsPlugin for starting and 
     stopping the server using MarquesasHttpServerInstance.RunningServer.
   * Updated PluginAppSettings.cs to store newly added App.config settings instead of displaying a warning.
   <br><br>
   * Converted Helper classes to static classes
   * Added SecondsBetweenRefresh to Admin and app.config
   * Added buttons to launch URL of main server page
   * Moved SelectedGames up one level, will further refine SelectedGame vs SelectedGames vs Game
   * Added initial default page with links to all current pages
   <br><br>
   * Added Manual Support
   * Added HTML Error pages
   * Split up game heiarchy into SelectedGame (game being played?) vs SelectedGames (not being played, with possible multiple selections)
   * Renamed SelectedGames to SelectedGame (without the S)
   * Changed SelectedGame (previously SelectedGames) so the root /SelectedGame only returns information if one and only one game is selected
   * Added SelectedGames (with the S) JSON responses to return information on multiple games
   * Gracefully handles empty properties
   * Added Binary support
   * Continued to refine default HTML page
   <br><br>
   * Add port use detection on startup
   * Reworked help to use Tool Tips
   * Added first time run dialog
   * Changed background color on autoloading pages to black
   <br><br>
   * Updated update helper to handle multiple files instead of just one Dll in preperation for socket support
   * Addded commented out socket code
   * Completed Etag support for default html page (cache Page/MD5Sum)
   * Fixed issue when a selected game had no available resource, the html page would not refresh as expected
   <br><br>
   * Launchbox's upgrade to .Net core moved Unbroken.LaunchBox.Plugins.dll from the Metadata directory to the Core directory
   * Update .Net Framework to 4.8
   * Fixed broken Binary Requests for Video, Music, and Manuals. This should have fixed the problems with
     Chrome and the Manual, both /Manual and /Binary/Manual.
   * Fixed a refresh issue where every other Auto Refresh would bring back the "No Resource" available page.
   * Fixed bug when no game was selected.
   * Added more precise version checking.
   * Added version number to settings panel.
   <br><br>
   * Added support for IGame Methods.
   * Added option for a read only mode (Write Enabled) now that write IGame Methods have been added.
   * Links to IGame Set Methods do not generate HTML links so data isn't accidentally overridden.
   <br><br>
   * Added /GetAllGames that supports AllProperties (/GetAllGames/AllProperties)
   * Added GetAllGamesLimit to App.config to limit number of entries returned for /GetAllGames/AllProperties. Default is 50.
   * Added partial match search terms on the querystring that are ANDED for GetAllGames for Title, Publisher, and Platform (?Title=Asteroids)
   * Added /PlayGame which matches games on Title, PublisherAndTitle, Id, and LaunchBoxDbId. Examples: /PlayGame/Title/EXACT_TITLE, /PlayGame/PublisherAndTitle/EXACT_PUBLISHERNAME/EXACT_TITLE, /PlayGame/LaunchBoxDbId/123456
   * Added support for /Game to pull back game information for games not selected but in XML files. Supports the same input as /PlayGame and requires exact matches.
   * Added /GetAllPlatforms with partial match search terms on the querystring that are ANDED for Title, Manufacturer, and Developer (?Title=Commodore 64)
   * Added /Platform/Title/EXACT_TITLE/[IncludeGameInfo] for platform support
   * Added the new API calls to the Index page under API Requests->Game & Platform Calls.
   <br><br>
    * Added headers for Access-Control-Allow-Origin configurable in the app.config file.
    * Added Javascript only example page