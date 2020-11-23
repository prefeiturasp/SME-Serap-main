*** Settings ***
Documentation       Robo de teste para validar a seguinte jornada do usuário:
...                 1. Acessar o grupo de provas não iniciadas
...                 2. Iniciar a prova
...                 3. Sair (salvar) a prova iniciada para gerar o registro de inicio

Resource            exam.lib.robot

*** Keywords ***
UC Init Exam
    Open Exams Page
    Expand Group Not Started
    Start First Exam