*/CRE -- CRE Quality Gate Conversion Indicator *** DO NOT REMOVE ***
?SEARCH =S00U05
?SEARCH =S01U00
?SEARCH =S69U38
?SEARCH =S71U02
?SEARCH =S71U03
?SEARCH =S71U04
?SEARCH =S71U05
*/\new
*/*****************************************************************************
*/
*/ A. ALLGEMEINES:
*/
*/    Projekt:     Daimler AG
*/    Arb-Gebiet:  %%PRODUKT%%
*/    Programm:    %%SERVER%%  Server zum Pflegen Tabelle %%TBL%%
*/
*/    Autor:       %%AUTOR%%
*/    Datum:       %%DATUM%%
*/
*/    Aenderungen:
*/
*/    Marke  Datum     Autor     Text
*/    #000#  %%DATUM%% %%AUTOR%% %%ANF%%: Pflege %%TBL%%
*/
*/
*/ B. AUFGABE:
*/
*/    Dieser Server stellt folgende Transaktionen zur Verfuegung:
*/
*/    TRCOD   Aktion
*/    ----------------------------------------------------------------------
*/    %%TRANSCODE-LESEN%%      Liste von Eintraegen in Tabelle %%TBL%% lesen (komplett)
*/    %%TRANSCODE-SCHREIBEN%%      Mehrfach-Datenpflege
*/
*/
*/ C. SCHNITTSTELLE:
*/
*/    Die Messages unterscheiden sich bei den Transaktionen (nicht).
*/
*/    TR-Code: %%TRANSCODE-LESEN%%
*/    Eingangs-Message: RPC-DIALOG-HDR, %%SERVER%%-MSG-1
*/    Ausgangs-Message: RPC-DIALOG-HDR, %%SERVER%%-MSG-1
*/
*/    TR-Code: %%TRANSCODE-SCHREIBEN%%
*/    Eingangs-Message: RPC-DIALOG-HDR, %%SERVER%%-MSG-2
*/    Ausgangs-Message: RPC-DIALOG-HDR, %%SERVER%%-MSG-2
*/
*/
*/ D. DATEI-/TABELLENUEBERSICHT:
*/                                                        SIUD
*/    %%TBL%%  :  %%Tabellen-Bezeichnung%%                xxxx
*/
*/
*/ E. AUFGERUFENE UNTERPROGRAMME:
*/
*/    S00U05  :  Multi-Reply in Anwendungsserver
*/    S01U00  :  Transaktions-Logging
*/    S71U05  :  Meldungsausgabe
*/
*/
*/ F. PROGRAMMABLAUF:
*/
*/##X
*/T   Trcod: Allgemein
*/    ----------------
*/A   Der Server prueft zunaechst grundsaetzlich fuer alle Transaktionen       
*/    mittels der folgenden Copy-Strecke in der Section B100-READ-MSG-IN,
*/    ob ein Multi-Reply-faehiger Request gesendet wurde:
*/       COPY INIT-MULTI-REPLY              OF "=S00CMR"
*/       REPLACING ==<DUMMY-REQUEST>==      BY ==%%SERVER%%-REQ-0==.
*/E
*/A   Ist dies der Fall, so wird
*/    - der MULTI-REPLY-HDR-IN in einer lokalen Variable WS-MULTI-REPLY-HDR
*/      gesichert,
*/    - der RPC-DIALOG-HDR-IN in die Dummy-Request ab dem ersten Offset ver-
*/      schoben,
*/    - die Multi-Reply-Schnittstelle initialisiert.
*/E
*/T   Trcod %%TRANSCODE-LESEN%%: Liste von Eintraegen in Tabelle %%TBL%% lesen (komplett)
*/    -----------------------------------------------------------------
*/A   Es werden alle Eintraege mit allen Attributen in Relation %%TBL%%
*/    gelesen und in die Message %%SERVER%%-MSG-1 des Ausgangstelegramms
*/    uebernommen.
*/E
*/A   Zuerst wird geprueft, ob ein Multi-Reply-faehiger Request gesendet wurde.
*/    Ist dies nicht der Fall, wird:
*/    - eine entsprechende EMS-Fehlermeldung gelesen
*/    - die EMS-Meldung in den RPC-DIALOG-HDR uebernommen
*/    - das Feld TRANS-STEUERUNG des RPC-DIALOG-HDR mit "AB" (=Abort) besetzt
*/    - das Feld RPL-MSG-ID des RPC-DIALOG-HDR mit "LEER" besetzt
*/    - die Verarbeitung mit einer leeren Reply beendet.
*/E
*/A   Anschliessend wird der Cursor zur Datenermittlung goeeffnet. Es werden   
*/    alle ermittelten Daten in einer Schleifenverarbeitung in die Reply
*/    uebernommen.
*/E
*/A   Falls in der Relation %%TBL%% kein entsprechender Eintrag gefunden
*/    wird, wird
*/    - eine entsprechende EMS-Warnungsmeldung gelesen
*/    - die EMS-Meldung in den RPC-DIALOG-HDR uebernommen
*/    - das Feld TRANS-STEUERUNG des RPC-DIALOG-HDR mit "AB" (=Abort) besetzt
*/    - das Feld RPL-MSG-ID des RPC-DIALOG-HDR mit "LEER" besetzt
*/    - die Verarbeitung mit einer leeren Reply beendet.
*/E
*/A   Sind mehr Daten vorhanden, als in eine Reply passen, so wird in der
*/    Schleifenverarbeitung von nun an jede komplett gefuellte Reply mittels 
*/    der folgenden Copy-Strecke direkt an den TCP-IP-Distributor gesendet:
*/    COPY SENDEN-MIT-MULTI-REPLY        OF "=S00CMR"
*/         REPLACING ==<MSG-ID>==        BY ==MSG-3 OF RPL-MSG-ID==
*/                 , ==<REPLY>==         BY ==%%SERVER%%-RPL-1==.
*/E
*/A   Falls das Senden ueber die Multi-Reply-Schnittstelle mit einem Fehler
*/    ("SONSTIGER-FEHLER") quittiert wird, so wird:
*/    - eine entsprechende EMS-Fehlermeldung gelesen
*/    - die EMS-Meldung in den RPC-DIALOG-HDR uebernommen
*/    - das Feld TRANS-STEUERUNG des RPC-DIALOG-HDR mit "AB" (=Abort) besetzt
*/    - das Feld RPL-MSG-ID des RPC-DIALOG-HDR mit "LEER" besetzt
*/    - die Verarbeitung mit einer leeren Reply beendet.
*/E
*/A   Wurden alle Daten uebertragen, wird der TCP-IP-Distributor mittels der
*/    folgenden Copy-Strecke wieder geschlossen:
*/       COPY CLOSE-DISTRIBUTOR             OF "=S00CMR".
*/E
*/A   Falls das Schliessen des TCP-IP-Distributors mit einem Fehler
*/    ("SONSTIGER-FEHLER") quittiert wird, so wird:
*/    - eine entsprechende EMS-Fehlermeldung gelesen
*/    - die EMS-Meldung in den RPC-DIALOG-HDR uebernommen
*/    - das Feld TRANS-STEUERUNG des RPC-DIALOG-HDR mit "AB" (=Abort) besetzt
*/    - das Feld RPL-MSG-ID des RPC-DIALOG-HDR mit "LEER" besetzt
*/    - die Verarbeitung mit einer leeren Reply beendet.
*/E
*/A   Tritt waehrend der Verarbeitung ein anderer Fehler auf, wird
*/    - eine entsprechende EMS-Warnungsmeldung gelesen
*/    - die EMS-Meldung in den RPC-DIALOG-HDR uebernommen
*/    - das Feld TRANS-STEUERUNG des RPC-DIALOG-HDR mit "AB" (=Abort) besetzt
*/    - das Feld RPL-MSG-ID des RPC-DIALOG-HDR mit "LEER" besetzt
*/    - die Verarbeitung mit einer leeren Reply beendet.
*/E
*/A   Bei erfolgreicher Verarbeitung, wird
*/    - eine entsprechende EMS-Erfolgsmeldung gelesen
*/    - die EMS-Meldung in den RPC-DIALOG-HDR uebernommen
*/    - das Feld TRANS-STEUERUNG des RPC-DIALOG-HDR mit "OK" besetzt
*/    - das Feld RPL-MSG-ID des RPC-DIALOG-HDR mit "M3" besetzt
*/    - die Verarbeitung mit RPC-DIALOG-HDR und %%SERVER%%-MSG-1 als Reply
*/      beendet.
*/E
*/
*/T   Trcod %%TRANSCODE-SCHREIBEN%%: Mehrfach-Datenpflege 'Speichern'
*/    ------------------------------------------
*/A   Es werden alle Eintraege aus der Request-Tabelle in der Relation %%TBL%%
*/    gepflegt und in die Message %%SERVER%%-MSG-2 des Ausgangstelegramms
*/    uebernommen.
*/E
*/A   Die Verarbeitung in dieser Transaktion besteht aus einer Schleife
*/    ueber alle Zeilen der Request-Tabelle. Fuer jede Zeile gilt:
*/    - Ist das Feld BEARBEITUNGS-KZ leer, so tue nichts.
*/    - Enthaelt das Feld BEARBEITUNGS-KZ einen undefinierten Wert, dann       h
*/      Abbruch mit Fehler.
*/    - Enthaelt das Feld BEARBEITUNGS-KZ einen der definierten Werte ANLEGEN,
*/      LOESCHEN oder AENDERN, so wird fuer den Datensatz in dieser Zeile die
*/      entsprechende Verarbeitung aufgerufen.
*/E
*/A   Tritt waehrend der Verarbeitung ein Fehler auf, wird
*/    - eine entsprechende EMS-Warnungsmeldung gelesen
*/    - die EMS-Meldung in den RPC-DIALOG-HDR uebernommen
*/    - das Feld TRANS-STEUERUNG des RPC-DIALOG-HDR mit "AB" (=Abort) besetzt
*/    - die Verarbeitung mit RPC-DIALOG-HDR als Reply beendet.
*/E
*/A   Bei erfolgreicher Verarbeitung wird
*/    - eine entsprechende EMS-Erfolgsmeldung gelesen
*/    - die EMS-Meldung in den RPC-DIALOG-HDR uebernommen
*/    - das Feld TRANS-STEUERUNG des RPC-DIALOG-HDR mit "OK" besetzt
*/    - das Feld RPL-MSG-ID des RPC-DIALOG-HDR mit "M5" besetzt
*/    - die Verarbeitung mit RPC-DIALOG-HDR und %%SERVER%%-MSG-2 als Reply
*/      beendet.
*/E
*/##Y
*/
*/******************************************************************************
/
*-------------------------------------------------------------------------------
 IDENTIFICATION DIVISION.
*-------------------------------------------------------------------------------

    PROGRAM-ID.    %%SERVER%%.
    AUTHOR.        ABAT+ GMBH.
    INSTALLATION.  Daimler.
    DATE-WRITTEN.  %%DATUM%%
    DATE-COMPILED. %%DATUM%%

*-------------------------------------------------------------------------------
 ENVIRONMENT DIVISION.
*-------------------------------------------------------------------------------

 CONFIGURATION SECTION.
    SOURCE-COMPUTER. TANDEM-VLX.
    OBJECT-COMPUTER. TANDEM-VLX.

    SPECIAL-NAMES.
    SWITCH-1 ON STATUS IS TRANSACTION-LOGGING.

 INPUT-OUTPUT SECTION.
    FILE-CONTROL.

 COPY MSG-IO-SEL                       OF "=W999IO".

 COPY RECEIVE-CONTROL-PARAGRAPH        OF "=S00C0001".

/
*-------------------------------------------------------------------------------
 DATA DIVISION.
*-------------------------------------------------------------------------------

 FILE SECTION.

 COPY MSG-IO-FD                        OF "=W999IO".

 WORKING-STORAGE SECTION.

 01 %%SERVER%%-CRE-QUALITY-GATE-STR         PIC X(20) EXTERNAL.
 01 S00U05-CRE-QUALITY-GATE-STR         PIC X(20) EXTERNAL.
 01 S01U00-CRE-QUALITY-GATE-STR         PIC X(20) EXTERNAL.
 01 S69U38-CRE-QUALITY-GATE-STR         PIC X(20) EXTERNAL.
 01 S71U02-CRE-QUALITY-GATE-STR         PIC X(20) EXTERNAL.
 01 S71U03-CRE-QUALITY-GATE-STR         PIC X(20) EXTERNAL.
 01 S71U04-CRE-QUALITY-GATE-STR         PIC X(20) EXTERNAL.
 01 S71U05-CRE-QUALITY-GATE-STR         PIC X(20) EXTERNAL.

 01 EXT-EMS-MELD-USER                   PIC X(12) EXTERNAL.

 01 WS-MESSAGE-LEN                      PIC 9(5) COMP.

*                                      Standardcopys
 COPY MELDUNGSVARIABLEN                OF "=S00CWS02".

 COPY TIMESTAMP-VARIABLEN              OF "=S00CTMST".

 COPY EMS-MELD-LNK                     OF "=S71CMELD"
 REPLACING ==01 EMS-MELD-LNK==         BY ==01 EMS-MELD-LNK EXTERNAL==.

 EXTENDED-STORAGE SECTION.

 01 WS-GPC-ERROR                       NATIVE-2.


 COPY WS-IN-BUFFER                     OF "=W999IO"
 REPLACING ==).==                      BY ==) EXTERNAL.==.

 COPY RPC-DIALOG-HDR                   OF "=S00CHDR"
 REPLACING ==01 RPC-DIALOG-HDR==
        BY ==01 RPC-DIALOG-HDR-IN REDEFINES WS-IN-BUFFER==
           ==01 RPC-DIALOG-HDR==
        BY ==01 RPC-DIALOG-HDR-IN==.

 COPY %%SERVER%%-REQ-RPL-0                 OF "=%%SERVER%%"
 REPLACING ==01 %%SERVER%%-REQ-RPL-0==
        BY ==01 %%SERVER%%-REQ-0 REDEFINES WS-IN-BUFFER==
           ==%%SERVER%%-REQ-RPL-0==
        BY ==%%SERVER%%-REQ-0==.

 COPY MULTI-REPLY-HDR                  OF "=S00CHDR"
 REPLACING ==01 MULTI-REPLY-HDR==
        BY ==01 MULTI-REPLY-HDR-IN REDEFINES WS-IN-BUFFER==
           ==01 MULTI-REPLY-HDR==
        BY ==01 MULTI-REPLY-HDR-IN==.

 COPY RPC-DIALOG-MR-HDR                OF "=S00CHDR"
 REPLACING ==01 RPC-DIALOG-MR-HDR==
        BY ==01 RPC-DIALOG-MR-HDR-IN REDEFINES WS-IN-BUFFER==
           ==01 RPC-DIALOG-MR-HDR==
        BY ==01 RPC-DIALOG-MR-HDR-IN==.

 COPY %%SERVER%%-REQ-RPL-1                 OF "=%%SERVER%%"
 REPLACING ==01 %%SERVER%%-REQ-RPL-1==
        BY ==01 %%SERVER%%-REQ-1 REDEFINES WS-IN-BUFFER==
           ==%%SERVER%%-REQ-RPL-1==
        BY ==%%SERVER%%-REQ-1==.

 COPY %%SERVER%%-REQ-RPL-2                 OF "=%%SERVER%%"
 REPLACING ==01 %%SERVER%%-REQ-RPL-2==
        BY ==01 %%SERVER%%-REQ-2 REDEFINES WS-IN-BUFFER==
           ==%%SERVER%%-REQ-RPL-2==
        BY ==%%SERVER%%-REQ-2==.

 COPY WS-OUT-BUFFER                     OF "=W999IO"
 REPLACING ==).==                       BY ==) EXTERNAL.==.

 COPY RPC-DIALOG-HDR                   OF "=S00CHDR"
 REPLACING ==01 RPC-DIALOG-HDR==
        BY ==01 RPC-DIALOG-HDR-OUT REDEFINES WS-OUT-BUFFER==
           ==01 RPC-DIALOG-HDR==
        BY ==01 RPC-DIALOG-HDR-OUT==.

 COPY %%SERVER%%-REQ-RPL-0                 OF "=%%SERVER%%"
 REPLACING ==01 %%SERVER%%-REQ-RPL-0==
        BY ==01 %%SERVER%%-RPL-0 REDEFINES WS-OUT-BUFFER==
           ==%%SERVER%%-REQ-RPL-0==
        BY ==%%SERVER%%-RPL-0==.

 COPY MULTI-REPLY-HDR                  OF "=S00CHDR"
 REPLACING ==01 MULTI-REPLY-HDR==
        BY ==01 MULTI-REPLY-HDR-OUT REDEFINES WS-OUT-BUFFER==
           ==01 MULTI-REPLY-HDR==
        BY ==01 MULTI-REPLY-HDR-OUT==.

 COPY RPC-DIALOG-MR-HDR                OF "=S00CHDR"
 REPLACING ==01 RPC-DIALOG-MR-HDR==
        BY ==01 RPC-DIALOG-MR-HDR-OUT REDEFINES WS-OUT-BUFFER==
           ==01 RPC-DIALOG-MR-HDR==
        BY ==01 RPC-DIALOG-MR-HDR-OUT==.

 COPY %%SERVER%%-REQ-RPL-1                 OF "=%%SERVER%%"
 REPLACING ==01 %%SERVER%%-REQ-RPL-1==
        BY ==01 %%SERVER%%-RPL-1 REDEFINES WS-OUT-BUFFER==
           ==%%SERVER%%-REQ-RPL-1==
        BY ==%%SERVER%%-RPL-1==.

 COPY %%SERVER%%-REQ-RPL-2                 OF "=%%SERVER%%"
 REPLACING ==01 %%SERVER%%-REQ-RPL-2==
        BY ==01 %%SERVER%%-RPL-2 REDEFINES WS-OUT-BUFFER==
           ==%%SERVER%%-REQ-RPL-2==
        BY ==%%SERVER%%-RPL-2==.

*                                      Standardcopys
 COPY SOURCE-VERSION                   OF "=S00C0001".

 COPY IO-DATEN                         OF "=W999SR01".

 COPY ZUSTAND                          OF "=S00CWS01".

 COPY WSSTD                            OF "=S00CWS01".

 COPY SQL-STATUS                       OF "=S00CWS01".

*                                      fuer Transaktionslogging
 COPY REQ-HDR                          OF "=W999DD".

 COPY SFLR21-REC                       OF "=S01CREC".

*                                      individuelle Definitionen
 01  PROGRAMM-ID                       PIC X(8) VALUE "%%SERVER%%01".

 01 WS.
   02 I                                PIC S9(4) COMP.
   02 J                                PIC S9(4) COMP.
   02 CO-TIMESTAMP-LOW-VALUE           PIC X(23)
                                       VALUE "0001-01-01:00:00:00:000".

*                                      individuelle Copys
 COPY WS-%%SERVER%%                    OF "=%%SERVER%%".

 COPY S00U05-LNK                       OF "=S00U05C".

 COPY WS-MULTI-REPLY-DEFS              OF "=S00U05C".

 COPY WS-MULTI-REPLY-DEFS              OF "=S00CMR".

*                                      Datenstrukturen fuer Meldungen
 COPY EMS-MELD-EXT                     OF "=S71CMELD"
 REPLACING ==01 EMS-MELD-EXT==         BY ==01 EMS-MELD-EXT EXTERNAL==.

 COPY EMS-RPC-HDR-VAR                  OF "=S00CRPCE".

*                                      SQL-Definitionen
 EXEC SQL INCLUDE STRUCTURES ALL VERSION 315 END-EXEC.

 EXEC SQL INCLUDE SQLCA                END-EXEC.

 EXEC SQL INCLUDE SQLSA                END-EXEC.

*                                      SQL-DECLARE-SECTION
 EXEC SQL BEGIN DECLARE SECTION        END-EXEC.

 COPY HV-%%SERVER%%                        OF "=%%SERVER%%".

 EXEC SQL INVOKE =%%TBL%%-TAB AS %%TBL%%-ROW END-EXEC.

 EXEC SQL END DECLARE SECTION          END-EXEC.


*                                      Cursor zum Lesen der Eintraege in
*                                      Tabelle %%TBL%%
 EXEC SQL
         DECLARE   CURSOR_%%TBL%% CURSOR FOR
         SELECT    
%%CURSOR-FELDER%%
         FROM     =%%TBL%%-TAB
         FOR       BROWSE ACCESS
 END-EXEC.
/
*===============================================================================
*===============================================================================
*
*             H A U P T P R O G R A M M
*
*===============================================================================
*===============================================================================

*-------------------------------------------------------------------------------
 PROCEDURE DIVISION.
*-------------------------------------------------------------------------------

 DECLARATIVES.

 COPY MSG-IO-DCL                       OF "=S00CIO".

 END DECLARATIVES.

*/------------------------------------------------------------------------------
 S100-STEUERUNG SECTION.
