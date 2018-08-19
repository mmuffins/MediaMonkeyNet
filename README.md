# MediaMonkeyNet
[![nuget](https://img.shields.io/nuget/v/MediaMonkeyNet.svg)](https://www.nuget.org/packages/MediaMonkeyNet/)

A .net standard library to read state information and automate [MediaMonkey 5](https://www.mediamonkey.com/). The library uses  the [Chrome DevTools Protocol](https://chromedevtools.github.io/devtools-protocol/) to establish a connection to the MediaMonkey frontend which is running on the chromium engine. Once a session is opened, native MediaMonkey javascript commands are used to read and write information.

## Usage
See the included sample project for a full usage example.