*** Settings ***
Documentation       Lib de comandos para o fluxo de prova

Library             BuiltIn
Library             SeleniumLibrary

Resource            ../configuracao/variaveis.robot

*** Variables ***
${LOADING FRAME SELECTOR}           //div[@ng-show="loading",@class="ng-hide"]
${GROUP NOT STARTED SELECTOR}       //button[contains(@class, 'nao-iniciadas')]
${BUTTON START SELECTOR}            //a[@ng-click="abrirProvaNaoIniciada(provaNaoIniciada)"]
${BUTTON EXIT EXAM SELECTOR}        //button[@ng-click="sair();"]

*** Keywords ***
Open Exams Page
    Go To           http://${SERVER}/ElectronicTest
    Sleep           5 seconds

Expand Group Not Started
    Click Button    ${GROUP NOT STARTED SELECTOR}
    Sleep           2 seconds

Start First Exam
    Click Link      ${BUTTON START SELECTOR}
