# CleanConsole.Parse 🚀

> **A maneira limpa, declarativa e robusta de criar aplicações CLI em .NET 10.**

![License](https://img.shields.io/badge/license-MIT-blue.svg) ![Platform](https://img.shields.io/badge/platform-.NET%2010-purple.svg) ![Status](https://img.shields.io/badge/status-stable-green.svg)

O **CleanConsole.Parse** é uma biblioteca projetada para eliminar o código repetitivo (boilerplate) na leitura de argumentos de linha de comando. Ao invés de escrever loops complexos para analisar `string[] args`, você simplesmente decora uma classe C# com atributos, e a biblioteca cuida de todo o resto: parsing, validação de tipos, regras de negócio e geração de ajuda.

---

## 🎯 Por que usar?

Ideal para Desenvolvedores e Engenheiros de DevOps que constroem ferramentas de automação, CLIs ou utilitários de sistema.

*   **Declarativo:** Defina suas opções diretamente na classe (POCO). O código documenta a si mesmo.
*   **Type-Safe:** Conversão automática e segura para `int`, `double`, `bool` e `string`.
*   **Zero Dependências:** Biblioteca extremamente leve, sem dependências de terceiros (NuGet limpo).
*   **Validação Poderosa:** Regras de grupo complexas como "Escolha exatamente um" ou "Pelo menos um" configuráveis via atributos.
*   **UX Nativa:** Gera telas de ajuda (`--help`) formatadas e mensagens de erro amigáveis para o usuário final.

---

## 📦 Instalação

Atualmente, o projeto pode ser utilizado via referência direta ou build local.

```bash
# Clone o repositório
git clone https://github.com/seu-usuario/CleanConsole.Parse.git

# Adicione referência ao seu projeto .NET 10
dotnet add reference ../CleanConsole.Parse/CleanConsole.Parse.csproj
```

---

## ⚡ Exemplo Rápido (Quick Start)

Transforme uma classe simples em uma interface de linha de comando completa em minutos.

### 1. Defina seus Argumentos
Crie uma classe e use os atributos `[ProgramDefinition]` e `[Option]`.

```csharp
using CleanConsole.Parse;

[ProgramDefinition(Name = "FileCompressor", Description = "Ferramenta CLI para compressão de arquivos.")]
public class CompressionOptions
{
    [Option("input", ShortOptionName = "i", Description = "Caminho do arquivo de entrada")]
    public string InputFile { get; set; }

    [Option("level", Description = "Nível de compressão (1-9)")]
    public int CompressionLevel { get; set; } = 5; // Valor padrão

    [Option("verbose", ShortOptionName = "v", Description = "Habilita logs detalhados")]
    public bool Verbose { get; set; }
}
```

### 2. Processe no `Main`
No seu `Program.cs`, consuma o `ParseResult<CompressionOptions>`.

```csharp
var result = CleanParser.Parse<CompressionOptions>(args);

if (result.HelpRequested)
{
    Console.WriteLine(result.GetHelpDescription());
    return;
}

if (result.HasErrors)
{
    Console.Error.WriteLine("Ocorreram erros:");
    foreach (var error in result.Errors)
    {
        Console.Error.WriteLine($"- {error.Message}");
    }
    return;
}

var options = result.Options!;
Console.WriteLine($"Comprimindo '{options.InputFile}' (Nível {options.CompressionLevel})...");
Console.WriteLine();
Console.WriteLine(result.GetSelectedSummary());
```

### 3. Resultado
Se o usuário executar:
`myapp.exe --input=dados.dat --level=9 -v`

O objeto `options` será:
*   `InputFile`: "dados.dat"
*   `CompressionLevel`: 9
*   `Verbose`: true

E `result.GetSelectedSummary()` produzirá um resumo pronto para logar.

---

## 🛡️ Validação Avançada (Grupos)

Precisa garantir regras de negócio complexas? Use `[OptionGroup]`.

Exemplo: Um sistema de login que exige **Usuário+Senha** OU **Token**, mas nunca ambos.

```csharp
[OptionGroup("AuthMethod", OptionGroupRequirement.ExactOne)]
public class LoginOptions
{
    [Option("token", Group = "AuthMethod")]
    public string? AuthToken { get; set; }

    [Option("user", Group = "AuthMethod")]
    public string? Username { get; set; }
}
// result.HasErrors será verdadeiro se o usuário fornecer ambos ou nenhum.
```

Também é possível exigir que todas as opções de um conjunto sejam fornecidas usando `OptionGroupRequirement.All`.

```csharp
[OptionGroup("Sync", OptionGroupRequirement.All)]
public class SyncOptions
{
    [Option("source", Group = "Sync")]
    public string? Source { get; set; }

    [Option("target", Group = "Sync")]
    public string? Target { get; set; }

    [Option("audit", Group = "Sync")]
    public bool Audit { get; set; }
}
// Um ParseResult com HasErrors = true indicará as opções faltantes.
```

---

## 📚 Documentação Completa

Para aprofundar seu conhecimento, consulte nossa documentação técnica na pasta `docs/`:

*   [**Índice da Documentação**](docs/README.md)
*   [**Primeiros Passos**](docs/GETTING_STARTED.md) - Guia passo-a-passo.
*   [**Referência da API**](docs/API_REFERENCE.md) - Lista completa de atributos.
*   [**Arquitetura**](docs/ARCHITECTURE.md) - Como funciona "por baixo do capô".
*   [**Boas Práticas**](docs/BEST_PRACTICES.md) - Dicas de design CLI.

---

## 🤖 Estrutura do Projeto e Agentes

Este projeto utiliza uma metodologia única baseada em **Agentes Especialistas**. A manutenção e evolução do código são guiadas por "personas" técnicas definidas na pasta `agents/`.

*   **Architect:** Cuida da estrutura e API.
*   **Core Engineer:** Otimiza o algoritmo de parsing.
*   **UX Specialist:** Garante que as mensagens e docs sejam claros.
*   **QA Engineer:** Mantém a bateria de testes.

Consulte o arquivo [AGENTS.md](AGENTS.md) para entender como o projeto é organizado.

---

## 🤝 Contribuindo

Contribuições são bem-vindas! Se você encontrou um bug ou tem uma ideia de funcionalidade:
1.  Abra uma Issue.
2.  Faça um Fork e envie um Pull Request.
3.  Certifique-se de adicionar testes (veja `docs/CONTRIBUTING.md`).

## Licença

Este projeto está licenciado sob a licença MIT - veja o arquivo LICENSE para detalhes.