*-------------------------------------------------------------------------------
*/ Hauptprogramm mit Initialisierung und Verarbeitungsschleife
*/------------------------------------------------------------------------------

*                                      Initialisierung aller Variablen
    PERFORM A100-INIT

*                                      Verarbeiten Eingangstelegramm
    PERFORM E100-VERARBEITUNG
      UNTIL PROGRAMM-ENDE

    PERFORM Z100-ABSCHLUSS
    .
 S100-EXIT.
    STOP RUN.
/
*/------------------------------------------------------------------------------
 A100-INIT SECTION.
*-------------------------------------------------------------------------------
*/ Initialisierung aller Daten; Whenever-Directives zur Fehlerbehandlung
*/------------------------------------------------------------------------------

*   Durch die nachfolgenden WHENEVER DIRECTIVES werden nach SQL-Kommandos
*   Statements zur Fehlerbehandlung erzeugt. (zur Compilezeit)
    EXEC SQL WHENEVER NOT FOUND  PERFORM :U102-NOT-FOUND END-EXEC
    EXEC SQL WHENEVER SQLERROR   PERFORM :U103-ERROR     END-EXEC
    EXEC SQL WHENEVER SQLWARNING PERFORM :U104-WARNING   END-EXEC

    OPEN I-O MSG-IO-FILE, SHARED, SYNCDEPTH 1

    IF  NOT OK OF FILE-STATUS
    THEN
       PERFORM U901-IO-ERROR
       ENTER TAL "PROCESS_STOP_"
           USING OMITTED
               , OMITTED
               , 1
          GIVING WS-GPC-ERROR
    END-IF

