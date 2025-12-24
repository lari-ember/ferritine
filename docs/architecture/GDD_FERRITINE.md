# Game Design Document
## Maquete Viva: Cidade Simulada Híbrida
### *Um Ecossistema Ferroviário e Urbano Físico-Digital*

---

## 📋 SUMÁRIO EXECUTIVO

### Visão Geral
**Maquete Viva** é um projeto híbrido de longo prazo que combina uma maquete ferroviária física tradicional com uma simulação computacional profunda de uma cidade viva. O projeto integra hobby artesanal, aprendizado técnico (IoT, eletrônica, programação embarcada), simulação de agentes e realidade aumentada para criar um ecossistema urbano que "pulsa" com vida própria.

### Motivação Central
- **Origem**: Hobby familiar (mãe interessada em ferrorama)
- **Propósito**: Entretenimento, aprendizado técnico e criação de arte interativa
- **Filosofia**: Representar a evolução histórica dos transportes e da urbanização brasileira/mundial através de uma maquete que seja simultaneamente decoração, jogo e simulação

### Público-Alvo Primário
- Você e sua mãe (criadores e jogadores principais)
- Visitantes ocasionais que descobrirão camadas ocultas de complexidade

### Emoção-Alvo para Observadores
> *"Há algo pulsando aqui... isso é mais profundo do que parece"*

Despertar: ternura, admiração técnica, curiosidade crescente, conforto e alegria ao descobrir os detalhes.

---

## 🎯 PILARES DE DESIGN

### 1. **Dualidade Físico-Digital**
A maquete física é a "casca visível" de um mundo digital complexo. Cada trem, prédio e rua tem uma contraparte virtual com dados, história e comportamento.

### 2. **Evolução Histórica**
O projeto representa períodos históricos sobrepostos ou alternáveis:
- Era das Marias Fumaça (1860-1920)
- Industrialização (1920-1960)
- Modernização (1960-2000)
- Era Contemporânea (2000+)

Cada era tem estética, veículos, arquitetura e políticas próprias.

### 3. **Simulação Profunda**
Não é apenas movimento mecânico - é uma cidade com:
- Economia (oferta/demanda, preços, salários)
- Sociedade (agentes com rotinas, famílias, ambições)
- Política (eleições, obras públicas, desastres)
- Logística (transporte de carga, passageiros, construção civil)

### 4. **Aprendizado Contínuo**
O projeto é pedagógico: cada fase ensina eletrônica, IoT, programação embarcada, design de sistemas, modelagem 3D, etc.

### 5. **Modularidade e Expansibilidade**
Tudo pode crescer: novos bairros, novas tecnologias, novas eras históricas, novos modos de interação.

---

## 🌍 ESCOPO DO MUNDO

### Escala Física Inicial
- **Maquete Fase 1**: Mesa/tábua de 1m² (~100x100cm)
- **Escala HO** (1:87) ou **N** (1:160) para ferrorama
- **Transporte**: Modular, pode ser movida entre ambientes
- **Visão Futura**: Integração arquitetônica no apartamento/casa (até 40m² de área total)

### Escala Temporal
- **Tempo Real vs Simulado**: Configurável (1 minuto real = 1 hora simulada, ou outros)
- **Ciclos**: Dia/noite, estações do ano, anos fiscais, eleições quadrienais
- **Aceleração**: Modo "pular" para construções (modo criativo/sandbox)

### Topologia da Cidade
- **Centro Histórico**: Estação ferroviária principal, prédios antigos
- **Distrito Industrial**: Fábricas, armazéns, pátio de manobras
- **Área Residencial**: Casas, prédios de apartamentos, escolas
- **Zona Rural**: Fazendas, campos, estradas de terra
- **Futuro**: Aeroporto, porto, subúrbios, favela/periferia

### Cenários Customizáveis
Cada "bloco" da maquete pode representar uma época ou estilo:
- Bairro vitoriano com marias fumaça
- Distrito anos 50 com carros clássicos
- Centro moderno com BRT e prédios de vidro

---

## 🚂 SISTEMAS DE TRANSPORTE

### Ferrovias (Prioridade 1)
**Físico**:
- Trilhos DCC (Digital Command Control) para controle individual de locomotivas
- Sensores de posição (reed switches, ópticos ou Hall effect)
- Sinais luminosos funcionais (LEDs)
- Desvios automatizados (servomotores)

**Digital**:
- Sistema de sinalização virtual (ocupação de blocos, precedência)
- Logística de carga (vagões carregam bens virtuais: carvão, grãos, manufaturados)
- Manutenção (desgaste, falhas, necessidade de revisão)
- Horários dinâmicos baseados em demanda

**Tipos de Trens**:
- Marias fumaça (carga/passageiros, era 1)
- Diesel-elétricos (carga, eras 2-3)
- Elétricos modernos (passageiros, era 4)
- Composições customizáveis (locomotiva + X vagões)

### Ônibus e BRT (Prioridade 2)
**Físico**:
- Miniaturas motorizadas (motores micro com baterias ou trilho slot car)
- Canaletas/faixas exclusivas em certas ruas para BRT
- Pontos de parada com LEDs
- Estações tubo (estilo Curitiba) para BRT biarticulado

**Digital**:
- Rotas programadas
- Lotação simulada (agentes virtuais embarcam/desembarcam)
- **BRT Biarticulado** para rotas de alto fluxo (250 passageiros)
  - Canaleta exclusiva para maior velocidade (60 km/h)
  - Embarque em nível (estações tubo)
  - Sistema de transporte rápido inspirado em Curitiba
- Ônibus convencionais (40 passageiros) para rotas locais
- Modo expresso para linhas diretas

### Carros Particulares (Prioridade 3)
**Físico**:
- Poucos carros (5-10), autônomos ou slot car
- Representam épocas (Ford T, Fusca, carros modernos)

**Digital**:
- Agentes específicos possuem carros
- Podem causar acidentes (sono, imprudência)
- Tráfego visualizado na tela mesmo sem miniatura física

### Veículos de Carga (Caminhões - Exclusão Inicial)
Por ora, carga terrestre é apenas ferroviária. Caminhões podem ser adicionados no futuro.

---

## 👥 SISTEMA DE AGENTES (Habitantes)

### Arquitetura de Agente
Cada habitante é uma entidade digital com:

#### Atributos Básicos
- **Nome, idade, gênero**
- **Família**: Pais, filhos, cônjuge
- **Moradia**: Casa/apartamento específico
- **Emprego**: Empresa, cargo, salário
- **Educação**: Nível de escolaridade

#### Atributos Físicos/Mentais
- **Saúde**: 0-100 (doenças, fadiga, envelhecimento)
- **Conhecimento**: Habilidades profissionais
- **Força física**: Para trabalhos manuais
- **Atenção**: Afeta qualidade do trabalho, direção
- **Preguiça**: Chance de faltar ao trabalho
- **Ambição**: Busca promoções, empreendimentos

#### Rotinas Dinâmicas
**Dia Típico de um Agente**:
```
06:00 - Acordar, higiene pessoal
06:30 - Café da manhã (em casa ou padaria)
07:00 - Deslocamento para trabalho (ônibus/trem/carro)
08:00 - Trabalho na fábrica/escritório/loja
12:00 - Almoço (restaurante, casa, marmita)
13:00 - Retorno ao trabalho
17:00 - Fim do expediente
17:30 - Atividade de lazer (parque, bar, cinema)
19:00 - Retorno para casa
20:00 - Jantar, TV, família
22:00 - Dormir
```

**Variações**:
- Finais de semana (lazer, compras, visitas)
- Feriados (eventos culturais, religião)
- Eventos especiais (casamentos, shows, comícios)

#### Sistema de Necessidades
- **Fome/Sede**: Agente busca comida
- **Cansaço**: Afeta produtividade, pode dormir no trabalho/trânsito
- **Felicidade**: Influenciada por salário, lazer, família, política
- **Saúde**: Doenças exigem hospital, afastamento do trabalho

#### Tomada de Decisões
- **Busca de Emprego**: Desempregados procuram vagas
- **Mudança de Moradia**: Famílias crescem, buscam casas maiores
- **Empreendedorismo**: Agente abre loja/empresa
- **Participação Política**: Vota, participa de protestos

---

## 🏗️ SISTEMA DE CONSTRUÇÃO E ECONOMIA

### Economia Simulada
#### Moedas e Recursos
- **Dinheiro**: Usado por agentes e prefeitura
- **Materiais de Construção**: Madeira, tijolo, concreto, aço
- **Bens de Consumo**: Alimentos, roupas, eletrônicos
- **Recursos Naturais**: Carvão, minério, grãos (da zona rural)

#### Cadeia Produtiva
```
Fazenda → Grãos → Trem → Moinho → Farinha → Padaria → Agente
Mina → Carvão → Trem → Usina → Energia → Indústria → Bens
Floresta → Madeira → Serraria → Construção → Casas
```

### Construção de Edifícios
#### Processo Realista
**Exemplo: Construção de Estação Ferroviária**
1. **Decisão**: Prefeitura ou empresa aprova projeto
2. **Planejamento**: Definir terreno, orçamento, prazo
3. **Licitação**: Contratar construtora (se sistema político exigir)
4. **Logística de Materiais**:
   - Trem transporta tijolos, aço, cimento
   - Caminhões (futuros) levam ao canteiro
5. **Mão de Obra**:
   - Agentes trabalhadores são contratados
   - Trabalham X horas/dia, recebem salário
6. **Construção Faseada**:
   - Fundação (10% progresso)
   - Estrutura (30%)
   - Paredes (60%)
   - Acabamento (100%)
7. **Impacto Local**:
   - Trânsito desviado
   - Ruído afeta moradores
   - Emprego temporário aumenta renda local

**Modo Sandbox**: Botão "construir instantaneamente" pula essas etapas.

#### Tipos de Construção
- **Residencial**: Casas, prédios de apartamentos
- **Comercial**: Lojas, restaurantes, mercados
- **Industrial**: Fábricas, armazéns, oficinas
- **Infraestrutura**: Estações, pontes, viadutos, escolas, hospitais
- **Decorativo**: Praças, monumentos, parques

### Loteamento e Imobiliário
- Agentes ou empresas compram terrenos
- Preços baseados em localização, infraestrutura
- Sistema de zoneamento (residencial, industrial, misto)

---

## 🏛️ SISTEMA POLÍTICO E GESTÃO PÚBLICA

### Prefeitura
#### Prefeito e Eleições
- **Eleições**: A cada 4 anos (tempo simulado)
- **Candidatos**: Agentes com alta ambição + recursos
- **Votos**: Agentes votam baseado em felicidade, propostas
- **Mandato**: Prefeito toma decisões de cidade

#### Políticas Públicas
- **Orçamento**: Arrecadação de impostos vs gastos
- **Obras**: Construção de escolas, hospitais, expansão de transporte
- **Regulação**: Zoneamento, horário de funcionamento, segurança

#### Eventos Políticos
- **Protestos**: Se felicidade média cai muito
- **Corrupção**: Prefeito desvia verbas (afeta obras)
- **Desastres**: Enchentes, incêndios exigem resposta

### Empresa de Transporte (Jogador)
#### Gerenciamento Ferroviário
Você controla a empresa ferroviária:
- **Frota**: Comprar/vender locomotivas e vagões
- **Rotas**: Definir linhas de carga e passageiros
- **Horários**: Ajustar frequência baseado em demanda
- **Preços**: Definir tarifa de carga e passagem
- **Manutenção**: Agendar revisões, evitar acidentes

#### Finanças da Empresa
- **Receita**: Tarifa de passageiros + frete de carga
- **Despesas**: Combustível, salários, manutenção, impostos
- **Investimento**: Expansão de trilhos, compra de trens novos

---

## 🤖 SISTEMA DE IA E AUTOMAÇÃO

### IA para Gestão da Cidade (Modo Autônomo)
Quando você não está jogando, a IA mantém a cidade funcionando:
- **Ajuste de Horários**: Aumenta frequência de trens em horário de pico
- **Previsão de Demanda**: Compra materiais antes de construções grandes
- **Gestão de Crises**: Responde a incêndios, doenças, acidentes
- **Equilíbrio Econômico**: Evita inflação/deflação extrema

### IA para Conteúdo Gerado
#### Notícias da Cidade
Sistema gera manchetes baseadas em eventos:
- *"Novo Hospital Inaugurado no Bairro Industrial"*
- *"Greve de Maquinistas Paralisa Trens por 2 Dias"*
- *"Acidente na Linha Norte: 3 Feridos"*

#### Geração de Agentes
IA cria novos habitantes ao longo do tempo:
- Nascimentos (filhos de agentes existentes)
- Imigração (novos agentes chegam por trem)
- Nomes, histórias e atributos gerados proceduralmente

#### Planejamento Urbano Assistido
IA sugere onde construir:
- *"Bairro Oeste precisa de escola (muitas crianças)"*
- *"Falta habitação popular perto da fábrica"*

### IA para Projeção Temporal
Se sistema fica desligado por dias/semanas, IA calcula:
- Quantos dias passaram
- Eventos que ocorreram (nascimentos, mortes, construções)
- Estado econômico resultante
- Notícias acumuladas

---

## 📱 INTERFACES E MODOS DE INTERAÇÃO

### Maquete Física
**Interação Direta**:
- **Botões**: Acionar desvios, ligar/desligar trens
- **Telas LCD**: Mostrar horários, status de trens
- **LEDs**: Sinais, iluminação de prédios, semáforos

### Painel de Controle Digital (PC/Desktop)
**Dashboard Principal**:
- **Mapa 3D**: Visualização indie/voxel da cidade
- **Estatísticas**: População, economia, felicidade
- **Controle de Trens**: Interface DCC virtual
- **Gestão**: Financeiro, políticas, construção

**Visualizações**:
- **Gráficos**: Demanda de transporte, economia ao longo do tempo
- **Logs Textuais**: Eventos recentes, notícias
- **Árvore de Decisões**: Escolher políticas, obras

### Aplicativo Mobile
**Funções**:
- Monitorar status da cidade remotamente
- Receber notificações de eventos importantes
- Controlar trens básicos (iniciar/parar)

### Realidade Aumentada (AR)
**Com Smartphone ou Óculos Meta**:
- Apontar câmera para maquete física
- Ver camadas de informação sobrepostas:
  - Nomes de ruas, prédios
  - Estatísticas de cada edifício
  - Agentes virtuais "andando" nas ruas
  - Visualização de carga nos trens
  - Projeção de construções futuras

**Exemplo de Uso**:
> Você aponta o celular para a estação. Na tela, vê agentes virtuais esperando o trem, horários flutuando no ar, e uma notificação: "Trem atrasado 5min - falha mecânica".

---

## 🎨 INSTRUÇÕES DE DESIGN DE INTERFACE E QUALIDADE DE VIDA

### Botões com Claridade Visual
- **Hierarquia de Sinalização**: Sempre priorizar simbologia clara > cor > formato.
  - Exemplo: Ícone de "check" para confirmar, "X" para cancelar, seta para avançar.
  - O formato do botão deve reforçar a ação (ex: botão de confirmação arredondado, botão de cancelar com cantos retos).
- **Feedback Visual**: Botões devem responder ao hover/click com animação sutil (mudança de cor, sombra, leve "bounce").
- **Tamanho Mínimo**: Botões nunca menores que 40x40px para toque confortável.

### Paleta de Cores e Prioridades
- **Prioridade para Cores Pastéis**: Usar tons suaves para conforto visual e evitar fadiga.
  - Azul Pastel: #A3C9F9 (RGBa: 163,201,249,0.85)
  - Verde Pastel: #B8F2E6 (RGBa: 184,242,230,0.85)
  - Amarelo Pastel: #FFFACD (RGBa: 255,250,205,0.85)
  - Lilás Pastel: #E0BBE4 (RGBa: 224,187,228,0.85)
  - Cinza Neutro: #F5F5F5 (RGBa: 245,245,245,1)
- **Botão Cancelar/Fechar**: Vermelho claro já usado: #FF7E7F (RGBa: 255,126,127,1)
- **Botão Confirmar**: Verde já usado: #6FF090 (RGBa: 111,240,144,1)
- **Evitar**: Vermelhos ou verdes muito saturados para não cansar a vista.
- **Contraste**: Garantir contraste mínimo WCAG AA para texto e ícones.

### Boas Práticas de Qualidade de Vida para o Jogador
- **Acessibilidade**: 
  - Modo alto contraste e modo daltônico.
  - Ícones sempre acompanhados de texto ou tooltip.
  - Fontes grandes e legíveis (mínimo 16px, preferencialmente 18px+).
- **Legibilidade**: 
  - Evitar excesso de informação em uma tela só.
  - Preferir agrupamento visual (cards, painéis, overlays).
- **Ergonomia**:
  - Menus e botões acessíveis com poucos cliques.
  - Navegação por teclado e mouse.
  - Feedback sonoro opcional para ações importantes.
- **Customização**:
  - Permitir ao jogador ajustar tamanho da fonte, cores e disposição dos painéis.
- **Redução de Frustração**:
  - Confirmação para ações destrutivas (ex: deletar, resetar).
  - Undo/redo sempre que possível.
- **Tutorial Contextual**:
  - Dicas rápidas e tooltips contextuais ao passar o mouse.
  - Modo "primeira vez" com explicações visuais.
- **Pacing e Conforto**:
  - Permitir pausar, acelerar ou desacelerar o tempo.
  - Não exigir ações rápidas ou reflexos para progressão.

---


## 🎨 ESTILO VISUAL E ESTÉTICA

### Maquete Física
**Estilo**: Misto histórico, detalhado mas não hiper-realista
- **Materiais**: MDF, isopor, impressão 3D, miniaturas comerciais
- **Nível de Detalhe**: Museu (texturas, pequenos detalhes visíveis)
- **Iluminação**: LEDs para postes, prédios, trens
- **Vegetação**: Árvores, arbustos, grama sintética

### Simulação Digital
**Estilo Indie/Voxel**:
- **Referências**: Minecraft, Townscaper, Mini Metro
- **Paleta**: Cores vibrantes mas não saturadas
- **Animações**: Suaves, personagens estilizados
- **UI**: Minimalista, clara, texto grande e legível

### Evolução Histórica Visual
Cada era tem paleta e arquitetura própria:
- **Era 1 (1860-1920)**: Sépia, tijolos vermelhos, madeira
- **Era 2 (1920-1960)**: Art déco, concreto, cinza e bege
- **Era 3 (1960-2000)**: Brutalismo, vidro, tons pastéis
- **Era 4 (2000+)**: Aço, vidro espelhado, LED, cores frias

---

## 🔧 TECNOLOGIAS E STACK TÉCNICO

### Hardware (Físico)
#### Eletrônica Básica
- **Microcontroladores**: Arduino Uno/Mega (iniciante), ESP32 (WiFi/IoT)
- **Sensores**:
  - Reed switch (detecção de trem)
  - Sensor óptico infravermelho
  - Sensor Hall effect
- **Atuadores**:
  - Servomotores (desvios, semáforos)
  - Motores DC (trens, carros)
  - LEDs e LED strips (iluminação)
- **Alimentação**:
  - Fonte 12V para trilhos DCC
  - Fonte 5V para eletrônica
  - Baterias para veículos autônomos

#### Ferramentas Necessárias (Futuro)
- Multímetro digital
- Ferro de solda + solda
- Chaves de fenda, alicate
- Cola quente, estilete
- Dremel ou mini furadeira

#### Materiais de Maquete
- MDF, compensado (base)
- Isopor, EVA (terreno, relevo)
- Impressão 3D (prédios customizados)
- Corte a laser (fachadas detalhadas)
- Miniaturas comerciais (Frateschi, Bachmann, etc.)

### Software (Digital)
#### Linguagens e Frameworks
- **Linguagem Principal**: Python 3.11+
- **Simulação e Lógica**: Python (backend)
- **Visualização 3D**: Unity Engine com texturas voxel
- **Interface Web**: Flask ou FastAPI (dashboard)
- **Programação Embarcada**: Arduino IDE (C++), MicroPython (ESP32)

#### Arquitetura de Software
```
┌─────────────────────────────────────┐
│  Interface Usuário (Desktop/Web/AR) │
├─────────────────────────────────────┤
│  Motor de Simulação (Python)        │
│  - Agentes                           │
│  - Economia                          │
│  - Transporte                        │
│  - Construção                        │
├─────────────────────────────────────┤
│  Banco de Dados (SQLite/PostgreSQL) │
├─────────────────────────────────────┤
│  Servidor IoT (MQTT/WebSocket)      │
├─────────────────────────────────────┤
│  Hardware (ESP32 → Sensores/Trens)  │
└─────────────────────────────────────┘
```

#### Bibliotecas Python Chave
- **Simulação**:
  - `simpy` (eventos discretos)
  - `mesa` (agentes baseados em modelo)
  - `numpy`, `pandas` (dados)
- **Visualização**:
  - `pygame` (2D/isométrico)
  - `matplotlib`, `plotly` (gráficos)
- **IoT**:
  - `paho-mqtt` (comunicação com ESP32)
  - `pyserial` (comunicação com Arduino)
- **IA**:
  - `scikit-learn` (previsão de demanda)
  - `transformers` (geração de texto/notícias)

#### Banco de Dados
**Estrutura**:
- **Tabela Agentes**: id, nome, idade, emprego, moradia, atributos
- **Tabela Edifícios**: id, tipo, localização, proprietário
- **Tabela Trens**: id, modelo, posição, carga, status
- **Tabela Eventos**: timestamp, tipo, descrição
- **Tabela Economia**: timestamp, PIB, inflação, desemprego

### Comunicação IoT
**Protocolo MQTT**:
- **Broker**: Mosquitto (servidor central)
- **Tópicos**:
  - `cidade/trem/1/posicao` (sensor envia posição)
  - `cidade/trem/1/velocidade` (servidor envia comando)
  - `cidade/semaforo/3/estado` (servidor envia vermelho/verde)

---

## 📚 PLANO DE APRENDIZADO E IMPLEMENTAÇÃO

### Fase 0: Fundamentos (Mês 1-2)
**Objetivo**: Adquirir conhecimento base antes de começar

#### Teoria de Eletrônica Básica
**Conteúdo**:
- Lei de Ohm, tensão, corrente, resistência
- Componentes: resistores, capacitores, LEDs, transistores
- Circuitos série e paralelo
- Protoboard e multímetro

**Recursos**:
- Curso online gratuito: *"Eletrônica para Iniciantes"* (YouTube - WR Kits)
- Livro: *"Eletrônica Para Leigos"* - Cathleen Shamieh
- Simulador online: Tinkercad Circuits

**Exercício Prático**: Montar circuito simples no Tinkercad (LED piscando)

#### Introdução ao Arduino
**Conteúdo**:
- O que é microcontrolador
- Pinos digitais e analógicos
- Upload de código (sketch)
- Blink LED, leitura de sensor

**Recursos**:
- Tutoriais oficiais: arduino.cc/tutorials
- Projeto guiado: Semáforo simples

**Exercício Prático**: Comprar Arduino Uno starter kit (R$ 150-250), montar projetos básicos

#### Python para Simulação
**Conteúdo** (se ainda não domina):
- Classes e objetos (POO)
- Listas, dicionários, loops
- Bibliotecas externas (pip install)

**Recursos**:
- Curso: *"Python Orientado a Objetos"* (Curso em Vídeo - Gustavo Guanabara)

**Exercício Prático**: Criar classe `Agente` com atributos e método `trabalhar()`

#### Ferroramas Básicos
**Conteúdo**:
- Tipos de escala (HO, N, O)
- DCC vs DC (analógico)
- Trilhos, desvios, eletrificação

**Recursos**:
- Vídeos: Canal *"Ferromodelismo Brasil"* (YouTube)
- Fóruns: Ferro Fórum Brasil

**Exercício Prático**: Visitar loja de ferrorama (online ou física), entender preços

---

### Fase 1: Simulação Digital Básica (Mês 3-4)
**Objetivo**: Criar motor de simulação sem hardware

#### Milestone 1.1: Mundo Estático
**Implementar**:
- Classe `Cidade` com grid 2D
- Classe `Edificio` (casa, fábrica, estação)
- Classe `Rua` e `Trilho`
- Renderizar mapa simples (Pygame)

**Resultado**: Tela mostrando cidade 2D estática

#### Milestone 1.2: Agentes Simples
**Implementar**:
- Classe `Agente` com atributos básicos
- Método `trabalhar()`, `descansar()`
- 10 agentes com rotinas hardcoded
- Visualizar agentes como pontos no mapa

**Resultado**: Agentes "teleportam" entre casa e trabalho

#### Milestone 1.3: Economia Básica
**Implementar**:
- Agentes recebem salário
- Gastam dinheiro em comida
- Fábricas produzem bens
- Sistema de oferta/demanda simples

**Resultado**: Dashboard mostrando economia funcionando

#### Milestone 1.4: Transporte Ferroviário Virtual
**Implementar**:
- Classe `Trem` com posição nos trilhos
- Movimento automático em loop
- Agentes embarcam/desembarcam
- Carga transportada entre estações

**Resultado**: Trens virtuais funcionando na simulação

---

### Fase 2: Hardware Básico (Mês 5-7)
**Objetivo**: Primeiro contato com eletrônica física

#### Milestone 2.1: Circuito de Iluminação
**Implementar**:
- Arduino controla LEDs em prédios
- Python envia comando via Serial
- LEDs acendem/apagam baseado em hora do dia simulado

**Hardware**:
- Arduino Uno
- 5-10 LEDs
- Resistores
- Jumpers

**Orçamento**: ~R$ 200

#### Milestone 2.2: Sensor de Trem
**Implementar**:
- Reed switch detecta trem passando
- Arduino envia dado para Python
- Python atualiza posição do trem na simulação

**Hardware**:
- Reed switch (R$ 5-10 cada)
- Ímã (colar embaixo do trem)

**Orçamento**: ~R$ 50

#### Milestone 2.3: Controle de Desvio
**Implementar**:
- Servomotor aciona desvio de trilho
- Python decide rota do trem
- Arduino move servo

**Hardware**:
- Servomotor 9g
- Mecanismo de desvio (comercial ou impresso 3D)

**Orçamento**: ~R$ 80

---

### Fase 3: Maquete Física Inicial (Mês 8-12)
**Objetivo**: Construir maquete 1m² funcional

#### Milestone 3.1: Base e Topografia
**Construir**:
- Base MDF 100x100cm
- Relevo em isopor/EVA
- Pintura de terreno

**Orçamento**: ~R$ 300

#### Milestone 3.2: Trilhos e Primeiro Trem
**Comprar/Construir**:
- Kit básico de trilhos HO (oval simples)
- Fonte DCC ou DC
- Locomotiva básica + 2 vagões

**Orçamento**: ~R$ 500-800

#### Milestone 3.3: Primeiros Edifícios
**Construir**:
- 3-5 prédios em MDF/impressão 3D
- Estação ferroviária
- Detalhamento (janelas, portas, texturas)

**Orçamento**: ~R$ 300 (se impressão 3D, pagar serviço)

#### Milestone 3.4: Integração Física-Digital
**Implementar**:
- Trem físico detectado por sensores
- Posição física sincronizada com simulação
- Luzes dos prédios controladas pela simulação

**Resultado**: Maquete 1m² com trem funcionando + simulação sincronizada

---

### Fase 4: Expansão e Refinamento (Ano 2+)
#### Possibilidades:
- **Mais Veículos**: Ônibus, carros autônomos
- **Mais Sensores**: Temperatura, luminosidade (dia/noite automático)
- **Realidade Aumentada**: App mobile com AR
- **Expansão Física**: Módulos adicionais, conectar 2-3 tábuas
- **Eras Históricas**: Trocar miniaturas para representar épocas diferentes
- **IA Avançada**: Reinforcement learning para otimização de rotas

