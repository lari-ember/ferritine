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

## 🛠️ GUIA DE CONSTRUÇÃO DA MAQUETE

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
├── frontend/
│   ├── dashboard/
│   │   ├── index.html
│   │   ├── style.css
│   │   ├── app.js              # Dashboard web
│   │   └── components/
│   │       ├── map_view.js     # Mapa 3D
│   │       ├── stats_panel.js  # Estatísticas
│   │       └── control_panel.js# Controles
│   │
│   └── mobile/
│       └── (futuro: app React Native/Flutter)
│
├── visualization/
│   ├── pygame_renderer.py      # Renderização 2D/isométrica
│   ├── unity_integration/      # Integração com Unity Engine (texturas voxel)
│   └── ar_overlay/             # (futuro: AR)
│
├── hardware/
│   ├── arduino/
│   │   ├── train_sensor/
│   │   │   └── train_sensor.ino
│   │   ├── switch_control/
│   │   │   └── switch_control.ino
│   │   └── lighting_control/
│   │       └── lighting_control.ino
│   │
│   └── esp32/
│       ├── main_controller/
│       │   └── main_controller.ino
│       └── wireless_sensors/
│           └── wireless_sensors.ino
│
├── data/
│   ├── city.db                 # Banco SQLite
│   ├── config.yaml             # Configurações
│   ├── scenarios/              # Cenários salvos
│   └── history/                # Logs históricos
│
├── docs/
│   ├── architecture.md         # Documentação técnica
│   ├── api_reference.md        # Documentação da API
│   ├── user_guide.md           # Manual do usuário
│   └── hardware_guide.md       # Guia de montagem
│
├── tests/
│   ├── test_simulation.py
│   ├── test_economy.py
│   ├── test_agents.py
│   └── test_iot.py
│
├── requirements.txt            # Dependências Python
├── README.md
└── LICENSE
```

### Código de Exemplo: Classe Agente Básica

```python
# backend/simulation/agent.py

import random
from enum import Enum
from typing import Optional, List

class AgentState(Enum):
    SLEEPING = "sleeping"
    AT_HOME = "at_home"
    COMMUTING = "commuting"
    WORKING = "working"
    LEISURE = "leisure"
    SHOPPING = "shopping"

class Agent:
    """
    Representa um habitante da cidade com rotinas e necessidades
    """
    
    def __init__(self, agent_id: int, name: str, age: int, 
                 home_id: int, job_id: Optional[int] = None):
        # Identificação
        self.id = agent_id
        self.name = name
        self.age = age
        
        # Localização
        self.home_id = home_id
        self.current_location_id = home_id
        self.current_vehicle_id = None  # Se estiver em transporte
        
        # Emprego
        self.job_id = job_id
        self.workplace_id = None  # ID do edifício de trabalho
        self.salary = 0
        self.money = random.randint(500, 2000)
        
        # Atributos físicos/mentais
        self.health = random.randint(70, 100)
        self.energy = random.randint(50, 100)
        self.happiness = random.randint(60, 90)
        self.hunger = random.randint(0, 30)
        
        # Habilidades (0-100)
        self.knowledge = random.randint(30, 80)
        self.strength = random.randint(40, 90)
        self.attention = random.randint(50, 95)
        
        # Traços de personalidade
        self.laziness = random.randint(0, 50)  # Chance de faltar trabalho
        self.ambition = random.randint(20, 100)  # Busca promoções
        
        # Estado atual
        self.state = AgentState.AT_HOME
        self.current_activity = "idle"
        
        # Família
        self.family_members = []  # IDs de outros agentes
        self.is_married = False
        
        # Histórico
        self.history_events = []
    
    def update(self, world, current_time):
        """
        Atualiza o agente a cada tick de simulação
        """
        # Atualizar necessidades
        self._update_needs(world.time_delta)
        
        # Máquina de estados
        if self.state == AgentState.SLEEPING:
            self._sleep(world, current_time)
        elif self.state == AgentState.AT_HOME:
            self._decide_activity(world, current_time)
        elif self.state == AgentState.COMMUTING:
            self._commute(world)
        elif self.state == AgentState.WORKING:
            self._work(world, current_time)
        elif self.state == AgentState.LEISURE:
            self._leisure(world, current_time)
        elif self.state == AgentState.SHOPPING:
            self._shop(world)
    
    def _update_needs(self, time_delta_minutes):
        """
        Atualiza fome, energia, etc.
        """
        # Fome aumenta com o tempo
        self.hunger += time_delta_minutes * 0.05
        self.hunger = min(100, self.hunger)
        
        # Energia diminui se acordado
        if self.state != AgentState.SLEEPING:
            self.energy -= time_delta_minutes * 0.1
            self.energy = max(0, self.energy)
        
        # Felicidade influenciada por necessidades
        if self.hunger > 70:
            self.happiness -= 0.1
        if self.energy < 20:
            self.happiness -= 0.1
        
        self.happiness = max(0, min(100, self.happiness))
    
    def _decide_activity(self, world, current_time):
        """
        Decide o que fazer com base no horário e necessidades
        """
        hour = current_time.hour
        
        # Dormir à noite
        if hour >= 22 or hour < 6:
            self.state = AgentState.SLEEPING
            return
        
        # Fome alta? Comer
        if self.hunger > 70:
            self._eat(world)
            return
        
        # Dia de semana e horário de trabalho
        if world.is_weekday() and 7 <= hour < 17 and self.job_id:
            # Chance de faltar por preguiça
            if random.randint(0, 100) < self.laziness:
                self.history_events.append(f"Faltou ao trabalho por preguiça")
                self.state = AgentState.LEISURE
            else:
                self._go_to_work(world)
        
        # Tempo livre
        else:
            # 50% chance de fazer compras se tiver dinheiro
            if random.random() < 0.3 and self.money > 50:
                self.state = AgentState.SHOPPING
            else:
                self.state = AgentState.LEISURE
    
    def _go_to_work(self, world):
        """
        Inicia deslocamento para o trabalho
        """
        if self.workplace_id == self.current_location_id:
            self.state = AgentState.WORKING
        else:
            # Encontrar rota
            route = world.find_route(self.current_location_id, 
                                      self.workplace_id)
            if route:
                self.state = AgentState.COMMUTING
                self.target_location = self.workplace_id
                # Embarcar em veículo (trem ou ônibus)
                vehicle = world.get_best_vehicle(route)
                if vehicle:
                    vehicle.add_passenger(self)
                    self.current_vehicle_id = vehicle.id
    
    def _work(self, world, current_time):
        """
        Trabalha e ganha salário
        """
        hour = current_time.hour
        
        # Trabalha das 8h às 17h
        if hour >= 17:
            # Fim do expediente
            self.money += self.salary / 30  # Salário diário
            self.energy -= 30
            self.history_events.append(f"Trabalhou e ganhou R${self.salary/30:.2f}")
            self._go_home(world)
        else:
            # Trabalhar reduz energia e aumenta fome
            self.energy -= 0.2
            self.hunger += 0.3
            
            # Chance de acidente/erro se atenção baixa
            if random.randint(0, 100) > self.attention:
                world.trigger_event("workplace_accident", self)
    
    def _commute(self, world):
        """
        Está em trânsito
        """
        if self.current_vehicle_id:
            vehicle = world.get_vehicle(self.current_vehicle_id)
            if vehicle and vehicle.current_station_id == self.target_location:
                # Chegou ao destino
                vehicle.remove_passenger(self)
                self.current_vehicle_id = None
                self.current_location_id = self.target_location
                
                if self.target_location == self.workplace_id:
                    self.state = AgentState.WORKING
                elif self.target_location == self.home_id:
                    self.state = AgentState.AT_HOME
        
        # Se estiver dirigindo e com sono
        if self.energy < 15 and random.random() < 0.05:
            world.trigger_event("car_accident", self)
            self.health -= random.randint(10, 40)
    
    def _eat(self, world):
        """
        Consome comida
        """
        food_cost = 15
        if self.money >= food_cost:
            self.money -= food_cost
            self.hunger = max(0, self.hunger - 50)
            self.happiness += 5
            world.economy.add_transaction("food", food_cost)
    
    def _go_home(self, world):
        """
        Retorna para casa
        """
        if self.current_location_id == self.home_id:
            self.state = AgentState.AT_HOME
        else:
            self.state = AgentState.COMMUTING
            self.target_location = self.home_id
    
    def _sleep(self, world, current_time):
        """
        Dorme e recupera energia
        """
        self.energy += 0.5
        self.energy = min(100, self.energy)
        
        # Acorda às 6h
        if current_time.hour >= 6:
            self.state = AgentState.AT_HOME
    
    def _leisure(self, world, current_time):
        """
        Atividade de lazer
        """
        # Lazer aumenta felicidade
        self.happiness += 0.2
        self.energy -= 0.1
        
        # À noite, volta para casa
        if current_time.hour >= 20:
            self._go_home(world)
    
    def _shop(self, world):
        """
        Faz compras
        """
        if self.money > 100:
            spending = random.randint(30, 100)
            self.money -= spending
            self.happiness += 10
            world.economy.add_transaction("retail", spending)
        
        self.state = AgentState.AT_HOME
    
    def get_status(self):
        """
        Retorna status atual do agente
        """
        return {
            "id": self.id,
            "name": self.name,
            "age": self.age,
            "state": self.state.value,
            "location": self.current_location_id,
            "health": self.health,
            "energy": self.energy,
            "happiness": self.happiness,
            "money": self.money,
            "hunger": self.hunger
        }
