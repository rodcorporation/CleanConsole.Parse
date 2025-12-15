# Product Requirements Document (PRD)
## Projeto: CLI Argument Parser (.NET 10)

### 1. Visão Geral do Produto
Biblioteca desenvolvida em **.NET 10 (C#)** para realizar o parsing de argumentos de linha de comando. A biblioteca mapeia entradas textuais (`string[] args`) diretamente para propriedades de uma classe (POCO), utilizando **Attributes** para configuração, validação e definição de regras de negócio complexas.

---

### 2. Requisitos Funcionais (RF)

#### RF01 - Motor de Mapeamento (Reflection)
O sistema deve receber um array de strings (os argumentos) e um tipo de classe de destino.
* O motor deve instanciar a classe alvo.
* O motor deve iterar sobre as propriedades da classe, identificando aquelas decoradas com o atributo `[Option]`.
* O motor deve atribuir os valores capturados na CLI às propriedades correspondentes via *Reflection*.

#### RF02 - Geração Automática de Ajuda
O sistema deve ser capaz de gerar um texto de ajuda automaticamente quando solicitado ou em caso de erro fatal (se configurado).
* O texto deve conter: Nome e Descrição do Programa, Lista de opções (`OptionName`/`ShortOptionName`) e indicação de obrigatoriedade/grupos.

#### RF03 - Conversão de Tipos de Dados
O parser deve realizar a conversão segura (type-safe) dos valores para os tipos das propriedades .NET.
* **Tipos Suportados:**
    * **`string`:** Texto literal.
    * **`int`:** Números inteiros (`Int32`).
    * **`double`:** Números decimais (`Double`).
    * **`bool`:** Flags lógicas (Presença = `true`).

#### RF04 - Sintaxe Estrita de Comandos
O parser deve aceitar e validar formatos específicos de entrada, rejeitando variações ambíguas.

1.  **Prefixos Aceitos:**
    * Hífen simples: `-p`
    * Hífen duplo: `--port`
    * Barra: `/port`
2.  **Separadores de Valor:**
    * Dois pontos: `:` (ex: `-p:80`)
    * Sinal de igual: `=` (ex: `-p=80`)
3.  **Proibição de Espaços (Regra Crítica):**
    * É **proibido** usar espaço para separar o nome do argumento do seu valor.
    * *Inválido:* `-p 80`
    * **Ação:** Lançar exceção imediata se detectado token solto que deveria ser valor de uma opção anterior.

#### RF05 - Parse de Booleanos (Flags)
Propriedades do tipo `bool` funcionam como "Flags".
* Não exigem valor explícito. A presença do argumento define a propriedade como `true`.
* Se fornecido valor explícito (ex: `--verbose:false`), o parser deve respeitar.

#### RF06 - Definição de Parâmetros (Atributo `[Option]`)
As propriedades mapeáveis devem ser decoradas com `[Option]`.
* **Propriedades do Atributo:**
    1.  **`OptionName` (Obrigatório):** Identificador longo **sem prefixos** (ex: "output").
    2.  **`ShortOptionName` (Opcional):** Identificador curto **sem prefixos** (ex: "o"). Pode ser nulo.
    3.  **`Group` (Opcional):** Nome do grupo ao qual pertence.
* **Busca:** O parser deve tentar o *match* adicionando os prefixos (`-`, `--`, `/`) aos nomes definidos.

#### RF07 - Regras de Grupo (Atributo `[OptionGroup]`)
Define regras combinatórias em nível de Classe.
* **Propriedades do Atributo:**
    1.  **`Name`:** Identificador único do grupo.
    2.  **`GroupType`:** Tipo da validação (Enum).
* **Tipos (`OptionGroupRequirement`):**
    * `ExactOne`: **Exatamente uma** propriedade do grupo deve ser preenchida.
    * `AtLeastOne`: **Pelo menos uma** propriedade do grupo deve ser preenchida.

#### RF08 - Tratamento de Erros e Exceções Objetivas
O sistema deve lançar exceções com mensagens **claras e acionáveis** (Actionable Error Messages).

* **Erro de Espaço:** "Erro de sintaxe no argumento '{0}'. Espaços não são permitidos. Use o formato '{0}:valor' ou '{0}=valor' para corrigir."
* **Valor Ausente:** "O argumento '{0}' exige um valor, mas nenhum foi fornecido."
* **Tipo Inválido:** "O valor '{0}' não é válido para o argumento '{1}'. Esperava-se um '{2}'."
* **Violação ExactOne:** "Conflito de opções: O grupo '{0}' exige exatamente uma opção, mas foram fornecidas: {1}."
* **Violação AtLeastOne:** "Requisito não atendido: Pelo menos uma opção do grupo '{0}' deve ser fornecida."

#### RF09 - Validação de Metadados (Start-up Check)
Validação da integridade da classe antes do parsing. Erros de inicialização se:
* Houver `OptionName` ou `ShortOptionName` duplicados.
* Uma propriedade referenciar um grupo não definido em `[OptionGroup]`.
* Houver `[OptionGroup]` com nomes duplicados.

#### RF10 - Configuração e Resumo (Atributo `[ProgramDefinition]`)
Metadados da aplicação em nível de classe.
* **Propriedades:** `Name`, `Description`, `PrintSummary`.
* **Comportamento:** Se `PrintSummary = true`, imprimir no Console um resumo das opções ativadas e seus valores após o parsing.

---

### 3. Matriz de Cobertura de Testes (QA)

| ID | Categoria | Cenário | Resultado Esperado |
|:---|:---|:---|:---|
| **T01** | **Sintaxe** | Formatos `-p=10`, `/p:10`, `--port=10` | **Sucesso**: Propriedade populada. |
| **T02** | **Sintaxe (Erro)** | `-p 10` (uso de espaço) | **Exceção**: Mensagem orientando usar `=` ou `:`. |
| **T03** | **Sintaxe (Erro)** | `-p` (sem valor, tipo int) | **Exceção**: Mensagem de valor ausente. |
| **T04** | **Tipos** | Int, Double e Bool válidos | **Sucesso**. |
| **T05** | **Tipos (Erro)** | Texto alfanumérico em campo Int | **Exceção**: Mensagem de conversão inválida. |
| **T06** | **Grupos** | `ExactOne`: Nenhuma opção passada | **Exceção**: Requisito não atendido. |
| **T07** | **Grupos** | `ExactOne`: 2 ou mais opções | **Exceção**: Ambiguidade/Conflito. |
| **T08** | **Grupos** | `AtLeastOne`: 1 ou mais opções | **Sucesso**. |
| **T09** | **Busca** | Definição `p`, Input `-p` ou `/p` | **Sucesso**: Match independente do prefixo. |
| **T10** | **Resumo** | `PrintSummary = true` | **Sucesso**: Output no console com lista de valores. |

---

### 4. Referência Técnica (Exemplo C#)

```csharp
using System;

// RF10 - Definição do Programa
[ProgramDefinition(
    Name = "DataProcessor",
    Description = "Processador de arquivos batch.",
    PrintSummary = true
)]
// RF07 - Definição de Grupo na Classe
[OptionGroup(Name = "InputConfig", GroupType = OptionGroupRequirement.ExactOne)]
public class ApplicationArgs
{
    // RF06 - Nomes limpos (sem traços)
    [Option(OptionName = "file", ShortOptionName = "f", Group = "InputConfig")]
    public string? FilePath { get; set; }

    [Option(OptionName = "url", ShortOptionName = "u", Group = "InputConfig")]
    public string? UrlPath { get; set; }

    [Option(OptionName = "retry", ShortOptionName = "r")]
    public int RetryCount { get; set; }

    // RF05 - Flag
    [Option(OptionName = "verbose", ShortOptionName = "v")]
    public bool IsVerbose { get; set; }
}

/* Exemplos de Execução:
   > app.exe -f=dados.txt -r:5 /v       (VÁLIDO)
   > app.exe --url="[http://api.com](http://api.com)"     (VÁLIDO)
   > app.exe -r 5                       (INVÁLIDO - Espaço)
*/