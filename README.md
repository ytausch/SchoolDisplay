# SchoolDisplay

[![CI](https://github.com/ytausch/SchoolDisplay/workflows/CI/badge.svg)](https://github.com/ytausch/SchoolDisplay/actions)

An easy-to-use digital bulletin board designed for schools.

![Banner](docs/assets/banner.png)

## How does it work?

This Windows application displays successively and repeatedly all PDF files in a configured directory in full screen mode and scrolls through them from top to bottom. It is also possible to use [UNC paths](https://en.wikipedia.org/wiki/Path_(computing)#Universal_Naming_Convention) that refer to network shares - changes are applied in real-time.

SchoolDisplay is a fully featured digital bulletin board - for example for use in schools.


## Getting Started
### Prerequisites
**Windows** with [**.NET Framework 4.8**](https://dotnet.microsoft.com/download/dotnet-framework/thank-you/net48-web-installer) installed or [preinstalled](https://docs.microsoft.com/en-us/dotnet/framework/get-started/system-requirements).

### Installation
1. Download the [latest release](https://github.com/ytausch/SchoolDisplay/releases) (`SchoolDisplay_x.y.z.zip`) and unzip it
2. Edit the configuration file `SchoolDisplay.exe.config` - make sure to change `PdfFilePath` and your display times.
3. Run `SchoolDisplay.exe` and have fun! :tada:
