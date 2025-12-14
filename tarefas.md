# Lista de Tarefas do Projeto CLI Argument Parser

## 1. Configuração do Projeto
- [x] **1.1 Criar Solução:** Criar solução .NET 10 (`CleanConsole.Parse.sln`).
- [x] **1.2 Criar Projeto Core:** Criar projeto de biblioteca de classes (`CleanConsole.Parse`) rodando em .NET 10.
- [x] **1.3 Criar Projeto de Testes:** Criar projeto de testes unitários (`CleanConsole.Parse.Tests`) usando xUnit.
- [x] **1.4 Configurar Referências:** Adicionar referência do projeto Core no projeto de Testes.

## 2. Definição de Metadados (Atributos)
- [x] **2.1 Implementar `[ProgramDef]`:** Criar atributo com propriedades `Name` (string), `Description` (string), e `PrintSummary` (bool) conforme RF10.
- [x] **2.2 Implementar `OptionGroupType`:** Criar Enum com valores `ExactOne` e `AtLeastOne` conforme RF07.
- [x] **2.3 Implementar `[OptionGroup]`:** Criar atributo de classe com `Name` (string) e `GroupType` (Enum) conforme RF07.
- [x] **2.4 Implementar `[Option]`:** Criar atributo de propriedade com `OptionName` (obrigatório), `ShortOptionName` (opcional), e `Group` (opcional) conforme RF06.

## 3. Estrutura do Parser e Validação de Inicialização (Startup Check)
- [x] **3.1 Criar Classe Parser:** Criar a classe principal responsável pelo parsing, aceitando um tipo genérico `T`.
- [x] **3.2 Criar Exceção Customizada:** Implementar `CleanParserException` para encapsular erros de parsing, facilitando o tratamento pelo consumidor (RF08).
- [x] **3.3 Validar Tipos Suportados (Refinamento RF03):** Validar na inicialização se todas as propriedades decoradas com `[Option]` são de tipos suportados (`string`, `int`, `double`, `bool`). Lançar exceção se houver tipos não suportados (ex: `DateTime`).
- [x] **3.4 Validar Duplicidade de Opções (RF09):** Implementar verificação para garantir que não existem `OptionName` ou `ShortOptionName` duplicados na classe alvo.
- [x] **3.5 Validar Referência de Grupos (RF09):** Garantir que toda propriedade com `Group` definido aponte para um `[OptionGroup]` existente na classe.
- [x] **3.6 Validar Duplicidade de Grupos (RF09):** Garantir que não existam múltiplos atributos `[OptionGroup]` com o mesmo nome.
- [x] **3.7 Validar Formato dos Nomes:** Garantir que `OptionName` e `ShortOptionName` definidos no atributo não contenham prefixos (`-`, `/`) ou caracteres inválidos (espaços, separadores) para evitar ambiguidade.

## 4. Tokenização e Sintaxe (Core Logic)
- [ ] **4.1 Implementar Splitter de Argumentos Seguro:** Criar lógica para separar chave e valor usando a *primeira* ocorrência de `:` ou `=`. Isso permite valores que contenham esses caracteres (ex: URLs ou Connection Strings).
- [ ] **4.2 Validar Formato Estrito (RF04):** Implementar validação que rejeita argumentos sem separador (exceto flags), garantindo a proibição de espaços (Exceção: "Erro de sintaxe no argumento '{0}'...").
- [ ] **4.3 Normalizar e Validar Prefixos:** Implementar lógica para identificar e remover prefixos (`-`, `--`, `/`).
- [ ] **4.4 Tratamento de Aspas:** Implementar lógica para sanitizar o valor, removendo aspas envolventes se houver (ex: `--msg="Olá"` -> valor: `Olá`).

## 5. Mapeamento e Conversão de Tipos
- [ ] **5.1 Loop de Reflection (RF01):** Implementar iteração sobre as propriedades da classe alvo.
- [ ] **5.2 Estratégia de Sobrescrita:** Definir comportamento para argumentos repetidos (ex: `-p:80 -p:90`). Adotar estratégia "Last Wins" (o último valor prevalece) para tipos primitivos.
- [ ] **5.3 Conversão String:** Mapear valores para propriedades `string`.
- [ ] **5.4 Conversão Int (RF03, RF08):** Mapear valores para `int` com `int.TryParse`.
- [ ] **5.5 Conversão Double (RF03, RF08):** Mapear valores para `double` forçando `CultureInfo.InvariantCulture` para garantir que o formato `.` (ponto) seja aceito consistentemente independente do Locale do OS.
- [ ] **5.6 Lógica Bool/Flag (RF05):** Implementar lógica onde a presença do argumento define `true`. Suportar valores explícitos (`:true`, `:false`) case-insensitive.
- [ ] **5.7 Validar Valor Ausente (RF08):** Lançar exceção se um argumento não booleano for fornecido sem valor ("O argumento '{0}' exige um valor...").

## 6. Validação de Regras de Negócio (Grupos)
- [ ] **6.1 Coletar Estado dos Grupos:** Rastrear quais opções de cada grupo foram efetivamente preenchidas.
- [ ] **6.2 Validar `ExactOne` (RF07, RF08):** Lançar erro se contagem != 1 ("Conflito de opções: O grupo '{0}' exige exatamente uma opção...").
- [ ] **6.3 Validar `AtLeastOne` (RF07, RF08):** Lançar erro se contagem == 0 ("Requisito não atendido: Pelo menos uma opção do grupo '{0}' deve ser fornecida.").

## 7. Funcionalidades de Saída (UX)
- [ ] **7.1 Gerador de Ajuda (RF02):** Criar método `GetHelpText()`.
- [ ] **7.2 Formatação de Tabela (UX):** Garantir que a lista de opções no Help Text seja alinhada verticalmente (padding) para melhor leitura no console.
- [ ] **7.3 Implementar `PrintSummary` (RF10):** Adicionar verificação final que imprime os valores mapeados no console se a flag estiver ativa.
- [ ] **7.4 Integração de Erro:** Garantir que a `CleanParserException` contenha referência ao gerador de ajuda ou dados suficientes para exibi-la.

## 8. Testes (QA)
- [ ] **8.1 Testes de Sintaxe (T01, T02):** Validar formatos aceitos e rejeição de espaços.
- [ ] **8.2 Testes de Aspas e Separadores:** Validar valores com aspas e valores contendo `:` ou `=` (ex: connection strings).
- [ ] **8.3 Testes de Tipos Primitivos e Cultura (T03, T04, T05):** Validar conversões (incluindo Double com ponto) e mensagens de erro de tipo.
- [ ] **8.4 Testes de Tipos Não Suportados:** Validar se o parser rejeita propriedades `DateTime` ou complexas na inicialização.
- [ ] **8.5 Testes de Grupos (T06, T07, T08):** Validar lógica de `ExactOne` e `AtLeastOne`.
- [ ] **8.6 Testes de Prefixos e Busca (T09):** Garantir flexibilidade de prefixos.
- [ ] **8.7 Testes de Resumo (T10):** Verificar output visual.
- [ ] **8.8 Testes de Metadados:** Garantir que definições inválidas na classe (duplicidades) lancem erro na inicialização.