# Marquesas Server
Small footprint HTTP Server Plugin for LaunchBox. Provides the following:

   Standard HTTP interface for retrieving HTML pages for the current game containing.
   Any *ImagePath property can be retrieved from the IGame object (Unbroken.LaunchBox.Plugins.Data.IGame.*)
   by specifying /Image/ followed by the Image descriptor (the part preceeding ImagePath)

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

	  * /Manual - Not yet implemented 

   JSON API for retrieving:

      * /StateManager (Unbroken.LaunchBox.Plugins.PluginHelper.StateManager)
	  * /StateManager/IsInGame
	  * /StateManager/* (Unbroken.LaunchBox.Plugins.PluginHelper.StateManager.*)

	  * /SelectedGames (Unbroken.LaunchBox.Plugins.Data.IGame)
	  * /SelectedGames/* (Unbroken.LaunchBox.Plugins.Data.IGame.*)
	  


Todo:
* Add port use detection on startup
* Need to continue to refinen default html page and add documentation to it
* Complete Etag support for default html page (cache Page/MD5Sum)
* Add support for /Game/ID to pull back game information for games not selected but in XML files.
  ID could be the games ID, Title, or possibly some other identifier
* Memory/CPU profile

Changes:
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