*                                      Meldungen initialisieren
    MOVE -1 TO SQLMSG-FILENUM
    .
 A100-EXIT.
    EXIT.
/
*/------------------------------------------------------------------------------
 B100-READ-MSG-IN SECTION.
*-------------------------------------------------------------------------------
*/ Warten auf Message und Lesen der Message, Initialisierung von Daten.
*/------------------------------------------------------------------------------

*                                     Lesen einer Message von $RECEIVE
    INITIALIZE WS-IN-BUFFER.
    INITIALIZE WS-OUT-BUFFER.

    READ MSG-IO-FILE INTO WS-IN-BUFFER

    IF NOT OK OF FILE-STATUS
    THEN
       SET PROGRAMM-ENDE TO TRUE
    ELSE
       SET OK OF ZUSTAND TO TRUE
    END-IF

*                                     Initialisierung der Multi-Schnittstelle
*                                     und Trennen des MULTI-REPLY-HDR und des
*                                     RPC-DIALOG-HDR vom Request
    COPY INIT-MULTI-REPLY              OF "=S00CMR"
    REPLACING ==<DUMMY-REQUEST>==      BY ==%%SERVER%%-REQ-0==.

*                                     Bei Transaktion-Logging Daten vorbesetzen
    COPY TRANSLOG-INIT                 OF "=S01TLOG"
    REPLACING ==PARAM-SA==             BY ==REQ-MSG-ID OF RPC-DIALOG-HDR-IN==,
              ==PARAM-VK==             BY ==TRCOD OF RPC-DIALOG-HDR-IN==.

    IF  TRANSACTION-LOGGING
    THEN
       MOVE PROGRAMM-ID
         TO TRTYP                      OF REQ-HDR
       MOVE TRCOD                      OF RPC-DIALOG-HDR-IN
         TO TRCOD                      OF REQ-HDR
       MOVE FSPRA                      OF RPC-DIALOG-HDR-IN
         TO FSPRA                      OF REQ-HDR
       MOVE SPACES
         TO TRMID                      OF REQ-HDR
       MOVE USER-ID                    OF RPC-DIALOG-HDR-IN
         TO USRID                      OF REQ-HDR
       ACCEPT DATUM OF REQ-HDR FROM DATE
       ACCEPT ZEIT  OF REQ-HDR FROM TIME
    END-IF

