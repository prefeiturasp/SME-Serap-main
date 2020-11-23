*** Settings ***
Documentation       Variaveis globais do ambiente de testes

*** Variables ***
${Use Selenium Server}      True
${Selenium Server URL}      http://10.50.1.234:4444/wd/hub

${SERVER}                   %{SERVER}
${BROWSER}                  %{BROWSER}
${DELAY}                    0
${VALID USER}               %{SME_USER}
${VALID PASSWORD}           %{SME_PASS}
