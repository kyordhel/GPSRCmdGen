GPSR Command Generator
======================

Random command generator for the General Purpose Service Robot test of the RoboCup @Home
Based on the requirements stated on the rulebook for RoboCup @Home (check tag for specific year). [Go](http://www.robocupathome.org/rules)

[Download compiled version for current rulebook (Stable)](http://github.com/kyordhel/GPSRCmdGen/blob/master/bin/stable/GPSRCmdGen.zip?raw=true)

If you want to compile from source on Ubuntu:

    sudo apt-get install mono-complete
    git clone http://github.com/kyordhel/GPSRCmdGen.git
    cd GPSRCmdGen
    make
    
To build and test GPSR command generator use
    make gpsr
or
    mono bin/Release/GPSRCmdGen.exe


To build and test EEGPSR command generator use
    make eegpsr
or
    mono bin/Release/EEGPSRCmdGen.exe

RoboCup@Home teams and team members are welcome to post GitHub issues for clarifications, questions and also contribute with the project etc.
