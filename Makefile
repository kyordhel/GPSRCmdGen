# Makefile for compiling GPSRCmdGen under mono.

## MAIN ###################

PREFIX = GPSRCmdGen

## #################################### ##
##  VARIABLES                           ##
## #################################### ##

SOLUTION     = $(PREFIX).sln
PATH_DEBUG   = bin/Debug
PATH_RELEASE = bin/Release
BIN_DEBUG    = $(PATH_DEBUG)/$(PREFIX).exe
BIN_RELEASE  = $(PATH_RELEASE)/$(PREFIX).exe
XBUILDFLAGS  = /p:Platform="x86"
DEBUG        = $(XBUILDFLAGS) /p:OutputPath="$(PATH_DEBUG)" /p:Configuration=Debug
RELEASE      = $(XBUILDFLAGS) /p:OutputPath="$(PATH_RELEASE)" /p:Configuration=Release

## OPTIONS ################

SILENT = @

## COMMANDS ###############
MONO   = mono
XBUILD = xbuild
REMOVE = rm -R -f

## COLORS #################

RESET       = tput sgr0
#
BLACK       = tput setaf 0
BLACK_BG    = tput setab 0
DARKGREY    = tput setaf 0; tput bold
RED         = tput setaf 1
RED_BG      = tput setab 1
LIGHTRED    = tput setaf 1; tput bold
GREEN       = tput setaf 2
GREEN_BG    = tput setab 2
LIME        = tput setaf 2; tput bold
BROWN       = tput setaf 3
BROWN_BG    = tput setab 3
YELLOW      = tput setaf 3; tput bold
BLUE        = tput setaf 4
BLUE_BG     = tput setab 4
BRIGHTBLUE  = tput setaf 4; tput bold
PURPLE      = tput setaf 5
PURPLE_BG   = tput setab 5
PINK        = tput setaf 5; tput bold
CYAN        = tput setaf 6
CYAN_BG     = tput setab 6
BRIGHTCYAN  = tput setaf 6; tput bold
LIGHTGREY   = tput setaf 7
LIGHTGREYBG = tput setab 7
WHITE       = tput setaf 7; tput bold

## NAMED-HELPER ###########

MENU  = $(CYAN)
ITEM  = $(BRIGHTCYAN)
DONE  = $(CYAN)
CHECK = $(GREEN)
ERROR = $(RED)

## #################################### ##
##  R U L E S                           ##
## #################################### ##

all: clean debug release

run:
	$(SILENT) $(MAKE) clean
	$(SILENT) $(MAKE) release
	$(SILENT) $(MONO) $(BIN_RELEASE)

debug:
	$(SILENT) $(XBUILD) $(DEBUG) $(SOLUTION)

release:
	$(SILENT) $(XBUILD) $(RELEASE) $(SOLUTION)

cleanall:
	$(SILENT) $(MAKE) clean
	$(SILENT) $(REMOVE) $(PATH_DEBUG)
	$(SILENT) $(REMOVE) $(PATH_RELEASE)

clean:
	$(SILENT) $(XBUILD) $(DEBUG) $(SOLUTION) /t:Clean
	$(SILENT) $(XBUILD) $(RELEASE) $(SOLUTION) /t:Clean