*                                     Linkage initialisieren
    MOVE 0                             TO MELDUNGSFEHLER-NR OF EMS-MELD-LNK
    MOVE PROGRAMM-ID                   TO PROGRAMMNAME      OF EMS-MELD-LNK
    MOVE TRCOD OF RPC-DIALOG-HDR-IN    TO TRCOD             OF EMS-MELD-LNK
    MOVE FSPRA OF RPC-DIALOG-HDR-IN    TO FSPRA             OF EMS-MELD-LNK
    MOVE SPACES                        TO PARAM-TABELLE     OF EMS-MELD-LNK

*                                     RPC-Reply initialisieren
    MOVE RPC-DIALOG-HDR-IN             TO RPC-DIALOG-HDR-OUT
    INITIALIZE SERVER-KONTEXT          OF RPC-DIALOG-HDR-OUT
    SET TRANS-OK OF TRANS-STEUERUNG    OF RPC-DIALOG-HDR-OUT TO TRUE

*                                     Flags initialisieren
    SET SQL-STATUS-OK TO TRUE

    MOVE USER-ID                       OF RPC-DIALOG-HDR-IN
      TO EXT-EMS-MELD-USER.

 B100-EXIT.
    EXIT.
/
*/------------------------------------------------------------------------------
 E100-VERARBEITUNG SECTION.
*-------------------------------------------------------------------------------
*/ Lesen der Message, bei EOF Programmende,
*/ sonst Transaktionsverzweigung und Reply
*/------------------------------------------------------------------------------

*                                      Message lesen
    PERFORM B100-READ-MSG-IN

    IF  NOT PROGRAMM-ENDE OF ZUSTAND
    THEN
*                                      Message verarbeiten und Reply senden
       PERFORM E200-VERTEILUNG
       PERFORM Y100-WRITE-REPLY
    END-IF
    .
 E100-EXIT.
    EXIT.