```

---

## 🔌 CÓDIGO DE EXEMPLO: Arduino + Sensor

```cpp
// hardware/arduino/train_sensor/train_sensor.ino

/*
 * Sensor de Trem com Reed Switch
 * Detecta quando trem passa e envia via Serial para Python
 */

const int REED_PIN = 2;        // Pino do reed switch
const int LED_PIN = 13;        // LED indica detecção
const int SENSOR_ID = 1;       // ID único deste sensor

bool lastState = HIGH;         // Estado anterior (sem ímã)
unsigned long lastDebounceTime = 0;
const unsigned long DEBOUNCE_DELAY = 50;  // Anti-bounce

void setup() {
  Serial.begin(9600);
  pinMode(REED_PIN, INPUT_PULLUP);  // Pull-up interno
  pinMode(LED_PIN, OUTPUT);
  
  Serial.println("Sensor de Trem Iniciado");
  Serial.print("Sensor ID: ");
  Serial.println(SENSOR_ID);
}

void loop() {
  int reading = digitalRead(REED_PIN);
  
  // Debounce
  if (reading != lastState) {
    lastDebounceTime = millis();
  }
  
  if ((millis() - lastDebounceTime) > DEBOUNCE_DELAY) {
    // Mudança estável detectada
    if (reading == LOW) {  // Ímã presente (trem passou)
      digitalWrite(LED_PIN, HIGH);
      
      // Envia JSON para Python
      Serial.print("{\"sensor_id\":");
      Serial.print(SENSOR_ID);
      Serial.print(",\"event\":\"train_detected\",\"timestamp\":");
      Serial.print(millis());
      Serial.println("}");
      
      delay(500);  // Evita múltiplas leituras
    } else {
      digitalWrite(LED_PIN, LOW);
    }
  }
  
  lastState = reading;
  delay(10);
}
```

### Código Python: Leitura do Sensor

```python
# backend/iot/serial_handler.py

import serial
import json
import threading
import time

class ArduinoSensorReader:
    """
    Lê dados dos sensores Arduino via porta serial
    """
    
    def __init__(self, port='/dev/ttyUSB0', baudrate=9600):
        self.port = port
        self.baudrate = baudrate
        self.serial_conn = None
        self.running = False
        self.callbacks = {}
        
    def connect(self):
        """
        Estabelece conexão serial
        """
        try:
            self.serial_conn = serial.Serial(
                self.port, 
                self.baudrate, 
                timeout=1
            )
            time.sleep(2)  # Aguarda Arduino resetar
            print(f"Conectado ao Arduino em {self.port}")
            return True
        except serial.SerialException as e:
            print(f"Erro ao conectar: {e}")
            return False
    
    def start_reading(self):
        """
        Inicia thread de leitura
        """
        self.running = True
        self.thread = threading.Thread(target=self._read_loop)
        self.thread.daemon = True
        self.thread.start()
    
    def _read_loop(self):
        """
        Loop de leitura contínua
        """
        while self.running:
            if self.serial_conn and self.serial_conn.in_waiting:
                try:
                    line = self.serial_conn.readline().decode('utf-8').strip()
                    
                    if line.startswith('{'):  # JSON
                        data = json.loads(line)
                        self._handle_sensor_data(data)
                    else:
                        print(f"Arduino: {line}")
                        
                except json.JSONDecodeError:
                    print(f"JSON inválido: {line}")
                except Exception as e:
                    print(f"Erro na leitura: {e}")
            
            time.sleep(0.01)
    
    def _handle_sensor_data(self, data):
        """
        Processa dados do sensor
        """
        sensor_id = data.get('sensor_id')
        event = data.get('event')
        
        print(f"Sensor {sensor_id}: {event}")
        
        # Chama callback registrado
        if sensor_id in self.callbacks:
            self.callbacks[sensor_id](data)
    
    def register_callback(self, sensor_id, callback_func):
        """
        Registra função para ser chamada quando sensor detectar algo
        """
        self.callbacks[sensor_id] = callback_func
    
    def send_command(self, command):
        """
        Envia comando para Arduino
        """
        if self.serial_conn:
            self.serial_conn.write((command + '\n').encode('utf-8'))
    
    def stop(self):
        """
        Para a leitura e fecha conexão
        """
        self.running = False
        if self.serial_conn:
            self.serial_conn.close()

# Exemplo de uso
if __name__ == "__main__":
    reader = ArduinoSensorReader(port='COM3')  # Ajustar porta
    
    if reader.connect():
        # Callback quando trem for detectado
        def on_train_detected(data):
            print(f"🚂 Trem detectado no sensor {data['sensor_id']}!")
            # Aqui você atualizaria a simulação
        
        reader.register_callback(1, on_train_detected)
        reader.start_reading()
        
        try:
            while True:
                time.sleep(1)
        except KeyboardInterrupt:
            reader.stop()
            print("Desconectado")
