sudo apt-get update
sudo apt-get upgrade -y


http://blog.brettalton.com/2010/04/28/installing-guest-additions-in-virtualbox-for-an-ubuntu-server-guest/

sudo apt-get install -y dkms build-essential linux-headers-generic linux-headers-$(uname -r)

sudo mount /dev/cdrom /media/cdrom

Installing the Window System drivers ...fail!
(Could not find the X.Org or XFree86 Window System.)

sudo apt-get install -y xserver-xorg xserver-xorg-core


copy/paste

vim --version | grep xterm_clipboard
sudo apt-get install -y vim-nox vim-gtk
