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

### Download
Laden Sie die aktuellste Version (`SchoolDisplay_x.y.z.zip`) auf der [Release-Seite](https://github.com/ytausch/SchoolDisplay/releases) herunter und entpacken Sie das ZIP-Archiv. Unter Windows 10 können Sie letzteres beispielsweise durch Anwählen der Option "Alle extrahieren" im Rechtsklickmenü des Windows Explorers tun.

### Kein Installationsprogramm
SchoolDisplay wird nicht mit einem Installationsprogramm ausgeliefert - daher sind auch keine Administratorrechte notwendig. Wenn Sie das Archiv an einem wiederauffindbaren Ort (zum Beispiel auf dem Desktop) entpackt haben, sind Sie bereits bereit für den nächsten Schritt.

## Konfiguration
Bevor Sie SchoolDisplay verwenden können, müssen Sie zunächst die mitgelieferte Konfigurationsdatei `SchoolDisplay.exe.config` anpassen. Öffnen Sie dafür die Datei mit einem Texteditor - sie können beispielsweise das standardmäßig installierte Windows-Programm "Editor" verwenden.

Relevant für Ihre Konfiguration sind die Zeilen der Konfigurationsdatei, die nach dem folgenden Schema aufgebaut sind:

    <add key="BEZEICHNUNG" value="WERT" />

Dabei steht in jeder Zeile `BEZEICHNUNG`für die Bezeichnung und `WERT` für den Wert eines Konfigurationsparameters.

**Bitte passen Sie lediglich die WERTE der Konfigurationsparameter innerhalb der Anführungszeichen an und lassen Sie den restlichen Inhalt der Datei unangetastet.** Andernfalls kann die Konfigurationsdatei möglicherweise nicht mehr gelesen werden.

### Konfigurationsparameter
| Bezeichnung | Erläuterung | Standardwert |
| --- | --- | --- |
| `PdfDirectoryPath`* | Das Verzeichnis, aus dem alle angezieigten PDF-Dokumente bezogen werden. <br> Sie können an dieser Stelle auch sogenannte UNC-Pfade im Format `\\Server\Freigabe` angeben, um eine Windows-Dateifreigabe anzugeben.  | (keiner)     |
| `ScrollSpeed` | Die Scrollgeschwindigkeit - je höher dieser Wert, desto schneller werden die Dokumente von oben nach unten durchgeblättert. Ein Wert zwischen 10 und 40 wird empfohlen, abhängig von Ihrer Bildschirmauflösung und -größe. | 25 |
| `PauseTime` | Zeit in Millisekunden, die gewartet wird, bevor bei einem fertig angezeigten Dokument zur nächsten Datei gesprungen wird. | 2500 |
| `DisplayAlwaysOn`* | Soll das Display dauerhaft eingeschaltet werden? (`true` : aktiviert, `false` : deaktiviert) <br> Wird diese Einstellung mit `true`aktiviert, überschreibt dies die nachfolgenden Einstellungen zur Zeitsteuerung. | false |
| `DisplayStartTime`* | Uhrzeit, zu der das Display eingeschaltet werden soll (hh:mm) | 07:00 |
| `DisplayStopTime`* | Uhrzeit, zu der das Display ausgeschaltet werden soll (hh:mm) | 18:00 |
| `ActiveOnWeekends` | Soll das Display Samstags und Sonntags auch eingeschaltet sein? (true/false wie oben) | true |
| `ErrorDisplayDelay` | Verzögerung in Millisekunden bei Anzeige einer PDF-Fehlermeldung. | 5000 |
| `EmptyPollingDelay` | Wie oft soll bei einem leeren PDF-Verzeichnis auf neue Inhalte geprüft werden (in Millisekunden)? <br> *Hinweis: SchoolDisplay erkennt zusätzlich Dateiänderungen in Echtzeit - diese Einstellung greift nur, wenn die Echtzeitprüfung fehlschlägt.* | 30000 |

Wir empfehlen mindestens die Anpassung der mit (\*) markierten Einstellungsmöglichkeiten.
