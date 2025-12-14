# CleanConsole.Parse 🚀

**CleanConsole.Parse** é uma biblioteca .NET 10 leve, declarativa e *type-safe* para parsing de argumentos de linha de comando (CLI).

Diferente de outras bibliotecas que exigem configurações fluentes complexas, o **CleanConsole.Parse** foca na simplicidade do **mapeamento direto Classe-Argumento** via Atributos.

## 🌟 Destaques

*   **Declarativo:** Configure tudo via atributos (`[Option]`, `[OptionGroup]`) na sua classe POCO.
*   **Type-Safe:** Conversão automática para `int`, `double`, `bool`.
*   **Zero Dependências:** Biblioteca leve, sem dependências externas.
*   **Validação Rica:** Regras de grupo (`ExactOne`, `AtLeastOne`) e validação de sintaxe estrita.
*   **Mensagens Amigáveis:** Erros claros e acionáveis para o usuário final.

## 📚 Documentação

A documentação completa está disponível na pasta `docs/`:

*   [**Primeiros Passos**](docs/GETTING_STARTED.md): Como instalar e criar seu primeiro CLI.
*   [**Referência da API**](docs/API_REFERENCE.md): Detalhes de todos os Atributos e Configurações.
*   [**Arquitetura**](docs/ARCHITECTURE.md): Como funciona o motor de parsing internamente.
*   [**Boas Práticas**](docs/BEST_PRACTICES.md): Dicas para criar CLIs robustas.

## 📦 Exemplo Rápido

```csharp
[ProgramDef(Name = "Compressor", Description = "Comprime arquivos.")]
public class Options
{
    [Option(OptionName = "source", ShortOptionName = "s")]
    public string SourceFile { get; set; }

    [Option(OptionName = "level", ShortOptionName = "l")]
    public int Level { get; set; } = 5;
}

// Uso:
var opts = CleanParser.Parse<Options>(args);
```

## 🤝 Contribuindo

Contribuições são bem-vindas! Consulte o [Guia de Contribuição](docs/CONTRIBUTING.md) para começar.

## Licença

MIT
