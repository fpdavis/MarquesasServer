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
	  * /StateManager/* (Unbroken.LaunchBox.Plugins.PluginHelper.StateManager.*)
	  * /StateManager/SelectedGames (Unbroken.LaunchBox.Plugins.Data.IGame)
	  * /StateManager/SelectedGames/* (Unbroken.LaunchBox.Plugins.Data.IGame.*)
	  * /StateManager/IsInGame



Todo:
* Add quick links in settings.
* Complete renaming of project to Marquesas Server since it will be able to do more than
  just serve up Marques when completed.
* Implement /Manual
* Need to return a default html page with links
* Memory/CPU profile
* Gracefully handle empty properties, currently returns 404s

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