#!/usr/bin/env python3
"""
Script para criar automaticamente labels e milestones no GitHub
para o projeto Ferritine.

Uso:
    python scripts/create_github_structure.py --token YOUR_GITHUB_TOKEN

Requer:
    pip install requests python-dotenv
"""

import argparse
import os
import sys
from datetime import datetime, timedelta
from typing import Dict, List
import requests
from dotenv import load_dotenv

# Carregar variáveis de ambiente
load_dotenv()

# Configurações
REPO_OWNER = "lari-ember"
REPO_NAME = "ferritine"
GITHUB_API = "https://api.github.com"


class GitHubStructureCreator:
    """Cria estrutura de labels e milestones no GitHub"""

    def __init__(self, token: str):
        self.token = token
        self.headers = {
            "Authorization": f"token {token}",
            "Accept": "application/vnd.github.v3+json"
        }
        self.base_url = f"{GITHUB_API}/repos/{REPO_OWNER}/{REPO_NAME}"

    def create_label(self, name: str, description: str, color: str) -> Dict:
        """Cria um label no repositório"""
        url = f"{self.base_url}/labels"
        data = {
            "name": name,
            "description": description,
            "color": color.replace("#", "")
        }
        
        response = requests.post(url, headers=self.headers, json=data)
        
        if response.status_code == 201:
            print(f"✅ Label criado: {name}")
            return response.json()
        elif response.status_code == 422:
            print(f"⚠️  Label já existe: {name}")
            return {}
        else:
            print(f"❌ Erro ao criar label {name}: {response.status_code}")
            print(f"   {response.json()}")
            return {}

    def create_milestone(self, title: str, description: str, due_date: str = None) -> Dict:
        """Cria um milestone no repositório"""
        url = f"{self.base_url}/milestones"
        data = {
            "title": title,
            "description": description,
            "state": "open"
        }
        
        if due_date:
            data["due_on"] = due_date
        
        response = requests.post(url, headers=self.headers, json=data)
        
        if response.status_code == 201:
            print(f"✅ Milestone criado: {title}")
            return response.json()
        elif response.status_code == 422:
            print(f"⚠️  Milestone já existe: {title}")
            return {}
        else:
            print(f"❌ Erro ao criar milestone {title}: {response.status_code}")
            print(f"   {response.json()}")
            return {}

    def create_all_labels(self):
        """Cria todos os labels do projeto"""
        print("\n📋 Criando Labels...\n")

        labels = [
            # Por Tipo
            ("feat", "Nova funcionalidade", "0366d6"),
            ("bug", "Correção de bug", "d73a4a"),
            ("docs", "Documentação", "0075ca"),
            ("test", "Testes", "d4c5f9"),
            ("refactor", "Refatoração", "fbca04"),
            ("chore", "Manutenção/tarefas auxiliares", "fef2c0"),
            ("hardware", "Relacionado a hardware/eletrônica", "e99695"),
            ("simulation", "Sistema de simulação", "1d76db"),
            ("iot", "Internet das Coisas", "5319e7"),
            
            # Por Prioridade
            ("priority: critical", "Crítico, bloqueia outras funcionalidades", "b60205"),
            ("priority: high", "Alta prioridade", "d93f0b"),
            ("priority: medium", "Prioridade média", "fbca04"),
            ("priority: low", "Baixa prioridade", "0e8a16"),
            
            # Por Fase
            ("phase-0: fundamentals", "Fase 0: Fundamentos", "5319e7"),
            ("phase-1: digital", "Fase 1: Simulação Digital", "7057ff"),
            ("phase-2: basic-hardware", "Fase 2: Hardware Básico", "8b72ff"),
            ("phase-3: physical-model", "Fase 3: Maquete Física", "a18dff"),
            ("phase-4: expansion", "Fase 4: Expansão", "b8a9ff"),
            
            # Por Área
            ("area: agents", "Sistema de agentes", "0e8a16"),
            ("area: economy", "Sistema econômico", "1a7f37"),
            ("area: transport", "Sistema de transporte", "22863a"),
            ("area: politics", "Sistema político", "2ea44f"),
            ("area: construction", "Sistema de construção", "2da44e"),
            ("area: ui", "Interface de usuário", "38b44a"),
            ("area: database", "Banco de dados", "46c85c"),
            ("area: api", "API REST/WebSocket", "57d769"),
            ("area: world", "Mundo/Cidade", "6ae07f"),
            
            # Por Complexidade
            ("complexity: beginner", "Fácil, bom para iniciantes", "c5def5"),
            ("complexity: intermediate", "Complexidade média", "fbca04"),
            ("complexity: advanced", "Avançado, requer experiência", "d93f0b"),
            
            # Especiais
            ("good first issue", "Boa primeira issue para novos contribuidores", "7057ff"),
            ("help wanted", "Precisa de ajuda da comunidade", "008672"),
            ("blocked", "Bloqueada por outra issue", "e99695"),
            ("wip", "Work in Progress (em andamento)", "d4c5f9"),
            ("research", "Pesquisa necessária", "c5def5"),
            ("ai", "Inteligência Artificial / Machine Learning", "5319e7"),
        ]

        for name, description, color in labels:
            self.create_label(name, description, color)

    def create_all_milestones(self):
        """Cria todos os milestones do projeto"""
        print("\n🎯 Criando Milestones...\n")

        # Data base: hoje
        today = datetime.now()

        milestones = [
            (
                "Milestone 0: Fundamentos e Infraestrutura",
                "Estabelecer fundamentos do projeto, documentação básica e aprendizado inicial.\n\n"
                "Objetivos:\n"
                "- Reorganizar estrutura do projeto\n"
                "- Configurar logging, configuração e banco de dados\n"
                "- Criar documentação técnica\n"
                "- Documentar currículos de aprendizado (eletrônica, IoT, simulação)",
                (today + timedelta(weeks=8)).isoformat() + "Z"
            ),
            (
                "Milestone 1.1: Mundo Estático",
                "Criar estrutura básica da cidade com grid 2D, edifícios e ruas.\n\n"
                "Objetivos:\n"
                "- Grid 2D implementado\n"
                "- Sistema de edifícios funcionando\n"
                "- Ruas e trilhos no grid\n"
                "- Visualização 2D com Pygame",
                (today + timedelta(weeks=10)).isoformat() + "Z"
            ),
            (
                "Milestone 1.2: Agentes Simples",
                "Implementar agentes com rotinas básicas e atributos.\n\n"
                "Objetivos:\n"
                "- Agentes com atributos físicos/mentais\n"
                "- Máquina de estados implementada\n"
                "- Rotinas diárias funcionando\n"
                "- Visualização de agentes no mapa",
                (today + timedelta(weeks=12)).isoformat() + "Z"
            ),
            (
                "Milestone 1.3: Economia Básica",
                "Sistema de salários, gastos e produção simples.\n\n"
                "Objetivos:\n"
                "- Sistema econômico básico\n"
                "- Cadeia produtiva implementada\n"
                "- Dashboard de estatísticas",
                (today + timedelta(weeks=14)).isoformat() + "Z"
            ),
            (
                "Milestone 1.4: Transporte Ferroviário Virtual",
                "Trens virtuais funcionando na simulação.\n\n"
                "Objetivos:\n"
                "- Classe Train implementada\n"
                "- Rotas e horários funcionando\n"
                "- Embarque/desembarque de passageiros\n"
                "- Transporte de carga\n"
                "- Visualização de trens",
                (today + timedelta(weeks=16)).isoformat() + "Z"
            ),
            (
                "Milestone 2.1: Circuito de Iluminação",
                "Arduino controlando LEDs via Python.\n\n"
                "Objetivos:\n"
                "- Comunicação Serial Python-Arduino\n"
                "- Firmware de controle de LEDs\n"
                "- Iluminação sincronizada com dia/noite",
                (today + timedelta(weeks=19)).isoformat() + "Z"
            ),
            (
                "Milestone 2.2: Sensor de Trem",
                "Detecção de trem com reed switch.\n\n"
                "Objetivos:\n"
                "- Firmware de sensor de trem\n"
                "- Integração com simulação",
                (today + timedelta(weeks=21)).isoformat() + "Z"
            ),
            (
                "Milestone 2.3: Controle de Desvio",
                "Servomotor controlando desvios de trilho.\n\n"
                "Objetivos:\n"
                "- Firmware de controle de servo\n"
                "- Integração com simulação",
                (today + timedelta(weeks=23)).isoformat() + "Z"
            ),
            (
                "Milestone 3.1: Base e Topografia",
                "Construir base física MDF 100x100cm com relevo.\n\n"
                "Objetivos:\n"
                "- Projeto da base documentado\n"
                "- Guia de construção de relevo",
                (today + timedelta(weeks=27)).isoformat() + "Z"
            ),
            (
                "Milestone 3.2: Trilhos e Primeiro Trem",
                "Sistema ferroviário físico funcionando.\n\n"
                "Objetivos:\n"
                "- Guia de instalação de trilhos\n"
                "- Controle DCC integrado",
                (today + timedelta(weeks=31)).isoformat() + "Z"
            ),
            (
                "Milestone 3.3: Primeiros Edifícios",
                "3-5 prédios construídos e instalados.\n\n"
                "Objetivos:\n"
                "- Modelos 3D criados\n"
                "- Técnicas de construção documentadas",
                (today + timedelta(weeks=35)).isoformat() + "Z"
            ),
            (
                "Milestone 3.4: Integração Física-Digital",
                "Sincronização completa entre maquete física e simulação.\n\n"
                "Objetivos:\n"
                "- Sincronização física-digital funcionando\n"
                "- Dashboard web implementado",
                (today + timedelta(weeks=39)).isoformat() + "Z"
            ),
        ]

        for title, description, due_date in milestones:
            self.create_milestone(title, description, due_date)


def main():
    """Função principal"""
    parser = argparse.ArgumentParser(
        description="Cria estrutura de labels e milestones no GitHub"
    )
    parser.add_argument(
        "--token",
        help="GitHub Personal Access Token",
        default=os.getenv("GITHUB_TOKEN")
    )
    parser.add_argument(
        "--labels-only",
        action="store_true",
        help="Criar apenas labels"
    )
    parser.add_argument(
        "--milestones-only",
        action="store_true",
        help="Criar apenas milestones"
    )

    args = parser.parse_args()

    if not args.token:
        print("❌ Erro: GitHub token não fornecido.")
        print("Use --token YOUR_TOKEN ou defina GITHUB_TOKEN no arquivo .env")
        sys.exit(1)

    print(f"\n🚀 Criando estrutura para {REPO_OWNER}/{REPO_NAME}\n")

    creator = GitHubStructureCreator(args.token)

    if args.labels_only:
        creator.create_all_labels()
    elif args.milestones_only:
        creator.create_all_milestones()
    else:
        creator.create_all_labels()
        creator.create_all_milestones()

    print("\n✨ Concluído!\n")


if __name__ == "__main__":
    main()
