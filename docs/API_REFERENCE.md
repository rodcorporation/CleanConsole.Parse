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
| `Description` | `string` | Não | Descrição da opção para exibição na ajuda. |

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
| `require` | `OptionGroupRequirement` | Sim | A regra de validação a ser aplicada (`ExactOne`, `AtLeastOne`, `None`, `AtMostOne` ou `All`). |
| `Description` | `string` | Não | Descrição do grupo para exibição na ajuda. |

## Enums

### `OptionGroupRequirement`
Define o comportamento de validação de um grupo.

*   **`ExactOne`**: Exatamente uma das opções do grupo deve ser fornecida. Erro se 0 ou >1.
*   **`AtLeastOne`**: Pelo menos uma opção deve ser fornecida. Erro se 0.
*   **`None`**: Nenhuma opção do grupo é obrigatória. Qualquer número de opções (0 a N) pode ser fornecido.
*   **`AtMostOne`**: Nenhuma opção do grupo é obrigatória, mas no máximo uma pode ser fornecida. Erro se >1.
*   **`All`**: Todas as opções do grupo devem ser fornecidas. Erro se qualquer membro estiver ausente.

## Resultados do Parser

### `ParseResult<T>`
Retorno padrão de `CleanParser.Parse<T>()`. Encapsula o objeto de opções, erros e payload de ajuda.

| Propriedade | Tipo | Descrição |
| :--- | :--- | :--- |
| `Options` / `Value` | `T?` | Instância populada quando o parsing conclui; nula em falhas. |
| `IsSuccess` | `bool` | Verdadeiro quando não houve `HasErrors` nem `HelpRequested`. |
| `HasErrors` | `bool` | Indica se a coleção `Errors` contém itens. |
| `Errors` | `IReadOnlyList<ParseError>` | Lista de erros agregados na execução. |
| `HelpRequested` | `bool` | Verdadeiro quando o usuário solicitou ajuda (`--help`, `-h`, `/?`). |
| `Help` | `ParseHelpPayload?` | Payload estruturado usado por `GetHelpDescription()`. |
| `GetHelpDescription()` | `string` | Produz o texto de ajuda conforme especificação de UX. |
| `GetSelectedSummary()` | `string` | Retorna um resumo formatado das opções selecionadas. |

### `ParseError`
Representa um erro coletado durante o parsing.

| Propriedade | Tipo | Descrição |
| :--- | :--- | :--- |
| `Kind` | `ParseErrorKind` | Categoria do erro (configuração, conversão, sintaxe, regras de grupo ou help). |
| `Message` | `string` | Mensagem pronta para exibição ao usuário. |
| `OptionName` | `string?` | Nome da opção ou grupo relacionado ao erro, quando aplicável. |

### `ParseErrorKind`
Enumeração que categoriza erros para diagnóstico rápido.

* `Configuration`
* `Conversion`
* `Syntax`
* `GroupRule`
* `HelpRequest`

### `ParseHelpPayload`
Describe a estrutura bruta usada para montar a saída de ajuda. Normalmente você não instancia esta classe diretamente, mas pode inspecionar as propriedades quando `HelpRequested` for verdadeiro.

| Propriedade | Tipo | Descrição |
| :--- | :--- | :--- |
| `Title` / `Description` | `string?` | Metadados vindos de `[ProgramDefinition]`. |
| `Usage` | `string?` | Linha de uso padrão (`app [options]`). |
| `Options` | `IReadOnlyList<ParseHelpOption>` | Opções renderizadas na ajuda. |
| `Groups` | `IReadOnlyList<ParseHelpGroup>` | Grupos com descrições e regras. |
| `Examples` | `IReadOnlyList<string>` | Exemplos adicionais definidos pelo integrador. |

## Exceções

### `CleanParserException`
`Parse<T>` retorna `ParseResult<T>` e não lança mais `CleanParserException` para erros de entrada. A exceção permanece disponível para cenários de configuração incorreta detectados em tempo de inicialização (ex.: chamadas diretas a `CleanParser.GetHelpText<T>()` ou utilitários internos que validam atributos sem contexto de execução). Use-a para identificar falhas críticas de configuração durante o desenvolvimento.
