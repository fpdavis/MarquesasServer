# Marquesas Server
Small Footprint HTTP Server Plugin for LaunchBox

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

   * /Manual

JSON API for retrieving

   * /StateManager (Unbroken.LaunchBox.Plugins.PluginHelper.StateManager)
   * /StateManager/IsInGame
   * /StateManager/* (Unbroken.LaunchBox.Plugins.PluginHelper.StateManager.*)

   * /SelectedGames (Unbroken.LaunchBox.Plugins.Data.IGame)
   * /SelectedGames/* (Unbroken.LaunchBox.Plugins.Data.IGame.*)
	  
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
compreshinsive list of dynamically generated direct links and detailed information.

  Source: https://github.com/fpdavis/MarquesasServer
Binaries: https://forums.launchbox-app.com/files/file/675-marquesas-http-server
Binaries: https://github.com/fpdavis/MarquesasServer/releases

Todo (in order of importance):

   * Add support for SuperSockets
   * Need to continue to refine default index page and add documentation to it
   * Problems with Chrome and the Manual, both /Manual and /Binary/Manual
   * Add support for /Game/ID to pull back game information for games not selected but in XML files.
     ID could be the games ID, Title, or possibly some other identifier
   * Memory/CPU profile

Changes (oldest to newest):

   * Moved the spin up of the http server into its own class
   * Created new placeholder object for game data
   * Commented out the ProcessMonitor for now as it won't be needed as a plugin
   * Added Base64 encoded image serving with caching
   * Identified HTML to use for serving Base64 encoded image as maximum sized
     background image that maintains aspect ratio and does not exceed the
     bounds of the browser window.

   * Added settings form

   * Tied in form settings to backend code
   * Split out abstract HTTP Server class.
   * Created list object to store HttpServer references.
   * Changed Update.cs to use Application Name from Resource string.

   * Implemented server side and client side caching. Client side caching uses 
     Etag/If-None-Match. This has been tested in Chrome and Edge. The If-None-Match
     sounds promising but may not be as consistant and predictable as a simple 
     javascript ajax call to check if a change has occured.
   * Converted to a Windows executable to be able to test form.
   * Settings Form now loads and saves data.
   * Added Readme.md file.

   * Upgraded HTTPServer for better threading, stopping, SSL support, and many more options for future expansion.
   * Removed option to select IP Address, can now bind to all available addresses without extra thread overhead.
     This should not be an issue in most cases.
   * Added opton for Secure port number.
   * Added ability to enable or disable each of the ports.

   * Added API Interface for retrieving StateManager information.
   * Removed unused files.
   * Centered the status form.
   * Added communications pathway between ISystemMenuItemPlugin and ISystemEventsPlugin for starting and 
     stopping the server using MarquesasHttpServerInstance.RunningServer.
   * Updated PluginAppSettings.cs to store newly added App.config settings instead of displaying a warning.

   * Converted Helper classes to static classes
   * Added SecondsBetweenRefresh to Admin and app.config
   * Added buttons to launch URL of main server page
   * Moved SelectedGames up one level, will further refine SelectedGame vs SelectedGames vs Game
   * Added initial default page with links to all current pages

   * Added Manual Support
   * Added HTML Error pages
   * Split up game heiarchy into SelectedGame (game being played?) vs SelectedGames (not being played, with possible multiple selections)
   * Renamed SelectedGames to SelectedGame (without the S)
   * Changed SelectedGame (previously SelectedGames) so the root /SelectedGame only returns information if one and only one game is selected
   * Added SelectedGames (with the S) JSON responses to return information on multiple games
   * Gracefully handles empty properties
   * Added Binary support
   * Continued to refine default HTML page

   * Add port use detection on startup
   * Reworked help to use Tool Tips
   * Added first time run dialog
   * Changed background color on autoloading pages to black

   * Updated update helper to handle multiple files instead of just one Dll in preperation for socket support
   * Addded commented out socket code
   * Completed Etag support for default html page (cache Page/MD5Sum)
   * Fixed issue when a selected game had no available resource, the html page would not refresh as expected