```

---

## 🎨 EXEMPLOS DE CENÁRIOS E NARRATIVAS

### Cenário 1: "O Grande Inverno de 1923"

**Contexto**: Cidade recém-industrializada enfrenta inverno rigoroso

**Eventos Encadeados**:
1. **Dia 1**: Temperatura cai drasticamente (-5°C)
2. **Dia 3**: Trilhos congelam, trens atrasam 2h
3. **Dia 5**: Estoque de carvão (aquecimento) na cidade esgota
4. **Dia 7**: Trem de carga com carvão fica preso em nevasca
5. **Dia 10**: População protesta, felicidade cai 40%
6. **Dia 12**: Prefeito declara estado de emergência
7. **Dia 15**: Voluntários organizam comboio alternativo
8. **Dia 18**: Carvão chega, crise resolve

**Manchetes Geradas**:
- *"Inverno Paralisa Transporte Ferroviário"*
- *"Famílias Sofrem Sem Aquecimento"*
- *"Heróis Anônimos Salvam a Cidade"*

**Impacto Permanente**:
- Cidade constrói armazém de emergência
- Lei exige reserva mínima de combustível
- Monumento aos voluntários é erguido

---

### Cenário 2: "A Eleição de Maria Santos"

**Contexto**: Primeira mulher eleita prefeita (1978)

**Narrativa**:
- Maria Santos, professora de 45 anos, decide candidatar-se
- Campanha focada em educação e transporte público
- Enfrenta preconceito e oposição conservadora
- Vence eleição por 52% dos votos
- **Mandato**: Constrói 3 escolas, expande linha de ônibus
- Reeleita em 1982 com 67% dos votos

**Legado**:
- Escola principal leva seu nome
- Aumenta participação de mulheres na política
- Felicidade média sobe 15% durante seu mandato

---

### Cenário 3: "O Acidente da Curva do Diabo"

**Contexto**: Colisão entre trem de passageiros e carga (1956)

**Sequência**:
1. Maquinista João Silva está com fadiga alta (trabalhou 12h)
2. Atenção cai, não vê sinal vermelho
3. Trem de passageiros entra em bloqueio ocupado
4. Colide com trem de carga parado
5. **Vítimas**: 12 feridos, 2 mortos (incluindo João)
6. Investigação: Falha humana + falta de manutenção em sinal
7. **Mudanças**: Lei limita jornada a 8h, sinais são modernizados

**Eventos Posteriores**:
- Viúva de João recebe pensão vitalícia
- Sindicato dos ferroviários fortalecido
- Curva recebe placa memorial
- Taxa de acidentes cai 60% nos anos seguintes

---

## 🌐 INTEGRAÇÃO COM REALIDADE AUMENTADA

### Visão Técnica

#### Hardware Necessário
- **Smartphone** com ARCore (Android) ou ARKit (iOS)
- **Óculos Meta Quest** (opcional, futuro)

#### Software
- **Unity** com AR Foundation (ou Vuforia)
- **Python** envia dados via WebSocket para Unity
- **Marcadores**: QR codes ou imagens na maquete

### Funcionalidades AR

#### Nível 1: Informações Básicas
Apontar câmera para prédio mostra:
- Nome do edifício
- Tipo (residencial, comercial, industrial)
- Número de ocupantes
- Valor imobiliário

#### Nível 2: Agentes Virtuais
Ver agentes 3D andando nas ruas:
- Modelos low-poly estilizados
- Animações de caminhada
- Balões de diálogo com pensamentos

**Exemplo**:
> Agente "Carlos Oliveira" caminhando para estação  
> Balão: "Espero que o trem não esteja atrasado hoje..."

#### Nível 3: Dados Temporais
Visualizar linha do tempo:
- Slider virtual controla época histórica
- Prédios mudam aparência (preto e branco → colorido)
- Veículos mudam (maria fumaça → trem elétrico)

#### Nível 4: Simulação Overlay
Ver camadas de dados sobrepostas:
- **Heatmap de Tráfego**: Ruas verdes (fluido) a vermelhas (congestionado)
- **Conexões Econômicas**: Linhas animadas mostrando fluxo de bens
- **Satisfação Popular**: Prédios brilham verde (felizes) ou vermelho (insatisfeitos)

### Exemplo de Interação AR

**Cena**:
1. Você aponta celular para a maquete
2. Aparece menu flutuante: [Info] [Agentes] [História] [Controle]
3. Seleciona "Agentes"
4. 20 bonecos 3D aparecem andando pelas ruas
5. Toca em um boneco
6. Ficha aparece:
   ```
   Nome: Ana Silva
   Idade: 28
   Profissão: Professora
   Estado: Indo para o trabalho
   Felicidade: 75/100
   Dinheiro: R$ 1.245
   ```
7. Botão "Seguir Rotina" - câmera acompanha Ana durante o dia

---

## 📈 ROADMAP DE DESENVOLVIMENTO (5 ANOS)

### Ano 1: Fundação
**Q1 (Jan-Mar)**:
- ✅ Aprender eletrônica básica
- ✅ Montar primeiros circuitos (LEDs, sensores)
- ✅ Estudar Python para simulação

**Q2 (Abr-Jun)**:
- ✅ Criar classes básicas (Agente, Cidade)
- ✅ Simulação funcionando (10 agentes, economia simples)
- ✅ Visualização 2D em Pygame

**Q3 (Jul-Set)**:
- ✅ Primeiro Arduino comunicando com Python
- ✅ Sensor de trem funcionando
- ✅ Planejar layout da maquete (desenho)

**Q4 (Out-Dez)**:
- ✅ Construir base física (1m²)
- ✅ Montar trilhos básicos
- ✅ Primeiro trem rodando com sensor integrado

### Ano 2: Expansão Física
**Q1**:
- Construir 5-8 prédios detalhados
- Sistema de iluminação (LEDs em prédios)
- 50 agentes na simulação

**Q2**:
- Adicionar desvios automatizados (servos)
- Sistema de sinais ferroviários
- Economia mais complexa (3 indústrias)

**Q3**:
- Primeiro ônibus motorizado
- Ruas pavimentadas com detalhes
- 100 agentes

**Q4**:
- Dashboard web funcional
- Visualização 3D básica
- Modo história (Capítulo 1)

### Ano 3: Automação e IA
**Q1**:
- IA para previsão de demanda
- Sistema político (eleições)
- 200 agentes

**Q2**:
- Geração procedural de notícias
- Eventos aleatórios complexos
- Múltiplos trens operando simultaneamente

**Q3**:
- App mobile básico (monitoramento)
- Modo autônomo (IA gerencia sozinha)
- Expansão física para 1,5m²

**Q4**:
- Sistema de construção realista
- Logística de materiais
- Modo desafio (5 cenários)

### Ano 4: Realidade Aumentada
**Q1**:
- Aprender Unity e AR Foundation
- Primeiros testes com marcadores

**Q2**:
- AR Nível 1 (informações básicas)
- Agentes 3D caminhando

**Q3**:
- AR Nível 2-3 (dados temporais, overlay)
- Interação touch (selecionar agentes)

**Q4**:
- AR completo e polido
- Teste com óculos Meta Quest
- 500 agentes na simulação

### Ano 5: Refinamento e História
**Q1**:
- Implementar todas as 4 eras históricas
- Trocar miniaturas físicas (marias fumaça → modernos)
- Modo história completo (4 capítulos)

**Q2**:
- Sistema de narrativa profundo
- 1000 agentes
- Famílias com gerações (netos dos fundadores)

**Q3**:
- Expansão física para 3m² (módulos)
- Aeroporto ou porto (novo sistema de transporte)
- Veículos autônomos avançados

**Q4**:
- Polimento geral
- Documentação completa
- Projeto "finalizável" (sempre expansível)

---

## 🎯 CRITÉRIOS DE SUCESSO

### Técnicos
- ✅ Simulação roda estável com 500+ agentes
- ✅ Hardware detecta 99% dos eventos físicos corretamente
- ✅ Sincronização física-digital com lag <100ms
- ✅ IA mantém cidade equilibrada por 30+ dias sem intervenção
- ✅ AR funciona fluentemente em smartphone médio

### Experiência do Usuário
- ✅ Observador casual nota "algo especial" em <2 minutos
- ✅ Ao descobrir a simulação, reação é "UAU, tem TUDO isso?"
- ✅ Sensação de "cidade viva" é percebida
- ✅ Interface intuitiva (alguém usa sem manual)

### Pessoais
- ✅ Você e sua mãe se divertem construindo
- ✅ Aprendizado técnico efetivo (eletrônica, IoT, IA)
- ✅ Sensação de realização a cada marco
- ✅ Projeto nunca parece "obrigação"

### Emocionais (para visitantes)
- ✅ Ternura (detalhes amorosos)
- ✅ Admiração (complexidade técnica)
- ✅ Curiosidade (vontade de explorar mais)
- ✅ Conforto (nostalgia, acolhimento)
- ✅ Alegria (surpresa positiva)

---

## 🔮 VISÕES DE FUTURO (Além de 5 Anos)

### Maquete-Rede
- Conectar maquetes de amigos/família via internet
- Comércio entre cidades (trem vai fisicamente de A para B)
- Migrações de agentes entre cidades

### Gamificação Avançada
- Multiplayer: Cada jogador gerencia uma empresa (trem, ônibus, táxi)
- Competição econômica
- Cooperação em crises (enchentes regionais)

### Museu/Exposição
- Instalação pública
- Visitantes votam em políticas (painel touch)
- Tela gigante mostra simulação
- QR codes para ver AR em smartphones pessoais

### Pesquisa Acadêmica
- Publicar paper sobre simulação de agentes
- Modelagem de transporte urbano
- IA para cidades inteligentes
- Educação STEAM (ciência, tecnologia, engenharia, artes, matemática)

### Versão VR Completa
- Andar virtualmente pelas ruas da cidade
- Primeira pessoa como agente
- Dirigir trens/ônibus manualmente
- Interação social com NPCs

### Kit Comercial
- Versão simplificada para venda
- Manual detalhado de montagem
- Software pré-configurado
- Comunidade online de construtores

---

## 🛡️ GERENCIAMENTO DE RISCOS

### Riscos Técnicos

#### Risco 1: Componentes Eletrônicos Queimados
**Probabilidade**: Alta (iniciante)  
**Impacto**: Médio (custo de R$ 50-200)  
**Mitigação**:
- Sempre usar resistores com LEDs
- Testar circuitos no Tinkercad primeiro
- Comprar componentes extras (backup)
- Seguir tutoriais passo a passo
- Usar multímetro para verificar tensões

#### Risco 2: Simulação Lenta/Travada
**Probabilidade**: Média (muitos agentes)  
**Impacto**: Alto (frustrante)  
**Mitigação**:
- Otimizar código (profiling com cProfile)
- Limitar número de agentes inicialmente
- Usar estruturas de dados eficientes (numpy, spatial indexing)
- Simular apenas área visível em alta resolução
- Background em baixa resolução

#### Risco 3: Dessincronização Física-Digital
**Probabilidade**: Média  
**Impacto**: Alto (quebra imersão)  
**Mitigação**:
- Sensores redundantes (2 por seção)
- Algoritmo de correção (Kalman filter)
- Calibração periódica
- Logs detalhados para debug

#### Risco 4: Falha em Componentes Físicos
**Probabilidade**: Média (desgaste natural)  
**Impacto**: Médio  
**Mitigação**:
- Manutenção preventiva agendada
- Peças de reposição em estoque
- Design modular (fácil substituição)
- Documentar montagem com fotos

### Riscos de Projeto

#### Risco 5: Escopo Creep (Crescimento Descontrolado)
**Probabilidade**: Muito Alta  
**Impacto**: Alto (nunca termina)  
**Mitigação**:
- **Definir MVP** (Minimum Viable Product) claro
- Trabalhar em fases fechadas
- Celebrar marcos intermediários
- Lista "futuras ideias" separada
- Aceitar que é projeto contínuo, mas ter entregáveis

#### Risco 6: Perda de Motivação
**Probabilidade**: Média (projeto longo)  
**Impacto**: Muito Alto  
**Mitigação**:
- Vitórias rápidas (primeiro LED funcionando = celebrar!)
- Trabalhar com sua mãe (social, não solitário)
- Documentar progresso (fotos antes/depois)
- Variar atividades (física + software + design)
- Não transformar em obrigação

#### Risco 7: Orçamento Estourado
**Probabilidade**: Alta (projetos sempre custam mais)  
**Impacto**: Médio  
**Mitigação**:
- Planilha de gastos rigorosa
- Contingência de 30% no orçamento
- Compras faseadas (não tudo de uma vez)
- Reaproveitar materiais (caixas, papelão)
- DIY quando possível (não comprar tudo pronto)

#### Risco 8: Falta de Espaço Físico
**Probabilidade**: Média (apartamento alugado)  
**Impacto**: Alto  
**Mitigação**:
- Design modular (pode desmontar/remontar)
- Base com rodas (mover para guardar)
- Versão menor que o planejado inicialmente
- Priorizar simulação digital enquanto não há espaço

### Riscos Pessoais

#### Risco 9: Conflitos Familiares
**Probabilidade**: Baixa  
**Impacto**: Alto (projeto conjunto)  
**Mitigação**:
- Comunicação clara de expectativas
- Divisão de tarefas justa
- Respeitar ritmo de cada um
- Direito a pausas/descanso

#### Risco 10: Frustração com Curva de Aprendizado
**Probabilidade**: Média  
**Impacto**: Médio  
**Mitigação**:
- Esperar erros (faz parte!)
- Tutoriais para iniciantes (não avançados)
- Comunidades online de suporte
- Celebrar pequenos progressos
- Lembrar: objetivo é diversão e aprendizado

---

## 📚 BIBLIOTECA DE RECURSOS

### Livros Recomendados

#### Eletrônica e Arduino
1. **"Eletrônica Para Leigos"** - Cathleen Shamieh  
   - Iniciante absoluto, teoria básica

2. **"Arduino Básico"** - Michael McRoberts  
   - Projetos práticos, explicações claras

3. **"Make: Electronics"** - Charles Platt  
   - Aprender fazendo, muitos experimentos

#### Programação e Simulação
1. **"Python Fluente"** - Luciano Ramalho  
   - Aprofundar Python, POO avançada

2. **"Automate the Boring Stuff with Python"** - Al Sweigart  
   - Gratuito online, prático

3. **"Nature of Code"** - Daniel Shiffman  
   - Simulações, comportamento emergente (gratuito)

#### Ferromodelismo
1. **"The Model Railroader's Guide to Industries Along the Tracks"**  
   - Detalhes de logística ferroviária

2. **"Track Planning for Realistic Operation"** - John Armstrong  
   - Design de layouts funcionais

### Canais de YouTube

#### Eletrônica
- **WR Kits**: Tutoriais em português, iniciante
- **Brincando com Ideias**: Projetos criativos, Arduino
- **The Ben Heck Show**: Projetos avançados (inglês)

#### Ferromodelismo
- **Ferromodelismo Brasil**: Comunidade nacional
- **Luke Towan**: Maquetes incríveis, técnicas (inglês)
- **Boylei Hobby Time**: Tutoriais de construção (inglês)

#### Programação e Simulação
- **Curso em Vídeo** (Gustavo Guanabara): Python completo
- **Coding Train**: Simulações criativas (inglês)
- **Sebastian Lague**: Game dev, simulações (inglês)

### Sites e Fóruns

#### Comunidades BR
- **Ferro Fórum Brasil**: Forum de ferromodelismo
- **Arduino.cc Forum**: Suporte oficial
- **Reddit r/modeltrains**: Comunidade internacional

#### Tutoriais e Projetos
- **Instructables**: Projetos DIY passo a passo
- **Hackaday**: Projetos de hardware/IoT
- **Thingiverse**: Modelos 3D gratuitos

#### Lojas Recomendadas (Brasil)
- **Frateschi**: Ferromodelismo nacional
- **Usinainfo**: Eletrônica/Arduino, bom preço
- **FilipeFlop**: Kits, tutoriais gratuitos
- **Mercado Livre**: Componentes avulsos baratos

### Softwares Gratuitos

#### Design e Modelagem
- **Inkscape**: Vetor (para corte a laser)
- **Blender**: Modelagem 3D
- **Tinkercad**: CAD simples, online
- **SketchUp Free**: Arquitetura/maquetes

#### Eletrônica
- **Tinkercad Circuits**: Simulação Arduino online
- **Fritzing**: Desenhar circuitos, PCB
- **Arduino IDE**: Programação de microcontroladores

#### Programação
- **Visual Studio Code**: Editor de código
- **PyCharm Community**: IDE Python
- **Git/GitHub**: Versionamento (backup do código)

---

## 🎨 CONCEITOS ARTÍSTICOS E ESTÉTICOS

### Paleta de Cores por Era

#### Era 1: Vapor e Pioneirismo (1860-1920)
**Cores Dominantes**:
- Sépia, tons terrosos
- Vermelho tijolo envelhecido
- Madeira escura (marrom chocolate)
- Verde musgo (vegetação)
- Preto (fumaça, ferro)

**Materiais**:
- Madeira aparente
- Tijolos à vista
- Ferro fundido
- Pedra

**Iluminação**:
- Amarelo quente (lampiões a gás)
- Poucas luzes, pontuais

#### Era 2: Industrialização (1920-1960)
**Cores Dominantes**:
- Cinza concreto
- Bege industrial
- Azul petróleo (maquinário)
- Vermelho ferrugem
- Preto e branco (contraste Art Déco)

**Materiais**:
- Concreto
- Metal
- Vidro (ainda raro)
- Tijolo industrial

**Iluminação**:
- Branco frio (elétrica)
- Postes de rua frequentes

#### Era 3: Modernização (1960-2000)
**Cores Dominantes**:
- Pastéis (rosa, azul bebê, amarelo claro)
- Laranja anos 70
- Cinza brutalista
- Verde limão (decoração)

**Materiais**:
- Vidro abundante
- Plástico
- Alumínio
- Laminados

**Iluminação**:
- Neon (letreiros)
- Fluorescente (interiores)
- Muita luz, urbano

#### Era 4: Contemporâneo (2000+)
**Cores Dominantes**:
- Cinza aço escovado
- Azul LED
- Verde sustentável
- Vidro espelhado (reflexos)
- RGB (displays)

**Materiais**:
- Vidro high-tech
- Painéis solares
- LED strips
- Compósitos

**Iluminação**:
- LED branco puro
- Iluminação de destaque (arquitetônica)
- Telas/displays

### Princípios de Composição Visual

#### Regra dos Terços
Dividir maquete em grid 3x3:
- Pontos focais (estação, praça) em interseções
- Não centralizar tudo

#### Hierarquia Visual
- **Protagonista**: Estação ferroviária (maior, mais detalhada)
- **Coadjuvantes**: Fábrica, igreja, prédios altos
- **Fundo**: Casas, vegetação

#### Profundidade
- **Primeiro plano**: Detalhes finos, cores saturadas
- **Segundo plano**: Menos detalhe, cores médias
- **Fundo**: Sugerido, cores desbotadas (perspectiva atmosférica)

#### Guia do Olhar
- Trilhos e ruas guiam olhar do observador
- Curvas são mais interessantes que retas
- Variação de altura (morros) cria interesse

### Weathering (Envelhecimento)
Técnicas para realismo:

#### Sujeira e Poluição
- **Lavagem (Wash)**: Tinta diluída em frestas
- **Dry Brush**: Pincel seco, tinta clara em arestas
- **Pó**: Giz pastel raspado, fuligem em chaminés

#### Desgaste
- **Lixar Bordas**: Simular tinta descascada
- **Ferrugem**: Tinta laranja/marrom em metais
- **Mofo**: Verde musgo em cantos úmidos

#### Uso
- **Trilhos**: Topo brilhante (uso), laterais enferrujadas
- **Portas**: Maçanetas desgastadas (toque frequente)
- **Ruas**: Centro mais claro (tráfego), cantos sujos

---

## 🧪 EXPERIMENTOS E PROTÓTIPOS SUGERIDOS

### Experimento 1: Micro Cidade (1 Semana)
**Objetivo**: Testar conceitos antes da maquete grande

**Materiais**:
- Caixa de papelão (30x30cm)
- Papel, cola, canetinhas
- 1 Arduino + 3 LEDs
- Python simples (5 agentes)

**O Que Fazer**:
1. Desenhar cidade no papelão (3 prédios, 1 rua)
2. Fazer prédios de papel
3. LEDs = janelas acesas
4. Simulação controla LEDs (se é noite, acende)
5. Ver funcionando!

**Aprendizado**: Ciclo completo em pequena escala

### Experimento 2: Corrida de Trens (1 Dia)
**Objetivo**: Testar sensores e cronometragem

**Materiais**:
- Trilho em oval
- 2 sensores reed
- Arduino
- 2 trens

**O Que Fazer**:
1. Colocar sensores em lados opostos do oval
2. Soltar 2 trens simultaneamente
3. Arduino mede tempo entre passagens
4. Exibir qual trem é mais rápido

**Aprendizado**: Precisão de sensores, timing

### Experimento 3: Agente Autônomo (2 Dias)
**Objetivo**: IA básica de agente

**Sem Hardware**:
- Pygame: quadrado representa agente
- Implementar máquina de estados
- Agente vai de casa (canto inferior) para trabalho (canto superior)
- Se energia <20, vai dormir antes

**Aprendizado**: Lógica de decisão, estados

### Experimento 4: Rede Neural Simples (3 Dias)
**Objetivo**: Prever demanda de trem

**Dados Simulados**:
- Gerar 1000 dias de dados fictícios
- Features: hora do dia, dia da semana, clima
- Target: número de passageiros

**Treinar**:
- Scikit-learn: RandomForestRegressor
- Avaliar precisão

**Aplicar**:
- Usar modelo para ajustar frequência de trens na simulação

**Aprendizado**: Machine learning prático

---

## 🎊 CELEBRAÇÕES E MARCOS

### Sistema de Conquistas Reais

#### 🔧 Marcos Técnicos
- **"Primeira Faísca"**: LED acende pela primeira vez
- **"Engenheiro Junior"**: Circuito complexo (10+ componentes)
- **"Mestre dos Sensores"**: 5 tipos diferentes funcionando
- **"Pythonista"**: 1000 linhas de código escritas
- **"Urbanista Digital"**: 100 agentes vivos simultaneamente
- **"Deus dos Trens"**: DCC controlando 3 trens independentemente

#### 🏗️ Marcos Físicos
- **"Primeiro Tijolo"**: Base da maquete pronta
- **"Arquiteto"**: 10 prédios construídos
- **"Paisagista"**: Vegetação completa instalada
- **"Iluminador"**: Sistema de iluminação completo
- **"Mestre Constructor"**: Maquete 1m² finalizada

#### 🎮 Marcos de Gameplay
- **"Fundador"**: Primeira cidade criada
- **"Magnata"**: R$ 1 milhão virtual acumulado
- **"Político"**: Primeira eleição realizada
- **"Historiador"**: 100 eventos históricos registrados
- **"Deus Ex Machina"**: IA gerencia 30 dias sem intervenção

### Rituais de Comemoração

**Quando atingir marco importante**:
1. **Foto/Vídeo**: Documentar o momento
2. **Diário**: Escrever o que aprendeu
3. **Celebração Simbólica**: 
   - Pequena: Café especial, doce favorito
   - Média: Jantar fora, cinema
   - Grande: Comprar ferramenta/peça desejada
4. **Compartilhar**: Mostrar para amigos/família

---

## 📖 EXEMPLO DE SESSÃO DE JOGO TÍPICA

### Sessão 1: Sábado de Tarde (3 horas)

**14h00 - Início**:
- Ligar sistema, simulação carrega estado salvo
- Cidade tem 250 agentes, 15 prédios, 3 trens
- Última sessão foi há 1 semana (7 dias simulados passaram)

**14h05 - Resumo Automático da IA**:
```
📰 Resumo dos Últimos 7 Dias:

