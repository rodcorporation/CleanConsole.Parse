# Boas Práticas

Para extrair o máximo do **CleanConsole.Parse**, siga estas recomendações:

## Design de CLI
1.  **Use `ShortOptionName` com parcimônia:** Reserve nomes curtos (`-f`, `-o`) apenas para as opções mais usadas.
2.  **Valores Padrão:** Inicialize as propriedades na classe C# (ex: `public int Retries { get; set; } = 3;`). O parser respeitará o valor padrão se o argumento não for fornecido (a menos que seja obrigatório por um Grupo).
3.  **Nomes Claros:** Evite abreviações obscuras em `OptionName`. `--output-directory` é melhor que `--out-dir`.

## Segurança e Robustez
1.  **Bloco Try-Catch:** Sempre envolva a chamada do `Parse` em um bloco `try-catch` capturando `CleanParserException`.
2.  **Não confie no Input:** Mesmo tipado, valide lógica de negócio adicional (ex: se o arquivo existe) *após* o parsing.

## Versionamento
O `[ProgramDef]` não possui campo de versão explícito. Recomenda-se imprimir a versão do Assembly no início da execução da sua CLI.
