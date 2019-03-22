RoboCup@Home Command Generator
==============================

Set of sentence generators for the RoboCup @Home tasks.
The latest updates follows following the tasks, rules, and specifications of the Rulebook for Sydney 2019.

To learn more about the rules, visit the [RoboCup@Home website](http://athome.robocup.org) and the [Rulebook repository](https://github.com/RoboCupAtHome/RuleBook/).

The solution includes generators for the following tasks:
- General Purpose Service Robot (GPSR)
- Enhanced General Purpose Service Robot (EGPSR)


## Minimum System Requirements
You need either the Microsoft .NET framework or Mono versions 4.5 or above.
Although the code itself is compatible with the specification of the .NET Framework 2.0, the included solution files target a later build for compatibility.

### Windows

 - Using Miscrosoft .NET
    - Download and install [.NET Framework 4.5](https://www.microsoft.com/en-us/download/details.aspx?id=42642) or later
    - Download and install [Visual Studio Express](https://visualstudio.microsoft.com/vs/express/)
- Using Mono
    - Download and install Mono release 4.5 or later ( [32bit](https://download.mono-project.com/archive/4.8.0/windows-installer/mono-4.8.0.495-gtksharp-2.12.42-win32-1.msi) [64bit](https://download.mono-project.com/archive/4.8.0/windows-installer/mono-4.8.0.495-x64-1.msi) )
    - Download and install [Mono Develop](http://www.monodevelop.com/download/)

### Linux

- Ubuntu

    `sudo apt-get install mono-complete cmake git-all`

- Debian

    `# apt install mono-devel cmake git-all`

- CentOS

    `# yum install mono-devel cmake git-all`

- Fedora

    `# dnf install mono-devel cmake git-all`

Normally, running the aforementioned command in your linux distribution should suffice.
Should this is not the case, it may be necessary to add the package repository.
To learn more, please visit the [Mono Project Website](https://www.mono-project.com/download/stable/)

## Building
First clone the repository (you will need git installed)

    git clone http://github.com/kyordhel/GPSRCmdGen.git

The building procedure depends on operating system.
If you want to compile from source on:

### Windows
First, download and unzip the solution from [https://github.com/kyordhel/GPSRCmdGen/archive/master.zip](https://github.com/kyordhel/GPSRCmdGen/archive/master.zip).
Once this is done, open the solution file and select *run* from the *build* menu.

### Linux
First, clone the repository (you will need git installed)

    git clone http://github.com/kyordhel/GPSRCmdGen.git

Then, all you need is to run `cmake`

    cd GPSRCmdGen
    make

## Testing

### GPSR
To build and test GPSR command generator use

    make gpsr
or, to simply run it execute

    mono bin/Release/GPSRCmdGen.exe

### EGPSR
To build and test EEGPSR command generator use

    make egpsr
or, to simply run it execute

    mono bin/Release/EGPSRCmdGen.exe

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
