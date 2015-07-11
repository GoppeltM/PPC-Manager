See below for german text

== Summary ==

The "Ping-Pong Café Manager" was developed as a tool to help a german table-tennis club (http://ttc-langensteinbach.de) to manage its yearly tournament, the "Ping-Pong-Café".
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
External components are managed via Visual Studio package manager.

* Based on VB.NET, .NET 4.0 and WPF
* Developed with Visual Studio 2012
* Tested on Windows 7 and Windows 8 (x86 and x64)
* Setup built by Wix
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
* Implements customizable game rules like "sets to win" and "consider set difference", and dropping players due to unforeseen circumstances

== Where to start? ==
* If you are interested in the pairing rules of the swiss-system tournament, look for the PPC Manager\Model folder
* If you are interested in printing, look under PPC Manager\Output
* If you are interested in the xml file format, look for SpeicherStandSchema.xsd and TournamentPortal.dtd
* If you are interested in the reasoning behind specific implementations, look at the commit log. I tried to document each change there carefully.


Marius Goppelt
MGoppelt@outlook.com


----------------------------------------------------------------------------------------------------------------


== Überblick ==

Der "Ping-Pong Café Manager" wurde im Auftrag eines deutschen Tischtennisvereins entwickelt (http://ttc-langensteinbach.de), um den Ablauf des jährlichen "Ping-Pong-Café" Turniers zu organisieren.
Das Turnier versucht das Schweizer Turnier System (see http://en.wikipedia.org/wiki/Swiss-system_tournament) zu adaptieren, und zwar aus mehreren Gründen:

* Anders wie klassische K.O. Systeme steigt kein Spieler vorzeitig aufgrund eines verlorenen Spiels aus. Jeder spielt die selbe Rundenzahl
* Anders als Turniersysteme in denen jeder gegen jeden spielt, hat man nach einer limitierten Anzahl von Runden einen eindeutigen Gewinner
* Anders wie Gruppenspiele (wie z.B. die Gruppenspiele der FIFA Weltmeisterschaft) unterliegt die Gruppenzusammensetzung nicht nur dem Zufall. Die meiste Spielzeit spielt jeder in Gruppen mit vergleichbarer Gegnerstärke

Der größte Nachteil dieses Systems ist, dass es für Menschen kaum verständlich ist, und innerhalb des engen Zeitrahmens zwischen zwei Runden es fast unmöglich ist die Paarungen von Hand auszurechnen. Daher habe ich die Spielregeln in diesem Programm umgesetzt.

== Warum eine Veröffentlichung dieses Projektes auf GitHub? ==
Mir ist bewusst, dass dieses Projekt sehr speziell auf die Anforderungen des genannten Tischtennisvereins zugeschnitten ist. Soweit wir wissen, gibt es deutschlandweit niemanden der Tischtennis mit genau diesem Regelsatz spielt.
In den letzten paar Jahren wurde das Turnier zunehmend bekannter, und der Landesverband wurde auf diesen Erfolg aufmerksam. Daher möchte ich anderen Tischtennisvereinen die Möglichkeit geben dieses Projekt als Beispiel und Vorlage für eigene Turnierformen zu benutzen.
Das Projekt ist weit älter als es der Upload auf GitHub suggeriert. Es war nie als internationales Projekt gedacht, daher bitte ich zu entschuldigen dass Kommentare und Code nur auf deutsch verfügbar sind.


== Merkmale und Anforderungen ==
Externe Komponenten werden vom Visual Studio Package Manager verwaltet.

* Basiert auf VB.NET, .NET 4.0 und WPF
* Entwickelt mit Visual Studio 2012
* Getestet auf Windows 7 und Windows 8 (x86 und x64)
* Setup auf Basis der Wix Plattform
* Unit Tests via Visual Studio Unit Testing (MSTest)
* Implementiert die Regeln des Schweizer Turniersystems, wenn auch mit leichten Anpassungen und Korrekturen. Erlaubt bis zu (Spielerzahl - 1) Runden
* Aufgeteilt in zwei Bereiche:
** Eine Anwendung die Spieleranmeldungen am Turniertag verwaltet und überprüft (StartlistenEditor)
** Eine Anwendung die separat die Ergebnisse und Paarungen jeder Turnierklasse verwaltet (unterstützt mehrere Parallelinstanzen)
* Versteht und arbeitet nativ im XML Export Schema der Baden-Württembergischen Tischtennisverbände (http://ttvbw.click-tt.de)
* Alle Turnierdaten sind gesammelt in einer XML Datei
* Unterstützt diverse Druckaufträge via XPS
* Erlaubt Datenexporte von Turnierdaten zu Microsoft Excel (2007 oder höher) via OpenXML
* Erlaubt einzelne Runden speziell als "Playoff" zu spielen, um Schweizer-System und z.B. K.O. System Runden zu mischen
* Erlaubt diverse Anpassungen des Turniermodus, wie z.B. Anpassung der Gewinnsätze, Berücksichtigung des Satzverhältnisses, und 

== Wo anfangen? ==
* Wenn Sie an den Paarungsregeln des Schweizer Systems interessiert sind, schauen Sie im Ordner unter "PPC Manager\Model"
* Wenn Sie Interesse am Drucken haben, schauen Sie unter "PPC Manager\Output"
* Wenn Sie Interesse am verwendeten Datenformat haben, werfen Sie einen Blick auf SpeicherStandSchema.xsd und TournamentPortal.dtd
* Wenn Sie an spezifischen Implementierungsdetails und deren Hintergründen interessiert sind, schauen Sie ins Commit Log. Ich habe versucht dort alle Änderungen so sorgfältig es geht zu dokumentieren.

Marius Goppelt
MGoppelt@outlook.com

