# Índice do Projeto CleanConsole.Parse

Este documento serve como um mapa central para navegar pela estrutura do projeto, documentação e definição dos agentes responsáveis pela manutenção e evolução do software.

## 📄 Definições do Produto

*   [**PRD.md**](PRD.md): **Product Requirements Document**. O documento mestre que define o escopo, requisitos funcionais (RF), regras de negócio e a matriz de testes do projeto.
*   [**tarefas.md**](tarefas.md): O histórico de execução e tarefas do projeto.

## 🤖 Agentes Especialistas (`agents/`)

Os "Agentes" representam as áreas de responsabilidade técnica para a manutenção contínua do projeto. Consulte o [Índice de Agentes](agents/README.md) para mais detalhes.

*   [**Architect (Arquitetura)**](agents/architect.md): Responsável pela infraestrutura, evolução da API pública (Atributos) e regras de inicialização.
*   [**Core Engineer (Engenharia)**](agents/core_engineer.md): Responsável pela lógica interna de parsing, performance, reflection e segurança de tipos (Type Safety).
*   [**UX Specialist (Experiência)**](agents/ux_specialist.md): Foca na documentação, clareza das mensagens de erro e formatação visual da saída no console.
*   [**QA Engineer (Qualidade)**](agents/qa_engineer.md): Garante a estabilidade através da manutenção da suíte de testes e cobertura de novos cenários.

## 📚 Documentação Técnica (`docs/`)

Guias e referências para desenvolvedores e usuários da biblioteca. Consulte o [Índice da Documentação](docs/README.md) para uma visão completa.

*   [**Primeiros Passos (Getting Started)**](docs/GETTING_STARTED.md): Guia rápido para instalação e criação do seu primeiro CLI.
*   [**Referência da API (API Reference)**](docs/API_REFERENCE.md): Detalhes técnicos de todos os Atributos e Configurações.
*   [**Arquitetura**](docs/ARCHITECTURE.md): Explicação profunda sobre o funcionamento interno e decisões de design.
*   [**Boas Práticas**](docs/BEST_PRACTICES.md): Recomendações para criar CLIs robustas.
*   [**Guia de Contribuição**](docs/CONTRIBUTING.md): Diretrizes para desenvolvedores que desejam colaborar.

---
*Este arquivo reflete a estrutura atual de manutenção do projeto.*