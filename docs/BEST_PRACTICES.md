# Boas Práticas

Para extrair o máximo do **CleanConsole.Parse**, siga estas recomendações:

## Design de CLI
1.  **Use `ShortOptionName` com parcimônia:** Reserve nomes curtos (`-f`, `-o`) apenas para as opções mais usadas.
2.  **Valores Padrão:** Inicialize as propriedades na classe C# (ex: `public int Retries { get; set; } = 3;`). O parser respeitará o valor padrão se o argumento não for fornecido (a menos que seja obrigatório por um Grupo).
3.  **Nomes Claros:** Evite abreviações obscuras em `OptionName`. `--output-directory` é melhor que `--out-dir`.
4.  **Agrupe Dependências:** Quando um conjunto de opções só faz sentido completo (ex.: `--source`, `--target`, `--audit`), utilize um `[OptionGroup]` com `OptionGroupRequirement.All` para evitar configurações parciais.

## Segurança e Robustez
1.  **Trate o `ParseResult`:** Prefira o padrão `var result = CleanParser.Parse<T>(args)` e teste `result.HelpRequested` e `result.HasErrors` antes de acessar `result.Options`. Reserve `CleanParserException` para falhas estruturais (ex.: configuração inválida carregada dinamicamente).
2.  **Não confie no Input:** Mesmo tipado, valide lógica de negócio adicional (ex: se o arquivo existe) *após* o parsing.
3.  **Relate Erros de Forma Amigável:** Ao iterar sobre `result.Errors`, agrupe mensagens por `error.OptionName` para ajudar o usuário a corrigir múltiplos problemas de uma só vez.

## Versionamento
O `[ProgramDefinition]` não possui campo de versão explícito. Recomenda-se imprimir a versão do Assembly no início da execução da sua CLI.
