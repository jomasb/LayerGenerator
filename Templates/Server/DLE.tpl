!/******************************************************************************
!/
!/ A. ALLGEMEINES:
!/
!/    Projekt:     Daimler AG
!/    Arb-Gebiet:  %%PRODUKT%%
!/    Quelle:      %%SERVER%%
!/
!/    Autor:       %%AUTOR%%
!/    Datum:       %%DATUM%%
!/
!/    Aenderungen:
!/
!/    Marke  Datum     Autor     Text
!/    #000#  %%DATUM%% %%AUTOR%% %%ANF%%: Pflege %%TBL%%
!/
!/
!/ B. BESCHREIBUNG:
!/
!/    Definitionen zum Server %%SERVER%%
!/    - Working-Storage-Variablen
!/    - Host-Variablen
!/    - Request-Strukturen
!/    - Reply-Strukturen
!/
!/******************************************************************************

*======================================================================
?SECTION DELETE
*======================================================================
DELETE DEF %%SERVER%%-REQ-RPL-0.
DELETE DEF %%SERVER%%-REQ-RPL-1.
DELETE DEF %%SERVER%%-REQ-RPL-2.
DELETE DEF %%SERVER%%-MSG-1.
DELETE DEF %%SERVER%%-MSG-2.
DELETE DEF %%SERVER%%-TAB-ZEILE-KOMPLETT.
DELETE DEF %%SERVER%%-TAB-ZEILE-PF.
DELETE DEF WS-%%SERVER%%.
DELETE DEF HV-%%SERVER%%.

DELETE CONSTANT CO-%%SERVER%%-ANZ-%%TBL%%-KOMPLETT.
DELETE CONSTANT CO-%%SERVER%%-ANZ-%%TBL%%-PF.

*======================================================================
?SECTION INSERT
*======================================================================
?COMMENTS

* ------------------------------------------------------------
* - Konstanten
* ------------------------------------------------------------

CONSTANT CO-%%SERVER%%-ANZ-%%TBL%%-KOMPLETT VALUE %%ANZ-KOMPLETT%%.
CONSTANT CO-%%SERVER%%-ANZ-%%TBL%%-PF       VALUE %%ANZ-PF%%.

* ------------------------------------------------------------
* - Messages
* ------------------------------------------------------------
                                       
*                                      Tabellenzeile fuer Liste
DEF %%SERVER%%-TAB-ZEILE-KOMPLETT.
%%FELDER%%
END.

*                                      Tabellenzeile fuer Pflege
DEF %%SERVER%%-TAB-ZEILE-PF.
  02 BEARBEITUNGS-KZ                   TYPE BEARBEITUNGS-KZ-88.
%%FELDER%%
END.

*                                      Liste %%TBL%% (komplett)
DEF %%SERVER%%-MSG-1.
  02 %%TBL%%-TABELLE.
    03 %%TBL%%-TAB-ANZ                 PIC 9(4).
    03 %%TBL%%-TAB                     OCCURS CO-%%SERVER%%-ANZ-%%TBL%%-KOMPLETT.
      04 %%TBL%%-TAB-ZEILE             TYPE %%SERVER%%-TAB-ZEILE-KOMPLETT.
END.

*                                      Pflege %%TBL%%
DEF %%SERVER%%-MSG-2.
  02 %%TBL%%-TABELLE.
    03 %%TBL%%-TAB-ANZ                 PIC 9(4).
    03 %%TBL%%-TAB                     OCCURS CO-%%SERVER%%-ANZ-%%TBL%%-PF.
      04 %%TBL%%-TAB-ZEILE             TYPE %%SERVER%%-TAB-ZEILE-PF.
END.

* ------------------------------------------------------------
* - Request/Reply
* ------------------------------------------------------------

*                                      Dummy-Request zur Sicherung des
*                                      RPC-DIALOG-HDR-IN
DEF %%SERVER%%-REQ-RPL-0.
  02 FILLER                            TYPE RPC-DIALOG-HDR.
END.

*                                      Liste %%TBL%%
DEF %%SERVER%%-REQ-RPL-1.
  02 FILLER                            TYPE RPC-DIALOG-HDR.
  02 %%SERVER%%-MSG-1                      TYPE *.
END.

*                                      Pflege %%TBL%%
DEF %%SERVER%%-REQ-RPL-2.
  02 FILLER                            TYPE RPC-DIALOG-HDR.
  02 %%SERVER%%-MSG-2                      TYPE *.
END.

* ------------------------------------------------------------
* - Working
* ------------------------------------------------------------
DEF WS-%%SERVER%%.
  02 WS-%%SERVER%%-CONSTANT-VALUES.
    03 WS-INDICATOR-VAR-KO             TYPE INDICATOR-VAR-KO.
    03 WS-MAX-ANZ-%%TBL%%-KOMPLETT      PIC 9(3)
                                       VALUE CO-%%SERVER%%-ANZ-%%TBL%%-KOMPLETT.
    03 WS-MAX-ANZ-%%TBL%%-PF            PIC 9(3)
                                       VALUE CO-%%SERVER%%-ANZ-%%TBL%%-PF.
  02 WS-%%SERVER%%-INIT.
    03 WS-LUPD-TIMESTAMP-STRUCT        TYPE DATETIMEYMDHSF3-STRUKTUR.
    03 WS-SCHLEIFEN-ENDE-SW            TYPE KZ-JA-NEIN-88.
END.


* ------------------------------------------------------------
* - Hosts
* ------------------------------------------------------------
DEF HV-%%SERVER%%.
  02 HV-LUPD-TIMESTAMP                 TYPE LUPD-TIMESTAMP.
END.


*======================================================================
?SECTION OUTPUT
*======================================================================
?SETCOBOL85
?COBOL =%%SERVER%% !
?COBLEVEL 1

OUTPUT DEF %%SERVER%%-REQ-RPL-0.
OUTPUT DEF %%SERVER%%-REQ-RPL-1.
OUTPUT DEF %%SERVER%%-REQ-RPL-2.

OUTPUT DEF WS-%%SERVER%%.
OUTPUT DEF HV-%%SERVER%%.

?NOCOBOL
