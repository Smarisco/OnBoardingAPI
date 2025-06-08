# Scripts de Banco de Dados - Onboarding Inclusivo

Este diretório contém os scripts SQL necessários para criar e popular o banco de dados da aplicação.

## Pré-requisitos

- SQL Server instalado.
- Um banco de dados vazio criado, chamado `OnboardingDB`.

## Como Usar

Execute os scripts na seguinte ordem para configurar o banco de dados corretamente:

1. Cria todas as tabelas, chaves primárias e chaves estrangeiras necessárias para a aplicação. Observando que a Interacao é a ultima a ser criada.

2. **`CREATE_ResultadoAnalise.sql`**: Este script insere os dados fictícios de interação utilizados nos experimentos e análises descritos no artigo.
