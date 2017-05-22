# MarqueServer
Plugin for LaunchBox. Serves the Marque for the current game being played in LaunchBox.

Todo:
* Convert to plugin
* Basic server side caching is implemented but really need some type of client side
  caching. Either through If-Modified-Since/If-None-Match or through some custom
  javascript ajax. The If-None-Match sounds promising but probably isn't as consistant
  and predictable as a simple javascript ajax call to check if a change has occured.
* Figure out how to communicate between the on start code and the menu settings code.

Changes:
