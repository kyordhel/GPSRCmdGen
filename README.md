GPSR Command Generator
======================

Random command generator for the General Purpose Service Robot test of the RoboCup @Home
Based on the requirements stated on the rulebook for RoboCup @Home (check tag for specific year). [Go](http://www.robocupathome.org/rules)

[Download compiled GPSR version for current rulebook (Stable)](http://github.com/kyordhel/GPSRCmdGen/blob/master/GPSRCmdGen/bin/Release/GPSRCmdGen.exe?raw=true)
[Download compiled EEGPSR version for current rulebook (Stable)](http://github.com/kyordhel/GPSRCmdGen/blob/master/EEGPSRCmdGen/bin/Release/EEGPSRCmdGen.exe?raw=true)

If you want to compile from source on Ubuntu:

    sudo apt-get install mono-complete
    git clone http://github.com/kyordhel/GPSRCmdGen.git
    cd GPSRCmdGen
    make run
    
Later go to GPSRCmdGen/bin/Release and execute
    mono GPSRCmdGen.exe
for testing the GPSR command generator, or go to EEGPSRCmdGen/bin/Release
    mono EEGPSRCmdGen.exe
for testing the EEGPSR command generator

RoboCup@Home teams and team members are welcome to post GitHub issues for clarifications, questions and also contribute with the project etc.