---

## 💰 ORÇAMENTO ESTIMADO

### Orçamento Minimalista (Fase 1 - Digital)
| Item | Valor |
|------|-------|
| Arduino Uno Starter Kit | R$ 200 |
| Componentes extras (LEDs, sensores) | R$ 100 |
| **Total Fase 1** | **R$ 300** |

### Orçamento Intermediário (Fase 1-3)
| Item | Valor |
|------|-------|
| Eletrônica (Arduino, sensores, LEDs) | R$ 400 |
| Base e materiais de maquete | R$ 300 |
| Trilhos e trem básico HO | R$ 700 |
| Materiais de construção (prédios) | R$ 300 |
| Ferramentas básicas | R$ 300 |
| **Total Fase 1-3** | **R$ 2.000** |

### Orçamento Completo (Fase 1-4, longo prazo)
| Categoria | Valor Estimado |
|-----------|----------------|
| Eletrônica e IoT (ESP32, sensores avançados, servos) | R$ 800 |
| Ferramentas (multímetro, ferro de solda, dremel) | R$ 500 |
| Base e estrutura expandida (2-3m²) | R$ 800 |
| Sistema ferroviário completo (trilhos, desvios, múltiplos trens) | R$ 2.500 |
| Veículos (ônibus, carros) | R$ 600 |
| Construções e detalhamento | R$ 1.000 |
| Impressão 3D e corte a laser (serviços) | R$ 800 |
| Iluminação avançada (LED strips, controladores) | R$ 400 |
| Contingência (erros, testes, componentes extras) | R$ 600 |
| **Total Estimado 3-5 Anos** | **R$ 8.000** |

*Nota: Valores podem variar. Compras serão faseadas ao longo de anos.*

---

## 📊 MÉTRICAS E SISTEMAS DE PROGRESSO

### KPIs da Cidade
Indicadores que medem a saúde da simulação:

#### Economia
- **PIB**: Soma de toda produção de bens/serviços
- **Taxa de Desemprego**: % de agentes sem trabalho
- **Inflação**: Variação de preços ao longo do tempo
- **Receita da Empresa Ferroviária**: Lucro/prejuízo mensal

#### Sociedade
- **População Total**: Número de agentes
- **Taxa de Natalidade/Mortalidade**
- **Felicidade Média**: 0-100 (média de todos agentes)
- **Nível Educacional Médio**

#### Transporte
- **Passageiros Transportados/Dia**: Trens + ônibus
- **Toneladas de Carga Movidas/Mês**
- **Taxa de Pontualidade**: % de trens no horário
- **Acidentes**: Número de colisões/falhas

#### Infraestrutura
- **Cobertura de Transporte Público**: % população com acesso
- **Taxa de Ocupação Habitacional**: Casas disponíveis vs famílias
- **Escolas/Hospitais por 1000 habitantes**

### Progressão do Jogador
#### Achievements (Conquistas)
- 🚂 **Primeira Viagem**: Completar primeira rota de trem
- 🏗️ **Urbanista**: Construir 10 edifícios
- 👨‍👩‍👧‍👦 **Cidade Viva**: Atingir 100 agentes
- 💰 **Magnata Ferroviário**: Lucro acumulado de R$ 1M (virtual)
- 🏛️ **Democracia**: Realizar primeira eleição
- 🤖 **Automação Total**: IA gerenciando cidade por 30 dias sem intervenção
- 📜 **Historiador**: Documentar 100 eventos na linha do tempo

#### Sistema de Níveis
Baseado em complexidade implementada:

**Nível 1 - Simulador**: Cidade virtual básica funcionando  
**Nível 2 - Construtor**: Primeira maquete física  
**Nível 3 - Engenheiro**: Sensores e atuadores integrados  
**Nível 4 - Magnata**: Economia complexa balanceada  
**Nível 5 - Visionário**: Realidade Aumentada funcionando  
**Nível 6 - Deus Ex Machina**: Sistema totalmente autônomo e expansível

---

## 🎮 MODOS DE JOGO

### Modo História (Campaign)
Jogador progride através de eras históricas:

#### Capítulo 1: Era do Vapor (1860-1920)
**Objetivo**: Estabelecer primeira linha ferroviária lucrativa
- Construir estação central
- Comprar maria fumaça
- Transportar carvão e grãos
- Atingir população de 50 agentes

#### Capítulo 2: Industrialização (1920-1960)
**Objetivo**: Expandir para transporte de passageiros
- Construir 3 estações
- Implementar linha de ônibus
- Abrir fábrica têxtil
- População: 150 agentes

#### Capítulo 3: Modernização (1960-2000)
**Objetivo**: Eletrificar transporte e diversificar economia
- Substituir vapor por diesel/elétrico
- Sistema BRT com canaleta
- 5 indústrias diferentes
- População: 500 agentes

#### Capítulo 4: Era Digital (2000+)
**Objetivo**: Automação e sustentabilidade
- Trens automatizados
- Economia de serviços (tech, turismo)
- Transporte multimodal integrado
- População: 1000+ agentes

### Modo Sandbox (Criativo)
- **Recursos Infinitos**: Dinheiro ilimitado
- **Construção Instantânea**: Pular logística
- **Controle de Tempo**: Pausar, acelerar, retroceder
- **Desastres sob Demanda**: Triggerar eventos manualmente
- **Imortalidade**: Agentes não morrem

### Modo Desafio (Scenarios)
Cenários específicos com objetivos:

**Desafio 1: Resgate Econômico**
> A cidade está em recessão. Reduza desemprego para <5% em 2 anos.

**Desafio 2: Catástrofe**
> Enchente destruiu 30% dos trilhos. Restaure transporte em 6 meses.

**Desafio 3: Expansão Acelerada**
> Duplique a população em 3 anos mantendo felicidade >70.

**Desafio 4: Eficiência Máxima**
> Transporte 10.000 toneladas com apenas 3 trens.

### Modo Observação (Zen)
- IA gerencia tudo
- Jogador apenas observa
- Ideal para demonstrações, decoração
- Pode intervir a qualquer momento

---

## 🔀 SISTEMAS EMERGENTES E EVENTOS

### Eventos Aleatórios
Eventos que criam narrativa e desafios:

#### Clima e Desastres
- **Chuva Forte**: Reduz velocidade de trens e carros
- **Enchente**: Bloqueia trilhos, exige reparos
- **Seca**: Afeta produção agrícola, aumenta preços de alimentos
- **Incêndio**: Destrói edifícios, exige bombeiros
- **Terremoto** (raro): Danos em infraestrutura

#### Sociais
- **Greve**: Maquinistas param por X dias, exigindo negociação
- **Festival**: Aumento de demanda de transporte, alegria +20%
- **Epidemia**: Agentes ficam doentes, hospitais lotados
- **Protesto**: Bloqueio de ruas/trilhos se felicidade <30%
- **Onda de Imigração**: +50 agentes chegam de trem

#### Econômicos
- **Boom Industrial**: Demanda de carga +200% por 6 meses
- **Recessão**: Desemprego aumenta, demanda cai
- **Descoberta de Recurso**: Nova mina de carvão/minério abre
- **Falência de Empresa**: Grande empregador fecha, desemprego sobe

#### Tecnológicos
- **Invenção**: Nova tecnologia de trem (mais rápido, eficiente)
- **Obsolescência**: Marias fumaça ficam caras de manter
- **Eletrificação Disponível**: Opção de converter linhas

#### Políticos
- **Eleição Surpresa**: Novo prefeito com prioridades diferentes
- **Mudança de Lei**: Zoneamento, impostos, regulações
- **Escândalo de Corrupção**: Prefeito perde apoio
- **Investimento Federal**: Subsídio para expansão ferroviária

### Comportamentos Emergentes
Situações que surgem da interação de sistemas:

**Exemplo 1: Gentrificação**
1. Nova estação de trem é construída em bairro pobre
2. Preço dos terrenos próximos aumenta
3. Lojas e restaurantes abrem
4. Moradores originais (pobres) não conseguem pagar aluguel
5. Saem e vão para periferia
6. Demanda de transporte muda (mais viagens longas)

**Exemplo 2: Círculo Vicioso do Desemprego**
1. Fábrica fecha (recessão)
2. 50 agentes desempregados
3. Gastam menos em lojas
4. Lojas têm menos receita
5. Lojas demitem funcionários
6. Mais desemprego, economia piora

**Exemplo 3: Sucesso da Linha Ferroviária**
1. Nova linha conecta zona rural a cidade
2. Fazendeiros transportam mais grãos
3. Lucro aumenta, investem em expansão
4. Mais empregos rurais
5. População rural cresce
6. Demanda de transporte de passageiros aumenta
7. Empresa ferroviária adiciona vagões de passageiros
8. Lucro aumenta, ciclo virtuoso

---

## 🧩 MODULARIDADE E EXPANSÕES

### Módulos Físicos
Maquete pode crescer com módulos conectáveis:

#### Módulo Base (1m²)
- Centro histórico
- Estação principal
- 1 linha férrea em loop

#### Módulo Industrial (50x100cm)
- 3 fábricas
- Pátio de manobras
- Desvios e armazéns

#### Módulo Residencial (50x100cm)
- Bairro com 15-20 casas
- Escola, mercado
- Ponto de ônibus

#### Módulo Rural (50x100cm)
- Fazendas, campos
- Estrada de terra
- Estação rural pequena

#### Módulo Futuro (ideias)
- Porto fluvial
- Aeroporto
- Zona comercial (shopping)
- Subúrbio/periferia

### Expansões de Conteúdo
Atualizações de software que adicionam:

**Expansão: "Revolução Verde"**
- Sistema de agricultura detalhado
- Safras, pragas, irrigação
- Novos veículos rurais

**Expansão: "Metrópole"**
- Arranha-céus
- Metrô subterrâneo
- Trânsito denso, congestionamentos

**Expansão: "Patrimônio Histórico"**
- Edifícios protegidos (não podem ser demolidos)
- Turismo como indústria
- Restauração de trens antigos

**Expansão: "Caos Climático"**
- Mudanças climáticas afetam cidade
- Energia renovável vs fóssil
- Enchentes mais frequentes

---

## 📖 NARRATIVA E WORLDBUILDING

### História da Cidade
Cada cidade gerada tem backstory:

#### Fundação
- **Ano de fundação**: Ex: 1887
- **Motivo**: Entroncamento ferroviário, descoberta de minério, etc.
- **Fundadores**: 3-5 famílias pioneiras (sobrenomes geram linhagens)

#### Eventos Históricos Marcantes
O sistema gera linha do tempo:
- 1887: Fundação por famílias Silva, Oliveira e Santos
- 1892: Primeira igreja construída
- 1905: Grande enchente, ponte destruída
- 1923: Inauguração da fábrica têxtil Oliveira & Cia
- 1945: Greve geral de 15 dias
- 1978: Eletrificação da linha principal
- 2003: Cidade atinge 1000 habitantes

#### Personagens Históricos
Agentes importantes são lembrados:
- **João Silva** (1860-1932): Fundador, primeiro prefeito
- **Maria Santos** (1890-1965): Professora, fundou primeira escola
- **Carlos Oliveira** (1900-1980): Industrial, modernizou transporte

Seus descendentes podem ainda viver na cidade, carregando legado.

### Cultura e Identidade
#### Nome da Cidade
Gerado proceduralmente ou escolhido:
- Formato: [Sobrenome + Sufixo]
- Ex: **Santópolis**, **Vila Oliveira**, **Estação Silva**

Ou nomes geográficos:
- **Porto dos Trilhos**, **Vale do Vapor**, **Cidade dos Desvios**

#### Símbolos
- **Brasão**: Gerado com elementos (trem, montanha, rio, etc.)
- **Lema**: Ex: "Progresso sobre Trilhos"
- **Cores Oficiais**: Definidas aleatoriamente

#### Feriados Locais
- Dia da Fundação (celebração anual)
- Dia do Ferroviário (homenagem aos trabalhadores)
- Festivais sazonais (colheita, industrial)

### Jornalismo da Cidade
#### Jornal Local: "O Trilho"
IA gera notícias semanais:

**Manchete**: *"Nova Locomotiva Diesel Reduz Tempo de Viagem em 30%"*  
**Conteúdo**: Descrição da compra, entrevista fictícia com maquinista, impacto na economia.

**Seções**:
- Notícias (eventos da semana)
- Economia (preços, empregos)
- Obituário (agentes que morreram)
- Classificados (terrenos à venda, vagas de emprego)
- Esportes (times locais - futuro)

---

## 🎓 RECURSOS EDUCACIONAIS DETALHADOS

### Currículo de Eletrônica (8 semanas)

#### Semana 1-2: Fundamentos
**Teoria**:
- Átomos, elétrons, corrente elétrica
- Lei de Ohm: V = I × R
- Potência: P = V × I

**Prática**:
- Simulações no Tinkercad
- Calcular resistência para LED

**Vídeos**:
- WR Kits: "O que é Corrente Elétrica?"
- Manual do Mundo: "Como Funciona um LED"

#### Semana 3-4: Componentes
**Teoria**:
- Resistores (código de cores)
- Capacitores (armazenamento de energia)
- Transistores (chave eletrônica)
- Diodos e LEDs

**Prática**:
- Comprar kit de componentes
- Montar circuito em protoboard

**Projeto**: LED piscando com transistor

#### Semana 5-6: Arduino
**Teoria**:
- Arquitetura de microcontrolador
- Pinos digitais (HIGH/LOW)
- Pinos analógicos (0-1023)
- PWM (dimmer de LED)

**Prática**:
- Instalar Arduino IDE
- Upload de sketch Blink
- Controlar brilho de LED com potenciômetro

**Projeto**: Semáforo com 3 LEDs

