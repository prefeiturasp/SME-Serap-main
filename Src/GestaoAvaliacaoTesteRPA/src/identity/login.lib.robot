*** Settings ***
Documentation       Lib de comandos para o fluxo de login

Library             BuiltIn
Library             SeleniumLibrary

Resource            ../configuracao/variaveis.robot

*** Variables ***
${IDENTITY SERVER}          identity.sme.prefeitura.sp.gov.br
${LOGIN URL}                http://${IDENTITY SERVER}/Account/Login
${VALID USER}               %{SME_USER}
${VALID PASSWORD}           %{SME_PASS}

${PASSWORD FIELD SELECTOR}  //input[@name="Password"]
${USER FIELD SELECTOR}      //input[@name="Username"]

*** Keywords ***
Open Browser To Login Page
    Open Browser                    ${LOGIN URL}    ${BROWSER}
    Maximize Browser Window
    Set Selenium Speed              ${DELAY}

Login Into System
    Input Username  ${VALID USER}
    Input Password  ${VALID PASSWORD}
    Submit Credentials

Input Username
    [Arguments]     ${username}
    Click Element   ${USER FIELD SELECTOR}
    Input Text      ${USER FIELD SELECTOR}    ${username}

Input Password
    [Arguments]     ${password}
    Click Element   ${PASSWORD FIELD SELECTOR}
    Input Text      ${PASSWORD FIELD SELECTOR}    ${password}

Submit Credentials
    Submit Form
