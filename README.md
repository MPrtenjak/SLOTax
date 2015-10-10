# SLOTax

##Davčno potrjevanje računov /  Fiscal verification of invoices 

V letu 2016 bo Slovenija uvedla davčne blagajne in to je .NET DLL, s vso kodo potrebno za komunikacijo s FURS-om.
Tehnična dokumentacija je dosegljiva na spletu: http://datoteke.durs.gov.si/dpr/index.html 

In year 2016 Slovenia will implement fiskal verification of invoices and this project is .NET DLL with all the code needed 
to implement a communication with government servers. Technical specifications required by our government are available 
online: http://datoteke.durs.gov.si/dpr/index.html

##Testni program / Test program

Testno aplikacijo lahko naložite s spleta: https://github.com/MPrtenjak/SLOTax/releases/latest

You can download test program from web: https://github.com/MPrtenjak/SLOTax/releases/latest

![Alt desc](https://raw.githubusercontent.com/MPrtenjak/SLOTax/master/git_resources/screen1.gif)

##Kaj potrebujem za uporabo / What do I need to be able to use test program

Za uporabo vladnih strežnikov potrebujete ustrezno digitalno potrdilo, ki ga lahko zahtevate na spletni strani: http://www.datoteke.fu.gov.si/dpr/index.html

To be able to use government servers you are obligated to use correct digital certificates which can be acquired using this web page: http://www.datoteke.fu.gov.si/dpr/index.html

##Kaj je vključeno v projekt

Ta program bo

1. Vzel vaš xml zahtevek
1. Dodal zahtevano zaglavje
1. Dodal SOAP ovojnico
1. Izračunal zaščitno kodo
1. Ga digitalno podpisal
1. Poslal na FURS strežnik
1. Prejel enkratno identifikacijsko oznako 
1. Izračunal BarCode zaščitno kodo (za QR kodo in kodo 128)
1. Izrisal QR kodo

##What is included in project

This program will:

1. Add required header
1. Add SOAP envelope
1. Calculate protective mark of invoice
1. Digitally sign message
1. Send it to government servers
1. Get back Unique Identification Invoice Mark
1. Calculate BarCode for QR code and Code128
1. Draw QR Code