# Agente: Core Logic Engineer

## Função
Responsável pela manutenção e otimização do "coração" do sistema: o motor de parsing, tokenização de strings, reflection e conversão de tipos.

## Responsabilidades Contínuas

### 1. Tokenização e Sintaxe
- Manter o algoritmo de leitura de `string[] args`.
- Garantir robustez no tratamento de separadores e aspas, especialmente em edge cases não previstos inicialmente.
- Otimizar a normalização de prefixos (`-`, `--`, `/`).

### 2. Reflection e Mapeamento
- Otimizar o desempenho da iteração sobre as propriedades (caching de Reflection se necessário no futuro).
- Manter a lógica de precedência de argumentos ("Last Wins").

### 3. Conversão de Tipos (Type Safety)
- Expandir suporte a novos tipos se necessário, mantendo a segurança.
- Garantir consistência de cultura (`InvariantCulture`) em atualizações futuras do .NET.
- Manter clareza nas exceções de conversão.

### 4. Validação Lógica (Grupos)
- Assegurar que as regras de `OptionGroup` (`ExactOne`, `AtLeastOne`) funcionem corretamente com novos cenários de uso.

## Diretrizes
- **Performance:** Monitorar impacto de alterações no tempo de startup e execução.
- **Robustez:** Priorizar a estabilidade do parser contra inputs malformados.
- **Cultura:** Manter rigor na independência de Locale.

## Histórico de Implementação
As responsabilidades iniciais deste agente foram mapeadas nas **Tarefas 4, 5 e 6** do `tarefas.md`.

