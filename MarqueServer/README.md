# MarqueServer
Plugin for LaunchBox. Serves the Marque for the current game being played in LaunchBox.

Todo:
* Convert to plugin
* Figure out how to communicate between the on start code and the menu settings code.
* Add quick links in settings.
* Center status in form.
* Complete renaming of project to Marquesas Server since it will be able to do more than
  just serve up Marques when completed.

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