- População cresceu 12 pessoas (8 nascimentos, 2 imigrantes)
- Eleição municipal: Carlos Oliveira reeleito (58% votos)
- Nova fábrica têxtil foi construída (85% completa)
- Linha de trem Norte teve 3 atrasos por manutenção
- Economia estável: PIB +2%, desemprego 4%
- 2 acidentes menores de trânsito

Ações Recomendadas pela IA:
⚠️ Estação Sul precisa de reforma (desgaste alto)
💡 Demanda de habitação no Bairro Oeste aumentou 30%
🚂 Considere adicionar vagão extra na linha Centro-Norte
```

**14h15 - Exploração Visual**:
- Você usa AR no celular
- Aponta para maquete, vê agentes virtuais
- Seleciona "Maria Santos" (agente aleatório)
- Lê história dela: professora, 2 filhos, economizando para casa própria
- Decide ajudá-la: aumenta salário de professores (+10%)

**14h30 - Problema Surge**:
- Notificação: "🔥 Incêndio na Fábrica Oliveira!"
- Você pausa simulação
- Câmera mostra fábrica com efeitos de fumaça
- 12 trabalhadores presos dentro
- **Decisões**:
  - [A] Enviar bombeiros (custo R$ 5.000, salva todos)
  - [B] Esperar (custo R$ 0, 4 morrem)
  - [C] Improvisar resgate com civis (custo R$ 1.000, risco)

- Você escolhe A
- Animação: caminhão de bombeiros (LED vermelho pisca fisicamente!)
- Todos salvos, felicidade +5

**15h00 - Construção**:
- Decide construir escola no Bairro Oeste (demanda alta)
- Abre menu de construção
- Escolhe terreno vazio
- Estima:
  - Custo: R$ 50.000
  - Tempo: 45 dias (simulados)
  - Materiais: 200t tijolos, 50t aço
- Confirma
- Sistema agenda transporte de materiais via trem
- Contrata 8 trabalhadores

**15h20 - Interação Física**:
- Sua mãe chega, quer ver maquete
- Você mostra o incêndio (contado dramaticamente)
- Ela quer adicionar um jardim decorativo
- Vocês pausam simulação
- 20 minutos colando minúsculas flores de papel
- Retomam: jardim agora existe virtualmente também
- Sistema gera evento: "Inauguração da Praça das Flores"

**15h45 - Otimização de Transporte**:
- Você nota: trem da linha Centro-Norte sempre lotado
- Abre painel de trens
- Ve dados: 95% ocupação média (ideal: 70-80%)
- **Opções**:
  - [A] Aumentar frequência (mais viagens, mais custo combustível)
  - [B] Adicionar vagões (investimento único)
  - [C] Construir nova linha paralela (caro, longo prazo)

- Escolhe B
- Compra 2 vagões (R$ 15.000 cada)
- Fisicamente, você troca o trem na maquete (2 vagões → 4 vagões)
- Sensor detecta novo tamanho
- Simulação atualiza: ocupação cai para 70%, lucro aumenta

**16h30 - Acompanhar Rotina**:
- AR: modo "seguir agente"
- Você escolhe João Silva (maquinista)
- Câmera virtual acompanha dia dele:
  - 06h: Acorda, café
  - 07h: Vai de ônibus (físico) para estação
  - 08h: Embarca em trem (você controla fisicamente via DCC)
  - 08h-17h: Trabalha (trem vai e volta 3x)
  - 18h: Pub com amigos (representado, não físico)
  - 20h: Volta para casa
- Satisfação pessoal de ver rotina completa funcionando!

**17h00 - Encerramento**:
- Salva progresso
- Vê estatísticas da sessão:
  - 3 horas jogadas
  - 2 semanas simuladas
  - 15 decisões tomadas
  - R$ 85.000 gastos (virtual)
  - Felicidade geral: 78% → 81%
- Acelera simulação: próximos 7 dias em 5 minutos (IA gerencia)
- Desliga sistema, mas luzes da maquete ficam acesas (decoração)

---

## 🌟 MENSAGEM FINAL E FILOSOFIA

### A Beleza do Imperfeito

Este projeto não precisa ser perfeito. Aliás, **não deve ser**.

Os fios embaixo da maquete podem estar bagunçados. O primeiro prédio que você construir será torto. O código terá bugs. Agentes farão coisas absurdas. E **está tudo bem**.

A beleza está na jornada: 
- Na primeira vez que o LED acende e vocês pulam de alegria
- No sábado em que passam 4 horas só colando árvores
- No bug hilário onde todos os agentes decidem ir ao mesmo lugar
- Na textura de tijolo que você pintou à mão e ficou "errada" mas charmosa
- No trem descarrilhando e virando piada interna da família

### O Verdadeiro Objetivo

Este projeto não é sobre construir a maquete perfeita. É sobre:

🧠 **Aprender**: Eletrônica, programação, design, história, logística  
🤝 **Conectar**: Tempo de qualidade com sua mãe  
🎨 **Criar**: Dar vida a algo que antes só existia na imaginação  
😊 **Divertir**: Rir dos erros, celebrar vitórias, relaxar  
📈 **Crescer**: Desafiar-se, sair da zona de conforto  

### Quando Parar?

**Resposta curta**: Nunca.

**Resposta longa**: Este é um projeto vivo. Sempre haverá:
- Mais um prédio para construir
- Mais um recurso para implementar
- Mais uma era histórica para adicionar
- Mais um detalhe para refinar

E isso é lindo. Mas você pode (e deve) ter **entregas parciais**:
- "A maquete 1m² está pronta" (embora possa expandir)
- "O sistema básico funciona" (embora possa melhorar)
- "O Capítulo 1 da história está jogável" (embora hajam 3 capítulos restantes)

Celebre cada entrega. Depois, se quiser, continue. Mas sem obrigação.

### O Que Você Terá no Final

Daqui 3, 5, 10 anos, olhando para trás, você terá:

📦 Uma maquete física única, feita com suas mãos  
💾 Um sistema computacional complexo e funcional  
📚 Conhecimento profundo em múltiplas áreas  
🎓 Portfólio técnico impressionante  
❤️ Memórias preciosas com sua família  
😌 Sensação de realização profunda  

E quando alguém perguntar: *"Por que você fez isso?"*

Você pode simplesmente responder:

> *"Porque eu quis. E foi incrível."*

---

## 🚀 PRIMEIROS PASSOS PRÁTICOS

### Semana 1: Compromisso e Planejamento
**Segunda-feira**:
- Leia este documento inteiro novamente (sim, de novo!)
- Anote 3 coisas que mais te empolgaram
- Converse com sua mãe sobre o projeto

**Quarta-feira**:
- Criem juntas um "quadro de sonhos" (Pinterest, papel, etc.)
- Coletem imagens de maquetes que gostam
- Definam estética inicial (qual era começar?)

**Sexta-feira**:
- Comprem caderno dedicado ao projeto
- Primeira página: "Por que estamos fazendo isso?"
- Desenhem rascunho da maquete (não precisa ser bonito)

**Sábado**:
- Sessão de vídeos: 3-4 tutoriais de ferromodelismo
- Anotem perguntas que surgirem
- Pizza + filme ferroviário (sugestão: "O Expresso Polar")

### Semana 2: Primeiros Experimentos
**Dia 1-2 (2h)**:
- Instalar Python + Visual Studio Code
- Tutorial: "Hello World" em Python
- Criar primeiríssima classe `Agente`

**Dia 3-4 (2h)**:
- Registrar em loja de eletrônica online
- Adicionar ao carrinho (não comprar ainda): Arduino Uno kit
- Assistir: "O que é Arduino?" (WR Kits)

**Dia 5-6 (3h)**:
- Experimento Micro Cidade (descrito acima)
- Fazer com materiais que tem em casa
- Tirar fotos do processo

**Dia 7**:
- Revisão semanal: o que aprendemos?
- Decidir: vamos continuar? (Resposta esperada: SIM!)
- Se sim: fazer primeira compra (Arduino kit)

### Semana 3-4: Fundação
*(Continua conforme plano de estudo do Fase 0)*

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

Haverá momentos de frustração (LED que não acende, código que trava, prédio que desmorona).

Mas também haverá momentos **mágicos**:
- A primeira vez que o trem físico responde ao comando do Python
- Quando sua mãe diz "nossa, ficou lindo!"
- O visitante que percebe os detalhes e fica boquiaberto
- O agente virtual que toma uma decisão completamente inesperada
- O momento em que você olha para tudo e pensa: "eu fiz isso"

**Este GDD é seu mapa.** Mas você escreve a história.

Boa sorte, jovem urbanista digital. 

Que seus trilhos sejam retos, suas soldas firmes, seus códigos sem bugs (ok, poucos bugs), e sua cidade sempre viva e pulsante.

🚂 **Rumo ao progresso sobre trilhos!** 🚂

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

## 🎨 UI/UX: ONDE VOCÊ VAI SE DIFERENCIAR

### Princípios de Design de Interface

Inspirado em **Urbek** + **Cities: Skylines**, mas indo além:

#### 1. UI Explica Sistemas, Não "Ganho/Perda"

**Evitar**:
```
❌ "Felicidade: -10%"
```

**Preferir**:
```
✅ Ícone de pessoa frustrada piscando na estação
   Hover: "João Silva esperou 40min pelo trem"
