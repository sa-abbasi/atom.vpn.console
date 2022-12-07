# ATOM VPN Console Application


This repo has Atom VPN Console Application which listens commands on WebSockets and executes them through Atom VPN Windows SDK

## How to build the application

1. Clone this repo into your local folder
2. Open Atom.VPN.Console.sln in Visual Studio (2019/2022)
3. Right click on project icon and choose "Build"
Visual Studio will download Nuget packages and build the application
5. Run the application as Administrator for first time and Windows will ask to add the url/port in firewall, click Yes on windows dialog. Or you may manually allow rule entry in firewall for ws://0.0.0.0:8081
6. You may change this url from application setting in visual studio or from app.config file.

Electron app automatically launches this application but for troubleshooting/debugging you may launch this application manually or from visual studio. It listens for commands sent by Electron JS application.


It makes two log files, please check them for exceptions/errors or traces.

Log path can be changed from NLog.config
