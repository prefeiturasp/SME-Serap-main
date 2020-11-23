*** Settings ***
Documentation       Robo de teste para validar a seguinte jornada do usuário:
...                 1. Logar
...                 2. Selecionar o sistema SERAP
...                 3. Navegar no menu pro

Library             SeleniumLibrary

Resource            login.lib.robot

*** Variables ***
${SERAP LINK BUTTON SELECTOR}      //a[@id="ctl00_ContentPlaceHolder1__dltSistemas_ctl01_hplSistema"]

*** Keywords ***
Click Serap App Button
    Click Link  ${SERAP LINK BUTTON SELECTOR}

UC Login Identity
    Open Browser To Login Page
    Login Into System
    Wait Until Element Is Visible   ${SERAP LINK BUTTON SELECTOR}
    Click Serap App Button
