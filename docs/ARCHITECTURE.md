# Arquitetura do Projeto CleanConsole.Parse

## Visão Geral
O **CleanConsole.Parse** é uma biblioteca de *binding* de argumentos de linha de comando para objetos .NET. O objetivo principal é abstrair a complexidade de parsing de strings (`string[] args`) e oferecer uma experiência declarativa baseada em **Atributos** e **Reflection**.

## Fluxo de Execução

O processo de parsing segue um fluxo linear rigoroso para garantir integridade e validação:

```mermaid
graph TD
    A[Início: args[] + Tipo POCO] --> B[Startup Check]
    B -->|Erro na Definição| X[Exception: Configuração Inválida]
    B -->|Sucesso| C[Tokenização]
    C -->|Normalização| D[Identificação de Chave/Valor]
    D --> E[Reflection & Binding]
    E --> F[Validação de Tipos]
    F -->|Erro de Tipo| Y[Exception: Erro de Conversão]
    F --> G[Validação de Regras (Grupos)]
    G -->|Erro de Regra| Z[Exception: Regra Violada]
    G --> H[Sucesso: Objeto Populado]
```

## Componentes Principais

### 1. Camada de Definição (Attributes)
É a interface pública da biblioteca. Permite que o usuário decore suas classes para definir o comportamento do parser.
*   **`[ProgramDef]`**: Metadados globais da aplicação (Nome, Descrição).
*   **`[Option]`**: Mapeia uma propriedade a um argumento CLI.
*   **`[OptionGroup]`**: Define restrições lógicas entre propriedades (ex: "apenas um destes").

### 2. Motor de Parsing (Core)
Responsável por transformar strings brutas em dados estruturados.
*   **Start-up Validator:** Verifica se a classe de destino é válida (sem nomes duplicados, tipos suportados).
*   **Tokenizer:** Separa chaves de valores, trata aspas e normaliza prefixos (`-`, `--`, `/`).
*   **Binder:** Usa Reflection para instanciar a classe alvo e preencher as propriedades.

### 3. Sistema de Validação
*   **Type Safety:** Garante que "123" vire `int` e "true" vire `bool`.
*   **Logic Validation:** Verifica as regras de `ExactOne` ou `AtLeastOne` após o binding.

## Decisões de Design

*   **Imutabilidade da Configuração:** As regras são definidas em tempo de compilação via atributos e validadas na inicialização.
*   **Fail-Fast:** O sistema aborta a operação no primeiro erro encontrado (seja de configuração ou de input do usuário) para evitar estados inconsistentes.
*   **Cultura Invariante:** Para garantir portabilidade, números decimais sempre esperam o formato com ponto (`.`), independente da cultura do SO.
