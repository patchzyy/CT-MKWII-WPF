#!/usr/bin/env bash

# TODO Write down your name and studentnumber of the author(s)
# Markus Persson (1234567) and Rick Games (1000903)

# Global variables
# TODO Define (only) the variables which require global scope


# INSTALL

# TODO complete the implementation of this function
# Make sure to use sudo only if needed
function install_with_apt() {
    # Do NOT remove next line!    
    echo "function install_with_apt"

    package=$1
    # TODO 
        # add apt command to update apt sources list
        # add apt command to install the package
        # add apt command to autoremove packages        

    # TODO if something goes wrong then call function handle_error
}

# TODO complete the implementation of this function
# Make sure to use sudo only if needed
function install_package() {
    # Do NOT remove next line!
    echo "function install_package"

    # TODO read the arguments from $@
        # Make sure NOT to use empty argument values

    # TODO make sure the following dependencies have been installed
        # BuildTools (https://hub.spigotmc.org/jenkins/job/BuildTools/)
        # gdebi (https://manpages.debian.org/buster/gdebi-core/gdebi.1.en.html)
        # wget

    # TODO Install required packages with APT     
        # add apt command to update apt sources list
        # add apt command to install the package        
    
    # TODO General
        # the URLS needed to download installation files must be read automatically from dev.conf 
        # the logic for downloading from a URL and installing the application with the installationfile with the proper installation tool
        # specific actions that need to be taken for a specific application during this process should be handled in a separate if-else or switch statement
        # every intermediate steps need to be handeld carefully. error handeling should be dealt with using handle_error() and/or rolleback()
        # if a file is downloaded but canNOT be installed, a rollback is needed to be able to start from scratch
        # create a specific installation folder for the current package
        # make sure to provide the user with sufficient permissions to this folder
        # make sure to handle every intermediate mistake and rollback if something goes wrong like permission erros and unreachble URL etc.

    # TODO application specific logic
    # based on the name of the application additional steps might be needed

        # TODO SPIGOTSERVER 
        # Copy spigotstart.sh to ${HOME}/apps/spigotserver and provide the user with execute permission
        # spigotserver will be stored into ${HOME}/apps/spigotserver
        
        # TODO MINECRAFT 
        # Download minecraft.deb and install it with gdebi
        
    # TODO if something goes wrong then call function handle_error

}


# CONFIGURATION

# TODO complete the implementation of this function
# Make sure to use sudo only if needed
function configure_spigotserver() {
    # Do NOT remove next line!
    echo "function configure_spigotserver"

    # TODO Configure Firewall
        # make sure ufw has been installed    

    # TODO allow SSH port with ufw allow OpenSSH
        # use ufw to allow the port that is specified in dev.conf for the Spigot server to accept connections
        # make sure ufw has been enabled

    # TODO configure spigotserver to run creative gamemode instead of survival 
        # this can be done by running the sed command on the (automatically generated) file server.properties 
        # (https://minecraft.fandom.com/wiki/Server.properties)
        # with the argument 's/\(gamemode=\)survival/\1creative/'

    # TODO restart the spigot service

    # TODO if something goes wrong then call function handle_error

}

# TODO complete the implementation of this function
# Make sure to use sudo only if needed
function create_spigotservice() {
    # Do NOT remove next line!
    echo "function create_spigotservice"
    
    # TODO copy spigot.service to /etc/systemd/system/spigot.service

    # TODO reload the service daemon (systemctl daemon-reload)
    # TODO enable the service using systemctl

    # TODO if something goes wrong then call function handle_error

}

# ERROR HANDLING

# TODO complete the implementation of this function
function handle_error() {
    # Do NOT remove next line!
    echo "function handle_error"

    # TODO read the arguments from $@
        # Make sure NOT to use empty argument values

    # TODO print a specific error message
    # TODO exit this function with an integer value!=0

}

# TODO complete the implementation of this function
# Make sure to use sudo only if needed
function rollback_spigotserver() {
    # Do NOT remove next line!
    echo "function rollback_minecraft"

    # TODO if something goes wrong then call function handle_error

}

# TODO complete the implementation of this function
# Make sure to use sudo only if needed
function rollback_spigotserver {
    # Do NOT remove next line!
    echo "function rollback_spigotserver"

    # TODO if something goes wrong then call function handle_error

}


# UNINSTALL

# TODO complete the implementation of this function
# Make sure to use sudo only if needed
function uninstall_minecraft {
    # Do NOT remove next line!
    echo "function uninstall_minecraft"  

    # TODO remove the directory containing minecraft 

    # TODO if something goes wrong then call function handle_error

}

# TODO complete the implementation of this function
# Make sure to use sudo only if needed
function uninstall_spigotserver {
    # Do NOT remove next line!
    echo "uninstall_spigotserver"  
    
    # TODO remove the directory containing spigotserver 

    # TODO create a service by calling the function create_spigotservice

    # TODO if something goes wrong then call function handle_error

}

# TODO complete the implementation of this function
# Make sure to use sudo only if needed
function uninstall_spigotservice {
    # Do NOT remove next line!
    echo "uninstall_spigotservice"

    # TODO disable the spigotservice with systemctl disable
    # TODO delete /etc/systemd/system/spigot.service

    # TODO if something goes wrong then call function handle_error
    
}

# TODO complete the implementation of this function
# Make sure to use sudo only if needed
function remove() {
    # Do NOT remove next line!
    echo "function remove"

    # TODO Remove all packages and dependencies

    # TODO if something goes wrong then call function handle_error

}


# TEST

# TODO complete the implementation of this function
function test_minecraft() {
    # Do NOT remove next line!
    echo "function test_minecraft"

    # TODO Start minecraft 

    # TODO Check if minecraft is working correctly
        # e.g. by checking the logfile

    # TODO Stop minecraft after testing
        # use the kill signal only if minecraft canNOT be stopped normally

}

function test_spigotserver() {
    # Do NOT remove next line!
    echo "function test_spigotserver"    

    # TODO Start the spigotserver

    # TODO Check if spigotserver is working correctly
        # e.g. by checking if the API responds
        # if you need curl or aNOTher tool, you have to install it first

    # TODO Stop the spigotserver after testing
        # use the kill signal only if the spigotserver canNOT be stopped normally

}

function setup() {
    # Do NOT remove next line!
    echo "function setup"    

    # TODO Install required packages with APT     
 
}

function main() {
    # Do NOT remove next line!
    echo "function main"

    # TODO read the arguments from $@
        # make sure NOT to use empty argument values

    # TODO use a switch statement to execute

        # setup that creates the installation directory and installs all required dependencies           

        # remove that removes installation directory and uninstalls all required dependencies (even if they were already installed)

        # minecraft with an argument that specifies the one of the following actions
            # installation of minecraft client
            # test
            # uninstall of minecraft client

        # spigot with an argument that specifies the one of the following actions
            # installation of both spigot server and service
            # test
            # uninstall of both spigot server and service

}

main "$@"
