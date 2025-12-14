# Índice do Projeto CleanConsole.Parse

Este documento serve como um mapa central para navegar pela estrutura do projeto, documentação e definição dos agentes responsáveis pelo desenvolvimento.

## 📄 Definições do Produto

*   [**PRD.md**](PRD.md): **Product Requirements Document**. O documento mestre que define o escopo, requisitos funcionais (RF), regras de negócio e a matriz de testes do projeto. Tudo deriva daqui.
*   [**tarefas.md**](tarefas.md): A lista de tarefas (To-Do List) detalhada, quebrada em etapas técnicas e atribuída aos agentes. É o guia de execução do projeto.

## 🤖 Agentes Especialistas (`agents/`)

Os "Agentes" representam as personas e responsabilidades técnicas assumidas durante o desenvolvimento.

*   [**Architect (Arquitetura)**](agents/architect.md): Responsável pela estrutura da solução, definição da API pública (Atributos) e validação de inicialização. Garante que a fundação seja sólida.
*   [**Core Engineer (Engenharia)**](agents/core_engineer.md): Responsável pelo motor de parsing, tokenização, reflection e conversão de tipos. Constrói o "cérebro" da biblioteca.
*   [**UX Specialist (Experiência)**](agents/ux_specialist.md): Foca na interação com o usuário da CLI, gerando textos de ajuda claros e mensagens de erro amigáveis.
*   [**QA Engineer (Qualidade)**](agents/qa_engineer.md): Garante que tudo funcione conforme o esperado através de testes unitários rigorosos e validação de casos de borda.

## 📚 Documentação Técnica (`docs/`)

Guias e referências para desenvolvedores e usuários da biblioteca.

*   [**Architecture (Arquitetura)**](docs/ARCHITECTURE.md): Diagramas e explicações sobre o fluxo interno de funcionamento (Startup -> Tokenização -> Binding).
*   [**API Reference (Referência)**](docs/API_REFERENCE.md): Detalhamento técnico de todos os Atributos (`[Option]`, `[ProgramDef]`) e Enums disponíveis.
*   [**Getting Started (Primeiros Passos)**](docs/GETTING_STARTED.md): Guia rápido para novos usuários configurarem a biblioteca em seus projetos.
*   [**Best Practices (Boas Práticas)**](docs/BEST_PRACTICES.md): Recomendações de design e segurança para criar CLIs robustas com a ferramenta.
*   [**Contributing (Contribuição)**](docs/CONTRIBUTING.md): Diretrizes para desenvolvedores que desejam colaborar com o código fonte do projeto.

---
*Este arquivo deve ser mantido atualizado conforme novos documentos ou agentes sejam adicionados ao projeto.*