#### Semana 7-8: Sensores e Atuadores
**Teoria**:
- Reed switch (magnético)
- Sensor infravermelho
- Servomotor (controle de ângulo)

**Prática**:
- Detectar objeto com sensor IR
- Mover servo com potenciômetro
- Integrar múltiplos sensores

**Projeto Final**: Sistema de detecção de trem + servo para desvio

### Currículo de IoT (6 semanas)

#### Semana 1-2: Comunicação Serial
**Teoria**:
- Protocolo UART
- Baud rate
- Python + pyserial

**Prática**:
- Arduino envia dados para PC
- Python lê e exibe na tela
- Enviar comandos do Python para Arduino

**Projeto**: Dashboard Python que acende LED no Arduino

#### Semana 3-4: ESP32 e WiFi
**Teoria**:
- O que é ESP32
- Conectar à rede WiFi
- Servidor web básico

**Prática**:
- Programar ESP32 no Arduino IDE
- Criar página web que controla LED
- Acessar pelo celular

**Projeto**: Controle de LED via navegador

#### Semana 5-6: MQTT
**Teoria**:
- Protocolo publish/subscribe
- Broker Mosquitto
- Tópicos e mensagens

**Prática**:
- Instalar broker local
- ESP32 publica sensor
- Python assina tópico e recebe dados

**Projeto Final**: Sensor de temperatura envia dados, Python exibe em gráfico real-time

### Currículo de Simulação (12 semanas)

#### Semana 1-2: POO em Python
**Conceitos**:
- Classes e objetos
- Atributos e métodos
- Herança e polimorfismo

**Projeto**: Classes `Agente`, `Edificio`, `Veiculo`

#### Semana 3-4: Estruturas de Dados
**Conceitos**:
- Listas, dicionários, sets
- Grafos (representar malha viária)
- Filas (espera de passageiros)

**Projeto**: Mapa da cidade como grafo

#### Semana 5-6: Simulação de Eventos
**Conceitos**:
- Biblioteca `simpy`
- Processos e eventos
- Tempo simulado

**Projeto**: Trem percorrendo rota com tempo de viagem

#### Semana 7-8: Agentes Inteligentes
**Conceitos**:
- Máquinas de estado (trabalho, casa, lazer)
- Tomada de decisão (if/else, random)
- Pathfinding (A*)

**Projeto**: Agente com rotina diária completa

#### Semana 9-10: Economia Simulada
**Conceitos**:
- Oferta e demanda
- Preços dinâmicos
- Salário e consumo

**Projeto**: Mercado de alimentos funcionando

#### Semana 11-12: Integração
**Conceitos**:
- Arquitetura MVC
- Banco de dados SQLite
- Dashboard com Flask

**Projeto Final**: Sistema completo integrado

---

## 🏗️ GUIA DE CONSTRUÇÃO DA MAQUETE

### Materiais Detalhados

#### Base
**Opção 1: MDF**
- Tamanho: 100x100cm, espessura 15mm
- Custo: ~R$ 80
- Vantagens: Firme, fácil de pintar
- Desvantagens: Pesado

**Opção 2: Compensado**
- Tamanho: 100x100cm, espessura 10mm
- Custo: ~R$ 60
- Vantagens: Mais leve que MDF
- Desvantagens: Pode empenar

#### Topografia
**Isopor** (paisagem, montanhas):
- Placas de 2-5cm espessura
- Esculpir com faca quente ou estilete
- Texturizar com lixa
- Custo: R$ 30-50

**EVA** (detalhes, acabamento):
- Folhas de 2mm para ruas
- Fácil de cortar e colar
- Custo: R$ 20

#### Trilhos
**Escala HO (1:87) - Recomendada**:
- Trilho flexível: R$ 25-40 por metro
- Desvios: R$ 80-150 cada
- Fonte DCC: R$ 400-800
- Locomotiva HO: R$ 300-600

**Marcas**:
- Frateschi (nacional, mais barato)
- Bachmann (importada, qualidade média)
- Märklin (importada, alta qualidade, cara)

#### Construções
**Opção 1: Kits Comerciais**
- Frateschi/Auhagen: R$ 50-150 por edifício
- Vantagens: Detalhado, rápido
- Desvantagens: Caro, limitado

**Opção 2: Scratch Building (do zero)**
- Papelão Paraná: R$ 15 por folha
- Cola branca, estilete
- Imprimir texturas (tijolos, janelas)
- Custo por prédio: R$ 5-15

**Opção 3: Impressão 3D**
- Arquivo STL grátis (Thingiverse, Printables)
- Serviço de impressão: R$ 20-80 por prédio
- Qualidade excelente

**Opção 4: Corte a Laser**
- Desenhar em vetor (Inkscape grátis)
- Serviço de corte: R$ 30-100 por conjunto
- MDF 3mm, encaixe preciso

#### Vegetação
- **Árvores**: Comprar prontas (R$ 3-8 cada) ou fazer com esponja/arame
- **Grama**: Pó de gramado sintético (R$ 25 por 50g)
- **Arbustos**: Musgo seco pintado

#### Pintura
- Tinta acrílica (tons terrosos)
- Pincéis variados
- Spray (primer, acabamento)
- Custo: R$ 80-120

### Passo a Passo de Construção

#### Etapa 1: Planejamento (1 semana)
1. Desenhar planta baixa da cidade (papel quadriculado)
2. Definir posição de trilhos, ruas, prédios
3. Marcar relevo (elevações, vales)
4. Listar materiais necessários

#### Etapa 2: Base (1 fim de semana)
1. Cortar MDF no tamanho (marcenaria pode fazer)
2. Lixar bordas
3. Aplicar selador/primer
4. Marcar grid na superfície (lápis)

#### Etapa 3: Relevo (2 fins de semana)
1. Colar placas de isopor sobrepostas (elevações)
2. Esculpir formas de morros/vales
3. Cobrir com gaze e cola (reforço)
4. Texturizar com lixa
5. Pintar base (marrom, verde, cinza)

#### Etapa 4: Trilhos (1 fim de semana)
1. Fixar leito de trilho (EVA ou cortiça)
2. Pregar trilhos (pregos próprios para ferrorama)
3. Testar continuidade elétrica (multímetro)
4. Instalar desvios
5. Conectar fonte DCC

#### Etapa 5: Ruas (1 fim de semana)
1. Marcar ruas com lápis
2. Cortar EVA/papelão para pavimento
3. Colar ruas
4. Pintar asfalto (cinza escuro)
5. Adicionar meio-fio, calçadas

#### Etapa 6: Construções (4-8 fins de semana)
Construir prédios um por vez:
1. Cortar paredes em papelão/MDF
2. Colar estrutura
3. Adicionar janelas, portas
4. Pintar/texturizar
5. Adicionar detalhes (telhado, letreiros)
6. Fixar na base

#### Etapa 7: Vegetação (2 fins de semana)
1. Aplicar grama sintética
2. Plantar árvores
3. Adicionar arbustos
4. Criar jardins, praças

#### Etapa 8: Eletrônica (3-4 fins de semana)
1. Instalar LEDs em postes (furar base, passar fios)
2. LEDs em prédios (janelas, interiores)
3. Semáforos funcionais
4. Conectar tudo ao Arduino
5. Organizar fiação (embaixo da base)

#### Etapa 9: Acabamento (1 fim de semana)
1. Retocar pintura
2. Adicionar detalhes finais (pessoas, carros, placas)
3. Limpar resíduos de cola
4. Proteger com verniz fosco (opcional)

#### Etapa 10: Integração Digital (2-3 fins de semana)
1. Instalar sensores nos trilhos
2. Configurar ESP32/Arduino
3. Conectar à simulação Python
4. Testar sincronização física-digital
5. Ajustes e calibração

**Tempo Total Estimado**: 18-25 fins de semana (~4-6 meses)

---

## 🖥️ ARQUITETURA DE SOFTWARE DETALHADA

### Diagrama de Componentes
```
┌───────────────────────────────────────────┐
│         CAMADA DE APRESENTAÇÃO            │
│  ┌─────────┐  ┌──────────┐  ┌──────────┐ │
│  │Dashboard│  │ Mobile   │  │    AR    │ │
│  │  Web    │  │   App    │  │  Viewer  │ │
│  └────┬────┘  └─────┬────┘  └────┬─────┘ │
└───────┼─────────────┼────────────┼────────┘
        │             │            │
        └─────────────┴────────────┘
                      │
        ┌─────────────▼────────────────┐
        │     API REST / WebSocket     │
        └─────────────┬────────────────┘
                      │
┌─────────────────────▼──────────────────────┐
│         CAMADA DE LÓGICA (Python)          │
│  ┌──────────────────────────────────────┐  │
│  │      Motor de Simulação Principal    │  │
│  │  ┌────────────┐  ┌────────────────┐ │  │
│  │  │  Agentes   │  │    Economia    │ │  │
│  │  └────────────┘  └────────────────┘ │  │
│  │  ┌────────────┐  ┌────────────────┐ │  │
│  │  │ Transporte │  │   Construção   │ │  │
│  │  └────────────┘  └────────────────┘ │  │
│  │  ┌────────────┐  ┌────────────────┐ │  │
│  │  │  Política  │  │     Eventos    │ │  │
│  │  └────────────┘  └────────────────┘ │  │
│  └──────────────────────────────────────┘  │
│  ┌──────────────────────────────────────┐  │
│  │         Módulo de IA/ML              │  │
│  │  - Previsão de Demanda               │  │
│  │  - Geração de Notícias               │  │
│  │  - Otimização de Rotas               │  │
│  │  - Gestão de Crises                  │  │
│  └──────────────────────────────────────┘  │
└─────────────────────┬──────────────────────┘
                      │
        ┌─────────────▼────────────────┐
        │   Banco de Dados (SQLite)    │
        │  - Agentes, Edifícios        │
        │  - História, Eventos         │
        │  - Economia, Estatísticas    │
        └──────────────────────────────┘
                      │
┌─────────────────────▼──────────────────────┐
│      CAMADA DE HARDWARE (IoT)              │
│  ┌──────────────────────────────────────┐  │
│  │    Servidor MQTT / WebSocket         │  │
│  └─────────────┬────────────────────────┘  │
│                │                            │
│    ┌───────────▼──────────┐                │
│    │   ESP32 / Arduino    │                │
│    │  ┌────────────────┐  │                │
│    │  │    Sensores    │  │                │
│    │  │ - Reed Switch  │  │                │
│    │  │ - IR Sensor    │  │                │
│    │  └────────────────┘  │                │
│    │  ┌────────────────┐  │                │
│    │  │   Atuadores    │  │                │
│    │  │ - LEDs         │  │                │
│    │  │ - Servos       │  │                │
│    │  │ - DCC Control  │  │                │
│    │  └────────────────┘  │                │
│    └──────────────────────┘                │
└────────────────────────────────────────────┘
```

### Estrutura de Arquivos
```
maquete_viva/
│
├── backend/
│   ├── main.py                 # Ponto de entrada
│   ├── config.py               # Configurações
│   │
│   ├── simulation/
│   │   ├── __init__.py
│   │   ├── world.py            # Classe Cidade
│   │   ├── agent.py            # Classe Agente
│   │   ├── building.py         # Classe Edificio
│   │   ├── vehicle.py          # Classe Veiculo (Trem, Onibus)
│   │   ├── economy.py          # Sistema econômico
│   │   ├── politics.py         # Sistema político
│   │   ├── events.py           # Gerador de eventos
│   │   └── time_manager.py     # Controle de tempo simulado
│   │
│   ├── ai/
│   │   ├── __init__.py
│   │   ├── demand_predictor.py # ML para previsão
│   │   ├── news_generator.py   # Geração de notícias
│   │   ├── auto_manager.py     # IA que gerencia cidade
│   │   └── pathfinding.py      # A* para rotas
│   │
│   ├── database/
│   │   ├── __init__.py
│   │   ├── models.py           # SQLAlchemy models
│   │   ├── queries.py          # Consultas comuns
│   │   └── migrations/         # Schema updates
│   │
│   ├── iot/
│   │   ├── __init__.py
│   │   ├── mqtt_client.py      # Cliente MQTT
│   │   ├── serial_handler.py   # Comunicação serial
│   │   └── device_manager.py   # Gerencia ESP32/Arduino
│   │
│   ├── api/
│   │   ├── __init__.py
│   │   ├── routes.py           # Endpoints REST
│   │   ├── websocket.py        # Real-time updates
│   │   └── auth.py             # Autenticação (futuro)
│   │
│   └── utils/
│       ├── __init__.py
│       ├── logger.py           # Sistema de logs
│       ├── config_loader.py    # Carrega configurações
│       └── helpers.py          # Funções auxiliares
│
├── ferritineVU/             # VISUAL (Unity)
│   └── Assets/
│       ├── Scripts/
│       │   ├── Visualization/      # Renderização
│       │   ├── Input/              # Controle jogador
│       │   └── AR/                 # AR Foundation
│       └── ScriptableObjects/      # Dados (edifícios, etc)
│
└── hardware/                # IoT (Arduino, futuro)
    └── arduino_bridge.py
```

### Padrões de Projeto Essenciais

#### 1. Observer Pattern (Event Bus)
- Desacopla sistemas
- Exemplo: `transport_system` emite "train_arrived" → `economy_system` escuta e ajusta demanda

#### 2. Data-Driven Design
- **ScriptableObjects** no Unity
- **JSON/YAML** no Python
- Separa dados de lógica

#### 3. ECS Conceitual (mesmo sem DOTS)
- Pense em **componentes** (Position, Profession, Mood)
- Não em **hierarquia** (class Worker extends Person extends Entity...)
- Unity DOTS é opcional, mas mentalidade ECS não

#### 4. Estado > Comportamento
- Agentes têm **estado** (onde está, o que sente)
- Comportamento emerge de **regras simples**