/
*/------------------------------------------------------------------------------
 E200-VERTEILUNG SECTION.
*-------------------------------------------------------------------------------
*/ Aufruf der Transaktionen anhand der unterschiedlichen Transcodes
*/------------------------------------------------------------------------------

*                                      Transaktion anhand TRCOD aufrufen
    EVALUATE TRCOD OF RPC-DIALOG-HDR-IN
    WHEN     "%%TRANSCODE-LESEN%%"
       PERFORM E300-LESEN-%%TBL%%-LISTE
    WHEN     "%%TRANSCODE-SCHREIBEN%%"
       PERFORM E400-PFLEGEN-%%TBL%%-LISTE
    WHEN     OTHER
       PERFORM M520-INVALID-FUNCTION
    END-EVALUATE
    .
 E200-EXIT.
    EXIT.
/
*/------------------------------------------------------------------------------
 E300-LESEN-%%TBL%%-LISTE SECTION.
*-------------------------------------------------------------------------------
*/ Lesen Liste von Eintraegen in Tabelle %%TBL%%
*/------------------------------------------------------------------------------

    INITIALIZE WS-%%SERVER%%-INIT
    INITIALIZE %%SERVER%%-MSG-1            OF %%SERVER%%-RPL-1

    MOVE WS-MAX-ANZ-%%TBL%%-KOMPLETT    OF WS-%%SERVER%%
      TO %%TBL%%-TAB-ANZ                OF %%SERVER%%-RPL-1

*                                      Wird kein Multi-Reply-faehiger Request
*                                      gesendet, wird die Verarbeitung
*                                      abgebrochen!
    IF OK OF ZUSTAND
    THEN
       PERFORM E310-AUSWERTUNG-HDR-M-MR
    END-IF

*                                      Lesen Liste
    IF  OK OF ZUSTAND
    THEN
       PERFORM E320-LESEN-%%TBL%%
    END-IF

*                                      RPL-HDR und Reply
*                                      Abhaengigkeit von ZUSTAND belegen
    EVALUATE TRUE
*                                      Verarbeitung ok
    WHEN     OK OF ZUSTAND
       SET MSG-1                       OF RPL-MSG-ID
                                       OF RPC-DIALOG-HDR-OUT TO TRUE
*                                      Meldung 'Daten gelesen'
       MOVE "%%SYS%%0003" TO MELDUNGSCODE OF EMS-MELD-LNK
       PERFORM U090-MELDUNG

*                                      Logischer Fehler mit Abort
*                                      Meldungsausgabe bereits erfolgt
    WHEN     WARNUNG OF ZUSTAND
       SET TRANS-ABORTEN               OF TRANS-STEUERUNG
                                       OF RPC-DIALOG-HDR-OUT TO TRUE

*                                      Harter Abbruch
*                                      Meldungsausgabe bereits erfolgt
    WHEN     DATEI-FEHLER OF ZUSTAND
       SET TRANS-ABORTEN               OF TRANS-STEUERUNG
                                       OF RPC-DIALOG-HDR-OUT TO TRUE

*                                      Fehler, der zu ABORT fuehrt
    WHEN     OTHER
       SET TRANS-ABORTEN               OF TRANS-STEUERUNG
                                       OF RPC-DIALOG-HDR-OUT TO TRUE
       PERFORM U080-MELDUNG-F-ZUSTAND
    END-EVALUATE
    .

 E300-EXIT.
    EXIT.
/
*/-----------------------------------------------------------------------------
 E310-AUSWERTUNG-HDR-M-MR SECTION.
*------------------------------------------------------------------------------
*/ Lesen mit Multi-Reply, d.h. RPC-DIALOG-MR-HDR wird erwartet.
*/-----------------------------------------------------------------------------

*                                      Kein Multi-Reply-faehiger Request,
*                                      Verarbeitung abgebrochen!
    IF HDR-KENNUNG OF WS-MULTI-REPLY-HDR NOT = "MR"
    THEN
       SET DATEI-FEHLER OF ZUSTAND TO TRUE
       MOVE "%%SYS%%0001" TO MELDUNGSCODE OF EMS-MELD-LNK
       PERFORM U090-MELDUNG
    END-IF
    .

 E310-EXIT.
    EXIT.
/
*/------------------------------------------------------------------------------
 E320-LESEN-%%TBL%% SECTION.
*-------------------------------------------------------------------------------
*/ Lesen Liste von Eintraegen in Tabelle %%TBL%%
*/------------------------------------------------------------------------------

    INITIALIZE %%TBL%%-ROW

    PERFORM R200-OPEN-CURSOR-%%TBL%%-GE
    IF  NOT SQL-STATUS-OK
    THEN
       SET DATEI-FEHLER OF ZUSTAND TO TRUE
    END-IF

    IF OK OF ZUSTAND
    THEN
       SET NEIN OF WS-SCHLEIFEN-ENDE-SW TO TRUE

       PERFORM
       UNTIL   JA OF WS-SCHLEIFEN-ENDE-SW
       OR      NOT OK OF ZUSTAND

          PERFORM R300-FETCH-CURSOR-%%TBL%%-GE

          EVALUATE TRUE
          WHEN     SQL-STATUS-OK
             IF WS-MR-ANZ-DS-AKTUELL >= WS-MAX-ANZ-%%TBL%%-KOMPLETT
             THEN
*                                      Sind mehr Daten vorhanden, als in die
*                                      Reply passen, werden die Daten per
*                                      Multi-Reply gesendet
                COPY SENDEN-MIT-MULTI-REPLY OF "=S00CMR"
                     REPLACING ==<MSG-ID>==
                            BY ==MSG-1 OF RPL-MSG-ID==
                             , ==<REPLY>==
                            BY ==%%SERVER%%-RPL-1==.
             END-IF

             IF OK OF ZUSTAND
             THEN
*                                      Verwalten der Gesamt- und aktuellen
*                                      Anzahl Datensaetze
                COPY VERWALT-DS-ZAEHLER OF "=S00CMR".
             END-IF

             IF OK OF ZUSTAND
             THEN
                PERFORM E321-DATEN-IN-REPLY
             END-IF

          WHEN     SQL-STATUS-NOT-FOUND
             SET JA OF WS-SCHLEIFEN-ENDE-SW TO TRUE

             IF WS-MR-ANZ-DS-GESAMT = ZEROES
             THEN
