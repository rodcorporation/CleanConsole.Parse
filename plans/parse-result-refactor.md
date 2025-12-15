# Plano: Transformar Parse<T> em ParseResult<T>

## Épico 1 - Descoberta e Alinhamento de Contrato
1.1 Auditar CleanParser.Parse<T>() em [CleanConsole.Parse/CleanParser.cs](CleanConsole.Parse/CleanParser.cs) para documentar fluxos de sucesso, help e exceção.
1.2 Inventariar dependências e expectativas atuais nos testes de [CleanConsole.Parse.Tests](CleanConsole.Parse.Tests) e em atributos de [CleanConsole.Parse/Attributes](CleanConsole.Parse/Attributes).
1.3 Definir com arquitetura e UX a estrutura final de ParseResult<T> (nomes, tipos e semântica de Help, HasErrors, Errors e Options/Value).
1.4 Especificar requisitos de formato para GetHelpDescription e GetSelectedSummary, alinhando estilo de console com o agente UX.

## Épico 2 - Infraestrutura do ParseResult
2.1 Introduzir a classe ParseResult<T> com propriedades Help, HasErrors, Errors e Options/Value documentadas.
2.2 Ajustar assinatura de Parse<T>() para retornar ParseResult<T> sem quebrar compatibilidade de namespaces ou atributos.
2.3 Implementar fábrica interna para montar ParseResult<T> em fluxos de sucesso e help preservando o valor parseado.

## Épico 3 - Tratamento de Help e Saída
3.1 Atualizar detecção de help para preencher ParseResult.Help e evitar lançamentos de CleanParserException.
3.2 Implementar GetHelpDescription com formatação rica aproveitando utilitários existentes; validar com UX.
3.3 Implementar GetSelectedSummary listando apenas opções marcadas, respeitando grupos e aliases configurados.

## Épico 4 - Gestão de Erros
4.1 Mapear todos os pontos de CleanParser que hoje lançam CleanParserException ou outras exceções de parsing.
4.2 Converter cada ponto mapeado para preencher ParseResult.Errors e HasErrors, mantendo mensagens consistentes.
4.3 Garantir que erros de validação de grupos e requisitos em [CleanConsole.Parse/Attributes](CleanConsole.Parse/Attributes) também alimentem ParseResult.

## Épico 5 - Adequação de Consumidores Internos
5.1 Revisar métodos auxiliares de parsing que assumem retorno direto ou exceção, adequando-os ao novo contrato.
5.2 Atualizar fluxos internos que dependem de CleanParserException para interpretar ParseResult.HasErrors.

## Épico 6 - Validação Automatizada
6.1 Atualizar testes existentes de sucesso, erro e help para inspecionar ParseResult em vez de capturar exceções.
6.2 Adicionar cenários cobrindo múltiplos erros, ausência de seleção e formatação de help/summary.
6.3 Introduzir testes específicos para garantir que ParseResult nunca lança exceções públicas e mantém mensagens legíveis.
6.4 Revisar cobertura e garantir que pipelines/CI executem a nova suíte completa.

## Épico 7 - Documentação e Comunicação
7.1 Atualizar docs/API_REFERENCE.md e docs/GETTING_STARTED.md com exemplos usando ParseResult.
7.2 Registrar notas de migração em docs/BEST_PRACTICES.md ou README.md explicando a mudança de contrato e passos de adoção.
7.3 Preparar comunicado de release descrevendo impacto, benefícios e ações recomendadas para integradores.
7.4 Revisar docs/README.md e README.md raiz para refletir a nova experiência de parsing e apontar para o ParseResult.

## Épico 8 - Fechamento
8.1 Executar a suíte de testes completa garantindo ausência de regressões e gerar relatório.
8.2 Validar manualmente cenários críticos (help pedido, erro múltiplo, execução limpa) para confirmar experiência final.
