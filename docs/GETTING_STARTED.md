# Primeiros Passos (Getting Started)

Este guia mostra como configurar e usar o **CleanConsole.Parse** em sua aplicação .NET.

## Instalação
*(Placeholder para comando NuGet)*
```bash
dotnet add package CleanConsole.Parse
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

## Formatos de Comando Suportados

A biblioteca é flexível e aceita:

*   `-i:arquivo.txt`
*   `-i=arquivo.txt`
*   `--input:arquivo.txt`
*   `--input=arquivo.txt`
*   `/input=arquivo.txt`

**Atenção:** Espaços não são permitidos (ex: `-i arquivo.txt` é **inválido**).
