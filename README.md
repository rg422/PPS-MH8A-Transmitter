# PPS-MH8A-Transmitter
![image](https://user-images.githubusercontent.com/29156386/155590879-0a8db1dc-762b-49c2-bb24-b5917e53d6f1.png)

This project was birthed from the discussion here:
https://scubaboard.com/community/threads/reading-wireless-air-transmitter-using-arduino.601083/page-9

Some scuba computers utilize a [wireless air integration system](https://www.seadragonlife.com/best-wireless-air-integration-dive-computers/) to transmit the air pressure of the tank to the computer of the diver. This data is displayed for the diver to easily monitor, some computers also calculate the air time remaining and surface air consumption rate.

In this project we explore how the various pieces of data are encoded in the data packet transmitted by these devices and how an individual can create their own data packets and transmit them to their computer. In development is a receiver and transmitter based on an arduino that could display all nearby transmitters’ serial numbers and pressure values.

Software:
* Windows Standalone Encoder (Alpha version completed)
* Windows Standalone Decoder (not started)
* Arduino Decoder (not started)
* Arduino Encoder/Transmitter (not started/tbd)

**See the [Wiki](https://github.com/rg422/PPS-MH8A-Transmitter/wiki) for more information regarding recording, decoding, and transmitting the signal yourself using a computer's sound card or dedicated software defined radio.**

This project has been built by Ryan Gedminas and Nick "MadUKDiver" with contributions from the Scuba Board users; tursiops, -JD-, uw, dm9876 and Robbyg, amongst others who participated in the discussion.



