# Planejamento: Implementação de Novos Tipos de Agrupamento

**Agente Responsável:** Solutions Architect
**Data:** 15/12/2025

## 1. Objetivo
Expandir a funcionalidade de agrupamento de opções (`OptionGroup`) para suportar cenários onde as opções não são estritamente obrigatórias, oferecendo maior flexibilidade na definição de interfaces de linha de comando.

## 2. Novos Tipos de Agrupamento

### 2.1. `Optional` (Múltiplos Opcionais)
- **Descrição:** Nenhuma opção do grupo é obrigatória. Se o usuário fornecer opções, pode fornecer quantas quiser (0, 1, ou N).
- **Regra de Validação:** Nenhuma validação de quantidade mínima ou máxima. Apenas agrupa semanticamente ou para fins de documentação/futuro uso.
- **Caso de Uso:** Flags de configuração avançada que são todas opcionais e independentes, mas pertencem a uma categoria lógica (ex: "LogSettings").

### 2.2. `OptionalAtMostOne` (Opcional, Máximo 1)
- **Descrição:** Nenhuma opção do grupo é obrigatória. Porém, se o usuário fornecer, só pode fornecer no máximo uma.
- **Regra de Validação:** `Count <= 1`. (0 é válido, 1 é válido, >1 é erro).
- **Caso de Uso:** Seleção de modo de operação onde existe um padrão (nenhuma flag) ou uma sobreposição específica mutuamente exclusiva, mas opcional.

## 3. Impacto na Arquitetura

### 3.1. API Pública (`CleanConsole.Parse.Enums`)
- **Arquivo:** `OptionGroupType.cs`
- **Alteração:** Adição de dois novos membros ao enum.

```csharp
public enum OptionGroupType
{
    ExactOne,
    AtLeastOne,
    Optional,        // Novo
    OptionalAtMostOne // Novo
}
```

### 3.2. Lógica de Validação (`CleanConsole.Parse`)
- **Arquivo:** `CleanParser.cs`
- **Local:** Método `Parse<T>`, bloco "6. Validação de Regras de Negócio (Grupos)".
- **Alteração:** Adicionar os `else if` ou `switch` para tratar os novos tipos.

## 4. Estratégia de Testes (`CleanConsole.Parse.Tests`)
Novos testes devem ser adicionados em `GroupTests.cs` para cobrir os novos cenários.

### Casos de Teste - `Optional`
1. **Sucesso:** Nenhuma opção fornecida (Count = 0).
2. **Sucesso:** Uma opção fornecida (Count = 1).
3. **Sucesso:** Múltiplas opções fornecidas (Count > 1).

### Casos de Teste - `OptionalAtMostOne`
1. **Sucesso:** Nenhuma opção fornecida (Count = 0).
2. **Sucesso:** Uma opção fornecida (Count = 1).
3. **Falha:** Duas opções fornecidas (Count = 2) -> Deve lançar `CleanParserException`.

## 5. Plano de Tarefas

| ID | Tarefa | Responsável | Status |
|----|--------|-------------|--------|
| T-01 | Atualizar `OptionGroupType` com `Optional` e `OptionalAtMostOne`. | Architect | Pendente |
| T-02 | Implementar lógica de validação para `Optional` em `CleanParser.cs`. | Core Eng | Pendente |
| T-03 | Implementar lógica de validação para `OptionalAtMostOne` em `CleanParser.cs`. | Core Eng | Pendente |
| T-04 | Criar testes unitários para o grupo `Optional`. | QA Eng | Pendente |
| T-05 | Criar testes unitários para o grupo `OptionalAtMostOne`. | QA Eng | Pendente |
| T-06 | Verificar se a documentação de ajuda (`GetHelpText`) reflete corretamente os grupos. | Architect | Pendente |
| T-07 | Melhorar `GetHelpText` para exibir o *tipo* de restrição do grupo (ex: `(Max 1)`, `(Optional)`). | Core Eng | Pendente |
| T-08 | Atualizar `docs/API_REFERENCE.md` com os novos tipos de agrupamento e exemplos. | Architect | Pendente |

---
*Aprovado por:* Solutions Architect