*                                      Keine Eintraege gefunden
                SET WARNUNG OF ZUSTAND TO TRUE
                MOVE "%%SYS%%0002" TO MELDUNGSCODE OF EMS-MELD-LNK
                MOVE DATEI-MIT-FEHLER
                  TO PARAMETERTEXT     OF EMS-MELD-LNK (1)
                PERFORM U090-MELDUNG
             END-IF

          WHEN     OTHER
             SET DATEI-FEHLER OF ZUSTAND TO TRUE
          END-EVALUATE
       END-PERFORM
    END-IF

    PERFORM R400-CLOSE-CURSOR-%%TBL%%-GE
    IF  NOT SQL-STATUS-OK
    THEN
       SET DATEI-FEHLER OF ZUSTAND TO TRUE
    END-IF

*                                      Wurde Multi-Reply benutzt, so wird ohne
*                                      Abfrage des Programm-Zustandes der
*                                      Distributor geschlossen
    COPY CLOSE-DISTRIBUTOR             OF "=S00CMR".
    .

 E320-EXIT.
    EXIT.
/
*/------------------------------------------------------------------------------
 E321-DATEN-IN-REPLY SECTION.
*-------------------------------------------------------------------------------
*/ Uebernahme der Daten in Reply
*/------------------------------------------------------------------------------

    INITIALIZE %%TBL%%-TAB-ZEILE OF %%SERVER%%-RPL-1(WS-MR-ANZ-DS-AKTUELL)

    MOVE WS-MR-ANZ-DS-AKTUELL
      TO %%TBL%%-TAB-ANZ                 OF %%SERVER%%-RPL-1

%%DATEN-IN-REPLY%%
    .

 E321-EXIT.
    EXIT.
/
*/------------------------------------------------------------------------------
 E400-PFLEGEN-%%TBL%%-LISTE SECTION.
*-------------------------------------------------------------------------------
*/ Mehrfach-Datenpflege (DOT-NET) in Tabelle %%TBL%%
*/------------------------------------------------------------------------------

    INITIALIZE WS-%%SERVER%%-INIT

    MOVE %%SERVER%%-MSG-2                OF %%SERVER%%-REQ-2
      TO %%SERVER%%-MSG-2                OF %%SERVER%%-RPL-2

*                                      Mehrfach-Datenpflege
    IF  OK OF ZUSTAND
    THEN
       PERFORM E410-PFLEGEN-%%TBL%%-LISTE
    END-IF

*                                      RPL-HDR und Reply
*                                      Abhaengigkeit von ZUSTAND belegen
    EVALUATE TRUE
*                                      Verarbeitung ok
    WHEN     OK OF ZUSTAND
       SET MSG-2                       OF RPL-MSG-ID
                                       OF RPC-DIALOG-HDR-OUT TO TRUE
*                                      Meldung 'Daten gepflegt'
       MOVE "%%SYS%%0012" TO MELDUNGSCODE OF EMS-MELD-LNK
       PERFORM U090-MELDUNG

*                                      logischer Fehler ohne Abort
*                                      Meldungsausgabe bereits erfolgt
    WHEN     WARNUNG OF ZUSTAND
       SET MSG-2                       OF RPL-MSG-ID
                                       OF RPC-DIALOG-HDR-OUT TO TRUE
*      SET TRANS-ABORTEN               OF TRANS-STEUERUNG
*                                      OF RPC-DIALOG-HDR-OUT TO TRUE

*                                      harter Abbruch
*                                      Meldungsausgabe bereits erfolgt
    WHEN     DATEI-FEHLER OF ZUSTAND
       SET TRANS-ABORTEN               OF TRANS-STEUERUNG
                                       OF RPC-DIALOG-HDR-OUT TO TRUE

*                                      Fehler, der zu ABORT fuehrt
    WHEN     OTHER
       SET TRANS-ABORTEN               OF TRANS-STEUERUNG
                                       OF RPC-DIALOG-HDR-OUT TO TRUE
       PERFORM U080-MELDUNG-F-ZUSTAND
    END-EVALUATE
    .

 E400-EXIT.
    EXIT.
/
*/------------------------------------------------------------------------------
 E410-PFLEGEN-%%TBL%%-LISTE SECTION.
*-------------------------------------------------------------------------------
*/ Mehrfach-Datenpflege in Tabelle %%TBL%%
*/------------------------------------------------------------------------------

*                                      Timestamp zum Schreiben ermitteln
    COPY U000-ERMITTELN-TIMESTAMP      OF "=S00CTMST"
    REPLACING ==<TIMESTAMP-FRACTION>== BY ==FRACTION-3==
              ==<ZIELVARIABLE>==       BY ==WS-LUPD-TIMESTAMP-STRUCT==
                                       .

    MOVE WS-LUPD-TIMESTAMP-STRUCT      OF WS-%%SERVER%%
      TO HV-LUPD-TIMESTAMP             OF HV-%%SERVER%%

*                                      Initialisieren
    INITIALIZE %%TBL%%-ROW

    PERFORM VARYING I FROM 1 BY 1
    UNTIL   I > WS-MAX-ANZ-%%TBL%%-PF
    OR      I > %%TBL%%-TAB-ANZ OF %%TBL%%-TABELLE OF %%SERVER%%-REQ-2
    OR      NOT OK OF ZUSTAND

*                                      Zeilenwerte in Tabelle
       PERFORM E411-DATEN-AUS-REQUEST

       IF OK OF ZUSTAND
       THEN
          EVALUATE TRUE
          WHEN ANLEGEN  OF BEARBEITUNGS-KZ OF %%TBL%%-TABELLE OF %%SERVER%%-REQ-2(I)
             PERFORM E412-ANLEGEN-%%TBL%%

          WHEN AENDERN  OF BEARBEITUNGS-KZ OF %%TBL%%-TABELLE OF %%SERVER%%-REQ-2(I)
             PERFORM E413-AENDERN-%%TBL%%

          WHEN LOESCHEN OF BEARBEITUNGS-KZ OF %%TBL%%-TABELLE OF %%SERVER%%-REQ-2(I)
             PERFORM E414-LOESCHEN-%%TBL%%

          WHEN     OTHER
             IF BEARBEITUNGS-KZ OF %%TBL%%-TABELLE OF %%SERVER%%-REQ-2(I) = SPACES
             THEN
*                                      BEARBEITUNGS-KZ leer, so tue nichts
                CONTINUE
             ELSE
*                                      BEARBEITUNGS-KZ falsch definiert
                SET SONSTIGER-FEHLER   OF ZUSTAND TO TRUE
                MOVE "%%SYS%%0004" TO MELDUNGSCODE OF EMS-MELD-LNK
                MOVE BEARBEITUNGS-KZ   OF %%TBL%%-TABELLE OF %%SERVER%%-REQ-2(I)
                  TO PARAMETERTEXT     OF EMS-MELD-LNK (1)
                PERFORM U200-MELDUNGSPROTOKOLL

                MOVE "%%SYS%%0005" TO MELDUNGSCODE OF EMS-MELD-LNK
                PERFORM U090-MELDUNG
             END-IF
          END-EVALUATE
       END-IF
    END-PERFORM
    .

 E410-EXIT.
    EXIT.