```

#### 2. Overlays Temáticos

**Camadas Ativáveis**:
- **Overlay Transporte**: Fluxo de passageiros, gargalos
- **Overlay Conflito Social**: Tensões, reclamações, protestos
- **Overlay Acesso**: Quem consegue ir onde (desigualdade espacial)

**Técnica**:
- Mapa simples por padrão
- `Tab` ou botão ativa overlay
- Cores + ícones + animação

#### 3. HUD Contextual (Hover = Causa/Efeito)

**Exemplo**:
```
Mouse sobre estação →
  • 156 passageiros/dia
  • Atraso médio: 12min
  • Reclamações: 8
  • Impacto no bairro: valorização +15%
```

#### 4. Acessibilidade Como Design Core

**Não é "extra", é base**:
- Contraste alto (WCAG AAA)
- Ícones grandes + redundância (cor + forma + texto)
- Fontes legíveis (TextMeshPro no Unity)
- Opção de modo alto contraste / modo daltônico

**Referência**: Unity UI Toolkit + amostras "QuizU" e "Dragon Crashers"

---

## 🔁 JOGABILIDADE: LOOPS REAIS DO PROJETO

### Loop Principal

```
Observar → Intervir → Esperar → Ver Consequências → Interpretar
```

**Não é sobre**:
- "Ganhar"
- Score alto
- Eficiência máxima

**É sobre**:
- Entender sistemas
- Ver emergência
- Aceitar caos como dado

### Modo de Jogo: Contemplativo ≠ Raso

**Inspiração**: Urbek, Islanders

**Lição**:
- Não precisa microgerenciar tudo
- Sistemas bem feitos se auto-explicam
- Profundidade vem de **interações**, não de menus

**Para Ferritine**:
- Modo "Observação/Zen" (já descrito no GDD original)
- Aceleração de tempo permite "deixar rodar"
- Intervenções pontuais têm impacto longo

---

## 🧩 DESENVOLVIMENTO DE CÓDIGO: ARQUITETURA CORRETA

### Estrutura Recomendada

```
ferritine/
├── backend/
│   └── simulation/          # NÚCLEO (sem Unity)
│       ├── core/
│       │   ├── time_system.py      # Ticks discretos
│       │   ├── event_queue.py      # Fila de eventos
│       │   └── world_state.py      # Estado global
│       ├── systems/
│       │   ├── transport_system.py
│       │   ├── economy_system.py
│       │   └── social_system.py
│       └── agents/
│           └── agent.py             # Classe Agente
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
**Descrição**: Trocar tick rate (1 tick = 1 hora vs 1 tick = 1 minuto) e observar colapsos emergentes.  
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