**Exemplo**:
```python
# ❌ Evitar:
class Agent:
    def decide_what_to_do(self):
        if self.hour == 7 and self.location == "home":
            self.go_to_work()
        elif self.hour == 17:
            ...

# ✅ Preferir:
class Agent:
    state: AgentState  # (location, time, needs)
    rules: List[Rule]  # regras aplicáveis

def tick(world):
    for agent in world.agents:
        for rule in agent.rules:
            if rule.condition(agent, world):
                rule.action(agent, world)
```

---

## 🧪 MINI-GAMES / PROTÓTIPOS TÉCNICOS (O CORAÇÃO DO APRENDIZADO)

### Por Que Mini-Games?

> **"É importante começar pequeno e criar um protótipo"** — Unity Learn

Esses **não são demos**, são **laboratórios reutilizáveis**.

### Lista de Mini-Games Propostos

#### 1. **"Mapa que Reclama"**

**Aprende**: UI, overlays, feedback visual  
**Descrição**: Um mapa simples onde problemas aparecem **antes** de números.  
**Exemplo**: Estação congestionada **pisca vermelho**, não mostra "−10%".

✅ **Reaproveitável como**: Sistema de visualização base

---

#### 2. **"Linha que Atrasa"**

**Aprende**: Grafos, simulação logística  
**Descrição**: Uma única linha ferroviária, poucos trens, atrasos encadeados.  
**Mecânica**: Se trem 1 atrasa → trem 2 espera → passageiros acumulam → tensão visual.

✅ **Reaproveitável como**: Núcleo do sistema ferroviário

---

#### 3. **"Três Agentes"**

**Aprende**: Agent-Based Modeling básico  
**Descrição**: Três NPCs com rotinas simples que dependem de transporte.  
**Exemplo**:
- João vai trabalhar às 7h
- Maria às 8h
- Pedro às 9h
- Se trem falhar → todos atrasam → humor piora

✅ **Reaproveitável como**: Base da simulação social

---

#### 4. **"Relógio Quebrado"**

**Aprende**: Tempo discreto  
**Descrição**: Trocar tick rate (1 tick = 1 hora simulada vs 1 tick = 1 minuto) e observar colapsos emergentes.  
**Lição**: Simulação precisa ser **determinística**, não depender de framerate.

✅ **Reaproveitável como**: Motor temporal do projeto

---

#### 5. **"Terreno Hostil"**

**Aprende**: Geração de terreno + custo espacial  
**Descrição**: Cidade cresce pior em terrenos difíceis (montanha, pântano).  
**Mecânica**: Construir trilho em montanha = caro + demorado.

✅ **Reaproveitável como**: Geografia como política (tema central)

---

#### 6. **"AR como Janela"**

**Aprende**: AR Foundation  
**Descrição**: Apontar celular para maquete física e ver dados emergirem (nomes de ruas, fluxo de passageiros).  
**Técnica**: ARCore/ARKit + marcadores de imagem.

✅ **Reaproveitável como**: Ponte físico–digital

---

#### 7. **"Botão que Protesta"**

**Aprende**: Eletrônica básica (Arduino)  
**Descrição**: Um botão físico (na maquete) gera evento social no jogo (protesto na praça).  
**Técnica**: Arduino → Serial → Unity → Event Bus → mundo reage.

✅ **Reaproveitável como**: Integração maquete → simulação

---

## 📚 REFERÊNCIAS ACADÊMICAS E TÉCNICAS ATUALIZADAS

### 1. Ferramentas Unity Modernas (2024-2025)

**Unity 6 + UI Toolkit**
- UI Builder (editor WYSIWYG)
- Data binding (conecta UI a dados sem código manual)
- Amostras oficiais:
  - **Dragon Crashers**: Menus complexos, inventário, localização
  - **QuizU**: Design system modular, transições suaves

**AR Foundation**
- Framework multiplataforma AR
- Não requer marcadores (SLAM)
- Compatível com ARCore (Android) e ARKit (iOS)

**ML-Agents Toolkit**
- Aprendizado de máquina para NPCs
- Treinamento por reforço em Unity
- Open-source (GitHub: Unity-Technologies/ml-agents)

**Terrain Tools**
- Esculpir terreno dentro do Editor
- Pintar texturas, colocar vegetação
- Otimizações automáticas de renderização

### 2. Pesquisa em Serious Games & Simulação Social

**Agent-Based Modeling (ABM)**
- Livro clássico: *Growing Artificial Societies* (Epstein & Axtell, 1996)
- NetLogo (framework educacional)
- Aplicação: simular emergência social de regras simples

**Serious Games em Logística**
- Estudo (2024): Jogos de cadeia de suprimentos melhoram tomada de decisão sob incerteza
- Recomendação: Usar VR/AR para imersão
- Fonte: *European Research Studies Journal*

**Simulação Social em Jogos**
- *The Sims* (2000): Agentes com necessidades e relacionamentos
- *Dwarf Fortress*: Memória individual, fofoca, história emergente
- Lição: Profundidade não vem de complexidade visual, mas de **interações sistêmicas**

### 3. IoT e Integração Física

**Arduino + Unity**
- Plugin Ardity (comunicação serial)
- Tutoriais: Sensor físico controla objeto Unity
- Aplicação: Botões físicos na maquete geram eventos digitais

