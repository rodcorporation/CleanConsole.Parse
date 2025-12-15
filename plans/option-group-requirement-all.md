# Plano: OptionGroupRequirement.All

## Objetivo
Garantir que grupos de opções marcados com `OptionGroupRequirement.All` exijam que todas as opções pertencentes ao grupo sejam informadas pelo usuário, cobrindo parsing, validações, testes automatizados e documentação.

## Agente Responsável
Core Engineer — devido ao impacto direto na lógica de parsing, regras de validação e suíte de testes.

## Épicos e Tarefas

### Épico 1 — Preparação
1.1 Ler `CleanParser` e `GroupTests` para entender a validação atual de grupos.
1.2 Listar opções/grupos já usados em testes para reaproveitamento.

### Épico 2 — Enumeração
2.1 Adicionar `All` em `OptionGroupRequirement` com comentário XML claro.

### Épico 3 — Configuração e Helper
3.1 Criar helper interno (ex.: `GroupOptionMap`) que devolve coleção grupo → opções.
3.2 Ajustar `CleanParser.ValidateConfiguration` para usar o helper, contando opções por grupo.
3.3 Bloquear configuração onde um grupo `All` não possui opções.

### Épico 4 — Parsing e Rastreamento
4.1 Refatorar `CleanParser.Parse` para usar o helper ao mapear opções e registrar quais foram preenchidas.
4.2 Implementar regra `All`: todos os membros devem ser passados (último valor ganha).
4.3 Montar mensagem de erro listando opções ausentes usando dados do helper.

### Épico 5 — Mensagens e Summaries
5.1 Revisar mensagens de erro dos outros requisitos para manter padrão.
5.2 Confirmar que `PrintSummary` mostra valores corretos quando `All` é atendido.
5.3 Atualizar `GetHelpText` para exibir o requisito `All` de maneira consistente.

### Épico 6 — Testes Automatizados
6.1 Criar teste positivo com todas as opções do grupo `All` preenchidas (tipos variados).
6.2 Criar testes negativos: nenhuma opção, opção faltante, mistura com outros grupos.
6.3 Validar que mensagens indicam lista de opções ausentes.
6.4 Manter casos em `GroupTests` ou nova classe se simplificar leitura.

### Épico 7 — Documentação
7.1 Atualizar `docs/API_REFERENCE.md` e `docs/ARCHITECTURE.md` com a regra `All`.
7.2 Escrever exemplo completo em `docs/GETTING_STARTED.md` combinando `All` com outro requisito.
7.3 Incluir recomendações em `docs/BEST_PRACTICES.md` e mencionar no `README.md` se relevante.

### Épico 8 — Finalização
8.1 Rodar `dotnet test` e garantir sucesso geral.
8.2 Atualizar changelog/notas de versão caso existam.