## 📖 FONTES E REFERÊNCIAS

### Documentação Técnica
- Unity Documentation (docs.unity3d.com)
- Unity Learn (learn.unity.com)
- AR Foundation Guide (docs.unity3d.com)
- ML-Agents Toolkit (github.com/Unity-Technologies/ml-agents)

### Publicações Acadêmicas
- *European Research Studies Journal* (2024) - Serious Games em Logística
- *MDPI* (2024) - AR e Gamificação em Logística
- Epstein & Axtell (1996) - *Growing Artificial Societies*

### Tutoriais e Comunidade
- Arduino + Unity Integration (medium.com)
- MQTT for Unity (emqx.com)
- UI Toolkit Samples: Dragon Crashers, QuizU

### Jogos de Referência
- Urbek City Builder, Technicity, Factorio, Satisfactory, Cities: Skylines, Dwarf Fortress, The Sims, Minecraft (+ mods Create, CustomNPCs)

---

## APÊNDICES

### Apêndice A: Glossário Técnico

**Arduino**: Microcontrolador programável, cérebro de projetos eletrônicos  
**DCC (Digital Command Control)**: Sistema que permite controlar múltiplos trens independentemente no mesmo trilho  
**Agente**: Entidade virtual (habitante) com comportamentos e decisões próprias  
**IoT (Internet of Things)**: Conexão de dispositivos físicos à internet/redes  
**MQTT**: Protocolo de comunicação leve para IoT (publish/subscribe)  
**Reed Switch**: Sensor magnético, fecha circuito quando ímã se aproxima  
**Servo Motor**: Motor que pode girar para ângulo específico (0-180°)  
**PWM (Pulse Width Modulation)**: Técnica para controlar intensidade (ex: brilho de LED)  
**API**: Interface para comunicação entre software (ex: Python ↔ dashboard web)  
**Emergent Behavior**: Comportamentos complexos surgindo de regras simples  
**Pathfinding**: Algoritmo para encontrar caminho (ex: A* para rotas)  
**Weathering**: Técnicas de envelhecimento artificial em maquetes  
**HO Scale**: Escala 1:87 (1cm na maquete = 87cm real)  
**N Scale**: Escala 1:160 (menor que HO, permite mais detalhes em menos espaço)

### Apêndice B: Lista de Compras Fase 1 (Simulação Digital)

| Item | Quantidade | Preço Unitário | Total | Onde Comprar |
|------|------------|----------------|-------|--------------|
| Arduino Uno Starter Kit | 1 | R$ 200 | R$ 200 | Usinainfo/FilipeFlop |
| LEDs variados (pacote) | 1 | R$ 15 | R$ 15 | Mercado Livre |
| Reed Switch | 5 | R$ 5 | R$ 25 | Usinainfo |
| Ímãs de neodímio pequenos | 10 | R$ 2 | R$ 20 | Mercado Livre |
| Jumpers (pacote) | 1 | R$ 12 | R$ 12 | Mercado Livre |
| Protoboard | 2 | R$ 15 | R$ 30 | Usinainfo |
| Resistores (kit) | 1 | R$ 18 | R$ 18 | Usinainfo |
| **Total Fase 1** | | | **R$ 320** | |