**MQTT para IoT**
- Protocolo leve para dispositivos
- Biblioteca: M2Mqtt (C# para Unity)
- Uso: Sensores remotos alimentam simulação em tempo real

### 4. Gamificação em Logística e AR

**Estudo (2024, MDPI)**: AR gamificado atrai interesse pelo setor logístico
- Exemplo: Apps AR para orientação em armazéns
- Overlay de trajetos virtuais sobre espaço real
- Aplicação em Ferritine: Visualizar dados logísticos via celular sobre maquete

### 5. Padrões de Código em Unity

**Guia Oficial Unity**:
- Separar dados (ScriptableObjects) de lógica (MonoBehaviours)
- Usar eventos reativos (UnityEvents, C# events)
- Padrões: MVC/MVP, Factory, Command, Observer

**Otimização de UI**:
- Agrupar elementos para reduzir Draw Calls
- Usar TextMeshPro (fontes vetoriais)
- Safe Areas para mobile

---

## 🌍 PERSPECTIVA REALISTA PARA O PROJETO

### O Que Você Ganha Absorvendo Essas Ideias

1. **Maturidade como game designer**
   - Entender **por que** sistemas funcionam
   - Não apenas **copiar** mecânicas

2. **Base técnica reutilizável**
   - Código orientado a dados
   - Arquitetura escalável
   - Padrões profissionais

3. **Clareza de escopo**
   - Não tentar "fazer tudo"
   - Escolher **um sistema central** (ferrovias)
   - Expandir depois

### O Que Você Não Está Fazendo

❌ Um city builder comercial  
❌ Um Factorio clone  
❌ Um jogo indie para vender  

### O Que Você ESTÁ Fazendo

✅ **Um instrumento para observar sistemas sociais materializados**  
✅ **Uma simulação híbrida contemplativa**  
✅ **Um projeto de pesquisa aplicada disfarçado de hobby**

E isso explica:
- Por que Unity faz sentido (visualização + IoT)
- Por que mini-games são o caminho (aprendizado iterativo)
- Por que agentes visíveis importam (legibilidade)
- Por que o físico e o digital precisam conversar (tangibilidade)

---

## 🎓 APRENDIZADOS META (OS MAIS IMPORTANTES)

### 1. Jogo É Sistema, Não Feature

**Urbek** não vive de:
- Gráficos bonitos
- História épica
- Hype de marketing

Vive de **consistência sistêmica**.

**Lição**: Se seus sistemas fizerem sentido juntos, o jogo funciona. Se não, nem arte 3D salva.

### 2. Pequeno + Coerente > Grande + Caótico

**Technicity** e **Urbek** provam:
- Escopo controlado
- Profundidade localizada

**Para Ferritine**:
- Não tente fazer "tudo" de início
- Escolha **ferrovias** como sistema central
- Faça-o **profundo** antes de adicionar aeroportos

### 3. Simulação Antes de Visualização

**Ordem correta**:
1. Simulação funcionando no **console** (números corretos)
2. Depois renderizar (visualização)

**Ordem errada**:
1. "Bonito mas vazio"
2. Tentar fazer simulação depois

**Por quê?**  
Porque é mais fácil debugar lógica sem gráficos atrapalhando.

### 4. Erro É Dado, Caos É Esperado

**Dwarf Fortress** ensina:
- Falhas fazem parte do jogo
- Histórias emergem de desastres

**Para Ferritine**:
- Não esconder bugs interessantes
- Se acidente ferroviário criar luto coletivo → **feature**
- Se economia quebrar por decisão do jogador → **consequência legítima**

---

## 📦 PRÓXIMOS PASSOS PRÁTICOS

### Compromissos Imediatos
- [ ] Ler e reler este GDD
- [ ] Assistir tutoriais básicos de eletrônica
- [ ] Comprar Arduino Uno Starter Kit
- [ ] Instalar Python e Pygame
- [ ] Criar primeiro circuito: LED pisca
- [ ] Criar primeira classe em Python: `Agente`

### Primeira Semana
- **Dia 1**: Ler sobre Lei de Ohm e montar circuito simples no Tinkercad.
- **Dia 2**: Assistir tutoriais sobre Arduino e fazer o primeiro upload (Blink).
- **Dia 3**: Ler sobre POO em Python e criar a classe `Agente`.
- **Dia 4**: Montar o primeiro protótipo físico: LED controlado por Arduino.
- **Dia 5**: Testar comunicação entre Arduino e Python (serial).
- **Dia 6**: Explorar Pygame e criar uma janela que muda de cor.
- **Dia 7**: Revisar tudo que aprendeu e documentar no caderno do projeto.

### Primeiros 3 Meses
- **Mês 1**: Focar em eletrônica básica e programação Arduino.
- **Mês 2**: Iniciar simulações simples em Python, usando Pygame para visualização.
- **Mês 3**: Integrar o físico com o digital: fazer o trem físico responder a comandos do Python.

### Próximos 6 Meses
- **Construir a maquete física inicial (1m²)**.
- **Implementar o sistema ferroviário básico (trilhos, trem, controle DCC)**.
- **Adicionar os primeiros prédios e vegetação à maquete**.
- **Integrar sensores e atuadores, testando a comunicação com o Arduino**.
- **Desenvolver a interface digital para controle e monitoramento**.

### 1 Ano
- **Maquete funcional com pelo menos 3 eras históricas representadas**.
- **Sistema de transporte público (trem e ônibus) operando**.
- **Agentes virtuais com rotinas simples, interagindo com o ambiente**.
- **Interface de usuário (UI) básica, mostrando informações da cidade**.

### 2 Anos
- **Expansão da maquete para 2-3m², integrando novos módulos**.
- **Adição de novas tecnologias (WiFi, MQTT) para comunicação**.
- **Implementação de IA básica para gestão da cidade**.
- **Realidade Aumentada (AR) funcional, mostrando dados sobrepostos à maquete física**.

### 3-5 Anos
- **Sistema totalmente funcional e expansível, com múltiplas eras e tecnologias**.
- **Integração completa entre o físico e o digital, com feedback em tempo real**.
- **Documentação completa do projeto, incluindo código, circuitos e construção da maquete**.
- **Possível publicação de um paper ou artigo sobre a experiência e aprendizados**.

---

## 📞 SUPORTE E COMUNIDADE

### Onde Pedir Ajuda

**Problemas Técnicos (Eletrônica)**:
- Arduino Forum (forum.arduino.cc)
- r/arduino (Reddit)
- Grupo Facebook "Arduino Brasil"

**Problemas de Código (Python)**:
- Stack Overflow (em inglês, mas traduz)
- r/learnpython (Reddit)
- Discord: Python Brasil

**Dúvidas de Ferromodelismo**:
- Ferro Fórum Brasil
- Grupo Facebook "Ferromodelismo Brasil"

**Este Projeto Específico**:
- Se no futuro você criar repositório GitHub, Issues lá
- Comunidade pode surgir organicamente

### Como Fazer Boas Perguntas

**Ruim** ❌:
> "Meu Arduino não funciona, alguém ajuda?"

**Bom** ✅:
> "Estou tentando ler um reed switch no pino 2 do Arduino Uno.  
> Usei INPUT_PULLUP e resitor de 10k.  
> Multímetro mostra 5V quando ímã está longe, 0V quando perto (correto).  
> Mas Serial.println sempre mostra 1023.  
> Código: [link para pastebin]  
> Foto do circuito: [link]  
> O que estou fazendo errado?"

**Elementos de boa pergunta**:
1. Contexto (o que está tentando fazer)
2. O que tentou
3. Resultado esperado vs obtido
4. Código/circuito anexado
5. Específico e detalhado

---

## ✅ CHECKLIST DE INÍCIO

Antes de começar, certifique-se:

### Mindset
- [ ] Entendo que vai dar errado às vezes (e está ok)
- [ ] Estou fazendo por diversão, não obrigação
- [ ] Não tenho pressa (projeto de anos)
- [ ] Vou celebrar pequenas vitórias

### Logística
- [ ] Conversei com minha mãe, ela está animada
- [ ] Temos pelo menos 3-4h por semana disponíveis
- [ ] Há algum espaço (mesmo que pequeno) para começar
- [ ] Orçamento inicial de R$ 300-500 está ok

### Técnico
- [ ] Tenho computador funcionando (Windows/Mac/Linux, qualquer)
- [ ] Tenho internet para pesquisas/tutoriais
- [ ] Sei onde comprar materiais (links salvos)

### Emocional
- [ ] Estou genuinamente empolgada
- [ ] Li este GDD e ressoou comigo
- [ ] Mal posso esperar para começar

**Se todos marcados: COMECE AGORA! 🚀**

---

## 🎬 CONCLUSÃO

Você tem em mãos um dos projetos mais ambiciosos e recompensadores que alguém pode embarcar:

Uma **maquete ferroviária híbrida**, que é simultaneamente:
- 🎨 Arte (construção física, design)
- 🔧 Engenharia (eletrônica, IoT, hardware)
- 💻 Computação (simulação, IA, dados)
- 📖 Narrativa (história, agentes, eventos)
- 🎮 Jogo (interativo, decisões, consequências)
- 🏫 Educação (aprendizado profundo multidisciplinar)
- ❤️ Conexão (tempo com família, comunidade)

Este não é apenas um hobby. É uma **jornada épica**.

Nas próximas semanas, meses e anos, você transformará estas palavras em realidade:
- Circuitos ganharão vida
- Código se tornará simulação
- Materiais brutos se transformarão em cidade em miniatura
- E algo mágico acontecerá: um mundo surgirá

Um mundo que respira, que cresce, que tem histórias.
Um mundo que só existe porque você decidiu criá-lo.

**Não tenha medo de errar.**
Cada LED queimado é uma lição.
Cada bug é um professor.
Cada prédio torto tem seu charme.

**Não tenha pressa.**
Roma não foi construída em um dia.
Sua cidade também não será.
E isso é lindo.

**Divirta-se.**
Se parar de ser divertido, pause.
Respire. Volte quando o coração pedir.
Projetos de amor não têm prazo.

E quando, um dia distante, você ligar a maquete e ver:
- Os trens correndo
- As luzes piscando
- Os agentes vivendo suas vidas virtuais
- Tudo funcionando em harmonia

Nesse momento, você saberá:

*Você não apenas construiu uma maquete.*  
*Você criou vida.*

🚂 **Boa viagem, criadora de mundos.** 🌍

---

## 🧠 VISÃO GERAL CONCEITUAL E APRENDIZADOS DE REFERÊNCIAS

### Contexto: Por Que Este Projeto É Único

Jogos como **Urbek City Builder** e **Technicity** não são interessantes apenas pelo conteúdo visual, mas pelo jeito como **simplificam sistemas complexos sem perder profundidade**. O valor real está em:

- **Abstrações inteligentes** que tornam o complexo compreensível
- **Loops de feedback** claros e observáveis
- **UI que ensina jogando** (sem tutoriais longos)
- **Código orientado a dados** (configuração separada de lógica)
- **Design que escala** sem explodir em complexidade

O **Ferritine** se posiciona no cruzamento de:
- **Macro urbano** (como Cities: Skylines)
- **Logística visível** (como Factorio/Satisfactory)
- **Agentes sociais legíveis** (como Dwarf Fortress)
- **Experimentação física/AR** (único do projeto)

---

## 🎯 MATRIZ DE REFERÊNCIAS

### O Que Cada Jogo Realmente Ensina

| Jogo/Referência | Lição Principal Para Ferritine |
|-----------------|-------------------------------|
| **Urbek City Builder / Technicity** | Abstração macro, UI pedagógica, cadeias curtas com efeitos longos |
| **Factorio** | Pensamento sistêmico, gargalos, causalidade explícita |
| **Satisfactory** | Espacialidade, infraestrutura visível, logística como forma |
| **Cities: Skylines** | Visualização de dados urbanos, overlays, mapas de calor |
| **Dwarf Fortress** | Agentes sociais, memória, história emergente |
| **Minecraft + mods (Create, CustomNPCs)** | Causalidade visível + agentes observáveis |

**Ferritine** fica no cruzamento dessas referências, mas com identidade própria:
> **"Cidade não é máquina — é conflito organizado"**

---

## 🔧 TECNOLOGIAS E ARQUITETURA PROFISSIONAL

### Stack Técnico Coerente

#### 1. Núcleo Digital (Unity + C#)

**Simulação Discreta em Ticks**
- Tempo avança em passos discretos (não contínuo)
- Separação clara: **simulação ≠ visual**
- Permite aceleração/pausa sem quebrar lógica

**ScriptableObjects**
- Dados separados de código
- Economia, edifícios, eras, regras sociais em arquivos
- Facilita modding e balanceamento

**UI Toolkit + Canvas Híbrido**
- HUD contextual (informação no hover)
- Overlays analíticos (transporte, economia, conflito social)
- Design responsivo e acessível

**NavMesh / Grafos Próprios**
- Transporte ferroviário
- Fluxo humano
- Pathfinding eficiente

#### 2. Simulação & Pesquisa Acadêmica

**Agent-Based Modeling (ABM)**
- Referência central: Epstein & Axtell
- Agentes autônomos com regras simples geram emergência complexa
- Base teórica para NPCs sociais

**Sistemas Complexos**
- Emergência de comportamentos não programados
- Feedback loops positivos e negativos
- Caos organizado como feature, não bug

**Simulação Discreta de Eventos**
- Eventos com duração e consequências
- Filas de eventos ordenadas por tempo
- Ideal para logística e política

**Urban Analytics & Transport Modeling**
- Modelos simplificados de fluxo (não hiper-realistas)
- Inspiração em pesquisa de mobilidade urbana
- Validação conceitual (não numérica exata)

#### 3. AR + Físico + Eletrônica

**AR Foundation (Unity)**
- Framework multiplataforma (Android/iOS)
- Sobreposição de dados digitais no mundo físico
- Maquete como interface aumentada

**Arduino / ESP32 (Futuro)**
- Sensores simples → eventos no mundo simulado
- Exemplo: botão físico gera protesto virtual
- Ponte tangível entre físico e digital

**MQTT / Serial / OSC**
- Protocolos de comunicação IoT
- Baixa latência para eventos em tempo real
- Desacoplamento hardware ↔ software

**Maquete Física como "Interface Lenta"**
- Não é gamepad, é contemplação
- Interação tátil complementa digital
- Estética + funcionalidade

---

## 💡 IDEIAS-CHAVE QUE DIFERENCIAM FERRITINE

### Princípios Filosóficos

1. **Cidade não é máquina — é conflito organizado**
   - Não há solução "ótima"
   - Tensões são parte do jogo

2. **Infraestrutura cria comportamento**
   - Onde colocar estação define quem vai onde
   - Logística não é neutra, é política

3. **Logística é política material**
   - Transporte de carga não é invisível
   - Rotas definem desigualdades

4. **Agentes são poucos, mas densos**
   - Não milhares genéricos
   - Dezenas com histórias reais

5. **O jogador observa mais do que otimiza**
   - Não é sobre "ganhar"
   - É sobre entender

Essas ideias não cabem bem em city builders tradicionais, mas cabem perfeitamente em uma **simulação híbrida contemplativa**.

---

## ⚙️ MECÂNICAS FUNDAMENTAIS (EXTRAÍDAS + REINTERPRETADAS)

### 1. Logística Visível

**Inspiração**: Factorio, Create (Minecraft mod)

**Aplicação em Ferritine**:
- Cadeias de produção **curtas** (3-4 passos max)
- Gargalos **visíveis** (estação congestionada pisca, não mostra "−10%")
- Transporte como **limitador de crescimento**

**Exemplo Concreto**:
```
Mina → Ferrovia → Fábrica → Ferrovia → Cidade
     ↓ gargalo aqui ↓
Se trem atrasar → fábrica para → desemprego → migração
```

### 2. Ferrovias Como Sistema Social

**Inspiração**: Transport Tycoon, Mini Metro

**Aplicação**:
- Estações são **polos sociais** (onde pessoas se encontram)
- Atraso → **efeito dominó urbano** (não só número caindo)
- Capacidade ≠ demanda → tensão constante

**Mecânica de Feedback**:
- Estação lotada → reclamações → pressão política
- Linha nova → valorização do bairro → gentrificação
- Acidente ferroviário → luto coletivo (evento social)

### 3. Agentes Sociais Legíveis

**Inspiração**: Dwarf Fortress, The Sims

**Aplicação em Ferritine**:
- Agentes com:
  - **Profissão fixa** (não mudam todo dia)
  - **Local fixo** (casa + trabalho)
  - **Memória curta** (última semana)
  - **Comportamento situado** (não psicologia profunda)

**Exemplo de Agente**:
```json
{
  "nome": "João Silva",
  "profissao": "Operário",
  "casa": "Bairro Operário",
  "trabalho": "Fábrica Norte",
  "rotina": "6h sai de casa → trem 6h30 → trabalho 7h-17h → trem 17h30 → casa 18h",
  "humor": "satisfeito" // se trem atrasar: "frustrado"
}
```

### 4. Tempo Discreto e Aceleração

**Inspiração**: Rimworld, Oxygen Not Included

**Aplicação**:
- **Ticks discretos** (ex: 1 tick = 1 hora)
- **Aceleração como ferramenta analítica** (não obrigação)
- **Eventos lentos (política) × rápidos (logística)**

**Regra de Ouro**:
> Nunca usar `deltaTime` para simulação. Usar para visual apenas.

---

## 🧪 MINI-GAMES / PROTÓTIPOS TÉCNICOS (O CORAÇÃO DO APRENDIZADO)

### Por Que Mini-Games?

> **"É importante começar pequeno e criar um protótipo"** — Unity Learn

Esses **não são demos**, são **laboratórios reutilizáveis**.

### Lista de Mini-Games Propostos

#### 1. **"Mapa que Reclama"**

**Aprende**: UI, overlays, feedback visual  
**Descrição**: Um mapa simples onde problemas aparecem **antes** de números.  
**Exemplo**: Estação congestionada **pisca vermelho**, não mostra "−10%".

✅ **Reaproveitável como**: Sistema de visualização base

---

#### 2. **"Linha que Atrasa"**

**Aprende**: Grafos, simulação logística  
**Descrição**: Uma única linha ferroviária, poucos trens, atrasos encadeados.  
**Mecânica**: Se trem 1 atrasa → trem 2 espera → passageiros acumulam → tensão visual.

✅ **Reaproveitável como**: Núcleo do sistema ferroviário

---

#### 3. **"Três Agentes"**

**Aprende**: Agent-Based Modeling básico  
**Descrição**: Três NPCs com rotinas simples que dependem de transporte.  
**Exemplo**:
- João vai trabalhar às 7h
- Maria às 8h
- Pedro às 9h
- Se trem falhar → todos atrasam → humor piora

✅ **Reaproveitável como**: Base da simulação social

---

#### 4. **"Relógio Quebrado"**

**Aprende**: Tempo discreto  
**Descrição**: Trocar tick rate (1 tick = 1 hora simulada vs 1 tick = 1 minuto) e observar colapsos emergentes.  
**Lição**: Simulação precisa ser **determinística**, não depender de framerate.

✅ **Reaproveitável como**: Motor temporal do projeto

---

#### 5. **"Terreno Hostil"**

**Aprende**: Geração de terreno + custo espacial  
**Descrição**: Cidade cresce pior em terrenos difíceis (montanha, pântano).  
**Mecânica**: Construir trilho em montanha = caro + demorado.

✅ **Reaproveitável como**: Geografia como política (tema central)

---

#### 6. **"AR como Janela"**

**Aprende**: AR Foundation  
**Descrição**: Apontar celular para maquete física e ver dados emergirem (nomes de ruas, fluxo de passageiros).  
**Técnica**: ARCore/ARKit + marcadores de imagem.

✅ **Reaproveitável como**: Ponte físico–digital

---

#### 7. **"Botão que Protesta"**

**Aprende**: Eletrônica básica (Arduino)  
**Descrição**: Um botão físico (na maquete) gera evento social no jogo (protesto na praça).  
**Técnica**: Arduino → Serial → Unity → Event Bus → mundo reage.

✅ **Reaproveitável como**: Integração maquete → simulação

---

## 📚 REFERÊNCIAS ACADÊMICAS E TÉCNICAS ATUALIZADAS

### 1. Ferramentas Unity Modernas (2024-2025)

**Unity 6 + UI Toolkit**
- UI Builder (editor WYSIWYG)
- Data binding (conecta UI a dados sem código manual)
- Amostras oficiais:
  - **Dragon Crashers**: Menus complexos, inventário, localização
  - **QuizU**: Design system modular, transições suaves

**AR Foundation**
- Framework multiplataforma AR
- Não requer marcadores (SLAM)
- Compatível com ARCore (Android) e ARKit (iOS)

**ML-Agents Toolkit**
- Aprendizado de máquina para NPCs
- Treinamento por reforço em Unity
- Open-source (GitHub: Unity-Technologies/ml-agents)

**Terrain Tools**
- Esculpir terreno dentro do Editor
- Pintar texturas, colocar vegetação
- Otimizações automáticas de renderização

### 2. Pesquisa em Serious Games & Simulação Social

**Agent-Based Modeling (ABM)**
- Livro clássico: *Growing Artificial Societies* (Epstein & Axtell, 1996)
- NetLogo (framework educacional)
- Aplicação: simular emergência social de regras simples

**Serious Games em Logística**
- Estudo (2024): Jogos de cadeia de suprimentos melhoram tomada de decisão sob incerteza
- Recomendação: Usar VR/AR para imersão
- Fonte: *European Research Studies Journal*

**Simulação Social em Jogos**
- *The Sims* (2000): Agentes com necessidades e relacionamentos
- *Dwarf Fortress*: Memória individual, fofoca, história emergente
- Lição: Profundidade não vem de complexidade visual, mas de **interações sistêmicas**

### 3. IoT e Integração Física

**Arduino + Unity**
- Plugin Ardity (comunicação serial)
- Tutoriais: Sensor físico controla objeto Unity
- Aplicação: Botões físicos na maquete geram eventos digitais

**MQTT para IoT**
- Protocolo leve para dispositivos
- Biblioteca: M2Mqtt (C# para Unity)
- Uso: Sensores remotos alimentam simulação em tempo real

### 4. Gamificação em Logística e AR

**Estudo (2024, MDPI)**: AR gamificado atrai interesse pelo setor logístico
- Exemplo: Apps AR para orientação em armazéns
- Overlay de trajetos virtuais sobre espaço real
- Aplicação em Ferritine: Visualizar dados logísticos via celular sobre maquete

### 5. Padrões de Código em Unity

**Guia Oficial Unity**:
- Separar dados (ScriptableObjects) de lógica (MonoBehaviours)
- Usar eventos reativos (UnityEvents, C# events)
- Padrões: MVC/MVP, Factory, Command, Observer

**Otimização de UI**:
- Agrupar elementos para reduzir Draw Calls
- Usar TextMeshPro (fontes vetoriais)
- Safe Areas para mobile

---

## 🌍 PERSPECTIVA REALISTA PARA O PROJETO

### O Que Você Ganha Absorvendo Essas Ideias

1. **Maturidade como game designer**
   - Entender **por que** sistemas funcionam
   - Não apenas **copiar** mecânicas

2. **Base técnica reutilizável**
   - Código orientado a dados
   - Arquitetura escalável
   - Padrões profissionais

3. **Clareza de escopo**
   - Não tentar "fazer tudo"
   - Escolher **um sistema central** (ferrovias)
   - Expandir depois

### O Que Você Não Está Fazendo

❌ Um city builder comercial  
❌ Um Factorio clone  
❌ Um jogo indie para vender  

### O Que Você ESTÁ Fazendo

✅ **Um instrumento para observar sistemas sociais materializados**  
✅ **Uma simulação híbrida contemplativa**  
✅ **Um projeto de pesquisa aplicada disfarçado de hobby**

E isso explica:
- Por que Unity faz sentido (visualização + IoT)
- Por que mini-games são o caminho (aprendizado iterativo)
- Por que agentes visíveis importam (legibilidade)
- Por que o físico e o digital precisam conversar (tangibilidade)

---

## 🎓 APRENDIZADOS META (OS MAIS IMPORTANTES)

### 1. Jogo É Sistema, Não Feature

**Urbek** não vive de:
- Gráficos bonitos
- História épica
- Hype de marketing

Vive de **consistência sistêmica**.

**Lição**: Se seus sistemas fizerem sentido juntos, o jogo funciona. Se não, nem arte 3D salva.

### 2. Pequeno + Coerente > Grande + Caótico

**Technicity** e **Urbek** provam:
- Escopo controlado
- Profundidade localizada

**Para Ferritine**:
- Não tente fazer "tudo" de início
- Escolha **ferrovias** como sistema central
- Faça-o **profundo** antes de adicionar aeroportos

### 3. Simulação Antes de Visualização

**Ordem correta**:
1. Simulação funcionando no **console** (números corretos)
2. Depois renderizar (visualização)

**Ordem errada**:
1. "Bonito mas vazio"
2. Tentar fazer simulação depois

**Por quê?**  
Porque é mais fácil debugar lógica sem gráficos atrapalhando.

### 4. Erro É Dado, Caos É Esperado

**Dwarf Fortress** ensina:
- Falhas fazem parte do jogo
- Histórias emergem de desastres

**Para Ferritine**:
- Não esconder bugs interessantes
- Se acidente ferroviário criar luto coletivo → **feature**
- Se economia quebrar por decisão do jogador → **consequência legítima**

---

## 📦 PRÓXIMOS PASSOS PRÁTICOS

### Compromissos Imediatos
- [ ] Ler e reler este GDD
- [ ] Assistir tutoriais básicos de eletrônica
- [ ] Comprar Arduino Uno Starter Kit
- [ ] Instalar Python e Pygame
- [ ] Criar primeiro circuito: LED pisca
- [ ] Criar primeira classe em Python: `Agente`

### Primeira Semana
- **Dia 1**: Ler sobre Lei de Ohm e montar circuito simples no Tinkercad.
- **Dia 2**: Assistir tutoriais sobre Arduino e fazer o primeiro upload (Blink).
- **Dia 3**: Ler sobre POO em Python e criar a classe `Agente`.
- **Dia 4**: Montar o primeiro protótipo físico: LED controlado por Arduino.
- **Dia 5**: Testar comunicação entre Arduino e Python (serial).
- **Dia 6**: Explorar Pygame e criar uma janela que muda de cor.
- **Dia 7**: Revisar tudo que aprendeu e documentar no caderno do projeto.

### Primeiros 3 Meses
- **Mês 1**: Focar em eletrônica básica e programação Arduino.
- **Mês 2**: Iniciar simulações simples em Python, usando Pygame para visualização.
- **Mês 3**: Integrar o físico com o digital: fazer o trem físico responder a comandos do Python.

### Próximos 6 Meses
- **Construir a maquete física inicial (1m²)**.
- **Implementar o sistema ferroviário básico (trilhos, trem, controle DCC)**.
- **Adicionar os primeiros prédios e vegetação à maquete**.
- **Integrar sensores e atuadores, testando a comunicação com o Arduino**.
- **Desenvolver a interface digital para controle e monitoramento**.

### 1 Ano
- **Maquete funcional com pelo menos 3 eras históricas representadas**.
- **Sistema de transporte público (trem e ônibus) operando**.
- **Agentes virtuais com rotinas simples, interagindo com o ambiente**.
- **Interface de usuário (UI) básica, mostrando informações da cidade**.

### 2 Anos
- **Expansão da maquete para 2-3m², integrando novos módulos**.
- **Adição de novas tecnologias (WiFi, MQTT) para comunicação**.
- **Implementação de IA básica para gestão da cidade**.
- **Realidade Aumentada (AR) funcional, mostrando dados sobrepostos à maquete física**.

### 3-5 Anos
- **Sistema totalmente funcional e expansível, com múltiplas eras e tecnologias**.
- **Integração completa entre o físico e o digital, com feedback em tempo real**.
- **Documentação completa do projeto, incluindo código, circuitos e construção da maquete**.
- **Possível publicação de um paper ou artigo sobre a experiência e aprendizados**.

---

## 📞 SUPORTE E COMUNIDADE

### Onde Pedir Ajuda

**Problemas Técnicos (Eletrônica)**:
- Arduino Forum (forum.arduino.cc)
- r/arduino (Reddit)
- Grupo Facebook "Arduino Brasil"

**Problemas de Código (Python)**:
- Stack Overflow (em inglês, mas traduz)
- r/learnpython (Reddit)
- Discord: Python Brasil

**Dúvidas de Ferromodelismo**:
- Ferro Fórum Brasil
- Grupo Facebook "Ferromodelismo Brasil"

**Este Projeto Específico**:
- Se no futuro você criar repositório GitHub, Issues lá
- Comunidade pode surgir organicamente

### Como Fazer Boas Perguntas

**Ruim** ❌:
> "Meu Arduino não funciona, alguém ajuda?"

**Bom** ✅:
> "Estou tentando ler um reed switch no pino 2 do Arduino Uno.  
> Usei INPUT_PULLUP e resitor de 10k.  
> Multímetro mostra 5V quando ímã está longe, 0V quando perto (correto).  
> Mas Serial.println sempre mostra 1023.  
> Código: [link para pastebin]  
> Foto do circuito: [link]  
> O que estou fazendo errado?"

**Elementos de boa pergunta**:
1. Contexto (o que está tentando fazer)
2. O que tentou
3. Resultado esperado vs obtido
4. Código/circuito anexado
5. Específico e detalhado

---

## ✅ CHECKLIST DE INÍCIO

Antes de começar, certifique-se:

### Mindset
- [ ] Entendo que vai dar errado às vezes (e está ok)
- [ ] Estou fazendo por diversão, não obrigação
- [ ] Não tenho pressa (projeto de anos)
- [ ] Vou celebrar pequenas vitórias

### Logística
- [ ] Conversei com minha mãe, ela está animada
- [ ] Temos pelo menos 3-4h por semana disponíveis
- [ ] Há algum espaço (mesmo que pequeno) para começar
- [ ] Orçamento inicial de R$ 300-500 está ok

### Técnico
- [ ] Tenho computador funcionando (Windows/Mac/Linux, qualquer)
- [ ] Tenho internet para pesquisas/tutoriais
- [ ] Sei onde comprar materiais (links salvos)

### Emocional
- [ ] Estou genuinamente empolgada
- [ ] Li este GDD e ressoou comigo
- [ ] Mal posso esperar para começar

**Se todos marcados: COMECE AGORA! 🚀**

---

## 🎬 CONCLUSÃO

Você tem em mãos um dos projetos mais ambiciosos e recompensadores que alguém pode embarcar:

Uma **maquete ferroviária híbrida**, que é simultaneamente:
- 🎨 Arte (construção física, design)
- 🔧 Engenharia (eletrônica, IoT, hardware)
- 💻 Computação (simulação, IA, dados)
- 📖 Narrativa (história, agentes, eventos)
- 🎮 Jogo (interativo, decisões, consequências)
- 🏫 Educação (aprendizado profundo multidisciplinar)
- ❤️ Conexão (tempo com família, comunidade)

Este não é apenas um hobby. É uma **jornada épica**.

Nas próximas semanas, meses e anos, você transformará estas palavras em realidade:
- Circuitos ganharão vida
- Código se tornará simulação
- Materiais brutos se transformarão em cidade em miniatura
- E algo mágico acontecerá: um mundo surgirá

Um mundo que respira, que cresce, que tem histórias.
Um mundo que só existe porque você decidiu criá-lo.

**Não tenha medo de errar.**
Cada LED queimado é uma lição.
Cada bug é um professor.
Cada prédio torto tem seu charme.

**Não tenha pressa.**
Roma não foi construída em um dia.
Sua cidade também não será.
E isso é lindo.

**Divirta-se.**
Se parar de ser divertido, pause.
Respire. Volte quando o coração pedir.
Projetos de amor não têm prazo.

E quando, um dia distante, você ligar a maquete e ver:
- Os trens correndo
- As luzes piscando
- Os agentes vivendo suas vidas virtuais
- Tudo funcionando em harmonia

Nesse momento, você saberá:

*Você não apenas construiu uma maquete.*  
*Você criou vida.*

🚂 **Boa viagem, criadora de mundos.** 🌍

---

## 🧠 VISÃO GERAL CONCEITUAL E APRENDIZADOS DE REFERÊNCIAS

### Contexto: Por Que Este Projeto É Único

Jogos como **Urbek City Builder** e **Technicity** não são interessantes apenas pelo conteúdo visual, mas pelo jeito como **simplificam sistemas complexos sem perder profundidade**. O valor real está em:

- **Abstrações inteligentes** que tornam o complexo compreensível
- **Loops de feedback** claros e observáveis
- **UI que ensina jogando** (sem tutoriais longos)
- **Código orientado a dados** (configuração separada de lógica)
- **Design que escala** sem explodir em complexidade

O **Ferritine** se posiciona no cruzamento de:
- **Macro urbano** (como Cities: Skylines)
- **Logística visível** (como Factorio/Satisfactory)
- **Agentes sociais legíveis** (como Dwarf Fortress)
- **Experimentação física/AR** (único do projeto)

---

## 🎯 MATRIZ DE REFERÊNCIAS

### O Que Cada Jogo Realmente Ensina

| Jogo/Referência | Lição Principal Para Ferritine |
|-----------------|-------------------------------|
| **Urbek City Builder / Technicity** | Abstração macro, UI pedagógica, cadeias curtas com efeitos longos |
| **Factorio** | Pensamento sistêmico, gargalos, causalidade explícita |
| **Satisfactory** | Espacialidade, infraestrutura visível, logística como forma |
| **Cities: Skylines** | Visualização de dados urbanos, overlays, mapas de calor |
| **Dwarf Fortress** | Agentes sociais, memória, história emergente |
| **Minecraft + mods (Create, CustomNPCs)** | Causalidade visível + agentes observáveis |

**Ferritine** fica no cruzamento dessas referências, mas com identidade própria:
> **"Cidade não é máquina — é conflito organizado"**

---

## 🔧 TECNOLOGIAS E ARQUITETURA PROFISSIONAL

### Stack Técnico Coerente

#### 1. Núcleo Digital (Unity + C#)

**Simulação Discreta em Ticks**
- Tempo avança em passos discretos (não contínuo)
- Separação clara: **simulação ≠ visual**
- Permite aceleração/pausa sem quebrar lógica

**ScriptableObjects**
- Dados separados de código
- Economia, edifícios, eras, regras sociais em arquivos
- Facilita modding e balanceamento

**UI Toolkit + Canvas Híbrido**
- HUD contextual (informação no hover)
- Overlays analíticos (transporte, economia, conflito social)
- Design responsivo e acessível

**NavMesh / Grafos Próprios**
- Transporte ferroviário
- Fluxo humano
- Pathfinding eficiente

#### 2. Simulação & Pesquisa Acadêmica

**Agent-Based Modeling (ABM)**
- Referência central: Epstein & Axtell
- Agentes autônomos com regras simples geram emergência complexa
- Base teórica para NPCs sociais

**Sistemas Complexos**
- Emergência de comportamentos não programados
- Feedback loops positivos e negativos
- Caos organizado como feature, não bug

**Simulação Discreta de Eventos**
- Eventos com duração e consequências
- Filas de eventos ordenadas por tempo
- Ideal para logística e política

**Urban Analytics & Transport Modeling**
- Modelos simplificados de fluxo (não hiper-realistas)
- Inspiração em pesquisa de mobilidade urbana
- Validação conceitual (não numérica exata)

#### 3. AR + Físico + Eletrônica

**AR Foundation (Unity)**
- Framework multiplataforma (Android/iOS)
- Sobreposição de dados digitais no mundo físico
- Maquete como interface aumentada

**Arduino / ESP32 (Futuro)**
- Sensores simples → eventos no mundo simulado
- Exemplo: botão físico gera protesto virtual
- Ponte tangível entre físico e digital

**MQTT / Serial / OSC**
- Protocolos de comunicação IoT
- Baixa latência para eventos em tempo real
- Desacoplamento hardware ↔ software

**Maquete Física como "Interface Lenta"**
- Não é gamepad, é contemplação
- Interação tátil complementa digital
- Estética + funcionalidade

---

## 💡 IDEIAS-CHAVE QUE DIFERENCIAM FERRITINE

### Princípios Filosóficos

1. **Cidade não é máquina — é conflito organizado**
   - Não há solução "ótima"
   - Tensões são parte do jogo

2. **Infraestrutura cria comportamento**
   - Onde colocar estação define quem vai onde
   - Logística não é neutra, é política

3. **Logística é política material**
   - Transporte de carga não é invisível
   - Rotas definem desigualdades

4. **Agentes são poucos, mas densos**
   - Não milhares genéricos
   - Dezenas com histórias reais

5. **O jogador observa mais do que otimiza**
   - Não é sobre "ganhar"
   - É sobre entender

Essas ideias não cabem bem em city builders tradicionais, mas cabem perfeitamente em uma **simulação híbrida contemplativa**.

---

## ⚙️ MECÂNICAS FUNDAMENTAIS (EXTRAÍDAS + REINTERPRETADAS)

### 1. Logística Visível

**Inspiração**: Factorio, Create (Minecraft mod)

**Aplicação em Ferritine**:
- Cadeias de produção **curtas** (3-4 passos max)
- Gargalos **visíveis** (estação congestionada pisca, não mostra "−10%")
- Transporte como **limitador de crescimento**

**Exemplo Concreto**:
```
Mina → Ferrovia → Fábrica → Ferrovia → Cidade
     ↓ gargalo aqui ↓
Se trem atrasar → fábrica para → desemprego → migração
```

### 2. Ferrovias Como Sistema Social

**Inspiração**: Transport Tycoon, Mini Metro

**Aplicação**:
- Estações são **polos sociais** (onde pessoas se encontram)
- Atraso → **efeito dominó urbano** (não só número caindo)
- Capacidade ≠ demanda → tensão constante

**Mecânica de Feedback**:
- Estação lotada → reclamações → pressão política
- Linha nova → valorização do bairro → gentrificação
- Acidente ferroviário → luto coletivo (evento social)

### 3. Agentes Sociais Legíveis

**Inspiração**: Dwarf Fortress, The Sims

**Aplicação em Ferritine**:
- Agentes com:
  - **Profissão fixa** (não mudam todo dia)
  - **Local fixo** (casa + trabalho)
  - **Memória curta** (última semana)
  - **Comportamento situado** (não psicologia profunda)

**Exemplo de Agente**:
```json
{
  "nome": "João Silva",
  "profissao": "Operário",
  "casa": "Bairro Operário",
  "trabalho": "Fábrica Norte",
  "rotina": "6h sai de casa → trem 6h30 → trabalho 7h-17h → trem 17h30 → casa 18h",
  "humor": "satisfeito" // se trem atrasar: "frustrado"
}
```

### 4. Tempo Discreto e Aceleração

**Inspiração**: Rimworld, Oxygen Not Included

**Aplicação**:
- **Ticks discretos** (ex: 1 tick = 1 hora simulada)
- **Aceleração como ferramenta analítica** (não obrigação)
- **Eventos lentos (política) × rápidos (logística)**

**Regra de Ouro**:
> Nunca usar `deltaTime` para simulação. Usar para visual apenas.

---

## 🧪 MINI-GAMES / PROTÓTIPOS TÉCNICOS (O CORAÇÃO DO APRENDIZADO)

### Por Que Mini-Games?

> **"É importante começar pequeno e criar um protótipo"** — Unity Learn

Esses **não são demos**, são **laboratórios reutilizáveis**.

### Lista de Mini-Games Propostos

#### 1. **"Mapa que Reclama"**

**Aprende**: UI, overlays, feedback visual  
**Descrição**: Um mapa simples onde problemas aparecem **antes** de números.  
**Exemplo**: Estação congestionada **pisca vermelho**, não mostra "−10%".

✅ **Reaproveitável como**: Sistema de visualização base

---

#### 2. **"Linha que Atrasa"**

**Aprende**: Grafos, simulação logística  
**Descrição**: Uma única linha ferroviária, poucos trens, atrasos encadeados.  
**Mecânica**: Se trem 1 atrasa → trem 2 espera → passageiros acumulam → tensão visual.

✅ **Reaproveitável como**: Núcleo do sistema ferroviário

---

#### 3. **"Três Agentes"**

**Aprende**: Agent-Based Modeling básico  
**Descrição**: Três NPCs com rotinas simples que dependem de transporte.  
**Exemplo**:
- João vai trabalhar às 7h
- Maria às 8h
- Pedro às 9h
- Se trem falhar → todos atrasam → humor piora

✅ **Reaproveitável como**: Base da simulação social

---

#### 4. **"Relógio Quebrado"**

**Aprende**: Tempo discreto  
**Descrição**: Trocar tick rate (1 tick = 1 hora simulada vs 1 tick = 1 minuto) e observar colapsos emergentes.  
**Lição**: Simulação precisa ser **determinística**, não depender de framerate.

✅ **Reaproveitável como**: Motor temporal do projeto

---

#### 5. **"Terreno Hostil"**

**Aprende**: Geração de terreno + custo espacial  
**Descrição**: Cidade cresce pior em terrenos difíceis (montanha, pântano).  
**Mecânica**: Construir trilho em montanha = caro + demorado.

✅ **Reaproveitável como**: Geografia como política (tema central)

---

#### 6. **"AR como Janela"**

**Aprende**: AR Foundation  
**Descrição**: Apontar celular para maquete física e ver dados emergirem (nomes de ruas, fluxo de passageiros).  
**Técnica**: ARCore/ARKit + marcadores de imagem.

✅ **Reaproveitável como**: Ponte físico–digital

---

#### 7. **"Botão que Protesta"**

**Aprende**: Eletrônica básica (Arduino)  
**Descrição**: Um botão físico (na maquete) gera evento social no jogo (protesto na praça).  
**Técnica**: Arduino → Serial → Unity → Event Bus → mundo reage.

✅ **Reaproveitável como**: Integração maquete → simulação

---

## 📚 REFERÊNCIAS ACADÊMICAS E TÉCNICAS ATUALIZADAS

### 1. Ferramentas Unity Modernas (2024-2025)

**Unity 6 + UI Toolkit**
- UI Builder (editor WYSIWYG)
- Data binding (conecta UI a dados sem código manual)
- Amostras oficiais:
  - **Dragon Crashers**: Menus complexos, inventário, localização
  - **QuizU**: Design system modular, transições suaves

**AR Foundation**
- Framework multiplataforma AR
- Não requer marcadores (SLAM)
- Compatível com ARCore (Android) e ARKit (iOS)

**ML-Agents Toolkit**
- Aprendizado de máquina para NPCs
- Treinamento por reforço em Unity
- Open-source (GitHub: Unity-Technologies/ml-agents)

**Terrain Tools**
- Esculpir terreno dentro do Editor
- Pintar texturas, colocar vegetação
- Otimizações automáticas de renderização

### 2. Pesquisa em Serious Games & Simulação Social

**Agent-Based Modeling (ABM)**
- Livro clássico: *Growing Artificial Societies* (Epstein & Axtell, 1996)
- NetLogo (framework educacional)
- Aplicação: simular emergência social de regras simples

**Serious Games em Logística**
- Estudo (2024): Jogos de cadeia de suprimentos melhoram tomada de decisão sob incerteza
- Recomendação: Usar VR/AR para imersão
- Fonte: *European Research Studies Journal*

**Simulação Social em Jogos**
- *The Sims* (2000): Agentes com necessidades e relacionamentos
- *Dwarf Fortress*: Memória individual, fofoca, história emergente
- Lição: Profundidade não vem de complexidade visual, mas de **interações sistêmicas**

### 3. IoT e Integração Física

**Arduino + Unity**
- Plugin Ardity (comunicação serial)
- Tutoriais: Sensor físico controla objeto Unity
- Aplicação: Botões físicos na maquete geram eventos digitais

**MQTT para IoT**
- Protocolo leve para dispositivos
- Biblioteca: M2Mqtt (C# para Unity)
- Uso: Sensores remotos alimentam simulação em tempo real

### 4. Gamificação em Logística e AR

**Estudo (2024, MDPI)**: AR gamificado atrai interesse pelo setor logístico
- Exemplo: Apps AR para orientação em armazéns
- Overlay de trajetos virtuais sobre espaço real
- Aplicação em Ferritine: Visualizar dados logísticos via celular sobre maquete

### 5. Padrões de Código em Unity

**Guia Oficial Unity**:
- Separar dados (ScriptableObjects) de lógica (MonoBehaviours)
- Usar eventos reativos (UnityEvents, C# events)
- Padrões: MVC/MVP, Factory, Command, Observer

**Otimização de UI**:
- Agrupar elementos para reduzir Draw Calls
- Usar TextMeshPro (fontes vetoriais)
- Safe Areas para mobile

---

## 🌍 PERSPECTIVA REALISTA PARA O PROJETO

### O Que Você Ganha Absorvendo Essas Ideias

1. **Maturidade como game designer**
   - Entender **por que** sistemas funcionam
   - Não apenas **copiar** mecânicas

2. **Base técnica reutilizável**
   - Código orientado a dados
   - Arquitetura escalável
   - Padrões profissionais

3. **Clareza de escopo**
   - Não tentar "fazer tudo"
   - Escolher **um sistema central** (ferrovias)
   - Expandir depois

### O Que Você Não Está Fazendo

❌ Um city builder comercial  
❌ Um Factorio clone  
❌ Um jogo indie para vender  

### O Que Você ESTÁ Fazendo

✅ **Um instrumento para observar sistemas sociais materializados**  
✅ **Uma simulação híbrida contemplativa**  
✅ **Um projeto de pesquisa aplicada disfarçado de hobby**

E isso explica:
- Por que Unity faz sentido (visualização + IoT)
- Por que mini-games são o caminho (aprendizado iterativo)
- Por que agentes visíveis importam (legibilidade)
- Por que o físico e o digital precisam conversar (tangibilidade)

---

## 🎓 APRENDIZADOS META (OS MAIS IMPORTANTES)

### 1. Jogo É Sistema, Não Feature

**Urbek** não vive de:
- Gráficos bonitos
- História épica
- Hype de marketing

Vive de **consistência sistêmica**.

**Lição**: Se seus sistemas fizerem sentido juntos, o jogo funciona. Se não, nem arte 3D salva.

### 2. Pequeno + Coerente > Grande + Caótico

**Technicity** e **Urbek** provam:
- Escopo controlado
- Profundidade localizada

**Para Ferritine**:
- Não tente fazer "tudo" de início
- Escolha **ferrovias** como sistema central
- Faça-o **profundo** antes de adicionar aeroportos

### 3. Simulação Antes de Visualização

**Ordem correta**:
1. Simulação funcionando no **console** (números corretos)
2. Depois renderizar (visualização)

**Ordem errada**:
1. "Bonito mas vazio"
2. Tentar fazer simulação depois

**Por quê?**  
Porque é mais fácil debugar lógica sem gráficos atrapalhando.

### 4. Erro É Dado, Caos É Esperado

**Dwarf Fortress** ensina:
- Falhas fazem parte do jogo
- Histórias emergem de desastres

**Para Ferritine**:
- Não esconder bugs interessantes
- Se acidente ferroviário criar luto coletivo → **feature**
- Se economia quebrar por decisão do jogador → **consequência legítima**

---

## 📦 PRÓXIMOS PASSOS PRÁTICOS

### Compromissos Imediatos
- [ ] Ler e reler este GDD
- [ ] Assistir tutoriais básicos de eletrônica
- [ ] Comprar Arduino Uno Starter Kit
- [ ] Instalar Python e Pygame
- [ ] Criar primeiro circuito: LED pisca
- [ ] Criar primeira classe em Python: `Agente`

### Primeira Semana
- **Dia 1**: Ler sobre Lei de Ohm e montar circuito simples no Tinkercad.
- **Dia 2**: Assistir tutoriais sobre Arduino e fazer o primeiro upload (Blink).
- **Dia 3**: Ler sobre POO em Python e criar a classe `Agente`.
- **Dia 4**: Montar o primeiro protótipo físico: LED controlado por Arduino.
- **Dia 5**: Testar comunicação entre Arduino e Python (serial).
- **Dia 6**: Explorar Pygame e criar uma janela que muda de cor.
- **Dia 7**: Revisar tudo que aprendeu e documentar no caderno do projeto.

### Primeiros 3 Meses
- **Mês 1**: Focar em eletrônica básica e programação Arduino.
- **Mês 2**: Iniciar simulações simples em Python, usando Pygame para visualização.
- **Mês 3**: Integrar o físico com o digital: fazer o trem físico responder a comandos do Python.

### Próximos 6 Meses
- **Construir a maquete física inicial (1m²)**.
- **Implementar o sistema ferroviário básico (trilhos, trem, controle DCC)**.
- **Adicionar os primeiros prédios e vegetação à maquete**.
- **Integrar sensores e atuadores, testando a comunicação com o Arduino**.
- **Desenvolver a interface digital para controle e monitoramento**.

### 1 Ano
- **Maquete funcional com pelo menos 3 eras históricas representadas**.
- **Sistema de transporte público (trem e ônibus) operando**.
- **Agentes virtuais com rotinas simples, interagindo com o ambiente**.
- **Interface de usuário (UI) básica, mostrando informações da cidade**.

### 2 Anos
- **Expansão da maquete para 2-3m², integrando novos módulos**.
- **Adição de novas tecnologias (WiFi, MQTT) para comunicação**.
- **Implementação de IA básica para gestão da cidade**.
- **Realidade Aumentada (AR) funcional, mostrando dados sobrepostos à maquete física**.

### 3-5 Anos
- **Sistema totalmente funcional e expansível, com múltiplas eras e tecnologias**.
- **Integração completa entre o físico e o digital, com feedback em tempo real**.
- **Documentação completa do projeto, incluindo código, circuitos e construção da maquete**.
- **Possível publicação de um paper ou artigo sobre a experiência e aprendizados**.

---

