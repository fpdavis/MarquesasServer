# MarqueServer
Plugin for LaunchBox. Serves the Marque for the current game being played in LaunchBox.

Todo:
* Convert to plugin
* Figure out how to communicate between the on start code and the menu settings code.

Changes:
* Implemented server side and client side caching. Client side caching uses 
  Etag/If-None-Match. This has been tested in Chrome and Edge. The If-None-Match
  sounds promising but may not be as consistant and predictable as a simple 
  javascript ajax call to check if a change has occured.
* Converted to a Windows executable to be able to test form.
* Settings Form now loads and saves data.