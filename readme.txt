== Summary ==

The "Ping-Pong Café Manager" was developed as a tool to help a german table-tennis club (http://ttc-langensteinbach.de) to manage its yearly tournament, the "Ping-Pong Café".
They decided to implement the Swiss-system tournament (see http://en.wikipedia.org/wiki/Swiss-system_tournament) for several reasons:

* Unlike classical K.O. systems, no player drops out due to an early lost game. Everyone plays the same amount of rounds
* Unlike everyone vs everyone systems, you have a clear winner after a limited number of rounds
* Unlike group system (like, say the pre-rounds of FIFA WM), the groups don't feel arbitrary. Everyone plays most of the time in a group with opponents of similar strength

The biggest disadvantage of this system is that it is really hard to understand for any human, and it is almost impossible to calculate correctly the pairings of the next round in real-time by hand. Therefore, I implemented these rules in this tool.

== Why did I publish this project on GitHub? ==
I am aware that this project is very specifically tailored to the needs of the mentioned table-tennis club. As far as we are aware, there is no one in Germany that implements the same set of rules for table-tennis.
The tournament became quite popular locally in the last few years, the regional union became aware of this success. So I wanted to make this public for other table-tennis clubs as an example and potential base for own tournaments.
The project is far older than the upload in GitHub might suggest, so sorry for all the german-only code and commentary.

== Features and Requirements ==

* Based on VB.NET,  .NET 4.0 and WPF
* Developed with Visual Studio 2012
* Tested on Windows 7 and Windows 8 (x86 and x64)
* Unit Tests via Visual Studio Unit Testing (MSTest)
* Implements the swiss-system tournament rules with small deviations and corrections. Allows up to (playercount - 1) rounds.
* Splitted in two parts:
** An application that enables you to register and validate incoming players at the start of a tournament (StartlistenEditor)
** An application to manage the results and pairings of each tournament class seperately (supports multiple parallel instances)
* Natively understands player export xml of the regional table-tennis union (http://ttvbw.click-tt.de)
* Stores all tournament data in this xml file
* Supports printing multiple reports via XPS
* Allows exporting basic tournament data as Excel file (2007 and above) via OpenXML
* Supports playing special rounds as manual "playoff", to mix swiss-system and e.g. K.O. system rounds
* Implements customizable game rules like "sets to win" and "allow tie", and dropping players due to unforeseen circumstances

== Where to start? ==
* If you are interested in the pairing rules of the swiss-system tournament, look for the PPC Manager\Model folder
* If you are interested in printing, look under PPC Manager\Drucken
* If you are interested in the xml file format, look for SpeicherStandSchema.xsd and TournamentPortal.dtd
* If you are interested in the reasoning behind specific implementations, look at the commit log. I tried to document each change there carefully.


Marius Goppelt
MGoppelt@outlook.com