/
*/------------------------------------------------------------------------------
 E411-DATEN-AUS-REQUEST SECTION.
*-------------------------------------------------------------------------------
*/ Uebernahme der Daten aus Request
*/------------------------------------------------------------------------------

%%DATEN-AUS-REQUEST%%
    .

 E411-EXIT.
    EXIT.
/
*/------------------------------------------------------------------------------
 E412-ANLEGEN-%%TBL%% SECTION.
*-------------------------------------------------------------------------------
*/ Anlegen eines Satzes in Tabelle %%TBL%%
*/------------------------------------------------------------------------------

    PERFORM R500-INSERT-%%TBL%%

    EVALUATE TRUE
    WHEN     SQL-STATUS-OK
       MOVE HV-LUPD-TIMESTAMP       OF HV-%%SERVER%%
         TO LUPD-TIMESTAMP          OF %%TBL%%-TAB OF %%SERVER%%-RPL-2(I)

    WHEN     SQL-STATUS-DUP
*                                      Daten bereits vorhanden
       SET WARNUNG OF ZUSTAND  TO TRUE
       MOVE "%%SYS%%0006" TO MELDUNGSCODE OF EMS-MELD-LNK
       PERFORM U090-MELDUNG
*
       MOVE "%%SYS%%0007" TO MELDUNGSCODE OF EMS-MELD-LNK
       MOVE DATEI-MIT-FEHLER
         TO PARAMETERTEXT              OF EMS-MELD-LNK (1)
%%SCHLUESSELFELDER-PARAMETER%%
       PERFORM U200-MELDUNGSPROTOKOLL

    WHEN     OTHER
       SET DATEI-FEHLER OF ZUSTAND TO TRUE
    END-EVALUATE
    .

 E412-EXIT.
    EXIT.
/
*/------------------------------------------------------------------------------
 E413-AENDERN-%%TBL%% SECTION.
*-------------------------------------------------------------------------------
*/ Aendern eines Satzes in Tabelle %%TBL%%
*/------------------------------------------------------------------------------

    PERFORM R700-UPDATE-%%TBL%%

    EVALUATE TRUE
    WHEN     SQL-STATUS-OK
       MOVE HV-LUPD-TIMESTAMP       OF HV-%%SERVER%%
         TO LUPD-TIMESTAMP          OF %%TBL%%-TAB OF %%SERVER%%-RPL-2(I)

    WHEN     SQL-STATUS-NOT-FOUND
*                                      Daten nicht gefunden
       SET WARNUNG OF ZUSTAND  TO TRUE
       MOVE "%%SYS%%0008" TO MELDUNGSCODE OF EMS-MELD-LNK
       PERFORM U090-MELDUNG
*
       MOVE "%%SYS%%0009" TO MELDUNGSCODE OF EMS-MELD-LNK
       MOVE DATEI-MIT-FEHLER
         TO PARAMETERTEXT              OF EMS-MELD-LNK (1)
%%SCHLUESSELFELDER-PARAMETER%%
       PERFORM U200-MELDUNGSPROTOKOLL

       WHEN     OTHER
          SET DATEI-FEHLER OF ZUSTAND TO TRUE
    END-EVALUATE
    .

 E413-EXIT.
    EXIT.
/
*/------------------------------------------------------------------------------
 E414-LOESCHEN-%%TBL%% SECTION.
*-------------------------------------------------------------------------------
*/ Loeschen eines Satzes in Tabelle %%TBL%%
*/------------------------------------------------------------------------------

    PERFORM R600-DELETE-%%TBL%%

    EVALUATE TRUE
    WHEN     SQL-STATUS-OK
       CONTINUE

    WHEN     SQL-STATUS-NOT-FOUND
*                                      Daten nicht gefunden
       SET WARNUNG OF ZUSTAND  TO TRUE
       MOVE "%%SYS%%0010" TO MELDUNGSCODE OF EMS-MELD-LNK
       PERFORM U090-MELDUNG
*
       MOVE "%%SYS%%0011" TO MELDUNGSCODE OF EMS-MELD-LNK
       MOVE DATEI-MIT-FEHLER
         TO PARAMETERTEXT              OF EMS-MELD-LNK (1)
%%SCHLUESSELFELDER-PARAMETER%%  
       PERFORM U200-MELDUNGSPROTOKOLL

    WHEN     OTHER
       SET DATEI-FEHLER OF ZUSTAND TO TRUE
    END-EVALUATE
    .

 E414-EXIT.
    EXIT.
/
*/------------------------------------------------------------------------------
 R200-OPEN-CURSOR-%%TBL%%-GE SECTION.
*-------------------------------------------------------------------------------
*/ Oeffnen des Cursors CURSOR_%%TBL%%
*/------------------------------------------------------------------------------

*                                      Vorbesetzungen fuer Fehlermeldungen
    SET SQL-STATUS-OK TO TRUE
    MOVE "%%TBL%%"     TO DATEI-MIT-FEHLER

    EXEC SQL
         OPEN CURSOR_%%TBL%%
    END-EXEC

 COPY TRANSLOG-READ                    OF "=S01TLOG"
    .
 R200-EXIT.
    EXIT.
/
*/------------------------------------------------------------------------------
 R300-FETCH-CURSOR-%%TBL%%-GE SECTION.
*-------------------------------------------------------------------------------
*/ Lesen des Cursors CURSOR_%%TBL%%
*/------------------------------------------------------------------------------

*                                      Vorbesetzungen fuer Fehlermeldungen
    SET SQL-STATUS-OK TO TRUE
    MOVE "%%TBL%%"     TO DATEI-MIT-FEHLER

    EXEC SQL
         FETCH     CURSOR_%%TBL%%
         INTO     
%%FETCH-CURSOR%%
    END-EXEC

 COPY TRANSLOG-READ                    OF "=S01TLOG"
    .
 R300-EXIT.
    EXIT.
/
*/------------------------------------------------------------------------------
 R400-CLOSE-CURSOR-%%TBL%%-GE SECTION.
*-------------------------------------------------------------------------------
*/ Schliessen des Cursors CURSOR_%%TBL%%
*/------------------------------------------------------------------------------

*                                      Vorbesetzungen fuer Fehlermeldungen
    SET SQL-STATUS-OK TO TRUE
    MOVE "%%TBL%%"     TO DATEI-MIT-FEHLER

    EXEC SQL
         CLOSE CURSOR_%%TBL%%
    END-EXEC
    .

 R400-EXIT.
    EXIT.
/
*/------------------------------------------------------------------------------
 R500-INSERT-%%TBL%% SECTION.
