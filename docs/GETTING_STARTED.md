# Primeiros Passos (Getting Started)

Este guia mostra como configurar e usar o **CleanConsole.Parse** em sua aplicação .NET.

## Instalação

Como este projeto ainda não está publicado no NuGet.org, você pode utilizá-lo de duas formas:

1.  **Referência de Projeto (Local):** Adicione o projeto `CleanConsole.Parse.csproj` à sua solução e referencie-o diretamente.
2.  **Build Local:** Gere o pacote `.nupkg` localmente (`dotnet pack`) e configure uma fonte de pacotes local.

```bash
# Exemplo de adição via referência de projeto
dotnet add reference ../CleanConsole.Parse/CleanConsole.Parse.csproj
```

## Exemplo Rápido

### 1. Defina sua Classe de Argumentos
Crie uma classe POCO e decore-a com os atributos da biblioteca.

```csharp
using CleanConsole.Parse;

[ProgramDef(Name = "MeuApp", Description = "Exemplo de CLI.")]
public class MyArgs
{
    [Option(OptionName = "input", ShortOptionName = "i")]
    public string InputFile { get; set; }

    [Option(OptionName = "retry")]
    public int RetryCount { get; set; } = 3; // Valor padrão

    [Option(OptionName = "verbose", ShortOptionName = "v")]
    public bool Verbose { get; set; }
}
```

### 2. Realize o Parsing no `Program.cs`

```csharp
using CleanConsole.Parse;

try 
{
    // args vem do método Main(string[] args)
    var arguments = CleanParser.Parse<MyArgs>(args);

    Console.WriteLine($"Processando arquivo: {arguments.InputFile}");
    if (arguments.Verbose) Console.WriteLine("Modo verboso ativado.");
}
catch (CleanParserException ex)
{
    // O erro já vem formatado para o usuário
    Console.WriteLine($"Erro: {ex.Message}");
    
    // Imprime o ajuda automaticamente
    Console.WriteLine(CleanParser.GetHelpText<MyArgs>());
}
```

## Exemplo: Grupos com Regra `All`

Quando um conjunto de argumentos só faz sentido completo (por exemplo, caminho + tentativas + modo verboso habilitado juntos), utilize `OptionGroupRequirement.All`. Todas as opções marcadas com o mesmo grupo precisam aparecer na linha de comando; o parser indicará quais ficaram faltando.

```csharp
[ProgramDefinition(Name = "SyncTool", Description = "Sincronizador de diretórios.")]
[OptionGroup("SyncConfig", OptionGroupRequirement.All, Description = "Parâmetros obrigatórios da sincronização")]
[OptionGroup("Mode", OptionGroupRequirement.AtLeastOne, Description = "Escolha pelo menos um modo")]
public class SyncOptions
{
    [Option("source", Group = "SyncConfig", Description = "Pasta de origem")]
    public string Source { get; set; } = string.Empty;

    [Option("target", Group = "SyncConfig", Description = "Pasta de destino")]
    public string Target { get; set; } = string.Empty;

    [Option("audit", Group = "SyncConfig", Description = "Habilita auditoria detalhada")]
    public bool Audit { get; set; }

    [Option("mirror", Group = "Mode", Description = "Espelha destino com base na origem")]
    public bool Mirror { get; set; }

    [Option("backup", Group = "Mode", Description = "Gera cópia incremental")]
    public bool Backup { get; set; }
}
```

Uso típico:

```bash
sync.exe --source:"c:/dados" --target:"d:/mirror" --audit --mirror
```

Se qualquer opção do grupo `SyncConfig` estiver ausente, o `CleanParser` lançará uma `CleanParserException` indicando exatamente quais argumentos faltaram (ex.: `--source`, `--target`).

## Formatos de Comando Suportados

A biblioteca é flexível e aceita:

*   `-i:arquivo.txt`
*   `-i=arquivo.txt`
*   `--input:arquivo.txt`
*   `--input=arquivo.txt`
*   `/input=arquivo.txt`

**Atenção:** Espaços não são permitidos (ex: `-i arquivo.txt` é **inválido**).
