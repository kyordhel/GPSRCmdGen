Command Generator for @HOME
============================


Random sentence generators RoboCup @Home tests based on the requirements stated on the [rulebook for RoboCup @HomeGo](http://www.robocupathome.org/rules). Current version is based on 2018 rules.

Following tests are currently supported:
- Speech and Person Recognition
- General Purpose Service Robot (GPSR)
- Enhanced Endurance General Purpose Service Robot (EEGPSR)

[Download compiled version for current rulebook (Stable)](http://github.com/kyordhel/GPSRCmdGen/blob/master/bin/stable/binaries.zip?raw=true)

## Minimum System Requirements
- Windows (Miscrosoft .NET)
    - Download and install [.NET Framework 4.5](https://www.microsoft.com/en-us/download/details.aspx?id=42642) or later
- Windows (Mono)
    - Download and install Mono release 4.5 or later ( [32bit](https://download.mono-project.com/archive/4.8.0/windows-installer/mono-4.8.0.495-gtksharp-2.12.42-win32-1.msi) [64bit](https://download.mono-project.com/archive/4.8.0/windows-installer/mono-4.8.0.495-x64-1.msi) )
- Ubuntu Linux

    sudo apt-get install mono-complete

## Building
First clone the repository (you will need git installed)

    git clone http://github.com/kyordhel/GPSRCmdGen.git

Building procedure depends on operating system
If you want to compile from source on:
- Windows (with Visual Studio)
    - Download and install [Visual Studio Express 2012](https://www.microsoft.com/en-us/download/details.aspx?id=34673)
- Windows (with MonoDevelop)
    - Download and install [Mono Develop](http://www.monodevelop.com/download/)
- Linux

    cd GPSRCmdGen
    make

## Testing

### SPR Test
To build and test Speech and Person Recognition question generator use

    make spr
or, to simply run it execute

    mono bin/Release/GPSRCmdGen.exe

### GPSR
To build and test GPSR command generator use

    make gpsr
or, to simply run it execute

    mono bin/Release/GPSRCmdGen.exe

### EEGPSR
To build and test EEGPSR command generator use

    make eegpsr
or, to simply run it execute

    mono bin/Release/EEGPSRCmdGen.exe

## Training
Both GPSR and EEGPSR generators support building a large set of randomly generated sentences by means of the --bulk [N] flag where the optional parameter N is an integer number between 10 and 10000 with default value of 100. Simply run either

    mono bin/Release/GPSRCmdGen.exe --bulk 1000
or

    mono bin/Release/EEGPSRCmdGen.exe --bulk 1000
Generated sentences will be stored in a text files in a subdirectory named after the grammar used for command generation. QR codes are also generated.


## Reuse in other competitions and projects
Thanks to the MIT license, you can adapt this project to your own needs (acknowledgments are always appreciated). Feel free to use this generator.

The generators use free context grammars that contain wild-cards which are replaced by random values from xml configuration files. The grammar's format specification can be found [here](https://github.com/kyordhel/GPSRCmdGen/wiki/Grammar-Format-Specification) and [here](https://github.com/kyordhel/GPSRCmdGen/blob/master/CommonFiles/FormatSpecification.txt)


## Contributing
Contributions and questions are always welcome
