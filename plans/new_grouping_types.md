# Planejamento: Implementação de Novos Tipos de Agrupamento

**Agente Responsável:** Solutions Architect
**Data:** 15/12/2025

## 1. Objetivo
Expandir a funcionalidade de agrupamento de opções (`OptionGroup`) para suportar cenários onde as opções não são estritamente obrigatórias, oferecendo maior flexibilidade na definição de interfaces de linha de comando, em alinhamento com a filosofia de UX e clareza do projeto.

## 2. Novos Tipos de Agrupamento

### 2.1. `None` (Múltiplos Opcionais)
- **Descrição:** Nenhuma opção do grupo é obrigatória. O usuário pode fornecer quantas quiser (0, 1, ou N).
- **Caso de Uso:** Flags de configuração avançada que são todas opcionais e independentes, mas pertencem a uma categoria lógica (ex: "LogSettings").

### 2.2. `AtMostOne` (Opcional, Máximo 1)
- **Descrição:** Nenhuma opção do grupo é obrigatória. Porém, se o usuário fornecer, só pode fornecer no máximo uma.
- **Caso de Uso:** Seleção de modo de operação onde existe um padrão (nenhuma flag) ou uma sobreposição específica mutuamente exclusiva, mas opcional.

## 3. Impacto na Arquitetura

- **Enum:** `CleanConsole.Parse.Enums.OptionGroupRequirement.cs` - Adicionar novos membros.
- **Parser:** `CleanConsole.Parse.CleanParser.cs` - Adicionar lógica de validação.
- **Testes:** `CleanConsole.Parse.Tests.GroupTests.cs` - Adicionar novos casos de teste.

## 4. Plano de Execução Detalhado

O plano de tarefas foi completamente reestruturado em Épicos para refletir uma sequência de trabalho mais clara, da arquitetura à entrega final.

#### **Épico 1: Refatoração da Arquitetura e API**
*Objetivo: Modernizar a API e a estrutura do projeto antes de adicionar novos recursos.*

| ID | Tarefa | Responsável | Status |
|:---|:---|:---|:---|
| R-01 | Unificar todos os namespaces para `CleanConsole.Parse`. | Architect | Concluído |
| R-02 | Renomear atributo `ProgramDef` para `ProgramDefinition` e seu arquivo. | Architect | Pendente |
| R-03 | No atributo `[OptionGroup]`, renomear a propriedade `Type` para `Require`. | Architect | Pendente |
| R-04 | Adicionar a propriedade `string Description` ao `[OptionAttribute]`. | Architect | Pendente |
| R-05 | Adicionar a propriedade `string Description` ao `[OptionGroupAttribute]`. | Architect | Pendente |
| R-06 | **(Cancelado)** Mover `OptionGroupRequirement.cs` da pasta `Enums` para a raiz. | Architect | Cancelado |

#### **Épico 2: Implementação dos Novos Tipos de Grupo**
*Objetivo: Implementar a lógica central para os requisitos de grupo `None` e `AtMostOne`.*

| ID | Tarefa | Responsável | Status |
|:---|:---|:---|:---|
| F-01 | Atualizar o enum `OptionGroupRequirement` com os valores `None` e `AtMostOne`. | Core Eng | Pendente |
| F-02 | Implementar a lógica de validação para `None` no `CleanParser`. | Core Eng | Pendente |
| F-03 | Implementar a lógica de validação para `AtMostOne` no `CleanParser`. | Core Eng | Pendente |

#### **Épico 3: Experiência do Usuário (UX) e Geração de Ajuda**
*Objetivo: Garantir que a saída do console (ajuda e erros) seja clara e informativa.*

| ID | Tarefa | Responsável | Status |
|:---|:---|:---|:---|
| UX-01 | Desenhar o formato de exibição do texto de ajuda (`--help`) para os grupos, incluindo suas `Description` e requisito (`None`, `AtMostOne`, etc.). | UX Specialist | Pendente |
| UX-02 | Desenhar a mensagem de erro específica para a violação da regra `AtMostOne`. | UX Specialist | Pendente |
| UX-03 | Implementar a nova geração de `GetHelpText` para refletir o design da tarefa UX-01. | Core Eng | Pendente |

#### **Épico 4: Testes e Garantia de Qualidade (QA)**
*Objetivo: Assegurar que as novas funcionalidades e refatorações sejam robustas e não introduzam regressões.*

| ID | Tarefa | Responsável | Status |
|:---|:---|:---|:---|
| T-01 | Criar testes unitários para o requisito de grupo `None`. | QA Eng | Pendente |
| T-02 | Criar testes unitários para o requisito de grupo `AtMostOne` (sucesso e falha). | QA Eng | Pendente |
| T-03 | Criar teste unitário que valide a nova mensagem de erro da tarefa UX-02. | QA Eng | Pendente |
| T-04 | Criar testes para verificar se a `Description` de `[Option]` e `[OptionGroup]` aparece no `GetHelpText`. | QA Eng | Pendente |
| T-05 | Criar testes para as refatorações de nomenclatura (`ProgramDefinition`, `Require`). | QA Eng | Pendente |

#### **Épico 5: Documentação e Finalização**
*Objetivo: Realizar a revisão final e atualizar toda a documentação do projeto para refletir as mudanças.*

| ID | Tarefa | Responsável | Status |
|:---|:---|:---|:---|
| D-01 | Realizar a Revisão de Código (Code Review) de todas as implementações. | Architect | Pendente |
| D-02 | Atualizar `docs/API_REFERENCE.md` com todas as mudanças na API (atributos, propriedades e enums). | Architect | Pendente |
| D-03 | Atualizar `README.md` com exemplos das novas funcionalidades. | Architect | Pendente |
| D-04 | Atualizar `PRD.md` para refletir a nova arquitetura de namespace único e as novas funcionalidades. | Architect | Pendente |
| D-05 | Adicionar exemplos de uso dos novos tipos de grupo em `docs/BEST_PRACTICES.md` ou `GETTING_STARTED.md`. | Architect | Pendente |
| D-06 | Revisar e atualizar os arquivos em `agents/` se a refatoração da API impactar as responsabilidades. | Architect | Pendente |

---
*Aprovado por:* Solutions Architect
---