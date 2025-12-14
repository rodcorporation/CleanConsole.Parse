# Guia de Contribuição

Obrigado pelo interesse em contribuir com o **CleanConsole.Parse**! Siga estas diretrizes para manter a qualidade e consistência do projeto.

## Ambiente de Desenvolvimento
*   **SDK:** .NET 10.
*   **IDE:** Visual Studio 2022+ ou VS Code.

## Padrões de Código
*   **Idioma:** Código, Comentários e Commits em **Inglês** (padrão internacional open-source) ou **Português** (conforme definido pelo time, para este projeto específico estamos usando Português na documentação e Inglês no Código).
*   **Nomenclatura:** PascalCase para classes e métodos, camelCase para variáveis locais.
*   **Clean Code:** Métodos pequenos, responsabilidade única e nomes descritivos.

## Fluxo de Trabalho
1.  **Branching:** Crie uma branch para sua feature (`feature/nova-validacao`) ou correção (`fix/erro-parsing`).
2.  **Testes:** É **obrigatório** que qualquer nova funcionalidade venha acompanhada de testes unitários.
3.  **Verificação:** Execute todos os testes existentes antes de abrir um PR.

## Testes (QA)
O projeto utiliza **xUnit**. Temos uma matriz de cobertura rigorosa (T01-T10) definida no PRD.
*   **Localização:** Projeto `CleanConsole.Parse.Tests`.
*   **Comando:** `dotnet test`

## Regras Críticas
1.  **Sem Dependências Externas:** A biblioteca Core deve depender apenas do .NET Standard/Core, sem pacotes NuGet terceiros, para manter-se leve.
2.  **Performance:** Evite Reflection desnecessário. Faça cache de metadados se possível (embora para CLI, startup time seja mais crítico que throughput).
3.  **Actionable Errors:** Nunca lance uma `Exception` genérica. O usuário precisa saber o que errou.
