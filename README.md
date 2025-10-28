# Ferritine

[![Python Version](https://img.shields.io/badge/python-3.8%2B-blue.svg)](https://www.python.org/downloads/release/python-380/)
[![License](https://img.shields.io/badge/license-MIT-green.svg)](LICENSE)
[![Tests](https://img.shields.io/github/actions/workflow/status/ferritine/tests.yml?branch=main)]

Ferritine é um projeto de maquete híbrida físico-digital que combina agentes, trens e simulação para criar um ambiente interativo e dinâmico.

## Recursos
- Simulação de agentes com rotinas diárias.
- Integração com maquetes físicas.
- Testes unitários para garantir a qualidade do código.

## Pré-requisitos
- Python 3.8 ou superior
- Sistema operacional Linux ou compatível

## Como rodar
1. Clone o repositório:
   ```bash
   git clone https://github.com/ferritine/ferritine.git
   cd ferritine
   ```
2. Crie e ative um ambiente virtual:
   ```bash
   python -m venv .venv
   source .venv/bin/activate
   ```
3. Instale as dependências:
   ```bash
   pip install -r requirements.txt
   ```
4. Execute o programa:
   ```bash
   python main.py
   ```

## Estrutura do Projeto
- `app/models` — Modelos de domínio (Agente, Cidade).
- `app/tests` — Testes unitários.
- `VERSION` — Versão semântica atual.
- `main.py` — Ponto de entrada do programa.
- `requirements.txt` — Dependências do projeto.
- `README.md` — Documentação do projeto.
- `LICENSE` — Licença do projeto.

## Exemplos
Para simular uma cidade com agentes, execute o programa e observe a saída no terminal. O estado da cidade será atualizado a cada hora simulada.

## Contribuição
Contribuições são bem-vindas! Siga os passos abaixo:
1. Abra issues para relatar bugs ou sugerir novas funcionalidades.
2. Faça um fork do repositório e crie uma branch para suas alterações.
3. Envie um Pull Request para a branch `main`.

Certifique-se de seguir o padrão de código PEP 8 e incluir testes unitários para novas funcionalidades.

## Licença
Este projeto está licenciado sob a licença MIT. Veja o arquivo [LICENSE](LICENSE) para mais detalhes.
