Command Generator for @HOME
============================


Random sentence generators RoboCup @Home tests based on the requirements stated on the rulebook for RoboCup @Home. [Go](http://www.robocupathome.org/rules). Current version is based on 2017 rules.

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

    make gpsr
or

    mono bin/Release/GPSRCmdGen.exe

### GPSR
To build and test GPSR command generator use

    make gpsr
or

    mono bin/Release/GPSRCmdGen.exe

### EEGPSR 
To build and test EEGPSR command generator use

    make eegpsr
or

    mono bin/Release/EEGPSRCmdGen.exe

RoboCup@Home teams and team members are welcome to post GitHub issues for clarifications, questions and also contribute with the project etc.
