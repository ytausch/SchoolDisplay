# SchoolDisplay

[![CI](https://github.com/ytausch/SchoolDisplay/workflows/CI/badge.svg)](https://github.com/ytausch/SchoolDisplay/actions)

> An easy-to-use digital bulletin board designed for schools.

**Ein ausführliches deutsches Handbuch ist [hier](docs/README_DE.md) verfügbar.**

![Banner](docs/assets/banner.png)

## How does it work?

This Windows application displays successively and repeatedly all PDF files in a configured directory in full screen mode and scrolls through them from top to bottom. It is also possible to use [UNC paths](https://en.wikipedia.org/wiki/Path_(computing)#Universal_Naming_Convention) that refer to network shares - changes are applied in real-time.

SchoolDisplay is a fully featured digital bulletin board - not only for use in schools.

### Features
* Uses PDF as universal content format
* Supports network shares as file source
* Automatic scrolling (customizable speed)
* Real-time file updates
* Configuration of scheduled display on/off times
* Fullscreen mode
* A Clock :clock3:

## Getting Started
### Prerequisites
* Operating System: **Windows 7 or higher** (recommended: Windows 10)
* [**.NET Framework 4.8**](https://dotnet.microsoft.com/download/dotnet-framework/thank-you/net48-web-installer) ([preinstalled on some versions of Windows 10](https://docs.microsoft.com/en-us/dotnet/framework/get-started/system-requirements))

### Installation
1. Download the [latest release](https://github.com/ytausch/SchoolDisplay/releases) (`SchoolDisplay_x.y.z.zip`) and unzip it
2. Edit the configuration file `SchoolDisplay.exe.config` - make sure to change `PdfDirectoryPath` and your display times.
3. Run `SchoolDisplay.exe` and have some fun! :tada:

### Usage
Just change the files in your `PdfDirectoryPath` (you should probably use a network share to do that). The screen contents will update in real-time.

## Questions or improvements?
Feel free to [open an issue](https://github.com/ytausch/SchoolDisplay/issues/new) or a pull request.

## License
SchoolDisplay is licensed under the Apache License 2.0, but contains third-party software that may be subject to different terms and conditions. See [LICENSE](LICENSE) and [NOTICE](NOTICE) for more information.
