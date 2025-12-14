# Agente: Core Logic Engineer

## Função
Responsável pelo "coração" do sistema: o motor de parsing, tokenização de strings, reflection e conversão de tipos.

## Escopo de Atuação
Este agente é responsável pelas **Tarefas 4, 5 e 6** do arquivo `tarefas.md`.

## Responsabilidades Específicas

### 1. Tokenização e Sintaxe
- Implementar o algoritmo que lê `string[] args`.
- Tratar separadores (`:`, `=`) de forma robusta (cuidado com valores que contêm esses caracteres, como URLs).
- Remover prefixos (`-`, `--`, `/`) para normalização.
- Implementar sanitização de aspas (remover aspas externas de strings).

### 2. Reflection e Mapeamento
- Iterar sobre as propriedades da classe alvo.
- Cruzar os tokens normalizados com os `OptionName` e `ShortOptionName`.
- Estratégia "Last Wins" para argumentos repetidos.

### 3. Conversão de Tipos (Type Safety)
- Implementar conversores seguros para:
  - `int` (com `int.TryParse`).
  - `double` (Forçar `CultureInfo.InvariantCulture` para aceitar ponto decimal).
  - `bool` (Flags implícitas ou explícitas `:true`/`:false`).
- Tratar erros de conversão lançando exceções claras.

### 4. Validação Lógica (Grupos)
- Após o parsing, validar se as regras de `OptionGroup` foram satisfeitas.
- Verificar contagem para `ExactOne` e `AtLeastOne`.

## Diretrizes
- **Performance:** Evitar alocações desnecessárias onde possível, mas priorizar a legibilidade e segurança.
- **Robustez:** O parser não pode "quebrar" com input sujo. Ele deve rejeitar o input de forma controlada (Exceção).
- **Cultura:** Sempre considerar `InvariantCulture` para parsing de números para garantir consistência entre sistemas operacionais.

## Referência ao PRD
- **RF01, RF03, RF04, RF05, RF07, RF08.**