### Apêndice C: Lista de Compras Fase 2-3 (Maquete Física)

| Item | Quantidade | Preço Unitário | Total | Onde Comprar |
|------|------------|----------------|-------|--------------|
| Placa MDF 100x100cm | 1 | R$ 80 | R$ 80 | Madeireira local |
| Isopor (placas variadas) | - | - | R$ 50 | Loja de materiais |
| EVA (folhas) | 10 | R$ 2 | R$ 20 | Papelaria |
| Tinta acrílica (6 cores) | 6 | R$ 12 | R$ 72 | Loja de artes |
| Cola branca 1kg | 2 | R$ 18 | R$ 36 | Papelaria |
| Trilho flexível HO (5m) | 5 | R$ 30 | R$ 150 | Frateschi/ML |
| Desvio manual HO | 2 | R$ 85 | R$ 170 | Frateschi |
| Locomotiva básica HO | 1 | R$ 350 | R$ 350 | Frateschi |
| Vagões HO | 3 | R$ 80 | R$ 240 | Frateschi |
| Fonte DC básica | 1 | R$ 120 | R$ 120 | Frateschi/ML |
| Servo motor 9g | 3 | R$ 12 | R$ 36 | Usinainfo |
| LED strip 5m | 1 | R$ 40 | R$ 40 | Mercado Livre |
| Miniaturas sortidas | - | - | R$ 100 | Frateschi/ML |
| Pó de grama | 50g | R$ 25 | R$ 25 | Frateschi |
| Árvores (pacote 10) | 2 | R$ 35 | R$ 70 | Frateschi |
| Papelão Paraná | 5 | R$ 15 | R$ 75 | Papelaria |
| Ferramentas básicas | - | - | R$ 150 | Loja de ferramentas |
| **Total Fase 2-3** | | | **R$ 1.784** | |

**Total Geral Fases 1-3: ~R$ 2.100**

### Apêndice D: Cronograma Detalhado Ano 1

#### Janeiro
- Semana 1-2: Leitura de tutoriais, compra de Arduino kit
- Semana 3-4: Primeiros experimentos (Blink, sensores)

#### Fevereiro
- Semana 1-2: Python básico, primeiras classes
- Semana 3-4: Simulação simples (10 agentes, mapa 2D)

#### Março
- Semana 1-2: Comunicação Arduino ↔ Python
- Semana 3-4: Visualização Pygame

#### Abril
- Semana 1-2: Economia básica na simulação
- Semana 3-4: Sistema de transporte virtual

#### Maio
- Semana 1-2: Desenhar planta da maquete
- Semana 3-4: Comprar materiais físicos

#### Junho
- Semana 1-2: Construir base MDF
- Semana 3-4: Relevo em isopor

#### Julho
- Semana 1-2: Pintura de base
- Semana 3-4: Instalar trilhos

#### Agosto
- Semana 1-2: Testar trem físico
- Semana 3-4: Instalar sensores nos trilhos

#### Setembro
- Semana 1-2: Construir primeiros 3 prédios
- Semana 3-4: Construir mais 3 prédios

#### Outubro
- Semana 1-2: Sistema de iluminação (LEDs)
- Semana 3-4: Integrar iluminação com simulação

#### Novembro
- Semana 1-2: Vegetação e detalhes
- Semana 3-4: Acabamento geral

#### Dezembro
- Semana 1-2: Testes finais, calibração
- Semana 3-4: Primeira "apresentação" para família/amigos

### Apêndice E: Estrutura de Banco de Dados

```sql
-- Tabela de Agentes
CREATE TABLE agents (
    id INTEGER PRIMARY KEY,
    name TEXT NOT NULL,
    age INTEGER,
    gender TEXT,
    home_id INTEGER,
    job_id INTEGER,
    workplace_id INTEGER,
    salary REAL,
    money REAL,
    health INTEGER,
    energy INTEGER,
    happiness INTEGER,
    hunger INTEGER,
    knowledge INTEGER,
    strength INTEGER,
    attention INTEGER,
    laziness INTEGER,
    ambition INTEGER,
    is_married BOOLEAN,
    created_at TIMESTAMP,
    FOREIGN KEY (home_id) REFERENCES buildings(id),
    FOREIGN KEY (workplace_id) REFERENCES buildings(id)
);

-- Tabela de Edifícios
CREATE TABLE buildings (
    id INTEGER PRIMARY KEY,
    name TEXT,
    type TEXT, -- residential, commercial, industrial
    x INTEGER,
    y INTEGER,
    owner_id INTEGER,
    construction_progress INTEGER, -- 0-100
    condition INTEGER, -- estado de conservação 0-100
    value REAL,
    created_at TIMESTAMP,
    FOREIGN KEY (owner_id) REFERENCES agents(id)
);

-- Tabela de Veículos (Trens, Ônibus)
CREATE TABLE vehicles (
    id INTEGER PRIMARY KEY,
    type TEXT, -- train, bus
    model TEXT,
    current_station_id INTEGER,
    current_position REAL, -- posição no trilho/rua
    speed REAL,
    capacity INTEGER,
    current_passengers INTEGER,
    cargo_type TEXT,
    cargo_amount REAL,
    condition INTEGER, -- 0-100
    fuel_level REAL,
    FOREIGN KEY (current_station_id) REFERENCES buildings(id)
);

-- Tabela de Eventos Históricos
CREATE TABLE events (
    id INTEGER PRIMARY KEY,
    timestamp TIMESTAMP,
    type TEXT, -- construction, accident, election, disaster
    description TEXT,
    impact_happiness INTEGER,
    impact_economy REAL,
    related_agent_id INTEGER,
    related_building_id INTEGER,
    FOREIGN KEY (related_agent_id) REFERENCES agents(id),
    FOREIGN KEY (related_building_id) REFERENCES buildings(id)
);

-- Tabela de Estatísticas Econômicas
CREATE TABLE economy_stats (
    id INTEGER PRIMARY KEY,
    date DATE,
    gdp REAL,
    unemployment_rate REAL,
    inflation_rate REAL,
    average_happiness REAL,
    population INTEGER,
    total_money_supply REAL
);

-- Tabela de Relações Familiares
CREATE TABLE family_relations (
    id INTEGER PRIMARY KEY,
    agent_id INTEGER,
    related_agent_id INTEGER,
    relation_type TEXT, -- parent, child, spouse, sibling
    FOREIGN KEY (agent_id) REFERENCES agents(id),
    FOREIGN KEY (related_agent_id) REFERENCES agents(id)
);

-- Tabela de Rotas de Transporte
CREATE TABLE routes (
    id INTEGER PRIMARY KEY,
    vehicle_id INTEGER,
    start_station_id INTEGER,
    end_station_id INTEGER,
    departure_time TIME,
    arrival_time TIME,
    frequency_minutes INTEGER, -- a cada X minutos
    fare REAL,
    FOREIGN KEY (vehicle_id) REFERENCES vehicles(id),
    FOREIGN KEY (start_station_id) REFERENCES buildings(id),
    FOREIGN KEY (end_station_id) REFERENCES buildings(id)
);
```

### Apêndice F: Exemplo de Configuração YAML

```yaml
# config.yaml - Configurações da Simulação

simulation:
  time_scale: 60  # 1 minuto real = 60 minutos simulados
  tick_rate: 1    # Atualização a cada 1 segundo real
  auto_save_interval: 300  # Salvar a cada 5 minutos
  
city:
  name: "Santópolis"
  foundation_year: 1887
  starting_population: 50
  starting_money: 100000
  
world:
  grid_size: [100, 100]  # células
  cell_size: 10  # metros por célula
  
agents:
  max_agents: 1000
  birth_rate: 0.015  # por ano
  death_rate: 0.008
  immigration_rate: 0.005
  
economy:
  starting_gdp: 500000
  inflation_target: 0.03  # 3% ao ano
  unemployment_natural: 0.04  # 4% desemprego natural
  
  prices:
    food: 15
    housing_rent: 500
    train_ticket: 5
    bus_ticket: 3
    
  salaries:
    teacher: 3000
    factory_worker: 2500
    clerk: 2800
    manager: 5000
    
transport:
  train_max_speed: 80  # km/h
  bus_max_speed: 50
  car_max_speed: 60
  
  train_capacity: 200  # passageiros
  bus_capacity: 40
  
hardware:
  arduino_port: "COM3"  # ou /dev/ttyUSB0 no Linux
  baud_rate: 9600
  
  sensors:
    - id: 1
      type: "reed_switch"
      location: "station_north"
    - id: 2
      type: "reed_switch"
      location: "station_south"
      
  mqtt:
    broker: "localhost"
    port: 1883
    topics:
      train_position: "city/train/+/position"
      train_command: "city/train/+/speed"
      lights: "city/lights/+"
      
visualization:
  window_size: [1280, 720]
  fps: 60
  style: "indie"  # indie, realistic, pixel
  
  colors:
    background: [34, 139, 34]  # verde grama
    roads: [80, 80, 80]
    rails: [139, 69, 19]
    water: [30, 144, 255]
    
ai:
  enabled: true
  auto_manage: false  # IA só sugere, não executa
  difficulty: "medium"  # easy, medium, hard
  
  features:
    demand_prediction: true
    news_generation: true
    route_optimization: true
    crisis_management: true
```

