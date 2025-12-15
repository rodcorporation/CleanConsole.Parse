# Referência da API

Esta documentação detalha os atributos e classes públicas disponíveis para uso na biblioteca **CleanConsole.Parse**.

## Atributos

### `[ProgramDefinition]`
Define os metadados da aplicação CLI. Deve ser aplicado na classe de opções.

| Propriedade | Tipo | Obrigatório | Descrição |
| :--- | :--- | :--- | :--- |
| `Name` | `string` | Sim | Nome da aplicação (ex: "FileUploader"). |
| `Description` | `string` | Sim | Breve descrição do que a ferramenta faz. |
| `PrintSummary` | `bool` | Não | Se `true`, imprime os valores parseados no console ao final. Padrão: `false`. |

---

### `[Option]`
Marca uma propriedade para ser preenchida via argumento de linha de comando.

| Propriedade | Tipo | Obrigatório | Descrição |
| :--- | :--- | :--- | :--- |
| `OptionName` | `string` | Sim | O nome longo do argumento (ex: `port` para `--port`). **Não use prefixos.** |
| `ShortOptionName` | `string` | Não | O alias curto (ex: `p` para `-p`). **Não use prefixos.** |
| `Group` | `string` | Não | Nome do grupo de validação ao qual esta opção pertence. |

**Tipos Suportados:**
*   `string`
*   `int`
*   `double` (formato com ponto `.`)
*   `bool` (flags)

---

### `[OptionGroup]`
Define regras de validação para conjuntos de propriedades. Deve ser aplicado na classe.
O construtor aceita o nome do grupo e seu requisito de validação.

| Parâmetro | Tipo | Obrigatório | Descrição |
| :--- | :--- | :--- | :--- |
| `name` | `string` | Sim | Identificador único do grupo (referenciado em `[Option]`). |
| `require` | `OptionGroupRequirement` | Sim | A regra de validação a ser aplicada (`ExactOne`, `AtLeastOne`, `None`, ou `AtMostOne`). |

## Enums

### `OptionGroupRequirement`
Define o comportamento de validação de um grupo.

*   **`ExactOne`**: Exatamente uma das opções do grupo deve ser fornecida. Erro se 0 ou >1.
*   **`AtLeastOne`**: Pelo menos uma opção deve ser fornecida. Erro se 0.
*   **`None`**: Nenhuma opção do grupo é obrigatória. Qualquer número de opções (0 a N) pode ser fornecido.
*   **`AtMostOne`**: Nenhuma opção do grupo é obrigatória, mas no máximo uma pode ser fornecida. Erro se >1.

## Exceções

### `CleanParserException`
Todas as exceções lançadas pela biblioteca herdam desta classe. Capture-a para exibir mensagens de erro amigáveis ao usuário final. A mensagem da exceção já está formatada para exibição (Actionable Error Message).
