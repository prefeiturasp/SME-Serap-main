*** Settings ***
Documentation       Robot de teste com bateria completa do sistema SERAp

Library             SeleniumLibrary

Resource            configuracao/variaveis.robot
Resource            identity/login.test.robot
Resource            exam/exam.test.robot

*** Keywords ***
Teardown
    Close Browser

*** Test Cases ***
SERAP Full Test Cases
    UC Login Identity
    UC Init Exam
    [Teardown]  Teardown