### Apêndice G: Recursos Online Gratuitos

#### Tutoriais em Vídeo (YouTube)
1. **Eletrônica**:
   - Canal WR Kits: "Curso Completo Arduino" (playlist)
   - Brincando com Ideias: Projetos práticos
   
2. **Python**:
   - Curso em Vídeo (Gustavo Guanabara): Mundo 1, 2 e 3
   - Eduardo Mendes (Live de Python): POO avançada
   
3. **Ferromodelismo**:
   - Ferromodelismo Brasil: Tutoriais básicos
   - Luke Towan: Técnicas avançadas de paisagem (inglês)
   
4. **Simulação e Game Design**:
   - Coding Train: Processing + simulações
   - Sebastian Lague: Algoritmos de simulação

#### Documentação Online
- Arduino Official: arduino.cc/reference
- Python Docs: docs.python.org/3/
- Pygame Docs: pygame.org/docs
- SimPy Docs: simpy.readthedocs.io

#### Ferramentas Gratuitas
- Tinkercad Circuits: tinkercad.com (simulação Arduino)
- Fritzing: fritzing.org (desenhar circuitos)
- GIMP: gimp.org (edição de imagens para texturas)
- Audacity: audacityteam.org (sons para a cidade)

#### Comunidades BR
- Fórum Arduino Brasil: arduino.cc/forum (seção português)
- Grupo Telegram "Python Brasil"
- Discord "Programação BR"
- Reddit: r/brasil (perguntas gerais)

### Apêndice H: Ideias de Expansões Futuras

#### Expansão 1: "Vida Noturna"
- Bares, restaurantes, cinemas
- Agentes saem à noite
- Iluminação noturna detalhada (LEDs RGB)
- Eventos: shows, festas

#### Expansão 2: "Educação e Cultura"
- Sistema escolar completo (crianças vão à escola)
- Universidade (agentes estudam, se formam)
- Museus, bibliotecas
- Impacto no conhecimento dos agentes

#### Expansão 3: "Saúde e Medicina"
- Doenças mais complexas (gripe, fraturas, crônicas)
- Hospital funcional (leitos, médicos)
- Farmácias
- Envelhecimento realista (expectativa de vida)

#### Expansão 4: "Criminalidade"
- Sistema de segurança (polícia)
- Crimes (furtos, acidentes)
- Presídio
- Impacto na felicidade e economia

#### Expansão 5: "Turismo"
- Hotel
- Atrações turísticas
- Turistas (agentes temporários)
- Receita de turismo

#### Expansão 6: "Religião e Espiritualidade"
- Igrejas, templos
- Agentes religiosos
- Feriados religiosos
- Eventos (casamentos, funerais)

#### Expansão 7: "Esportes"
- Estádio
- Times locais
- Agentes assistem jogos
- Rivalidades

#### Expansão 8: "Meio Ambiente"
- Poluição (ar, água)
- Reciclagem
- Parques e preservação
- Animais (pássaros, cães, gatos)

#### Expansão 9: "Tecnologia Avançada"
- Internet na cidade (agentes navegam)
- Smartphones (comunicação instantânea)
- E-commerce (compras online)
- Trabalho remoto

#### Expansão 10: "Conectividade Regional"
- Aeroporto (voos para outras cidades)
- Porto fluvial/marítimo
- Rodovia interestadual
- Comércio inter-regional

### Apêndice I: Templates de Código Úteis

#### Template 1: Classe Base para Entidades

```python
# backend/simulation/entity.py

from abc import ABC, abstractmethod
from typing import Dict, Any

class Entity(ABC):
    """
    Classe base para todas as entidades do mundo
    (Agentes, Edifícios, Veículos)
    """
    
    _id_counter = 0
    
    def __init__(self, name: str):
        Entity._id_counter += 1
        self.id = Entity._id_counter
        self.name = name
        self.created_at = None  # Timestamp de criação
        
    @abstractmethod
    def update(self, world, delta_time):
        """
        Atualiza o estado da entidade
        Deve ser implementado por subclasses
        """
        pass
    
    @abstractmethod
    def get_state(self) -> Dict[str, Any]:
        """
        Retorna estado atual como dicionário
        Para serialização/salvamento
        """
        pass
    
    def __repr__(self):
        return f"{self.__class__.__name__}(id={self.id}, name='{self.name}')"
```

#### Template 2: Singleton para Gerenciador Global

```python
# backend/simulation/world_manager.py

class WorldManager:
    """
    Singleton que gerencia o mundo inteiro
    Acesso global de qualquer lugar do código
    """
    
    _instance = None
    
    def __new__(cls):
        if cls._instance is None:
            cls._instance = super().__new__(cls)
            cls._instance._initialized = False
        return cls._instance
    
    def __init__(self):
        if self._initialized:
            return
            
        self.world = None
        self.agents = []
        self.buildings = []
        self.vehicles = []
        self.events = []
        self.current_time = None
        self._initialized = True
    
    def get_agent(self, agent_id):
        """Busca agente por ID"""
        for agent in self.agents:
            if agent.id == agent_id:
                return agent
        return None
    
    def add_agent(self, agent):
        """Adiciona novo agente ao mundo"""
        self.agents.append(agent)
        
    def remove_agent(self, agent_id):
        """Remove agente (morte, emigração)"""
        self.agents = [a for a in self.agents if a.id != agent_id]

# Uso em qualquer arquivo:
# from backend.simulation.world_manager import WorldManager
# manager = WorldManager()
# agent = manager.get_agent(5)
```

#### Template 3: Event Bus (Padrão Observer)

```python
# backend/utils/event_bus.py

from typing import Callable, Dict, List

class EventBus:
    """
    Sistema de eventos desacoplado
    Qualquer parte do código pode emitir/escutar eventos
    """
    
    def __init__(self):
        self._listeners: Dict[str, List[Callable]] = {}
    
    def subscribe(self, event_type: str, callback: Callable):
        """
        Registra um listener para um tipo de evento
        """
        if event_type not in self._listeners:
            self._listeners[event_type] = []
        self._listeners[event_type].append(callback)
    
    def emit(self, event_type: str, data: Any = None):
        """
        Emite um evento, notificando todos os listeners
        """
        if event_type in self._listeners:
            for callback in self._listeners[event_type]:
                callback(data)
    
    def unsubscribe(self, event_type: str, callback: Callable):
        """
        Remove um listener
        """
        if event_type in self._listeners:
            self._listeners[event_type].remove(callback)

# Uso:
# event_bus = EventBus()
# 
# def on_train_arrival(data):
#     print(f"Trem {data['train_id']} chegou!")
# 
# event_bus.subscribe("train_arrived", on_train_arrival)
# event_bus.emit("train_arrived", {"train_id": 1, "station": "Norte"})
```


## 📜 LICENÇA E CRÉDITOS

### Sobre Este Documento

Este Game Design Document foi criado especificamente para o projeto **Maquete Viva** em 2025.

**Autoria**: Co-criado em colaboração entre você e Claude (Anthropic)  
**Propósito**: Guia técnico e conceitual para desenvolvimento  
**Natureza**: Documento vivo, pode (e deve) ser atualizado conforme projeto evolui

### Licença do Projeto

Sugestão para o seu projeto:

```
MIT License (Permissiva - recomendada para projetos pessoais/educacionais)

Permite que outros usem, modifiquem e distribuam seu código,
desde que mantenham o aviso de copyright.
```

Ou, se preferir mantê-lo privado inicialmente, pode não licenciar (todos os direitos reservados).

### Créditos e Agradecimentos

Ao longo do projeto, considere agradecer:
- Sua mãe (parceria e inspiração inicial)
- Criadores de tutoriais que você seguiu
- Comunidades online que ajudaram
- Fabricantes de ferramentas/hardware usados

### Citação Deste GDD

Se você compartilhar este documento ou projeto derivado:

```
"Maquete Viva: Game Design Document"
Criado em colaboração com Claude (Anthropic), 2025
Desenvolvido por: [Seu Nome]
```

---

## 🎊 PALAVRA FINAL

Chegamos ao fim deste Game Design Document.

Mas para você, é apenas o **começo**.

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

**FIM DO GAME DESIGN DOCUMENT**

*Versão 1.0 - Outubro 2025*

*"Sobre trilhos de imaginação, cidades nascem"*