*-------------------------------------------------------------------------------
*/ Eintragen eines Satzes in Tabelle %%TBL%%
*/------------------------------------------------------------------------------

*                                      Vorbesetzungen fuer Fehlermeldungen
    SET SQL-STATUS-OK TO TRUE
    MOVE "%%TBL%%"     TO DATEI-MIT-FEHLER

    MOVE PROGRAMM-ID
      TO LUPD-PROZESSNAME              OF %%TBL%%-ROW

    EXEC SQL
         INSERT
         INTO     =%%TBL%%-TAB (
%%INSERT-FELDER%%
                 )
         VALUES  (
%%INSERT-WERTE%%
                 )
    END-EXEC

 COPY TRANSLOG-WRITE                   OF "=S01TLOG"
    .
 R500-EXIT.
    EXIT.
/
*/------------------------------------------------------------------------------
 R600-DELETE-%%TBL%% SECTION.
*-------------------------------------------------------------------------------
*/ Loeschen eines Satzes in Tabelle %%TBL%%
*/------------------------------------------------------------------------------

*                                      Vorbesetzungen fuer Fehlermeldungen
    SET SQL-STATUS-OK TO TRUE
    MOVE "%%TBL%%"     TO DATEI-MIT-FEHLER

    EXEC SQL
         DELETE
         FROM     =%%TBL%%-TAB
         WHERE     
%%DELETE-FELDER-WERTE%%
    END-EXEC

 COPY TRANSLOG-WRITE                   OF "=S01TLOG"
    .
 R600-EXIT.
    EXIT.
/
*/------------------------------------------------------------------------------
 R700-UPDATE-%%TBL%% SECTION.
*-------------------------------------------------------------------------------
*/ Aktualisieren eines Satzes in Tabelle %%TBL%%
*/------------------------------------------------------------------------------

*                                      Vorbesetzungen fuer Fehlermeldungen
    SET SQL-STATUS-OK TO TRUE
    MOVE "%%TBL%%"     TO DATEI-MIT-FEHLER

    MOVE PROGRAMM-ID
      TO LUPD-PROZESSNAME              OF %%TBL%%-ROW

    EXEC SQL
         UPDATE   =%%TBL%%-TAB
         SET       
%%UPDATE-FELDER-WERTE%%
    END-EXEC

 COPY TRANSLOG-WRITE                   OF "=S01TLOG"
    .
 R700-EXIT.
    EXIT.
/
*/------------------------------------------------------------------------------
 Y100-WRITE-REPLY SECTION.
*-------------------------------------------------------------------------------
*/ Bestimmen der Ausgangszeit und Senden der Message an Requestor,
*/ Reply gesteuert ueber RPL-MSD-ID
*/------------------------------------------------------------------------------

*                                      Bei Transaction-Logging Aufruf
*                                      von S01U00
 COPY TRANSLOG                         OF "=S01TLOG".

    EVALUATE TRUE
*                                      Reply MSG-DATEN-1
    WHEN     MSG-1 OF RPL-MSG-ID OF RPC-DIALOG-HDR-OUT
       PERFORM Y200-WRITE-MSG-1

*                                      Reply MSG-DATEN-2
    WHEN     MSG-2 OF RPL-MSG-ID OF RPC-DIALOG-HDR-OUT
       PERFORM Y300-WRITE-MSG-2

*                                      Reply leer
    WHEN     LEER  OF RPL-MSG-ID OF RPC-DIALOG-HDR-OUT
       PERFORM Y900-WRITE-HDR

    WHEN     OTHER
       SET TRANS-ABORTEN               OF TRANS-STEUERUNG
                                       OF RPC-DIALOG-HDR-OUT TO TRUE
       SET LEER                        OF RPL-MSG-ID
                                       OF RPC-DIALOG-HDR-OUT TO TRUE
*                                      Programmierfehler
       PERFORM U070-MELDUNG-F-MSG
       PERFORM Y900-WRITE-HDR
    END-EVALUATE
    .
 Y100-EXIT.
    EXIT.
/
*/------------------------------------------------------------------------------
 Y200-WRITE-MSG-1 SECTION.
*-------------------------------------------------------------------------------
*/ Senden %%SERVER%%-RPL-1
*/------------------------------------------------------------------------------

    MOVE FUNCTION LENGTH( %%SERVER%%-RPL-1 )
      TO WS-MESSAGE-LEN
    WRITE MSG-BUFFER FROM %%SERVER%%-RPL-1
    .

 Y200-EXIT.
    EXIT.
/
*/------------------------------------------------------------------------------
 Y300-WRITE-MSG-2 SECTION.
*-------------------------------------------------------------------------------
*/ Senden %%SERVER%%-RPL-2
*/------------------------------------------------------------------------------

    MOVE FUNCTION LENGTH( %%SERVER%%-RPL-2 )
      TO WS-MESSAGE-LEN
    WRITE MSG-BUFFER FROM %%SERVER%%-RPL-2
    .

 Y300-EXIT.
    EXIT.
/
*/------------------------------------------------------------------------------
 Y900-WRITE-HDR SECTION.
*-------------------------------------------------------------------------------
*/ Senden des RPC-DIALOG-HDR
*/------------------------------------------------------------------------------

    MOVE FUNCTION LENGTH( RPC-DIALOG-HDR-OUT )
      TO WS-MESSAGE-LEN
    WRITE MSG-BUFFER FROM RPC-DIALOG-HDR-OUT
    .
 Y900-EXIT.
    EXIT.
/
*/------------------------------------------------------------------------------
 Z100-ABSCHLUSS SECTION.
*-------------------------------------------------------------------------------
*/ Schliessen aller Dateien
*/------------------------------------------------------------------------------

    CLOSE MSG-IO-FILE
    .
 Z100-EXIT.
    EXIT.
/
*                                      Copy-Strecken
?SOURCE =S00CRPCD (U102-NOT-FOUND)
?SOURCE =S00CRPCD (U103-ERROR)
?SOURCE =S00CRPCD (U104-WARNING)

 COPY U115-CHECK-UPDATE                OF "=S00CSR02".
 COPY M520-INVALID-FUNCTION            OF "=S00CRPCD".
 COPY U901-IO-ERROR                    OF "=S00CRPCD".
 COPY U000-MELDUNG-IN-RPC-HDR          OF "=S00CRPCD".
 COPY U070-MELDUNG-F-MSG               OF "=S00CRPCD".
 COPY U080-MELDUNG-F-ZUSTAND           OF "=S00CRPCD".
 COPY U090-MELDUNG                     OF "=S00CRPCD".
 COPY U120-MELDUNG-LESEN-LOGGEN        OF "=S00CRPCD".
 COPY U200-MELDUNGSPROTOKOLL           OF "=EMSS01".

 COPY U000-CONTROL-DISTRIBUTOR         OF "=S00CMR".

