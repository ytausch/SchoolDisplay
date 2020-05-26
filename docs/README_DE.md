# SchoolDisplay

[![CI](https://github.com/ytausch/SchoolDisplay/workflows/CI/badge.svg)](https://github.com/ytausch/SchoolDisplay/actions)

> Ein leicht zu bedienendes digitales schwarzes Brett, speziell für den Einsatz in Schulen entwickelt.

![Demo-Bild](assets/banner.png)

## Funktionsbeschreibung

### Was ist "SchoolDisplay"?

Mit SchoolDisplay können tagesaktuelle Informationen schnell und effizient einem großen Publikum zur Verfügung gestellt werden - zum Beispiel welche Unterrichtsstunden an einer Schule nicht wie geplant stattfinden können ("Vertetungsplan"). Grundsätzlich sind Inhalte jeder Art denkbar, solange sie im PDF-Format vorliegen.

SchoolDisplay ist eine Windows-Anwendung, die nacheinander und wiederholend alle PDF-Dokumente eines angegebenen Verzeichnisses im Vollbildmodus anzeigt und jeweils langsam von oben nach unten durchblättert. Änderungen an den Inhalten werden in Echtzeit übernommen.

### Was benötige ich, um SchoolDisplay einzusetzen?

* Einen öffentlich angebrachten Bildschirm (oder mehrere)
* Einen Windows-PC (Systemvoraussetzungen: siehe unten), der mit der Anzeige verbunden ist
* Eine Möglichkeit, aus der Ferne auf das konfigurierbare PDF-Verzeichnis der Anwendung zuzugreifen, zum Beispiel unter Verwendung eines Windows-Dateiservers. Sie können hierfür einen dezidierten Server verwenden oder eine [Netzwerkfreigabe direkt auf dem Display-PC einrichten](https://support.microsoft.com/de-de/help/4092694/windows-10-file-sharing-over-a-network).


## Installation
### Systemvoraussetzungen
* Betriebssystem: **Windows 7 oder höher** (empfohlen: Windows 10)
* [**.NET Framework 4.8**](https://dotnet.microsoft.com/download/dotnet-framework/thank-you/net48-web-installer): Es handelt sich hierbei um eine Laufzeitumgebung, die zwingend benötigte Bibliotheken und Systemschnittstellen bereitstellt. Das .NET Framework in der Version 4.8 ist in manchen Windows 10-Versionen [bereits standardmäßig enthalten](https://docs.microsoft.com/de-de/dotnet/framework/get-started/system-requirements#installation-requirements).

